using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class GestionProveedores : Page
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarProveedores();
            }
        }

        private void CargarProveedores(string filtro = "")
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT ProveedorId, Nombre, Contacto
                    FROM Proveedores
                    WHERE (@filtro = '' OR Nombre LIKE '%' + @filtro + '%' OR Contacto LIKE '%' + @filtro + '%')
                    ORDER BY Nombre";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@filtro", filtro);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvProveedores.DataSource = dt;
                gvProveedores.DataBind();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtContacto.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alerta",
                    "alert('Por favor, completa todos los campos.');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "INSERT INTO Proveedores (Nombre, Contacto) VALUES (@Nombre, @Contacto)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@Contacto", txtContacto.Text.Trim());
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            txtNombre.Text = "";
            txtContacto.Text = "";

            CargarProveedores();
            ScriptManager.RegisterStartupScript(this, GetType(), "ok", "alert('Proveedor registrado correctamente.');", true);
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim();
            CargarProveedores(filtro);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            CargarProveedores();
        }

        protected void gvProveedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProveedores.PageIndex = e.NewPageIndex;
            CargarProveedores(txtBuscar.Text.Trim());
        }

        protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int proveedorId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = "DELETE FROM Proveedores WHERE ProveedorId = @ProveedorId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProveedorId", proveedorId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                CargarProveedores(txtBuscar.Text.Trim());
                ScriptManager.RegisterStartupScript(this, GetType(), "elim", "alert('Proveedor eliminado correctamente.');", true);
            }
        }
    }
}
