using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class Venta : Page
    {
        string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarClientes();
                CargarLibros();
                CargarUnidades();
            }
        }

        private void CargarClientes()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT ClienteId, Nombre FROM Clientes", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlCliente.DataSource = dt;
                ddlCliente.DataTextField = "Nombre";
                ddlCliente.DataValueField = "ClienteId";
                ddlCliente.DataBind();
                ddlCliente.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", "0"));
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
                    object result = cmd.ExecuteScalar();
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
            if (ddlLibro.SelectedValue == "0" || ddlUnidad.SelectedValue == "0" || string.IsNullOrEmpty(txtCantidad.Text))
                return;

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

        protected void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            if (ddlCliente.SelectedValue == "0" || ddlLibro.SelectedValue == "0" || ddlUnidad.SelectedValue == "0" || string.IsNullOrEmpty(txtCantidad.Text))
                return;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SP_RegistrarVenta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClienteId", ddlCliente.SelectedValue);

                DataTable items = new DataTable();
                items.Columns.Add("LibroId", typeof(int));
                items.Columns.Add("UnidadMedidaId", typeof(int));
                items.Columns.Add("Cantidad", typeof(int));

                items.Rows.Add(Convert.ToInt32(ddlLibro.SelectedValue),
                               Convert.ToInt32(ddlUnidad.SelectedValue),
                               Convert.ToInt32(txtCantidad.Text));

                cmd.Parameters.AddWithValue("@Items", items);

                SqlParameter totalOut = new SqlParameter("@TotalOut", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Output,
                    Precision = 18,
                    Scale = 2
                };
                cmd.Parameters.Add(totalOut);

                con.Open();
                cmd.ExecuteNonQuery();

                txtTotal.Text = Convert.ToDecimal(totalOut.Value).ToString("0.00");
            }

            // Actualizar stock visualmente
            ddlLibro_SelectedIndexChanged(null, null);
        }
    }
}
