using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LuzDelSaber
{
    public partial class Inventario : Page
    {
        string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarInventario("L.LibroId", "ASC");
        }

        private void CargarInventario(string campoOrden, string tipoOrden, string filtro = "")
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = $@"
                    SELECT 
                        L.LibroId,
                        L.Titulo,
                        ISNULL(E.Nombre, 'Sin Editorial') AS Editorial,
                        ISNULL(C.Nombre, 'Sin Categoría') AS Categoria,
                        ISNULL(C.PrecioBase, 0) AS PrecioVenta,
                        ISNULL(L.StockUnidades, 0) AS StockUnidades
                    FROM Libros L
                    INNER JOIN Categorias C ON L.CategoriaId = C.CategoriaId
                    LEFT JOIN Editoriales E ON L.EditorialId = E.EditorialId
                    WHERE 
                        (@Filtro = '' OR 
                         L.Titulo LIKE '%' + @Filtro + '%' OR
                         E.Nombre LIKE '%' + @Filtro + '%' OR
                         C.Nombre LIKE '%' + @Filtro + '%')
                    ORDER BY {campoOrden} {tipoOrden}";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@Filtro", filtro ?? "");
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvLibros.DataSource = dt;
                gvLibros.DataBind();

                lblTotalLibros.Text = $"Total de libros: {dt.Rows.Count}";
            }
        }

        public string ObtenerColorStock(object stockObj)
        {
            int stock = 0;
            try { stock = Convert.ToInt32(stockObj); } catch { stock = 0; }

            if (stock < 5)
                return "color: red; font-weight:bold; font-size:0.85rem;";
            else if (stock <= 10)
                return "color: orange; font-weight:bold; font-size:0.85rem;";
            else
                return "color: green; font-weight:bold; font-size:0.85rem;";
        }

        protected void gvLibros_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int stock = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "StockUnidades"));
                Button btnEliminar = e.Row.FindControl("btnEliminar") as Button;
                if (btnEliminar != null)
                {
                    string rol = Session["Rol"]?.ToString() ?? "";
                    btnEliminar.Visible = (rol == "Gerente" && stock == 0);
                }
            }
        }

        protected void gvLibros_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvLibros.EditIndex = e.NewEditIndex;
            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }

        protected void gvLibros_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvLibros.EditIndex = -1;
            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }

        protected void gvLibros_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = gvLibros.Rows[e.RowIndex];
                int libroId = Convert.ToInt32(gvLibros.DataKeys[e.RowIndex].Value);
                string precioTexto = (row.FindControl("txtPrecio") as TextBox)?.Text.Replace("Q", "").Trim();
                string stockTexto = (row.FindControl("txtStock") as TextBox)?.Text.Trim();

                decimal.TryParse(precioTexto, out decimal precio);
                int.TryParse(stockTexto, out int stock);

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = @"UPDATE Libros 
                                     SET PrecioBase=@Precio, StockUnidades=@Stock
                                     WHERE LibroId=@LibroId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Stock", stock);
                    cmd.Parameters.AddWithValue("@LibroId", libroId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                gvLibros.EditIndex = -1;
                CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error al actualizar: " + ex.Message + "');</script>");
            }
        }

        protected void gvLibros_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int libroId = Convert.ToInt32(gvLibros.DataKeys[e.RowIndex].Value);
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "DELETE FROM Libros WHERE LibroId = @LibroId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@LibroId", libroId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }

        protected void btnReiniciar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            ddlOrdenarPor.SelectedIndex = 0;
            ddlOrden.SelectedIndex = 0;
            CargarInventario("L.LibroId", "ASC");
        }

        protected void gvLibros_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLibros.PageIndex = e.NewPageIndex;
            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }
    }
}
