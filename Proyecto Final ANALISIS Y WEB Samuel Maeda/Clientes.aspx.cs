using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class Clientes : Page
    {
        private string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["from"] == "ventas")
                    pnlVolverVentas.Visible = true;
            }
        }

        protected void btnVolverVentas_Click(object sender, EventArgs e)
        {
            Response.Redirect("Venta.aspx");
        }

        private void CargarClientes()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "SELECT Nombre, NIT, Telefono, CorreoElectronico FROM Clientes ORDER BY Nombre";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvClientes.DataSource = dt;
                gvClientes.DataBind();

                lblMensajeVacio.Visible = dt.Rows.Count == 0;
                gvClientes.Visible = dt.Rows.Count > 0;
            }
        }

        protected void btnRegistrarCliente_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string nit = txtNIT.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string correo = txtCorreo.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(nit) ||
                string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(correo))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertaCampos",
                    "alert('Por favor complete todos los campos antes de registrar.');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"INSERT INTO Clientes (Nombre, NIT, Telefono, CorreoElectronico)
                                 VALUES (@Nombre, @NIT, @Telefono, @Correo)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@NIT", nit);
                cmd.Parameters.AddWithValue("@Telefono", telefono);
                cmd.Parameters.AddWithValue("@Correo", correo);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            // Limpiar y recargar tabla
            LimpiarCampos();
            CargarClientes();
            ScriptManager.RegisterStartupScript(this, GetType(), "ok", "alert('Cliente registrado correctamente.');", true);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtNIT.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
        }
    }
}
