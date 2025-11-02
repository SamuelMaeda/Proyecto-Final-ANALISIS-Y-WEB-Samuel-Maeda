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
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoAplicado { get; set; }

        public decimal SubtotalBruto => Cantidad * PrecioUnitario;
        public decimal SubtotalConDescuento => SubtotalBruto - DescuentoAplicado;
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
                SqlCommand cmd = new SqlCommand("SELECT UnidadMedidaId, Nombre, ISNULL(DescuentoPorcentaje,0) AS DescuentoPorcentaje, ISNULL(CantidadPorUnidad,1) AS CantidadPorUnidad FROM UnidadMedida", con);
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
            if (int.TryParse(ddlEditorial.SelectedValue, out int id) && id > 0)
                CargarLibros(id);
            else
                CargarLibros(0);
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

        protected void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            if (ddlLibro.SelectedValue == "0" || ddlUnidad.SelectedValue == "0" || string.IsNullOrWhiteSpace(txtCantidad.Text)) return;

            int libroId = int.Parse(ddlLibro.SelectedValue);
            int unidadId = int.Parse(ddlUnidad.SelectedValue);
            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidadIngresada) || cantidadIngresada <= 0) return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT L.Titulo,
                           ISNULL(E.Nombre,'(Sin editorial)') AS Editorial,
                           C.Nombre AS Categoria,
                           ISNULL(C.PrecioBase,0) AS PrecioBase,
                           ISNULL(UM.Nombre,'') AS UnidadNombre,
                           ISNULL(UM.CantidadPorUnidad,1) AS CantidadPorUnidad,
                           ISNULL(UM.DescuentoPorcentaje,0) AS DescuentoPorcentaje
                    FROM Libros L
                    INNER JOIN Categorias C ON L.CategoriaId = C.CategoriaId
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
                int cantidadPorUnidad = Convert.ToInt32(dr["CantidadPorUnidad"]);
                decimal precio = Convert.ToDecimal(dr["PrecioBase"]);
                decimal descuentoUnidad = Convert.ToDecimal(dr["DescuentoPorcentaje"]);

                int cantidadTotal = cantidadIngresada * cantidadPorUnidad;
                decimal subtotalBruto = cantidadTotal * precio;
                decimal descuentoAplicado = subtotalBruto * (descuentoUnidad / 100m);

                var lista = ListaCompra;
                var existente = lista.Find(x => x.LibroId == libroId && x.UnidadMedidaId == unidadId);
                if (existente != null)
                {
                    existente.Cantidad += cantidadTotal;
                    existente.DescuentoAplicado += descuentoAplicado;
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
                        UnidadMedidaId = unidadId,
                        Cantidad = cantidadTotal,
                        PrecioUnitario = precio,
                        DescuentoAplicado = descuentoAplicado
                    });
                }
                ListaCompra = lista;
            }

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

            if (ListaCompra.Count == 0)
            {
                lblMensajeVacio.Visible = true;
                gvCompraActual.Visible = false;
                lblSubtotal.Text = "Subtotal sin descuento: Q0.00";
                lblDescuento.Text = "Descuento total aplicado: Q0.00";
                lblSubtotalConDesc.Text = "Subtotal con descuento: Q0.00";
                lblIVA.Text = "IVA (12%): Q0.00";
                lblTotalFinal.Text = "TOTAL FINAL (con IVA): Q0.00";
                lblAhorro.Text = "Ahorro total: Q0.00";
                return;
            }

            lblMensajeVacio.Visible = false;
            gvCompraActual.Visible = true;

            decimal subtotal = 0m, descuentoTotal = 0m;
            foreach (var it in ListaCompra)
            {
                subtotal += it.SubtotalBruto;
                descuentoTotal += it.DescuentoAplicado;
            }

            decimal subtotalConDesc = subtotal - descuentoTotal;
            decimal iva = subtotalConDesc * 0.12m;
            decimal totalFinal = subtotalConDesc + iva;

            lblSubtotal.Text = $"Subtotal sin descuento: Q{subtotal:N2}";
            lblDescuento.Text = $"Descuento total aplicado: Q{descuentoTotal:N2}";
            lblSubtotalConDesc.Text = $"Subtotal con descuento: Q{subtotalConDesc:N2}";
            lblIVA.Text = $"IVA (12%): Q{iva:N2}";
            lblTotalFinal.Text = $"TOTAL FINAL (con IVA): Q{totalFinal:N2}";
            lblAhorro.Text = $"Ahorro total: Q{descuentoTotal:N2}";
        }

        protected void btnLimpiarLista_Click(object sender, EventArgs e)
        {
            ListaCompra = new List<ItemCompra>();
            ActualizarTabla();
        }

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

            decimal subtotal = 0m, descuentoTotal = 0m;
            foreach (var it in ListaCompra)
            {
                subtotal += it.SubtotalBruto;
                descuentoTotal += it.DescuentoAplicado;
            }

            decimal subtotalConDesc = subtotal - descuentoTotal;
            decimal iva = subtotalConDesc * 0.12m;
            decimal totalFinal = subtotalConDesc + iva;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Compras (ProveedorId, Fecha, Total) VALUES (@P, GETDATE(), @T); SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@P", int.Parse(ddlProveedor.SelectedValue));
                cmd.Parameters.AddWithValue("@T", totalFinal);
                int compraId = Convert.ToInt32(cmd.ExecuteScalar());

                foreach (var it in ListaCompra)
                {
                    SqlCommand det = new SqlCommand(@"INSERT INTO CompraDetalles (CompraId, LibroId, UnidadMedidaId, Cantidad, PrecioUnitario)
                                                      VALUES (@C, @L, @U, @Cant, @P)", con);
                    det.Parameters.AddWithValue("@C", compraId);
                    det.Parameters.AddWithValue("@L", it.LibroId);
                    det.Parameters.AddWithValue("@U", it.UnidadMedidaId);
                    det.Parameters.AddWithValue("@Cant", it.Cantidad);
                    det.Parameters.AddWithValue("@P", it.PrecioUnitario);
                    det.ExecuteNonQuery();

                    SqlCommand up = new SqlCommand("UPDATE Libros SET StockUnidades = ISNULL(StockUnidades,0) + @Cant WHERE LibroId = @L", con);
                    up.Parameters.AddWithValue("@Cant", it.Cantidad);
                    up.Parameters.AddWithValue("@L", it.LibroId);
                    up.ExecuteNonQuery();
                }
            }

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
                        CONVERT(varchar(10), C.Fecha, 103) + ' ' + CONVERT(varchar(5), C.Fecha, 108) AS FechaHora,
                        P.Nombre AS Proveedor,
                        STUFF((
                            SELECT ', ' + 
                                   CAST(CASE WHEN UM.CantidadPorUnidad > 0 THEN (cd.Cantidad / UM.CantidadPorUnidad) ELSE cd.Cantidad END AS varchar(10))
                                   + ' ' + LOWER(ISNULL(UM.Nombre,'')) + ' de ' + ISNULL(L2.Titulo,'')
                            FROM CompraDetalles cd
                            LEFT JOIN Libros L2 ON cd.LibroId = L2.LibroId
                            LEFT JOIN UnidadMedida UM ON cd.UnidadMedidaId = UM.UnidadMedidaId
                            WHERE cd.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS LibrosComprados,
                        STUFF((
                            SELECT DISTINCT ', ' + ISNULL(Cat.Nombre,'')
                            FROM CompraDetalles cd
                            INNER JOIN Libros L2 ON cd.LibroId = L2.LibroId
                            INNER JOIN Categorias Cat ON L2.CategoriaId = Cat.CategoriaId
                            WHERE cd.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Categorias,
                        STUFF((
                            SELECT DISTINCT ', ' + ISNULL(E.Nombre,'')
                            FROM CompraDetalles cd
                            INNER JOIN Libros L2 ON cd.LibroId = L2.LibroId
                            LEFT JOIN Editoriales E ON L2.EditorialId = E.EditorialId
                            WHERE cd.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Editoriales,
                        CASE 
                            WHEN SUM(ISNULL(cd.PrecioUnitario,0) * ISNULL(cd.Cantidad,0) * (ISNULL(UM.DescuentoPorcentaje,0)/100.0)) > 0 
                            THEN 'Q' + FORMAT(SUM(cd.PrecioUnitario * cd.Cantidad * (UM.DescuentoPorcentaje/100.0)), 'N2')
                            ELSE 'Sin descuento'
                        END AS Descuentos,
                        (CASE WHEN C.Total IS NOT NULL THEN C.Total ELSE 0 END) AS TotalConIVA
                    FROM Compras C
                    LEFT JOIN CompraDetalles cd ON cd.CompraId = C.CompraId
                    LEFT JOIN UnidadMedida UM ON cd.UnidadMedidaId = UM.UnidadMedidaId
                    INNER JOIN Proveedores P ON C.ProveedorId = P.ProveedorId
                    GROUP BY C.CompraId, C.Fecha, P.Nombre, C.Total
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
