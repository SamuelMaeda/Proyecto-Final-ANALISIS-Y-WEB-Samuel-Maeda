using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        string patron = "LuzDelSaber"; // mismo que usas en login

        protected void btnReset_Click(object sender, EventArgs e)
        {
            string tokenStr = Request.QueryString["token"];
            if (string.IsNullOrEmpty(tokenStr))
            {
                lblMensaje.Text = "Token inválido o ausente.";
                return;
            }

            if (!Guid.TryParse(tokenStr, out Guid token))
            {
                lblMensaje.Text = "Token no válido.";
                return;
            }

            string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SP_ResetPassword", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Token", SqlDbType.UniqueIdentifier).Value = token;
                cmd.Parameters.Add("@NuevaPassword", SqlDbType.VarChar, 50).Value = tbNueva.Text;
                cmd.Parameters.Add("@Patron", SqlDbType.VarChar, 50).Value = patron;

                con.Open();
                int filas = cmd.ExecuteNonQuery();

                if (filas > 0)
                {
                    lblMensaje.Text = "✅ Contraseña actualizada correctamente. Ya puede iniciar sesión.";
                }
                else
                {
                    lblMensaje.Text = "❌ El token es inválido o ha expirado.";
                }
            }
        }
    }
}
