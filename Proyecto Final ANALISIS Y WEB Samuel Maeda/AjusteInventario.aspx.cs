using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class AjusteInventario : Page
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarLibros();

                txtDesde.Attributes["placeholder"] = "dd/mm/yy";
                txtHasta.Attributes["placeholder"] = "dd/mm/yy";

                CargarHistorial();
            }
        }

        private void CargarLibros()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT LibroId, Titulo FROM Libros WHERE Activo = 1 ORDER BY Titulo", con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlLibro.DataSource = dt;
                ddlLibro.DataTextField = "Titulo";
                ddlLibro.DataValueField = "LibroId";
                ddlLibro.DataBind();

                ddlLibro.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione un libro --", ""));
            }
        }

        protected void btnAplicar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlLibro.SelectedValue) ||
                string.IsNullOrEmpty(txtCantidad.Text) ||
                string.IsNullOrEmpty(ddlTipo.SelectedValue) ||
                string.IsNullOrEmpty(txtMotivo.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alerta",
                    "alert('Debe seleccionar un libro, tipo de ajuste, cantidad y motivo.');", true);
                return;
            }

            int libroId = int.Parse(ddlLibro.SelectedValue);
            int cantidadIngresada = int.Parse(txtCantidad.Text);
            string tipo = ddlTipo.SelectedValue;
            string motivo = txtMotivo.Text.Trim();
            string usuario = Session["Usuario"]?.ToString() ?? "Desconocido";

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // Obtener cantidad actual
                    SqlCommand cmd = new SqlCommand(
                        "SELECT StockUnidades FROM Libros WHERE LibroId = @LibroId",
                        con, trans);

                    cmd.Parameters.AddWithValue("@LibroId", libroId);

                    object result = cmd.ExecuteScalar();
                    int cantidadAntes = (result == null || result == DBNull.Value)
                        ? 0
                        : Convert.ToInt32(result);

                    // Calcular cantidad nueva
                    int cantidadDespues = cantidadAntes;

                    switch (tipo)
                    {
                        case "Robo":
                        case "Extravio":
                        case "Deterioro":
                            cantidadDespues = Math.Max(0, cantidadAntes - cantidadIngresada);
                            break;

                        case "Ajuste positivo":
                            cantidadDespues = cantidadAntes + cantidadIngresada;
                            break;

                        case "Error de conteo":
                            cantidadDespues = cantidadIngresada; // valor final real
                            break;
                    }

                    // Actualizar inventario
                    cmd = new SqlCommand(
                        "UPDATE Libros SET StockUnidades = @NuevaCantidad WHERE LibroId = @LibroId",
                        con, trans);

                    cmd.Parameters.AddWithValue("@NuevaCantidad", cantidadDespues);
                    cmd.Parameters.AddWithValue("@LibroId", libroId);
                    cmd.ExecuteNonQuery();

                    // Registrar ajuste
                    cmd = new SqlCommand(@"
                        INSERT INTO AjustesInventario
                        (LibroId, CantidadAjustada, TipoAjuste, Motivo, Usuario, CantidadAnterior, CantidadNueva, FechaAjuste)
                        VALUES
                        (@LibroId, @CantidadAjustada, @TipoAjuste, @Motivo, @Usuario, @CantidadAnterior, @CantidadNueva, GETDATE())",
                        con, trans);

                    cmd.Parameters.AddWithValue("@LibroId", libroId);
                    cmd.Parameters.AddWithValue("@CantidadAjustada", cantidadIngresada);
                    cmd.Parameters.AddWithValue("@TipoAjuste", tipo);
                    cmd.Parameters.AddWithValue("@Motivo", motivo);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@CantidadAnterior", cantidadAntes);
                    cmd.Parameters.AddWithValue("@CantidadNueva", cantidadDespues);
                    cmd.ExecuteNonQuery();

                    trans.Commit();

                    ScriptManager.RegisterStartupScript(this, GetType(), "ok",
                        "alert('Ajuste aplicado correctamente.');", true);

                    txtCantidad.Text = "";
                    txtMotivo.Text = "";
                    ddlLibro.SelectedIndex = 0;
                    ddlTipo.SelectedIndex = 0;

                    CargarHistorial();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        $"alert('Error: {ex.Message.Replace("'", "\\'")}');", true);
                }
            }
        }

        private void CargarHistorial()
        {
            DateTime desde = ParseOrDefaultDate(txtDesde.Text, new DateTime(1753, 1, 1));
            DateTime hasta = ParseOrDefaultDate(txtHasta.Text, new DateTime(9999, 12, 31));

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT 
                        CONVERT(VARCHAR(16), A.FechaAjuste, 103) AS FechaAjuste,
                        L.Titulo AS Libro,
                        A.TipoAjuste,
                        A.CantidadAjustada,
                        A.CantidadAnterior,
                        A.CantidadNueva,
                        A.Usuario,
                        A.Motivo
                    FROM AjustesInventario A
                    INNER JOIN Libros L ON A.LibroId = L.LibroId
                    WHERE A.FechaAjuste BETWEEN @desde AND @hasta
                    ORDER BY A.FechaAjuste DESC;";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvAjustes.DataSource = dt;
                gvAjustes.DataBind();
            }
        }

        protected void gvAjustes_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvAjustes.PageIndex = e.NewPageIndex;
            CargarHistorial();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvAjustes.PageIndex = 0;
            CargarHistorial();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDesde.Text = "";
            txtHasta.Text = "";
            gvAjustes.PageIndex = 0;
            CargarHistorial();
        }

        private DateTime ParseOrDefaultDate(string text, DateTime defaultValue)
        {
            if (DateTime.TryParse(text, out DateTime result))
            {
                if (result < new DateTime(1753, 1, 1)) return new DateTime(1753, 1, 1);
                if (result > new DateTime(9999, 12, 31)) return new DateTime(9999, 12, 31);
                return result;
            }
            return defaultValue;
        }
    }
}
