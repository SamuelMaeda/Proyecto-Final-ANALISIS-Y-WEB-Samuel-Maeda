using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class Ventas : Page
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarClientes();
                CargarLibros();
                CargarUnidades();
                CargarUltimasVentas();
                lblMensaje.Text = "";
            }
        }

        // -------------------- CARGAS --------------------
        private void CargarClientes()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT ClienteId, Nombre FROM Clientes ORDER BY Nombre", con);
                con.Open();
                ddlCliente.DataSource = cmd.ExecuteReader();
                ddlCliente.DataTextField = "Nombre";
                ddlCliente.DataValueField = "ClienteId";
                ddlCliente.DataBind();
                ddlCliente.Items.Insert(0, new ListItem("-- Seleccione un cliente --", "0"));
            }
        }
        protected void btnAgregarCliente_Click(object sender, EventArgs e)
        {
            // Redirige al formulario de clientes (si quieres abrir en otra pestaña, cambia aquí)
            Response.Redirect("Clientes.aspx?from=ventas");
        }

        private void CargarLibros()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT LibroId, Titulo FROM Libros WHERE Activo = 1 ORDER BY Titulo", con);
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
                SqlCommand cmd = new SqlCommand("SELECT UnidadMedidaId, Nombre FROM UnidadMedida ORDER BY UnidadMedidaId", con);
                con.Open();
                ddlUnidad.DataSource = cmd.ExecuteReader();
                ddlUnidad.DataTextField = "Nombre";
                ddlUnidad.DataValueField = "UnidadMedidaId";
                ddlUnidad.DataBind();
                ddlUnidad.Items.Insert(0, new ListItem("-- Seleccione una unidad --", "0"));
            }
        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e) { }

        protected void ddlLibro_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCategoria.Text = "";
            lblStock.Text = "-";
            if (ddlLibro.SelectedValue == "0") return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string q = @"SELECT ISNULL(C.Nombre,'') AS Categoria, ISNULL(L.StockUnidades,0) AS Stock
                             FROM Libros L LEFT JOIN Categorias C ON L.CategoriaId = C.CategoriaId
                             WHERE L.LibroId = @id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", ddlLibro.SelectedValue);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtCategoria.Text = dr["Categoria"].ToString();
                    lblStock.Text = dr["Stock"].ToString();
                }
            }
        }

        // -------------------- AGREGAR LIBRO --------------------
        protected void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            if (ddlLibro.SelectedValue == "0" || ddlUnidad.SelectedValue == "0" || string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Seleccione libro, unidad y cantidad.');", true);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidadIngresada) || cantidadIngresada <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Cantidad inválida.');", true);
                return;
            }

            DataTable dt = Session["VentaActual"] as DataTable ?? CrearEstructuraTablaVenta();

            int libroId = int.Parse(ddlLibro.SelectedValue);
            int unidadId = int.Parse(ddlUnidad.SelectedValue);
            string unidadNombre = ddlUnidad.SelectedItem.Text;
            decimal precioUnitario = ObtenerPrecioVenta(libroId);
            string editorial = ObtenerEditorial(libroId);

            int cantidadPorUnidad = 1;
            decimal descuentoPorcentaje = 0;
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT CantidadPorUnidad, DescuentoPorcentaje FROM UnidadMedida WHERE UnidadMedidaId = @id", con);
                cmd.Parameters.AddWithValue("@id", unidadId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    cantidadPorUnidad = dr["CantidadPorUnidad"] != DBNull.Value ? Convert.ToInt32(dr["CantidadPorUnidad"]) : 1;
                    descuentoPorcentaje = dr["DescuentoPorcentaje"] != DBNull.Value ? Convert.ToDecimal(dr["DescuentoPorcentaje"]) : 0m;
                }
            }

            int cantidadTotalUnidades = cantidadIngresada * cantidadPorUnidad;
            decimal subtotal = precioUnitario * cantidadTotalUnidades;
            decimal descuentoAplicado = Math.Round(subtotal * (descuentoPorcentaje / 100m), 2);
            decimal subtotalConDesc = subtotal - descuentoAplicado;

            DataRow row = dt.NewRow();
            row["LibroId"] = libroId;
            row["Titulo"] = ddlLibro.SelectedItem.Text;
            row["Editorial"] = editorial;
            row["Categoria"] = txtCategoria.Text;
            row["UnidadNombre"] = unidadNombre;
            row["UnidadMedidaId"] = unidadId;
            row["Cantidad"] = cantidadIngresada;
            row["PrecioUnitario"] = precioUnitario;
            row["DescuentoAplicado"] = descuentoAplicado;
            row["Subtotal"] = subtotal;
            row["SubtotalConDescuento"] = subtotalConDesc;
            dt.Rows.Add(row);

            Session["VentaActual"] = dt;
            gvVentaActual.DataSource = dt;
            gvVentaActual.DataBind();
            lblMensajeVacio.Visible = (dt.Rows.Count == 0);
            CalcularTotales(dt);
        }

        private DataTable CrearEstructuraTablaVenta()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LibroId", typeof(int));
            dt.Columns.Add("Titulo", typeof(string));
            dt.Columns.Add("Editorial", typeof(string));
            dt.Columns.Add("Categoria", typeof(string));
            dt.Columns.Add("UnidadNombre", typeof(string));
            dt.Columns.Add("UnidadMedidaId", typeof(int));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("PrecioUnitario", typeof(decimal));
            dt.Columns.Add("DescuentoAplicado", typeof(decimal));
            dt.Columns.Add("Subtotal", typeof(decimal));
            dt.Columns.Add("SubtotalConDescuento", typeof(decimal));
            return dt;
        }

        // -------------------- CALCULOS --------------------
        private void CalcularTotales(DataTable dt)
        {
            decimal subtotal = 0m, descuentoTotal = 0m, gananciaTotal = 0m;
            foreach (DataRow r in dt.Rows)
            {
                subtotal += Convert.ToDecimal(r["Subtotal"]);
                descuentoTotal += Convert.ToDecimal(r["DescuentoAplicado"]);
                gananciaTotal += Convert.ToDecimal(r["SubtotalConDescuento"]) * 0.25m;
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

        // -------------------- REGISTRAR VENTA --------------------
        protected void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            if (ddlCliente.SelectedValue == "0" || Session["VentaActual"] == null)
            {
                lblMensaje.Text = "Seleccione un cliente y agregue al menos un libro.";
                lblMensaje.CssClass = "text-danger";
                return;
            }

            DataTable dt = Session["VentaActual"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                lblMensaje.Text = "Agregue al menos un libro a la venta.";
                lblMensaje.CssClass = "text-danger";
                return;
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    decimal subtotal = 0m, descuentoTotal = 0m;
                    foreach (DataRow r in dt.Rows)
                    {
                        subtotal += Convert.ToDecimal(r["Subtotal"]);
                        descuentoTotal += Convert.ToDecimal(r["DescuentoAplicado"]);
                    }

                    decimal subtotalConDesc = subtotal - descuentoTotal;
                    decimal iva = Math.Round(subtotalConDesc * 0.12m, 2);
                    decimal totalFinal = subtotalConDesc + iva;

                    int ventaId;
                    using (SqlCommand cmdVenta = new SqlCommand(
                        "INSERT INTO Ventas (ClienteId, Fecha, Total, IVA, Ganancia) OUTPUT INSERTED.VentaId VALUES (@C, @F, @T, @IVA, @G)", con, tran))
                    {
                        cmdVenta.Parameters.AddWithValue("@C", int.Parse(ddlCliente.SelectedValue));
                        cmdVenta.Parameters.AddWithValue("@F", DateTime.Now);
                        cmdVenta.Parameters.AddWithValue("@T", totalFinal);
                        cmdVenta.Parameters.AddWithValue("@IVA", iva);
                        cmdVenta.Parameters.AddWithValue("@G", 0m);
                        ventaId = Convert.ToInt32(cmdVenta.ExecuteScalar());
                    }

                    foreach (DataRow r in dt.Rows)
                    {
                        int libroId = Convert.ToInt32(r["LibroId"]);
                        int unidadMedidaId = Convert.ToInt32(r["UnidadMedidaId"]);
                        int cantidad = Convert.ToInt32(r["Cantidad"]);
                        decimal precioUnitario = Convert.ToDecimal(r["PrecioUnitario"]);
                        decimal subtotalLinea = Convert.ToDecimal(r["Subtotal"]);
                        decimal descuentoLinea = Convert.ToDecimal(r["DescuentoAplicado"]);
                        decimal subtotalFinal = Convert.ToDecimal(r["SubtotalConDescuento"]);

                        // Obtener cantidad real por unidad (1, 6, 12, etc.)
                        int cantidadPorUnidad = ObtenerCantidadPorUnidad(unidadMedidaId);
                        int cantidadReal = cantidad * cantidadPorUnidad;

                        using (SqlCommand det = new SqlCommand(@"
                    INSERT INTO VentaDetalles (VentaId, LibroId, UnidadMedidaId, Cantidad, PrecioUnitario, DescuentoAplicado, Subtotal, SubtotalFinal)
                    VALUES (@V, @L, @U, @C, @P, @D, @S, @SF)", con, tran))
                        {
                            det.Parameters.AddWithValue("@V", ventaId);
                            det.Parameters.AddWithValue("@L", libroId);
                            det.Parameters.AddWithValue("@U", unidadMedidaId);
                            det.Parameters.AddWithValue("@C", cantidad);
                            det.Parameters.AddWithValue("@P", precioUnitario);
                            det.Parameters.AddWithValue("@D", descuentoLinea);
                            det.Parameters.AddWithValue("@S", subtotalLinea);
                            det.Parameters.AddWithValue("@SF", subtotalFinal);
                            det.ExecuteNonQuery();
                        }

                        // Restar del stock la cantidad real
                        using (SqlCommand upd = new SqlCommand(
                            "UPDATE Libros SET StockUnidades = ISNULL(StockUnidades,0) - @q WHERE LibroId = @L", con, tran))
                        {
                            upd.Parameters.AddWithValue("@q", cantidadReal);
                            upd.Parameters.AddWithValue("@L", libroId);
                            upd.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();

                    // Llamada consistente al método PDF: dt, ventaId, total, iva, subtotal, descuentos
                    GenerarFacturaPDF(dt, ventaId, totalFinal, iva, subtotal, descuentoTotal);

                    Session["VentaActual"] = null;
                    gvVentaActual.DataSource = null;
                    gvVentaActual.DataBind();

                    lblMensaje.Text = "✅ Venta registrada y factura generada correctamente.";
                    lblMensaje.CssClass = "text-success";
                    CargarUltimasVentas();
                }
                catch (Exception ex)
                {
                    try { tran.Rollback(); } catch { }
                    lblMensaje.Text = "❌ Error al registrar la venta: " + ex.Message;
                    lblMensaje.CssClass = "text-danger";
                }
            }
        }

        //Parte de REGISTRAR VENTA 
        private int ObtenerCantidadPorUnidad(int unidadMedidaId)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(CantidadPorUnidad, 1) FROM UnidadMedida WHERE UnidadMedidaId = @id", con);
                cmd.Parameters.AddWithValue("@id", unidadMedidaId);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        // -------------------- FACTURA PDF (PDFsharp) --------------------
        private void GenerarFacturaPDF(DataTable dt, int ventaId, decimal total, decimal iva, decimal subtotal, decimal descuentos)
        {
            try
            {
                string facturasPath = Server.MapPath("~/Facturas/");
                if (!Directory.Exists(facturasPath))
                    Directory.CreateDirectory(facturasPath);

                string pdfPath = Path.Combine(facturasPath, $"Factura_{ventaId}.pdf");

                PdfDocument pdf = new PdfDocument();
                pdf.Info.Title = $"Factura #{ventaId} - Librería Luz del Saber";

                PdfPage page = pdf.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont fontTitulo = new XFont("Arial", 16, XFontStyle.Bold);
                XFont fontSub = new XFont("Arial", 12, XFontStyle.Bold);
                XFont fontNormal = new XFont("Arial", 10, XFontStyle.Regular);
                XFont fontSmall = new XFont("Arial", 9, XFontStyle.Regular);

                double y = 40;

                // LOGO (si existe)
                string logoPath = Server.MapPath("~/Images/LuzDelSaberLOGO.jpg");
                if (File.Exists(logoPath))
                {
                    XImage logo = XImage.FromFile(logoPath);
                    gfx.DrawImage(logo, 40, y, 100, 80);
                }

                // DATOS EMPRESA
                gfx.DrawString("Librería Luz del Saber", fontTitulo, XBrushes.DarkBlue, new XRect(160, y + 10, page.Width, 30), XStringFormats.TopLeft);
                y += 35;
                gfx.DrawString("Del Estadio Sur 2-48, Zona 2 y 4ta. Calle Final,", fontNormal, XBrushes.Black, new XRect(160, y, page.Width, 20), XStringFormats.TopLeft);
                y += 15;
                gfx.DrawString("Col. San Bartolomé, Mazatenango", fontNormal, XBrushes.Black, new XRect(160, y, page.Width, 20), XStringFormats.TopLeft);

                // Espacio extra para evitar choque visual con logo
                y += 60;

                // INFORMACIÓN DE FACTURA
                gfx.DrawString($"Factura N°: {ventaId}", fontSub, XBrushes.Black, new XRect(40, y, 250, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", fontNormal, XBrushes.Black, new XRect(350, y, 200, 20), XStringFormats.TopLeft);
                y += 20;
                gfx.DrawString($"Cliente: {ddlCliente.SelectedItem.Text}", fontNormal, XBrushes.Black, new XRect(40, y, 400, 20), XStringFormats.TopLeft);
                y += 15;
                gfx.DrawString($"NIT: {ObtenerNITCliente(ddlCliente.SelectedValue)}", fontNormal, XBrushes.Black, new XRect(40, y, 400, 20), XStringFormats.TopLeft);
                y += 15;

                string atendidoPor = Session["Usuario"] != null ? Session["Usuario"].ToString() : "No registrado";
                gfx.DrawString($"Atendido por: {atendidoPor}", fontNormal, XBrushes.Black, new XRect(40, y, 400, 20), XStringFormats.TopLeft);
                y += 25;

                // Separador
                gfx.DrawLine(XPens.Black, 40, y, page.Width - 40, y);
                y += 15;

                // ENCABEZADOS DE TABLA
                gfx.DrawString("Título", fontSub, XBrushes.DarkBlue, new XRect(40, y, 150, 20), XStringFormats.TopLeft);
                gfx.DrawString("Unidad", fontSub, XBrushes.DarkBlue, new XRect(190, y, 70, 20), XStringFormats.TopLeft);
                gfx.DrawString("Cant.", fontSub, XBrushes.DarkBlue, new XRect(260, y, 50, 20), XStringFormats.TopLeft);
                gfx.DrawString("P. Unitario", fontSub, XBrushes.DarkBlue, new XRect(310, y, 80, 20), XStringFormats.TopLeft);
                gfx.DrawString("Subtotal", fontSub, XBrushes.DarkBlue, new XRect(400, y, 100, 20), XStringFormats.TopLeft);
                y += 20;

                gfx.DrawLine(XPens.Gray, 40, y, page.Width - 40, y);
                y += 10;

                // DETALLES DE LIBROS
                foreach (DataRow r in dt.Rows)
                {
                    gfx.DrawString(r["Titulo"].ToString(), fontSmall, XBrushes.Black, new XRect(40, y, 150, 20), XStringFormats.TopLeft);
                    gfx.DrawString(r["UnidadNombre"].ToString(), fontSmall, XBrushes.Black, new XRect(190, y, 70, 20), XStringFormats.TopLeft);
                    gfx.DrawString(r["Cantidad"].ToString(), fontSmall, XBrushes.Black, new XRect(260, y, 50, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"Q{Convert.ToDecimal(r["PrecioUnitario"]):N2}", fontSmall, XBrushes.Black, new XRect(310, y, 80, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"Q{Convert.ToDecimal(r["SubtotalConDescuento"]):N2}", fontSmall, XBrushes.Black, new XRect(400, y, 100, 20), XStringFormats.TopLeft);
                    y += 18;

                    if (y > page.Height - 120)
                    {
                        page = pdf.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }

                y += 10;
                gfx.DrawLine(XPens.Black, 40, y, page.Width - 40, y);
                y += 20;

                // TOTALES (formato claro)
                decimal subtotalConDesc = subtotal - descuentos;
                gfx.DrawString($"Subtotal sin descuento: Q{subtotal:N2}", fontNormal, XBrushes.Black, new XRect(320, y, 250, 20), XStringFormats.TopLeft); y += 15;
                gfx.DrawString($"Descuento total aplicado: Q{descuentos:N2}", fontNormal, XBrushes.Black, new XRect(320, y, 250, 20), XStringFormats.TopLeft); y += 15;
                gfx.DrawString($"Subtotal con descuento: Q{subtotalConDesc:N2}", fontNormal, XBrushes.Black, new XRect(320, y, 250, 20), XStringFormats.TopLeft); y += 15;
                gfx.DrawString($"IVA (12%): Q{iva:N2}", fontNormal, XBrushes.Black, new XRect(320, y, 250, 20), XStringFormats.TopLeft); y += 15;
                gfx.DrawString($"TOTAL FINAL (con IVA): Q{total:N2}", fontSub, XBrushes.DarkBlue, new XRect(320, y, 250, 20), XStringFormats.TopLeft);
                y += 30;

                gfx.DrawLine(XPens.Gray, 40, y, page.Width - 40, y);
                y += 15;
                gfx.DrawString("Gracias por su compra en Librería Luz del Saber", fontNormal, XBrushes.DarkBlue, new XRect(40, y, page.Width, 20), XStringFormats.TopLeft);
                y += 12;
                gfx.DrawString("Tel: (502) 5555-1234  •  luzdelsaber@gmail.com", fontNormal, XBrushes.Black, new XRect(40, y, page.Width, 20), XStringFormats.TopLeft);

                pdf.Save(pdfPath);

                pnlFactura.Visible = true;
                lblNoFactura.Visible = false;
                lnkDescargarFactura.NavigateUrl = "~/Facturas/Factura_" + ventaId + ".pdf";
                lnkDescargarFactura.Text = "📄 Descargar Factura PDF";
                lnkDescargarFactura.Visible = true;
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "❌ Error al generar la factura: " + ex.Message;
                lblMensaje.CssClass = "text-danger";
            }
        }

        // -------------------- UTILIDADES --------------------
        private string ObtenerNITCliente(string clienteId)
        {
            if (clienteId == "0") return "CF";
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(NIT,'CF') FROM Clientes WHERE ClienteId=@id", con);
                cmd.Parameters.AddWithValue("@id", clienteId);
                con.Open();
                return cmd.ExecuteScalar()?.ToString() ?? "CF";
            }
        }

        private decimal ObtenerPrecioVenta(int libroId)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(PrecioUnitario,0) FROM Libros WHERE LibroId=@id", con);
                cmd.Parameters.AddWithValue("@id", libroId);
                con.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        private string ObtenerEditorial(int libroId)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(E.Nombre,'Sin editorial') FROM Libros L LEFT JOIN Editoriales E ON L.EditorialId = E.EditorialId WHERE L.LibroId=@id", con);
                cmd.Parameters.AddWithValue("@id", libroId);
                con.Open();
                return cmd.ExecuteScalar()?.ToString() ?? "Sin editorial";
            }
        }

        private void CargarUltimasVentas()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string q = @"
        SELECT TOP 5
            CONVERT(VARCHAR(16), V.Fecha, 120) AS Fecha,
            ISNULL(C.Nombre,'Cliente General') AS Cliente,

            STUFF((
                SELECT ', ' +
                       CAST(VD.Cantidad AS NVARCHAR(10)) + ' ' +
                       CASE 
                         WHEN CHARINDEX('(', UM.Nombre) > 0 THEN LOWER(UM.Nombre)
                         WHEN RIGHT(UM.Nombre, 1) IN ('s','S') THEN LOWER(UM.Nombre)
                         WHEN VD.Cantidad > 1 THEN LOWER(UM.Nombre) + 's'
                         ELSE LOWER(UM.Nombre)
                       END
                       + ' de ' + L.Titulo
                FROM VentaDetalles VD
                INNER JOIN Libros L ON VD.LibroId = L.LibroId
                INNER JOIN UnidadMedida UM ON VD.UnidadMedidaId = UM.UnidadMedidaId
                WHERE VD.VentaId = V.VentaId
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS LibrosVendidos,

            STUFF((
                SELECT DISTINCT ', ' + ISNULL(CAT.Nombre, 'Sin categoría')
                FROM VentaDetalles VD
                INNER JOIN Libros L ON VD.LibroId = L.LibroId
                LEFT JOIN Categorias CAT ON L.CategoriaId = CAT.CategoriaId
                WHERE VD.VentaId = V.VentaId
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,2,'') AS Categoria,

            STUFF((
                SELECT DISTINCT ', ' + ISNULL(E.Nombre, 'Sin editorial')
                FROM VentaDetalles VD
                INNER JOIN Libros L ON VD.LibroId = L.LibroId
                LEFT JOIN Editoriales E ON L.EditorialId = E.EditorialId
                WHERE VD.VentaId = V.VentaId
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,2,'') AS Editorial,

            ISNULL(SUM(VD.DescuentoAplicado), 0) AS DescuentoAplicado,
            ISNULL(V.Total, 0) AS TotalConIVA
        FROM Ventas V
        LEFT JOIN Clientes C ON V.ClienteId = C.ClienteId
        LEFT JOIN VentaDetalles VD ON V.VentaId = VD.VentaId
        GROUP BY V.VentaId, V.Fecha, C.Nombre, V.Total
        ORDER BY V.Fecha DESC;
        ";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUltimasVentas.DataSource = dt;
                gvUltimasVentas.DataBind();
            }
        }

        protected void btnLimpiarLista_Click(object sender, EventArgs e)
        {
            Session["VentaActual"] = null;
            gvVentaActual.DataSource = null;
            gvVentaActual.DataBind();
            lblMensajeVacio.Visible = true;
        }

        protected void btnLimpiarTodo_Click(object sender, EventArgs e)
        {
            ddlCliente.SelectedIndex = 0;
            ddlLibro.SelectedIndex = 0;
            ddlUnidad.SelectedIndex = 0;
            txtCantidad.Text = "";
            lblStock.Text = "-";
            txtCategoria.Text = "";
            Session["VentaActual"] = null;
            gvVentaActual.DataSource = null;
            gvVentaActual.DataBind();
            lblMensajeVacio.Visible = true;
            lblMensaje.Text = "";
        }
    }
}
