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
                CargarLibros();
            }
        }

        private void CargarCategorias()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT CategoriaId, Nombre FROM Categorias", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                ddlCategoria.DataSource = dr;
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
                SqlDataReader dr = cmd.ExecuteReader();
                ddlEditorial.DataSource = dr;
                ddlEditorial.DataTextField = "Nombre";
                ddlEditorial.DataValueField = "EditorialId";
                ddlEditorial.DataBind();
                ddlEditorial.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", "0"));
            }
        }


        private void CargarLibros()
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
                    INNER JOIN Editoriales E ON L.EditorialId = E.EditorialId";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvLibros.DataSource = dt;
                gvLibros.DataBind();
            }
        }

        protected void ddlCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategoria.SelectedValue != "0")
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SELECT PrecioBase FROM Categorias WHERE CategoriaId = @id", con);
                    cmd.Parameters.AddWithValue("@id", ddlCategoria.SelectedValue);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    txtPrecio.Text = result != null ? "Q " + result.ToString() : "";
                }
            }
            else
            {
                txtPrecio.Text = "";
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            if (ddlCategoria.SelectedValue == "0" || ddlEditorial.SelectedValue == "0" ||
                string.IsNullOrWhiteSpace(txtTitulo.Text) || string.IsNullOrWhiteSpace(txtAutor.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor complete todos los campos obligatorios.');", true);
                return;
            }

            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand(@"
            INSERT INTO Libros (Titulo, Autor, CategoriaId, EditorialId, PrecioOverride, StockUnidades, Descripcion, FechaCreacion)
            VALUES (@Titulo, @Autor, @CategoriaId, @EditorialId, NULL, @Stock, @Descripcion, GETDATE())", con);

                cmd.Parameters.AddWithValue("@Titulo", txtTitulo.Text);
                cmd.Parameters.AddWithValue("@Autor", txtAutor.Text);
                cmd.Parameters.AddWithValue("@CategoriaId", Convert.ToInt32(ddlCategoria.SelectedValue));
                cmd.Parameters.AddWithValue("@EditorialId", Convert.ToInt32(ddlEditorial.SelectedValue));
                cmd.Parameters.AddWithValue("@Stock", string.IsNullOrEmpty(txtStock.Text) ? 0 : Convert.ToInt32(txtStock.Text));
                cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Libro agregado correctamente.');", true);
            LimpiarCampos();
            CargarLibros();
        }


        protected void gvLibros_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int libroId = Convert.ToInt32(gvLibros.DataKeys[e.RowIndex].Value);

            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Libros WHERE LibroId = @id", con);
                cmd.Parameters.AddWithValue("@id", libroId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            CargarLibros();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtTitulo.Text = "";
            txtAutor.Text = "";
            ddlCategoria.SelectedIndex = 0;
            ddlEditorial.SelectedIndex = 0;
            txtPrecio.Text = "";
            txtStock.Text = "";
            txtDescripcion.Text = "";
        }
    }
}
