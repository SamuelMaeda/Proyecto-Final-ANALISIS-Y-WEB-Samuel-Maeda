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
        public decimal PrecioCosto { get; set; }
        public decimal DescuentoAplicado { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal SubtotalConDescuento => Subtotal - DescuentoAplicado;
        public decimal GananciaUnitaria => PrecioUnitario - PrecioCosto;
        public decimal GananciaSubtotal => Cantidad * GananciaUnitaria;
    }

    public partial class Ventas : Page
    {
        private string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
        private bool PrecioVentaEsPorUnidadBase = true;

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
                           ISNULL(E.Nombre,'(Sin editorial)') AS Editorial,
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
                decimal precioUnitario = PrecioVentaEsPorUnidadBase
                    ? precioVenta
                    : (cantidadPorUnidad > 0 ? precioVenta / cantidadPorUnidad : precioVenta);

                // Descuento por unidad
                decimal descuento = 0m;
                if (unidadNombre.ToLower().Contains("caja grande"))
                    descuento = (cantidadTotal * precioUnitario) * 0.25m;
                else if (unidadNombre.ToLower().Contains("caja"))
                    descuento = (cantidadTotal * precioUnitario) * 0.10m;

                var lista = ListaVenta;
                var existente = lista.Find(x => x.Titulo == titulo && x.UnidadNombre == unidadNombre);
                if (existente != null)
                {
                    existente.Cantidad += cantidadTotal;
                    existente.DescuentoAplicado += descuento;
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
                        PrecioUnitario = precioUnitario,
                        PrecioCosto = precioBase,
                        DescuentoAplicado = descuento
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

            if (ListaVenta.Count == 0)
            {
                lblMensajeVacio.Visible = true;
                gvVentaActual.Visible = false;
                lblTotal.Text = "Subtotal sin descuento: Q0.00";
                lblDescuento.Text = "Descuento total aplicado: Q0.00";
                lblSubtotalConDesc.Text = "Subtotal con descuento: Q0.00";
                lblIVA.Text = "IVA (12%): Q0.00";
                lblTotalFinal.Text = "TOTAL FINAL (con IVA): Q0.00";
                lblGananciaTotal.Text = "Ganancia total: Q0.00";
                return;
            }

            lblMensajeVacio.Visible = false;
            gvVentaActual.Visible = true;

            decimal subtotal = 0m, descuentoTotal = 0m, gananciaTotal = 0m;
            foreach (var it in ListaVenta)
            {
                subtotal += it.Subtotal;
                descuentoTotal += it.DescuentoAplicado;
                gananciaTotal += it.GananciaSubtotal;
            }

            decimal subtotalConDesc = subtotal - descuentoTotal;
            decimal iva = subtotalConDesc * 0.12m;
            decimal totalFinal = subtotalConDesc + iva;

            lblTotal.Text = $"Subtotal sin descuento: Q{subtotal:N2}";
            lblDescuento.Text = $"Descuento total aplicado: Q{descuentoTotal:N2}";
            lblSubtotalConDesc.Text = $"Subtotal con descuento: Q{subtotalConDesc:N2}";
            lblIVA.Text = $"IVA (12%): Q{iva:N2}";
            lblTotalFinal.Text = $"TOTAL FINAL (con IVA): Q{totalFinal:N2}";
            lblGananciaTotal.Text = $"Ganancia total: Q{gananciaTotal:N2}";
        }

        protected void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            if (ddlCliente.SelectedValue == "0" || ListaVenta.Count == 0) return;

            decimal subtotal = 0m, descuentoTotal = 0m;
            foreach (var it in ListaVenta)
            {
                subtotal += it.Subtotal;
                descuentoTotal += it.DescuentoAplicado;
            }

            decimal subtotalConDesc = subtotal - descuentoTotal;
            decimal iva = subtotalConDesc * 0.12m;
            decimal totalFinal = subtotalConDesc + iva;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Ventas (ClienteId, Fecha, Total, IVA) VALUES (@C, GETDATE(), @T, @IVA); SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@C", int.Parse(ddlCliente.SelectedValue));
                cmd.Parameters.AddWithValue("@T", totalFinal);
                cmd.Parameters.AddWithValue("@IVA", iva);
                int ventaId = Convert.ToInt32(cmd.ExecuteScalar());

                foreach (var it in ListaVenta)
                {
                    SqlCommand det = new SqlCommand(@"
                        INSERT INTO VentaDetalles (VentaId, LibroId, UnidadMedidaId, Cantidad, PrecioUnitario, DescuentoAplicado)
                        VALUES (@V, @L, @U, @C, @P, @D)", con);
                    det.Parameters.AddWithValue("@V", ventaId);
                    det.Parameters.AddWithValue("@L", it.LibroId);
                    det.Parameters.AddWithValue("@U", it.UnidadMedidaId);
                    det.Parameters.AddWithValue("@C", it.Cantidad);
                    det.Parameters.AddWithValue("@P", it.PrecioUnitario);
                    det.Parameters.AddWithValue("@D", it.DescuentoAplicado);
                    det.ExecuteNonQuery();

                    SqlCommand upd = new SqlCommand("UPDATE Libros SET StockUnidades = ISNULL(StockUnidades,0) - @Cant WHERE LibroId=@L", con);
                    upd.Parameters.AddWithValue("@Cant", it.Cantidad);
                    upd.Parameters.AddWithValue("@L", it.LibroId);
                    upd.ExecuteNonQuery();
                }
            }

            ListaVenta = new List<ItemVenta>();
            ActualizarTabla();
            CargarUltimasVentas();
            ScriptManager.RegisterStartupScript(this, GetType(), "ok", "alert('Venta registrada correctamente con descuentos e IVA.');", true);
        }

        private void CargarUltimasVentas()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
            SELECT TOP 5
                CONVERT(varchar(10), V.Fecha, 103) + ' ' + CONVERT(varchar(5), V.Fecha, 108) AS FechaHora,
                ISNULL(C.Nombre, 'Cliente General') AS Cliente,
                STUFF((
                    SELECT ', ' +
                           CAST(
                               CASE 
                                   WHEN UM.CantidadPorUnidad > 0 
                                   THEN (vd.Cantidad / UM.CantidadPorUnidad) 
                                   ELSE vd.Cantidad 
                               END AS varchar(10)
                           ) + ' ' + LOWER(
                               CASE 
                                   WHEN UM.Nombre LIKE '%(%)%' 
                                       THEN LEFT(UM.Nombre, CHARINDEX('(', UM.Nombre) - 1) 
                                   ELSE UM.Nombre 
                               END
                           ) +
                           CASE 
                               WHEN UM.CantidadPorUnidad > 1 AND UM.Nombre NOT LIKE '%(%)%' 
                                   THEN ' (' + CAST(UM.CantidadPorUnidad AS varchar(10)) + ')' 
                               ELSE '' 
                           END +
                           ' de ' + ISNULL(L2.Titulo, '')
                    FROM VentaDetalles vd
                    LEFT JOIN Libros L2 ON vd.LibroId = L2.LibroId
                    LEFT JOIN UnidadMedida UM ON vd.UnidadMedidaId = UM.UnidadMedidaId
                    WHERE vd.VentaId = V.VentaId
                    FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, ''
                ) AS LibrosVendidos,
                STUFF((
                    SELECT DISTINCT ', ' + ISNULL(Cat.Nombre, '')
                    FROM VentaDetalles vd
                    INNER JOIN Libros L2 ON vd.LibroId = L2.LibroId
                    INNER JOIN Categorias Cat ON L2.CategoriaId = Cat.CategoriaId
                    WHERE vd.VentaId = V.VentaId
                    FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, ''
                ) AS Categorias,
                STUFF((
                    SELECT DISTINCT ', ' + ISNULL(E.Nombre, '')
                    FROM VentaDetalles vd
                    INNER JOIN Libros L2 ON vd.LibroId = L2.LibroId
                    LEFT JOIN Editoriales E ON L2.EditorialId = E.EditorialId
                    WHERE vd.VentaId = V.VentaId
                    FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, ''
                ) AS Editoriales,
                CASE 
                    WHEN SUM(ISNULL(vd.DescuentoAplicado,0)) > 0 
                    THEN CONCAT('Q', FORMAT(SUM(vd.DescuentoAplicado), 'N2')) 
                    ELSE 'Sin descuento'
                END AS DescuentoAplicado,
                V.Total AS TotalConIVA
            FROM Ventas V
            LEFT JOIN Clientes C ON V.ClienteId = C.ClienteId
            LEFT JOIN VentaDetalles vd ON vd.VentaId = V.VentaId
            GROUP BY V.VentaId, V.Fecha, C.Nombre, V.Total
            ORDER BY V.Fecha DESC;";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUltimasVentas.DataSource = dt;
                gvUltimasVentas.DataBind();
            }
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
    }
}
