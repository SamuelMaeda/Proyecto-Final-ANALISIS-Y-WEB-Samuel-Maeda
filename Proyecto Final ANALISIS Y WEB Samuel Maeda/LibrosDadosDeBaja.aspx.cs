using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LuzDelSaber
{
    public partial class LibrosDadosDeBaja : Page
    {
        string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Solo Gerente puede ver este módulo
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Gerente")
            {
                Response.Redirect("Index.aspx");
                return;
            }

            if (!IsPostBack)
                CargarLibrosBaja("L.LibroId", "ASC");
        }

        private void CargarLibrosBaja(string campoOrden, string tipoOrden, string filtro = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    string query = $@"
                        SELECT 
                            L.LibroId,
                            L.Titulo,
                            ISNULL(E.Nombre, 'Sin Editorial') AS Editorial,
                            ISNULL(C.Nombre, 'Sin Categoría') AS Categoria,

                            -- Precio de Compra por categoría
                            CASE 
                                WHEN C.Nombre = 'Fantasía' OR C.Nombre = 'Fantasia' THEN 50
                                WHEN C.Nombre = 'Novelas' THEN 100
                                WHEN C.Nombre = 'Ciencia' THEN 150
                                ELSE ISNULL(L.PrecioOverride, 0)
                            END AS PrecioCompra,

                            -- Precio de Venta por categoría
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
                        WHERE L.Activo = 0
                          AND (@Filtro = '' OR
                               L.Titulo LIKE '%' + @Filtro + '%' OR
                               E.Nombre LIKE '%' + @Filtro + '%' OR
                               C.Nombre LIKE '%' + @Filtro + '%')
                        ORDER BY {campoOrden} {tipoOrden}";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@Filtro", filtro ?? "");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvLibrosBaja.DataSource = dt;
                    gvLibrosBaja.DataBind();

                    lblTotalLibros.Text = $"Total de libros dados de baja: {dt.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                lblTotalLibros.Text = "Error cargando libros dados de baja: " + ex.Message;
            }
        }

        // 🔹 Colores del stock
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

        // 🔹 Muestra el botón "Reactivar" solo si el rol es Gerente
        protected void gvLibrosBaja_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btn = e.Row.FindControl("btnReactivar") as Button;

                if (btn != null)
                {
                    string rol = Session["Rol"]?.ToString() ?? "";
                    btn.Visible = (rol == "Gerente");
                }
            }
        }

        // 🔹 Reactivar libro
        protected void gvLibrosBaja_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reactivar")
            {
                int libroId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Libros SET Activo = 1 WHERE LibroId = @LibroId", con);
                    cmd.Parameters.AddWithValue("@LibroId", libroId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                CargarLibrosBaja(ddlOrdenarPor.SelectedValue,
                                 ddlOrden.SelectedValue,
                                 txtBuscar.Text.Trim());
            }
        }

        // 🔹 Paginación
        protected void gvLibrosBaja_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLibrosBaja.PageIndex = e.NewPageIndex;
            CargarLibrosBaja(ddlOrdenarPor.SelectedValue,
                             ddlOrden.SelectedValue,
                             txtBuscar.Text.Trim());
        }

        // 🔹 Buscar
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarLibrosBaja(ddlOrdenarPor.SelectedValue,
                             ddlOrden.SelectedValue,
                             txtBuscar.Text.Trim());
        }

        // 🔹 Limpiar filtros
        protected void btnReiniciar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            ddlOrdenarPor.SelectedIndex = 0;
            ddlOrden.SelectedIndex = 0;

            CargarLibrosBaja("L.LibroId", "ASC");
        }

        // 🔹 Regresar al inventario
        protected void btnVerActivos_Click(object sender, EventArgs e)
        {
            Response.Redirect("Inventario.aspx");
        }

        // 🔹 Orden dinámico
        protected void ddlOrdenarPor_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLibrosBaja(ddlOrdenarPor.SelectedValue,
                             ddlOrden.SelectedValue,
                             txtBuscar.Text.Trim());
        }

        protected void ddlOrden_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLibrosBaja(ddlOrdenarPor.SelectedValue,
                             ddlOrden.SelectedValue,
                             txtBuscar.Text.Trim());
        }
    }
}
