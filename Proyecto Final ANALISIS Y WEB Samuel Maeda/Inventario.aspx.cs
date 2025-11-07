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
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = $@"
                        SELECT 
                            L.LibroId,
                            L.Titulo,
                            ISNULL(ISNULL(E.Nombre, L.Editorial), 'Sin Editorial') AS Editorial,
                            ISNULL(C.Nombre, 'Sin Categoría') AS Categoria,

                            -- Precio de Compra (fijo por categoría)
                            CASE 
                                WHEN C.Nombre = 'Fantasía' OR C.Nombre = 'Fantasia' THEN 50
                                WHEN C.Nombre = 'Novelas' THEN 100
                                WHEN C.Nombre = 'Ciencia' THEN 150
                                ELSE ISNULL(L.PrecioOverride, 0)
                            END AS PrecioCompra,

                            -- Precio de Venta (fijo por categoría)
                            CASE 
                                WHEN C.Nombre = 'Fantasía' OR C.Nombre = 'Fantasia' THEN 80
                                WHEN C.Nombre = 'Novelas' THEN 130
                                WHEN C.Nombre = 'Ciencia' THEN 180
                                ELSE ISNULL(L.PrecioUnitario, 0)
                            END AS PrecioUnitario,

                            ISNULL(L.StockUnidades, 0) AS StockUnidades,
                            L.Activo
                        FROM Libros L
                        LEFT JOIN Categorias C ON L.CategoriaId = C.CategoriaId
                        LEFT JOIN Editoriales E ON L.EditorialId = E.EditorialId
                        WHERE L.Activo = 1
                          AND (@Filtro = '' OR L.Titulo LIKE '%' + @Filtro + '%' 
                              OR ISNULL(ISNULL(E.Nombre, L.Editorial), '') LIKE '%' + @Filtro + '%' 
                              OR ISNULL(C.Nombre,'') LIKE '%' + @Filtro + '%')
                        ORDER BY {campoOrden} {tipoOrden}";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@Filtro", filtro ?? "");
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvLibros.DataSource = dt;
                    gvLibros.DataBind();

                    lblTotalLibros.Text = $"Total de libros activos: {dt.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                lblTotalLibros.Text = "Error cargando inventario: " + ex.Message;
            }
        }

        // 🔹 Colores visuales del stock
        public string ObtenerColorStock(object stockObj)
        {
            int stock = 0;
            try { stock = Convert.ToInt32(stockObj); } catch { stock = 0; }

            if (stock == 0)
                return "color: red; font-weight:bold;";
            else if (stock < 5)
                return "color: orange; font-weight:bold;";
            else
                return "color: green; font-weight:bold;";
        }

        // 🔹 Mostrar/ocultar botón según rol
        protected void gvLibros_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int stock = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "StockUnidades"));
                Button btnDarDeBaja = e.Row.FindControl("btnDarDeBaja") as Button;

                if (btnDarDeBaja != null)
                {
                    string rol = Session["Rol"]?.ToString() ?? "";
                    btnDarDeBaja.Visible = (rol == "Gerente" && stock == 0);
                }
            }
        }

        // 🔹 Dar de baja un libro
        protected void gvLibros_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DarDeBaja")
            {
                int libroId = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = "UPDATE Libros SET Activo = 0 WHERE LibroId = @LibroId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@LibroId", libroId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
            }
        }

        // 🔹 Paginación
        protected void gvLibros_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLibros.PageIndex = e.NewPageIndex;
            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }

        // 🔹 Botones de acción
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

        protected void btnVerBaja_Click(object sender, EventArgs e)
        {
            Response.Redirect("LibrosDadosDeBaja.aspx");
        }

        // 🔹 Ordenamiento dinámico (nuevo)
        protected void ddlOrdenarPor_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }

        protected void btnAjustesInventario_Click(object sender, EventArgs e)
        {
            // Redirige al nuevo WebForm de Ajustes de Inventario
            Response.Redirect("~/AjusteInventario.aspx");
        }

        protected void ddlOrden_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarInventario(ddlOrdenarPor.SelectedValue, ddlOrden.SelectedValue, txtBuscar.Text.Trim());
        }
    }
}
