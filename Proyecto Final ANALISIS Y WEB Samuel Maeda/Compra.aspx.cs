using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    [Serializable]
    public class ItemCompra
    {
        public int LibroId { get; set; }
        public string Titulo { get; set; }
        public string Editorial { get; set; }
        public string Categoria { get; set; }
        public string UnidadNombre { get; set; }
        public int UnidadMedidaId { get; set; }
        public int Cantidad { get; set; } // cantidad en unidades (convertida)
        public decimal PrecioUnitario { get; set; }
        public int Descuento { get; set; } // porcentaje
        public decimal Subtotal => Cantidad * PrecioUnitario * (1 - Descuento / 100m);
    }

    public partial class Compras : Page
    {
        private string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        private List<ItemCompra> ListaCompra
        {
            get => ViewState["ListaCompra"] as List<ItemCompra> ?? new List<ItemCompra>();
            set => ViewState["ListaCompra"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarProveedores();
                CargarEditoriales();
                CargarUnidades();
                CargarLibros(0);
                CargarUltimasCompras();
                ActualizarTabla();
            }
        }

        private void CargarProveedores()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT ProveedorId, Nombre FROM Proveedores", con);
                con.Open();
                ddlProveedor.DataSource = cmd.ExecuteReader();
                ddlProveedor.DataTextField = "Nombre";
                ddlProveedor.DataValueField = "ProveedorId";
                ddlProveedor.DataBind();
                ddlProveedor.Items.Insert(0, new ListItem("-- Seleccione un proveedor --", "0"));
            }
        }

        private void CargarEditoriales()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT EditorialId, Nombre FROM Editoriales", con);
                con.Open();
                ddlEditorial.DataSource = cmd.ExecuteReader();
                ddlEditorial.DataTextField = "Nombre";
                ddlEditorial.DataValueField = "EditorialId";
                ddlEditorial.DataBind();
                ddlEditorial.Items.Insert(0, new ListItem("-- Todas las editoriales --", "0"));
            }
        }

        private void CargarLibros(int editorialId)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string sql = editorialId == 0
                    ? "SELECT LibroId, Titulo FROM Libros ORDER BY Titulo"
                    : "SELECT LibroId, Titulo FROM Libros WHERE EditorialId = @EditorialId ORDER BY Titulo";

                SqlCommand cmd = new SqlCommand(sql, con);
                if (editorialId != 0) cmd.Parameters.AddWithValue("@EditorialId", editorialId);

                con.Open();
                ddlLibro.DataSource = cmd.ExecuteReader();
                ddlLibro.DataTextField = "Titulo";
                ddlLibro.DataValueField = "LibroId";
                ddlLibro.DataBind();
                ddlLibro.Items.Insert(0, new ListItem("-- Seleccione un libro --", "0"));
            }
        }

        private void CargarUnidades()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // suponemos UnidadMedida tiene UnidadMedidaId, Nombre, CantidadPorUnidad, DescuentoPorcentaje
                SqlCommand cmd = new SqlCommand("SELECT UnidadMedidaId, Nombre FROM UnidadMedida", con);
                con.Open();
                ddlUnidad.DataSource = cmd.ExecuteReader();
                ddlUnidad.DataTextField = "Nombre";
                ddlUnidad.DataValueField = "UnidadMedidaId";
                ddlUnidad.DataBind();
                ddlUnidad.Items.Insert(0, new ListItem("-- Seleccione una unidad --", "0"));
            }
        }

        protected void ddlEditorial_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id;
            if (int.TryParse(ddlEditorial.SelectedValue, out id) && id > 0) CargarLibros(id);
            else CargarLibros(0);
        }

        protected void ddlLibro_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCategoria.Text = "";
            lblStock.Text = "-";
            if (ddlLibro.SelectedValue == "0") return;

            int libroId = int.Parse(ddlLibro.SelectedValue);
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"SELECT ISNULL(L.StockUnidades,0) AS Stock, C.Nombre AS Categoria
                                 FROM Libros L
                                 INNER JOIN Categorias C ON L.CategoriaId = C.CategoriaId
                                 WHERE L.LibroId = @LibroId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@LibroId", libroId);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblStock.Text = dr["Stock"].ToString();
                    txtCategoria.Text = dr["Categoria"].ToString();
                }
            }
        }

        protected void ddlUnidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            // no es obligatorio, queda aquí si quieres mostrar información al cambiar unidad
        }

        protected void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            if (ddlLibro.SelectedValue == "0" || ddlUnidad.SelectedValue == "0" || string.IsNullOrWhiteSpace(txtCantidad.Text)) return;

            int libroId = int.Parse(ddlLibro.SelectedValue);
            int unidadId = int.Parse(ddlUnidad.SelectedValue);
            int cantidadIngresada;
            if (!int.TryParse(txtCantidad.Text.Trim(), out cantidadIngresada) || cantidadIngresada <= 0) return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT L.Titulo,
                           ISNULL(E.Nombre,'') AS Editorial,
                           ISNULL(Cat.Nombre,'') AS Categoria,
                           ISNULL(L.PrecioOverride, Cat.PrecioBase) AS PrecioBase,
                           ISNULL(UM.CantidadPorUnidad, 1) AS CantidadPorUnidad,
                           ISNULL(UM.DescuentoPorcentaje, 0) AS DescuentoPorcentaje,
                           ISNULL(UM.Nombre, '') AS UnidadNombre,
                           UM.UnidadMedidaId
                    FROM Libros L
                    INNER JOIN Categorias Cat ON L.CategoriaId = Cat.CategoriaId
                    LEFT JOIN Editoriales E ON L.EditorialId = E.EditorialId
                    LEFT JOIN UnidadMedida UM ON UM.UnidadMedidaId = @UnidadId
                    WHERE L.LibroId = @LibroId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@LibroId", libroId);
                cmd.Parameters.AddWithValue("@UnidadId", unidadId);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.Read()) return;

                string titulo = dr["Titulo"].ToString();
                string editorial = dr["Editorial"].ToString();
                string categoria = dr["Categoria"].ToString();
                string unidadNombre = dr["UnidadNombre"].ToString();
                decimal precio = Convert.ToDecimal(dr["PrecioBase"]);
                int cantidadPorUnidad = Convert.ToInt32(dr["CantidadPorUnidad"]);
                int descuento = Convert.ToInt32(dr["DescuentoPorcentaje"]);
                int unidadMedidaId = Convert.ToInt32(dr["UnidadMedidaId"]);

                int cantidadTotal = cantidadIngresada * (cantidadPorUnidad > 0 ? cantidadPorUnidad : 1);

                // buscar existente por Titulo, Editorial, Categoria, UnidadNombre
                var lista = ListaCompra;
                var existente = lista.Find(x => x.Titulo == titulo && x.Editorial == editorial &&
                                                x.Categoria == categoria && x.UnidadNombre == unidadNombre);
                if (existente != null)
                {
                    existente.Cantidad += cantidadTotal;
                }
                else
                {
                    lista.Add(new ItemCompra
                    {
                        LibroId = libroId,
                        Titulo = titulo,
                        Editorial = editorial,
                        Categoria = categoria,
                        UnidadNombre = unidadNombre,
                        UnidadMedidaId = unidadMedidaId,
                        Cantidad = cantidadTotal,
                        PrecioUnitario = precio,
                        Descuento = descuento
                    });
                }

                ListaCompra = lista;
            }

            // limpiar inputs
            txtCantidad.Text = "";
            ddlUnidad.SelectedIndex = 0;
            ddlLibro.SelectedIndex = 0;
            lblStock.Text = "-";
            txtCategoria.Text = "";

            ActualizarTabla();
        }

        private void ActualizarTabla()
        {
            gvCompraActual.DataSource = ListaCompra;
            gvCompraActual.DataBind();

            decimal total = 0m;
            decimal ahorro = 0m;
            foreach (var it in ListaCompra)
            {
                total += it.Subtotal;
                ahorro += it.Cantidad * it.PrecioUnitario * (it.Descuento / 100m);
            }

            lblTotal.Text = $"Total: Q{total:N2}";
            lblAhorro.Text = $"Ahorro total: Q{ahorro:N2}";

            bool vacio = ListaCompra == null || ListaCompra.Count == 0;
            lblMensajeVacio.Visible = vacio;
            gvCompraActual.Visible = !vacio;
        }

        // Limpia solo la lista de compra (mantiene filtros/inputs)
        protected void btnLimpiarLista_Click(object sender, EventArgs e)
        {
            ListaCompra = new List<ItemCompra>();
            ActualizarTabla();
        }

        // Limpia todo el formulario (proveedor, editorial, libro, unidad, cantidad, lista)
        protected void btnLimpiarTodo_Click(object sender, EventArgs e)
        {
            ddlProveedor.SelectedIndex = 0;
            ddlEditorial.SelectedIndex = 0;
            CargarLibros(0);
            ddlUnidad.SelectedIndex = 0;
            ddlLibro.SelectedIndex = 0;
            txtCantidad.Text = "";
            lblStock.Text = "-";
            txtCategoria.Text = "";
            ListaCompra = new List<ItemCompra>();
            ActualizarTabla();
        }

        protected void btnRegistrarCompra_Click(object sender, EventArgs e)
        {
            if (ddlProveedor.SelectedValue == "0" || ListaCompra.Count == 0) return;

            decimal total = 0m;
            foreach (var it in ListaCompra) total += it.Subtotal;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                // insertar cabecera
                SqlCommand cmd = new SqlCommand("INSERT INTO Compras (ProveedorId, Fecha, Total) VALUES (@P, GETDATE(), @T); SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@P", int.Parse(ddlProveedor.SelectedValue));
                cmd.Parameters.AddWithValue("@T", total);
                int compraId = Convert.ToInt32(cmd.ExecuteScalar());

                // insertar detalles y actualizar stock
                foreach (var it in ListaCompra)
                {
                    // insertar detalle CON UnidadMedidaId
                    SqlCommand det = new SqlCommand(@"INSERT INTO CompraDetalles (CompraId, LibroId, UnidadMedidaId, Cantidad, PrecioUnitario)
                                                      VALUES (@C, @L, @U, @Cant, @P)", con);
                    det.Parameters.AddWithValue("@C", compraId);
                    det.Parameters.AddWithValue("@L", it.LibroId);
                    det.Parameters.AddWithValue("@U", it.UnidadMedidaId);
                    det.Parameters.AddWithValue("@Cant", it.Cantidad);
                    det.Parameters.AddWithValue("@P", it.PrecioUnitario);
                    det.ExecuteNonQuery();

                    // actualizar stock en Libros
                    SqlCommand up = new SqlCommand("UPDATE Libros SET StockUnidades = ISNULL(StockUnidades,0) + @Cant WHERE LibroId = @L", con);
                    up.Parameters.AddWithValue("@Cant", it.Cantidad);
                    up.Parameters.AddWithValue("@L", it.LibroId);
                    up.ExecuteNonQuery();
                }
            }

            // limpiar lista y refrescar
            ListaCompra = new List<ItemCompra>();
            ActualizarTabla();
            CargarUltimasCompras();
            ScriptManager.RegisterStartupScript(this, GetType(), "ok", "alert('Compra registrada correctamente.');", true);
        }

        private void CargarUltimasCompras()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT TOP 5
                        CONVERT(varchar(10), C.Fecha, 103) AS Fecha,
                        CONVERT(varchar(5), C.Fecha, 108) AS Hora,
                        P.Nombre AS Proveedor,
                        STUFF((
                            SELECT ', ' + CAST(cd.Cantidad AS varchar(20)) + ' unidades de ' + ISNULL(L2.Titulo, '')
                            FROM CompraDetalles cd
                            LEFT JOIN Libros L2 ON cd.LibroId = L2.LibroId
                            WHERE cd.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Productos,
                        STUFF((
                            SELECT DISTINCT ', ' + ISNULL(Cat.Nombre, '')
                            FROM CompraDetalles cd
                            INNER JOIN Libros L2 ON cd.LibroId = L2.LibroId
                            INNER JOIN Categorias Cat ON L2.CategoriaId = Cat.CategoriaId
                            WHERE cd.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Categorias,
                        STUFF((
                            SELECT DISTINCT ', ' + ISNULL(E.Nombre, '')
                            FROM CompraDetalles cd
                            INNER JOIN Libros L2 ON cd.LibroId = L2.LibroId
                            LEFT JOIN Editoriales E ON L2.EditorialId = E.EditorialId
                            WHERE cd.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Editoriales,
                        CASE 
                            WHEN EXISTS (
                                SELECT 1 FROM CompraDetalles cd
                                INNER JOIN UnidadMedida UM ON cd.UnidadMedidaId = UM.UnidadMedidaId
                                WHERE cd.CompraId = C.CompraId AND UM.DescuentoPorcentaje > 0
                            ) THEN 'Sí'
                            ELSE 'No'
                        END AS Descuentos,
                        C.Total
                    FROM Compras C
                    INNER JOIN Proveedores P ON C.ProveedorId = P.ProveedorId
                    ORDER BY C.Fecha DESC;";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUltimasCompras.DataSource = dt;
                gvUltimasCompras.DataBind();
            }
        }
    }
}