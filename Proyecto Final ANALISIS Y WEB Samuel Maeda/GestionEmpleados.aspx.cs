using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class GestionEmpleados : Page
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Solo permitir acceso si es gerente
                if (Session["Rol"] == null || !Session["Rol"].ToString().Equals("Gerente", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("Index.aspx");
                    return;
                }

                CargarEmpleados();
            }
        }

        private void CargarEmpleados(string filtro = "", string rol = "")
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT id, Usuario, Correo, Rol 
                    FROM Usuarios
                    WHERE (@Filtro = '' OR Usuario LIKE '%' + @Filtro + '%' OR Correo LIKE '%' + @Filtro + '%')
                      AND (@Rol = '' OR Rol = @Rol)
                    ORDER BY id DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Filtro", filtro);
                cmd.Parameters.AddWithValue("@Rol", rol);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvEmpleados.DataSource = dt;
                gvEmpleados.DataBind();
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(ddlRol.SelectedValue) ||
                string.IsNullOrWhiteSpace(txtContrasenia.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alerta",
                    "alert('Debe completar todos los campos antes de agregar un empleado.');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                string query = @"
                    INSERT INTO Usuarios (Usuario, Contrasenia, Correo, Rol)
                    VALUES (@Usuario, HASHBYTES('SHA2_256', @Contrasenia), @Correo, @Rol)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Usuario", txtUsuario.Text.Trim());
                cmd.Parameters.AddWithValue("@Contrasenia", txtContrasenia.Text.Trim());
                cmd.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());
                cmd.Parameters.AddWithValue("@Rol", ddlRol.SelectedValue);
                cmd.ExecuteNonQuery();
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "ok", "alert('Empleado agregado correctamente.');", true);
            LimpiarCampos();
            CargarEmpleados();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim();
            string rol = ddlFiltroRol.SelectedValue;
            CargarEmpleados(filtro, rol);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            ddlFiltroRol.SelectedIndex = 0;
            CargarEmpleados();
        }

        protected void gvEmpleados_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int id = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Usuarios WHERE id = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "ok", "alert('Empleado eliminado correctamente.');", true);
                CargarEmpleados();
            }
        }

        protected void gvEmpleados_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvEmpleados.PageIndex = e.NewPageIndex;
            CargarEmpleados(txtBuscar.Text.Trim(), ddlFiltroRol.SelectedValue);
        }

        private void LimpiarCampos()
        {
            txtUsuario.Text = "";
            txtCorreo.Text = "";
            txtContrasenia.Text = "";
            ddlRol.SelectedIndex = 0;
        }
    }
}
