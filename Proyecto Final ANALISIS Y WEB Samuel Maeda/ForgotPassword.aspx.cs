using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        string patron = "LuzDelSaber"; // mismo patrón que usas en login

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SP_GenerarTokenRecuperacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 100).Value = tbCorreo.Text;

                SqlParameter output = new SqlParameter("@Token", SqlDbType.UniqueIdentifier)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(output);

                con.Open();
                cmd.ExecuteNonQuery();

                if (output.Value != DBNull.Value)
                {
                    Guid token = (Guid)output.Value;

                    // En producción se enviaría un correo. Por ahora lo mostramos en pantalla.
                    lblMensaje.Text = "Enlace de recuperación: " +
                        "https://localhost:44381/ResetPassword.aspx?token=" + token;
                }
                else
                {
                    lblMensaje.Text = "El correo no está registrado.";
                }
            }
        }
    }
}
