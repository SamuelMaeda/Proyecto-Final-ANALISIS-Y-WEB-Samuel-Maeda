using System;
using System.Web.UI;

namespace LuzDelSaber
{
    public partial class Index : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Usuario"] == null || Session["Rol"] == null)
                {
                    Response.Redirect("Proyecto Final Analisis y Web.aspx");
                    return;
                }

                lblUsuario.Text = Session["Usuario"].ToString();
                lblRol.Text = Session["Rol"].ToString();

                AplicarPermisosPorRol(lblRol.Text);
            }
        }

        private void AplicarPermisosPorRol(string rol)
        {
            if (rol.Equals("Gerente", StringComparison.OrdinalIgnoreCase))
            {
                // El gerente puede ver todo
                panelLibros.Visible = true;
                panelCompras.Visible = true;
                panelVentas.Visible = true;
                panelReportes.Visible = true;
                panelInventario.Visible = true;
                panelClientes.Visible = true;
            }
            else if (rol.Equals("Cajero", StringComparison.OrdinalIgnoreCase))
            {
                // El cajero solo tiene acceso limitado
                panelLibros.Visible = false;   // No puede ver ni editar libros
                panelCompras.Visible = false;  // No gestiona compras
                panelVentas.Visible = true;
                panelReportes.Visible = true;
                panelInventario.Visible = true;
                panelClientes.Visible = true;
            }
            else
            {
                // Rol desconocido — restringir todo
                panelLibros.Visible = false;
                panelCompras.Visible = false;
                panelVentas.Visible = false;
                panelReportes.Visible = false;
                panelInventario.Visible = false;
                panelClientes.Visible = false;
            }
        }
    }
}
