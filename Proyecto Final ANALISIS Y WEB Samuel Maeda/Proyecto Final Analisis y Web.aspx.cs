using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class Proyecto_Final_Analisis_y_Web : Page
    {
        string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Mostrar mensaje si viene de cierre de sesión o acceso no autorizado
            if (Session["MensajeLogin"] != null)
            {
                lblError.Text = Session["MensajeLogin"].ToString();
                Session["MensajeLogin"] = null; // limpiar después de mostrar
            }

            // Bloquear botón atrás
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        }

        protected void BtnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = tbUsuario.Text.Trim();
            string contrasenia = tbPassword.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasenia))
            {
                lblError.Text = "Por favor ingresa tu usuario y contraseña.";
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SP_ValidarUsuario", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Contrasenia", contrasenia);
                    cmd.Parameters.AddWithValue("@Patron", "LuzDelSaber");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    con.Open();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        Session["Usuario"] = dt.Rows[0]["Usuario"].ToString();

                        // ✅ Guardar el rol del usuario en sesión (por defecto "Cajero" si no hay campo)
                        if (dt.Columns.Contains("Rol"))
                            Session["Rol"] = dt.Rows[0]["Rol"].ToString();
                        else
                            Session["Rol"] = "Cajero"; // fallback de seguridad

                        Response.Redirect("Index.aspx");
                    }
                    else
                    {
                        lblError.Text = "Usuario o contraseña incorrectos.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error al conectar con la base de datos: " + ex.Message;
            }
        }

    }
}
