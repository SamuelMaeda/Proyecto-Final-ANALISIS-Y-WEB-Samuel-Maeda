using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class BookRegister : System.Web.UI.Page
    {
        string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarLibros();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SP_AgregarLibro", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Titulo", txtTitulo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Autor", txtAutor.Text.Trim());
                    cmd.Parameters.AddWithValue("@Categoria", ddlCategoria.SelectedValue);
                    cmd.Parameters.AddWithValue("@Precio", Convert.ToDecimal(txtPrecio.Text.Trim()));
                    cmd.Parameters.AddWithValue("@Cantidad", Convert.ToInt32(txtCantidad.Text.Trim()));
                    cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                lblMensaje.Text = "✅ Libro agregado correctamente.";
                lblMensaje.CssClass = "text-success";

                // Limpiar los campos
                txtTitulo.Text = "";
                txtAutor.Text = "";
                ddlCategoria.SelectedIndex = 0;
                txtPrecio.Text = "";
                txtCantidad.Text = "";
                txtDescripcion.Text = "";

                // Refrescar listado
                CargarLibros();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "❌ Error: " + ex.Message;
                lblMensaje.CssClass = "text-danger";
            }
        }

        private void CargarLibros()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SP_ListarLibros", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvLibros.DataSource = dt;
                    gvLibros.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "❌ Error al cargar los libros: " + ex.Message;
                lblMensaje.CssClass = "text-danger";
            }
        }
    }
}
