using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class Compra : Page
    {
        string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarProveedores();
                CargarLibros();
                CargarUnidades();
            }
        }

        private void CargarProveedores()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT ProveedorId, Nombre FROM Proveedores", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlProveedor.DataSource = dt;
                ddlProveedor.DataTextField = "Nombre";
                ddlProveedor.DataValueField = "ProveedorId";
                ddlProveedor.DataBind();
                ddlProveedor.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", "0"));
            }
        }

        private void CargarLibros()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT LibroId, Titulo FROM Libros", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlLibro.DataSource = dt;
                ddlLibro.DataTextField = "Titulo";
                ddlLibro.DataValueField = "LibroId";
                ddlLibro.DataBind();
                ddlLibro.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", "0"));
            }
        }

        private void CargarUnidades()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT UnidadMedidaId, Nombre FROM UnidadMedida", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlUnidad.DataSource = dt;
                ddlUnidad.DataTextField = "Nombre";
                ddlUnidad.DataValueField = "UnidadMedidaId";
                ddlUnidad.DataBind();
                ddlUnidad.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", "0"));
            }
        }

        protected void ddlLibro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlLibro.SelectedValue != "0")
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlCommand cmd = new SqlCommand("SELECT StockUnidades FROM Libros WHERE LibroId = @id", con);
                    cmd.Parameters.AddWithValue("@id", ddlLibro.SelectedValue);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    txtStockActual.Text = result != null ? result.ToString() : "0";
                }
            }
            else
            {
                txtStockActual.Text = "";
            }

            txtTotal.Text = "";
        }

        protected void ddlUnidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularTotal();
        }

        private void CalcularTotal()
        {
            if (ddlLibro.SelectedValue == "0" || ddlUnidad.SelectedValue == "0" || string.IsNullOrEmpty(txtCantidad.Text)) return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT C.PrecioBase, UM.CantidadPorUnidad, UM.DescuentoPorcentaje
                    FROM Libros L
                    JOIN Categorias C ON L.CategoriaId = C.CategoriaId
                    JOIN UnidadMedida UM ON UM.UnidadMedidaId = @UnidadId
                    WHERE L.LibroId = @LibroId", con);
                cmd.Parameters.AddWithValue("@LibroId", ddlLibro.SelectedValue);
                cmd.Parameters.AddWithValue("@UnidadId", ddlUnidad.SelectedValue);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    decimal precioBase = Convert.ToDecimal(dr["PrecioBase"]);
                    int cantidadPorUnidad = Convert.ToInt32(dr["CantidadPorUnidad"]);
                    decimal descuento = Convert.ToDecimal(dr["DescuentoPorcentaje"]);

                    int cantidad = int.Parse(txtCantidad.Text);
                    decimal total = cantidad * (precioBase * cantidadPorUnidad) * (1 - descuento / 100);

                    txtTotal.Text = total.ToString("0.00");
                }
            }
        }

        protected void btnAgregarCompra_Click(object sender, EventArgs e)
        {
            if (ddlProveedor.SelectedValue == "0" || ddlLibro.SelectedValue == "0" || ddlUnidad.SelectedValue == "0" || string.IsNullOrEmpty(txtCantidad.Text))
                return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SP_RegistrarCompra", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProveedorId", ddlProveedor.SelectedValue);

                DataTable items = new DataTable();
                items.Columns.Add("LibroId", typeof(int));
                items.Columns.Add("UnidadMedidaId", typeof(int));
                items.Columns.Add("Cantidad", typeof(int));
                items.Columns.Add("PrecioUnitario", typeof(decimal));

                using (SqlCommand calc = new SqlCommand(@"
                    SELECT C.PrecioBase, UM.CantidadPorUnidad, UM.DescuentoPorcentaje
                    FROM Libros L
                    JOIN Categorias C ON L.CategoriaId = C.CategoriaId
                    JOIN UnidadMedida UM ON UM.UnidadMedidaId = @UnidadId
                    WHERE L.LibroId = @LibroId", con))
                {
                    calc.Parameters.AddWithValue("@LibroId", ddlLibro.SelectedValue);
                    calc.Parameters.AddWithValue("@UnidadId", ddlUnidad.SelectedValue);
                    con.Open();
                    SqlDataReader dr = calc.ExecuteReader();
                    decimal precioUnitario = 0;
                    if (dr.Read())
                    {
                        decimal baseP = Convert.ToDecimal(dr["PrecioBase"]);
                        int unidades = Convert.ToInt32(dr["CantidadPorUnidad"]);
                        decimal desc = Convert.ToDecimal(dr["DescuentoPorcentaje"]);
                        precioUnitario = baseP * unidades * (1 - desc / 100);
                    }
                    dr.Close();

                    items.Rows.Add(Convert.ToInt32(ddlLibro.SelectedValue),
                                   Convert.ToInt32(ddlUnidad.SelectedValue),
                                   Convert.ToInt32(txtCantidad.Text),
                                   precioUnitario);

                    cmd.Parameters.AddWithValue("@Items", items);
                    SqlParameter totalOut = new SqlParameter("@TotalOut", SqlDbType.Decimal)
                    {
                        Direction = ParameterDirection.Output,
                        Precision = 18,
                        Scale = 2
                    };
                    cmd.Parameters.Add(totalOut);

                    cmd.ExecuteNonQuery();

                    txtTotal.Text = Convert.ToDecimal(totalOut.Value).ToString("0.00");
                }
            }

            ddlLibro_SelectedIndexChanged(null, null);
        }
    }
}
