using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LuzDelSaber
{
    [Serializable]
    public class ItemVenta
    {
        public int LibroId { get; set; }
        public string Titulo { get; set; }
        public string Editorial { get; set; }
        public string Categoria { get; set; }
        public string UnidadNombre { get; set; }
        public int UnidadMedidaId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal GananciaUnitaria => PrecioUnitario - PrecioCosto; // cálculo basado en diferencia real
        public decimal GananciaSubtotal => Cantidad * GananciaUnitaria;
        public decimal PrecioCosto { get; set; } // nuevo: se obtiene del PrecioBase
    }

    public partial class Ventas : Page
    {
        private string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        private List<ItemVenta> ListaVenta
        {
            get => ViewState["ListaVenta"] as List<ItemVenta> ?? new List<ItemVenta>();
            set => ViewState["ListaVenta"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarClientes();
                CargarLibros();
                CargarUnidades();
                CargarUltimasVentas();
                ActualizarTabla();
            }
        }

        private void CargarClientes()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT ClienteId, Nombre FROM Clientes", con);
                con.Open();
                ddlCliente.DataSource = cmd.ExecuteReader();
                ddlCliente.DataTextField = "Nombre";
                ddlCliente.DataValueField = "ClienteId";
                ddlCliente.DataBind();
                ddlCliente.Items.Insert(0, new ListItem("-- Seleccione un cliente --", "0"));
            }
        }

        private void CargarLibros()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT LibroId, Titulo FROM Libros ORDER BY Titulo", con);
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
                SqlCommand cmd = new SqlCommand("SELECT UnidadMedidaId, Nombre FROM UnidadMedida", con);
                con.Open();
                ddlUnidad.DataSource = cmd.ExecuteReader();
                ddlUnidad.DataTextField = "Nombre";
                ddlUnidad.DataValueField = "UnidadMedidaId";
                ddlUnidad.DataBind();
                ddlUnidad.Items.Insert(0, new ListItem("-- Seleccione una unidad --", "0"));
            }
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
            if (ddlLibro.SelectedValue == "0" || ddlUnidad.SelectedValue == "0" || string.IsNullOrWhiteSpace(txtCantidad.Text))
                return;

            int libroId = int.Parse(ddlLibro.SelectedValue);
            int unidadId = int.Parse(ddlUnidad.SelectedValue);
            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidadIngresada) || cantidadIngresada <= 0) return;

            if (!int.TryParse(lblStock.Text, out int stockActual) || cantidadIngresada > stockActual)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "stockError",
                    "alert('Cantidad solicitada excede el stock disponible.');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT L.Titulo,
                           ISNULL(E.Nombre,'') AS Editorial,
                           ISNULL(Cat.Nombre,'') AS Categoria,
                           ISNULL(Cat.PrecioVenta, 0) AS PrecioVenta,
                           ISNULL(Cat.PrecioBase, 0) AS PrecioBase,
                           ISNULL(UM.CantidadPorUnidad, 1) AS CantidadPorUnidad,
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
                decimal precioVenta = Convert.ToDecimal(dr["PrecioVenta"]);
                decimal precioBase = Convert.ToDecimal(dr["PrecioBase"]);
                int cantidadPorUnidad = Convert.ToInt32(dr["CantidadPorUnidad"]);
                int unidadMedidaId = Convert.ToInt32(dr["UnidadMedidaId"]);

                int cantidadTotal = cantidadIngresada * (cantidadPorUnidad > 0 ? cantidadPorUnidad : 1);

                var lista = ListaVenta;
                var existente = lista.Find(x => x.Titulo == titulo && x.Editorial == editorial &&
                                                x.Categoria == categoria && x.UnidadNombre == unidadNombre);
                if (existente != null)
                {
                    existente.Cantidad += cantidadTotal;
                }
                else
                {
                    lista.Add(new ItemVenta
                    {
                        LibroId = libroId,
                        Titulo = titulo,
                        Editorial = editorial,
                        Categoria = categoria,
                        UnidadNombre = unidadNombre,
                        UnidadMedidaId = unidadMedidaId,
                        Cantidad = cantidadTotal,
                        PrecioUnitario = precioVenta,
                        PrecioCosto = precioBase
                    });
                }

                ListaVenta = lista;
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
            gvVentaActual.DataSource = ListaVenta;
            gvVentaActual.DataBind();

            decimal total = 0m;
            decimal gananciaTotal = 0m;
            foreach (var it in ListaVenta)
            {
                total += it.Subtotal;
                gananciaTotal += it.GananciaSubtotal;
            }

            lblTotal.Text = $"Total: Q{total:N2}";
            lblGananciaTotal.Text = $"Ganancia total: Q{gananciaTotal:N2}";

            bool vacio = ListaVenta == null || ListaVenta.Count == 0;
            lblMensajeVacio.Visible = vacio;
            gvVentaActual.Visible = !vacio;
        }

        protected void btnLimpiarLista_Click(object sender, EventArgs e)
        {
            ListaVenta = new List<ItemVenta>();
            ActualizarTabla();
        }

        protected void btnLimpiarTodo_Click(object sender, EventArgs e)
        {
            ddlCliente.SelectedIndex = 0;
            ddlLibro.SelectedIndex = 0;
            ddlUnidad.SelectedIndex = 0;
            txtCantidad.Text = "";
            lblStock.Text = "-";
            txtCategoria.Text = "";
            ListaVenta = new List<ItemVenta>();
            ActualizarTabla();
        }

        protected void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            if (ddlCliente.SelectedValue == "0" || ListaVenta.Count == 0) return;

            decimal total = 0m;
            foreach (var it in ListaVenta)
                total += it.Subtotal;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Ventas (ClienteId, Fecha, Total) VALUES (@C, GETDATE(), @T); SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@C", int.Parse(ddlCliente.SelectedValue));
                cmd.Parameters.AddWithValue("@T", total);
                int ventaId = Convert.ToInt32(cmd.ExecuteScalar());

                foreach (var it in ListaVenta)
                {
                    SqlCommand det = new SqlCommand(@"INSERT INTO VentaDetalles (VentaId, LibroId, UnidadMedidaId, Cantidad, PrecioUnitario)
                                                      VALUES (@V, @L, @U, @Cant, @P)", con);
                    det.Parameters.AddWithValue("@V", ventaId);
                    det.Parameters.AddWithValue("@L", it.LibroId);
                    det.Parameters.AddWithValue("@U", it.UnidadMedidaId);
                    det.Parameters.AddWithValue("@Cant", it.Cantidad);
                    det.Parameters.AddWithValue("@P", it.PrecioUnitario);
                    det.ExecuteNonQuery();

                    SqlCommand up = new SqlCommand("UPDATE Libros SET StockUnidades = ISNULL(StockUnidades,0) - @Cant WHERE LibroId = @L", con);
                    up.Parameters.AddWithValue("@Cant", it.Cantidad);
                    up.Parameters.AddWithValue("@L", it.LibroId);
                    up.ExecuteNonQuery();
                }
            }

            ListaVenta = new List<ItemVenta>();
            ActualizarTabla();
            CargarUltimasVentas();
            ScriptManager.RegisterStartupScript(this, GetType(), "ok", "alert('Venta registrada correctamente.');", true);
        }

        private void CargarUltimasVentas()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT TOP 5
                        CONVERT(varchar(10), V.Fecha, 103) AS Fecha,
                        CONVERT(varchar(5), V.Fecha, 108) AS Hora,
                        ISNULL(C.Nombre, 'Cliente General') AS Cliente,
                        STUFF((SELECT ', ' + CAST(vd.Cantidad AS varchar(20)) + ' unidades de ' + ISNULL(L2.Titulo, '')
                               FROM VentaDetalles vd
                               LEFT JOIN Libros L2 ON vd.LibroId = L2.LibroId
                               WHERE vd.VentaId = V.VentaId
                               FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Productos,
                        STUFF((SELECT DISTINCT ', ' + ISNULL(Cat.Nombre, '')
                               FROM VentaDetalles vd
                               INNER JOIN Libros L2 ON vd.LibroId = L2.LibroId
                               INNER JOIN Categorias Cat ON L2.CategoriaId = Cat.CategoriaId
                               WHERE vd.VentaId = V.VentaId
                               FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Categorias,
                        STUFF((SELECT DISTINCT ', ' + ISNULL(E.Nombre, '')
                               FROM VentaDetalles vd
                               INNER JOIN Libros L2 ON vd.LibroId = L2.LibroId
                               LEFT JOIN Editoriales E ON L2.EditorialId = E.EditorialId
                               WHERE vd.VentaId = V.VentaId
                               FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Editoriales,
                        V.Total,
                        SUM((Cat.PrecioVenta - Cat.PrecioBase) * vd.Cantidad) AS Ganancia
                    FROM Ventas V
                    LEFT JOIN Clientes C ON V.ClienteId = C.ClienteId
                    INNER JOIN VentaDetalles vd ON vd.VentaId = V.VentaId
                    INNER JOIN Libros L2 ON vd.LibroId = L2.LibroId
                    INNER JOIN Categorias Cat ON L2.CategoriaId = Cat.CategoriaId
                    GROUP BY V.VentaId, V.Fecha, V.Total, C.Nombre
                    ORDER BY V.Fecha DESC;";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUltimasVentas.DataSource = dt;
                gvUltimasVentas.DataBind();
            }
        }
    }
}
