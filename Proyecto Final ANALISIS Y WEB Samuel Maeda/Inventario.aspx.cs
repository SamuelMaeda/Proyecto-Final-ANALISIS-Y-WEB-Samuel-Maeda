using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace LuzDelSaber
{
    public partial class Inventario : System.Web.UI.Page
    {
        string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarInventario("LibroId", "ASC");
        }

        private void CargarInventario(string campoOrden, string tipoOrden, string filtro = "")
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT 
                        L.LibroId,
                        L.Titulo,
                        L.Autor,
                        E.Nombre AS Editorial,
                        C.Nombre AS Categoria,
                        C.PrecioBase,
                        L.StockUnidades,
                        L.Descripcion
                    FROM Libros L
                    INNER JOIN Categorias C ON L.CategoriaId = C.CategoriaId
                    LEFT JOIN Editoriales E ON L.EditorialId = E.EditorialId
                    WHERE 
                        (@Filtro = '' OR 
                         L.Titulo LIKE '%' + @Filtro + '%' OR
                         L.Autor LIKE '%' + @Filtro + '%' OR
                         E.Nombre LIKE '%' + @Filtro + '%' OR
                         C.Nombre LIKE '%' + @Filtro + '%')
                    ORDER BY " + campoOrden + " " + tipoOrden;

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@Filtro", filtro ?? "");
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvInventario.DataSource = dt;
                gvInventario.DataBind();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string campoOrden = ddlOrdenarPor.SelectedValue;
            string tipoOrden = ddlOrden.SelectedValue;
            string filtro = txtBuscar.Text.Trim();
            CargarInventario(campoOrden, tipoOrden, filtro);
        }

        protected void btnReiniciar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            ddlOrdenarPor.SelectedIndex = 0;
            ddlOrden.SelectedIndex = 0;
            CargarInventario("LibroId", "ASC");
        }

        protected void gvInventario_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInventario.PageIndex = e.NewPageIndex;
            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }
    }
}