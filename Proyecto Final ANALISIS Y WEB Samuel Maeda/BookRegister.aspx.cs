using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

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

        // AGREGAR LIBRO
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal precio;
                int cantidad;

                if (!decimal.TryParse(txtPrecio.Text.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out precio))
                {
                    lblMensaje.Text = "Ingrese un precio válido.";
                    lblMensaje.CssClass = "text-danger";
                    return;
                }

                if (!int.TryParse(txtCantidad.Text.Trim(), out cantidad))
                {
                    lblMensaje.Text = "Ingrese una cantidad válida.";
                    lblMensaje.CssClass = "text-danger";
                    return;
                }

                using (SqlConnection con = new SqlConnection(conexion))
                using (SqlCommand cmd = new SqlCommand("SP_AgregarLibro", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Titulo", txtTitulo.Text.Trim());
                    cmd.Parameters.AddWithValue("@Autor", txtAutor.Text.Trim());
                    cmd.Parameters.AddWithValue("@Categoria", ddlCategoria.SelectedValue);
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@Descripcion", string.IsNullOrWhiteSpace(txtDescripcion.Text) ? (object)DBNull.Value : txtDescripcion.Text.Trim());

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                lblMensaje.Text = "✅ Libro agregado correctamente.";
                lblMensaje.CssClass = "text-success";

                // Limpiar campos
                txtTitulo.Text = "";
                txtAutor.Text = "";
                ddlCategoria.SelectedIndex = 0;
                txtPrecio.Text = "";
                txtCantidad.Text = "";
                txtDescripcion.Text = "";

                CargarLibros();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "❌ Error: " + ex.Message;
                lblMensaje.CssClass = "text-danger";
            }
        }

        // CARGAR LIBROS
        private void CargarLibros()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                using (SqlCommand cmd = new SqlCommand("SP_ListarLibros", con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

        // EDITAR -> poner fila en modo edición
        protected void gvLibros_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvLibros.EditIndex = e.NewEditIndex;
            CargarLibros();
        }

        // CANCELAR EDICIÓN
        protected void gvLibros_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvLibros.EditIndex = -1;
            CargarLibros();
        }

        // ACTUALIZAR fila editada
        protected void gvLibros_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvLibros.DataKeys[e.RowIndex].Value);
                GridViewRow row = gvLibros.Rows[e.RowIndex];

                TextBox txtEditTitulo = row.FindControl("txtEditTitulo") as TextBox;
                TextBox txtEditAutor = row.FindControl("txtEditAutor") as TextBox;
                DropDownList ddlEditCategoria = row.FindControl("ddlEditCategoria") as DropDownList;
                TextBox txtEditPrecio = row.FindControl("txtEditPrecio") as TextBox;
                TextBox txtEditCantidad = row.FindControl("txtEditCantidad") as TextBox;

                if (txtEditTitulo == null || txtEditAutor == null || ddlEditCategoria == null || txtEditPrecio == null || txtEditCantidad == null)
                {
                    lblMensaje.Text = "Error al leer los datos editables. Asegúrate de usar TemplateFields con los IDs correctos.";
                    lblMensaje.CssClass = "text-danger";
                    return;
                }

                string nuevoTitulo = txtEditTitulo.Text.Trim();
                string nuevoAutor = txtEditAutor.Text.Trim();
                string nuevaCategoria = ddlEditCategoria.SelectedValue;

                if (!decimal.TryParse(txtEditPrecio.Text.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out decimal nuevoPrecio))
                {
                    lblMensaje.Text = "Precio inválido.";
                    lblMensaje.CssClass = "text-danger";
                    return;
                }

                if (!int.TryParse(txtEditCantidad.Text.Trim(), out int nuevaCantidad))
                {
                    lblMensaje.Text = "Cantidad inválida.";
                    lblMensaje.CssClass = "text-danger";
                    return;
                }

                using (SqlConnection con = new SqlConnection(conexion))
                using (SqlCommand cmd = new SqlCommand("SP_ActualizarLibro", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Titulo", nuevoTitulo);
                    cmd.Parameters.AddWithValue("@Autor", nuevoAutor);
                    cmd.Parameters.AddWithValue("@Categoria", nuevaCategoria);
                    cmd.Parameters.AddWithValue("@Precio", nuevoPrecio);
                    cmd.Parameters.AddWithValue("@Cantidad", nuevaCantidad);
                    cmd.Parameters.AddWithValue("@Descripcion", (object)DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                gvLibros.EditIndex = -1;
                CargarLibros();

                lblMensaje.Text = "✅ Libro actualizado correctamente.";
                lblMensaje.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "❌ Error al actualizar: " + ex.Message;
                lblMensaje.CssClass = "text-danger";
            }
        }

        // ELIMINAR fila
        protected void gvLibros_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvLibros.DataKeys[e.RowIndex].Value);

                using (SqlConnection con = new SqlConnection(conexion))
                using (SqlCommand cmd = new SqlCommand("SP_EliminarLibro", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                CargarLibros();

                lblMensaje.Text = "✅ Libro eliminado correctamente.";
                lblMensaje.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "❌ Error al eliminar: " + ex.Message;
                lblMensaje.CssClass = "text-danger";
            }
        }
    }
}
