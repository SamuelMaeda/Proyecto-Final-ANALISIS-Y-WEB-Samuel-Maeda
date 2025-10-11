using System;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verifica sesión activa
            if (Session["Usuario"] == null)
            {
                // Guarda mensaje de aviso para mostrar en el login
                Session["MensajeLogin"] = "Tu sesión ha finalizado o debes iniciar sesión primero.";
                Response.Redirect("Proyecto Final Analisis y Web.aspx");
            }
            else
            {
                lblUsuario.Text = Session["Usuario"].ToString();
            }

            // Bloquea el botón "Atrás" del navegador
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Limpia sesión y borra caché
            Session.Clear();
            Session.Abandon();

            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            // Envía mensaje al login
            Session["MensajeLogin"] = "Has cerrado sesión correctamente.";

            // Redirige al login
            Response.Redirect("Proyecto Final Analisis y Web.aspx");
        }
    }
}
