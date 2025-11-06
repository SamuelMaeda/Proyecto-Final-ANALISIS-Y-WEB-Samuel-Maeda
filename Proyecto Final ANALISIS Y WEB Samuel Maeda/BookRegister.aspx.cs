using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class BookRegister : Page
    {
        string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCategorias();
                CargarEditoriales();
                CargarUltimosLibros();
            }
        }

        private void CargarCategorias()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT CategoriaId, Nombre FROM Categorias", con);
                con.Open();
                ddlCategoria.DataSource = cmd.ExecuteReader();
                ddlCategoria.DataTextField = "Nombre";
                ddlCategoria.DataValueField = "CategoriaId";
                ddlCategoria.DataBind();
                ddlCategoria.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", "0"));
            }
        }

        private void CargarEditoriales()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT EditorialId, Nombre FROM Editoriales", con);
                con.Open();
                ddlEditorial.DataSource = cmd.ExecuteReader();
                ddlEditorial.DataTextField = "Nombre";
                ddlEditorial.DataValueField = "EditorialId";
                ddlEditorial.DataBind();
                ddlEditorial.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", "0"));
            }
        }

        protected void ddlCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategoria.SelectedValue != "0")
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SELECT PrecioBase, PrecioVenta FROM Categorias WHERE CategoriaId = @id", con);
                    cmd.Parameters.AddWithValue("@id", ddlCategoria.SelectedValue);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtPrecioCompra.Text = dr["PrecioBase"].ToString();
                        txtPrecioVenta.Text = dr["PrecioVenta"].ToString();
                    }
                }
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (ddlCategoria.SelectedValue == "0" || ddlEditorial.SelectedValue == "0" ||
                string.IsNullOrWhiteSpace(txtTitulo.Text) || string.IsNullOrWhiteSpace(txtAutor.Text))
            {
                lblMensaje.Text = "⚠️ Complete todos los campos obligatorios.";
                lblMensaje.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    INSERT INTO Libros (Titulo, Autor, CategoriaId, EditorialId, PrecioCompra, PrecioUnitario, StockUnidades, Descripcion, Activo, FechaCreacion)
                    VALUES (@Titulo, @Autor, @CategoriaId, @EditorialId, @PrecioCompra, @PrecioVenta, @Stock, @Descripcion, 1, GETDATE())";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Titulo", txtTitulo.Text.Trim());
                cmd.Parameters.AddWithValue("@Autor", txtAutor.Text.Trim());
                cmd.Parameters.AddWithValue("@CategoriaId", Convert.ToInt32(ddlCategoria.SelectedValue));
                cmd.Parameters.AddWithValue("@EditorialId", Convert.ToInt32(ddlEditorial.SelectedValue));
                cmd.Parameters.AddWithValue("@PrecioCompra", Convert.ToDecimal(txtPrecioCompra.Text));
                cmd.Parameters.AddWithValue("@PrecioVenta", Convert.ToDecimal(txtPrecioVenta.Text));
                cmd.Parameters.AddWithValue("@Stock", string.IsNullOrEmpty(txtStock.Text) ? 0 : Convert.ToInt32(txtStock.Text));
                cmd.Parameters.AddWithValue("@Descripcion", "");

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMensaje.Text = "✅ Libro agregado correctamente.";
            lblMensaje.CssClass = "text-success fw-bold";

            LimpiarCampos();
            CargarUltimosLibros();
        }

        private void LimpiarCampos()
        {
            txtTitulo.Text = "";
            txtAutor.Text = "";
            ddlCategoria.SelectedIndex = 0;
            ddlEditorial.SelectedIndex = 0;
            txtPrecioCompra.Text = "";
            txtPrecioVenta.Text = "";
            txtStock.Text = "";
        }

        private void CargarUltimosLibros()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT TOP 3 
                        L.LibroId,
                        L.Titulo,
                        ISNULL(E.Nombre, 'Sin Editorial') AS Editorial,
                        ISNULL(C.Nombre, 'Sin Categoría') AS Categoria,
                        L.PrecioCompra,
                        L.PrecioUnitario AS PrecioVenta,
                        L.StockUnidades
                    FROM Libros L
                    LEFT JOIN Editoriales E ON L.EditorialId = E.EditorialId
                    LEFT JOIN Categorias C ON L.CategoriaId = C.CategoriaId
                    WHERE L.Activo = 1
                    ORDER BY L.LibroId DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvUltimosLibros.DataSource = dt;
                gvUltimosLibros.DataBind();
            }
        }
    }
}
