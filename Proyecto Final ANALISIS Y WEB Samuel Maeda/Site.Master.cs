using System;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Si no hay sesión, mandar al login
                if (Session["Usuario"] != null)
                {
                    lblUsuario.Text = Session["Usuario"].ToString();
                }
                else
                {
                    Response.Redirect("Proyecto_Final_Analisis_y_Web.aspx"); // login
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Proyecto_Final_Analisis_y_Web.aspx"); // login
        }
    }
}
