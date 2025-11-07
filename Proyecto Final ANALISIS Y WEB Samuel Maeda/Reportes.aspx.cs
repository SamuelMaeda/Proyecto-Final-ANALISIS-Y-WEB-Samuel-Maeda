using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;

namespace Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda
{
    public partial class Reportes : Page
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        public string VentasMensualesJSON { get; private set; } = "{}";
        public string ComprasMensualesJSON { get; private set; } = "{}";
        public string TopProductosJSON { get; private set; } = "{}";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 🔹 Mostrar placeholder de fechas tipo dd/mm/yy sin autollenarlas
                txtDesdeCompras.Attributes["placeholder"] = "dd/mm/yy";
                txtHastaCompras.Attributes["placeholder"] = "dd/mm/yy";
                txtDesdeVentas.Attributes["placeholder"] = "dd/mm/yy";
                txtHastaVentas.Attributes["placeholder"] = "dd/mm/yy";

                CargarHistorialCompras();
                CargarHistorialVentas();
                CargarEstadisticas();
            }
        }

        #region COMPRAS
        protected void btnFiltrarCompras_Click(object sender, EventArgs e)
        {
            gvHistorialCompras.PageIndex = 0;
            CargarHistorialCompras();
        }

        protected void btnLimpiarCompras_Click(object sender, EventArgs e)
        {
            txtDesdeCompras.Text = "";
            txtHastaCompras.Text = "";
            gvHistorialCompras.PageIndex = 0;
            CargarHistorialCompras();
        }

        private void CargarHistorialCompras()
        {
            // 🔹 Si las fechas están vacías, usar rango completo
            DateTime desde = ParseOrDefaultDate(txtDesdeCompras.Text, new DateTime(1753, 1, 1));
            DateTime hasta = ParseOrDefaultDate(txtHastaCompras.Text, new DateTime(9999, 12, 31));

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT
                        CONVERT(varchar(16), C.Fecha, 103) AS FechaHora,
                        P.Nombre AS Proveedor,
                        STUFF((
                            SELECT ', ' +
                                (
                                    CASE 
                                        WHEN ISNULL(UM.CantidadPorUnidad, 0) > 1 THEN
                                            (
                                                CASE WHEN (cd2.Cantidad / NULLIF(UM.CantidadPorUnidad,0)) > 0
                                                    THEN CAST((cd2.Cantidad / NULLIF(UM.CantidadPorUnidad,0)) AS varchar(10)) + ' ' 
                                                         + LOWER(ISNULL(UM.Nombre,'')) +
                                                         CASE WHEN UM.Nombre LIKE '%(%' THEN '' ELSE ' (' + CAST(UM.CantidadPorUnidad AS varchar(10)) + ')' END
                                                    ELSE ''
                                                END
                                            )
                                            +
                                            CASE WHEN (cd2.Cantidad % NULLIF(UM.CantidadPorUnidad,1)) > 0
                                                THEN
                                                    CASE WHEN (cd2.Cantidad / NULLIF(UM.CantidadPorUnidad,0)) > 0 THEN ', ' ELSE '' END
                                                    + CAST((cd2.Cantidad % NULLIF(UM.CantidadPorUnidad,1)) AS varchar(10)) + ' unidad'
                                                ELSE ''
                                            END
                                            + ' de ' + ISNULL(L2.Titulo,'')
                                        ELSE
                                            CAST(cd2.Cantidad AS varchar(10)) + ' ' + LOWER(ISNULL(UM.Nombre,'')) + ' de ' + ISNULL(L2.Titulo,'')
                                    END
                                )
                            FROM CompraDetalles cd2
                            LEFT JOIN Libros L2 ON cd2.LibroId = L2.LibroId
                            LEFT JOIN UnidadMedida UM ON cd2.UnidadMedidaId = UM.UnidadMedidaId
                            WHERE cd2.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS LibrosComprados,

                        STUFF((
                            SELECT DISTINCT ', ' + ISNULL(Cat.Nombre,'')
                            FROM CompraDetalles cd2
                            INNER JOIN Libros L2 ON cd2.LibroId = L2.LibroId
                            INNER JOIN Categorias Cat ON L2.CategoriaId = Cat.CategoriaId
                            WHERE cd2.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Categorias,

                        STUFF((
                            SELECT DISTINCT ', ' + ISNULL(E.Nombre,'')
                            FROM CompraDetalles cd2
                            INNER JOIN Libros L2 ON cd2.LibroId = L2.LibroId
                            LEFT JOIN Editoriales E ON L2.EditorialId = E.EditorialId
                            WHERE cd2.CompraId = C.CompraId
                            FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, '') AS Editoriales,

                        CASE 
                            WHEN SUM(ISNULL(cd.PrecioUnitario,0) * ISNULL(cd.Cantidad,0) * (ISNULL(UM.DescuentoPorcentaje,0)/100.0)) > 0 
                                THEN 'Q' + FORMAT(SUM(cd.PrecioUnitario * cd.Cantidad * (UM.DescuentoPorcentaje/100.0)), 'N2')
                            ELSE 'Sin descuento'
                        END AS Descuentos,

                        ISNULL(C.Total, 0) AS TotalConIVA
                    FROM Compras C
                    LEFT JOIN CompraDetalles cd ON cd.CompraId = C.CompraId
                    LEFT JOIN UnidadMedida UM ON cd.UnidadMedidaId = UM.UnidadMedidaId
                    INNER JOIN Proveedores P ON C.ProveedorId = P.ProveedorId
                    WHERE C.Fecha BETWEEN @desde AND @hasta
                    GROUP BY C.CompraId, C.Fecha, P.Nombre, C.Total
                    ORDER BY C.Fecha DESC;
                ";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvHistorialCompras.DataSource = dt;
                gvHistorialCompras.DataBind();
            }
        }

        protected void gvHistorialCompras_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvHistorialCompras.PageIndex = e.NewPageIndex;
            CargarHistorialCompras();

            ScriptManager.RegisterStartupScript(this, GetType(), "activarTabCompras",
                "var tabCompras = new bootstrap.Tab(document.querySelector('#tabComprasBtn')); tabCompras.show();", true);
        }
        #endregion

        #region VENTAS
        protected void btnFiltrarVentas_Click(object sender, EventArgs e)
        {
            gvHistorialVentas.PageIndex = 0;
            CargarHistorialVentas();
        }

        protected void btnLimpiarVentas_Click(object sender, EventArgs e)
        {
            txtDesdeVentas.Text = "";
            txtHastaVentas.Text = "";
            gvHistorialVentas.PageIndex = 0;
            CargarHistorialVentas();
        }

        private void CargarHistorialVentas()
        {
            // 🔹 Si están vacías, usar rango amplio
            DateTime desde = ParseOrDefaultDate(txtDesdeVentas.Text, new DateTime(1753, 1, 1));
            DateTime hasta = ParseOrDefaultDate(txtHastaVentas.Text, new DateTime(9999, 12, 31));

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string q = @"
            SELECT
                CONVERT(VARCHAR(16), V.Fecha, 103) AS Fecha,
                ISNULL(C.Nombre,'Cliente General') AS Cliente,

                STUFF((
                    SELECT ', ' +
                           CAST(VD2.Cantidad AS varchar(10)) + ' ' +
                           LOWER(ISNULL(UM.Nombre,'')) +
                           CASE 
                               WHEN ISNULL(UM.CantidadPorUnidad,0) > 0 
                                    AND UM.Nombre NOT LIKE '%(%' 
                               THEN ' (' + CAST(UM.CantidadPorUnidad AS varchar(10)) + ')' 
                               ELSE '' 
                           END +
                           ' de ' + ISNULL(L2.Titulo,'')
                    FROM VentaDetalles VD2
                    INNER JOIN Libros L2 ON VD2.LibroId = L2.LibroId
                    LEFT JOIN UnidadMedida UM ON VD2.UnidadMedidaId = UM.UnidadMedidaId
                    WHERE VD2.VentaId = V.VentaId
                    FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, ''
                ) AS LibrosVendidos,

                STUFF((
                    SELECT DISTINCT ', ' + ISNULL(CAT.Nombre,'Sin categoría')
                    FROM VentaDetalles VD2
                    INNER JOIN Libros L3 ON VD2.LibroId = L3.LibroId
                    LEFT JOIN Categorias CAT ON L3.CategoriaId = CAT.CategoriaId
                    WHERE VD2.VentaId = V.VentaId
                    FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, ''
                ) AS Categoria,

                STUFF((
                    SELECT DISTINCT ', ' + ISNULL(E.Nombre,'Sin editorial')
                    FROM VentaDetalles VD2
                    INNER JOIN Libros L4 ON VD2.LibroId = L4.LibroId
                    LEFT JOIN Editoriales E ON L4.EditorialId = E.EditorialId
                    WHERE VD2.VentaId = V.VentaId
                    FOR XML PATH(''), TYPE).value('.', 'varchar(max)'), 1, 2, ''
                ) AS Editorial,

                ISNULL(SUM(VD.DescuentoAplicado),0) AS DescuentoTotal,
                ISNULL(V.Total,0) AS TotalConIVA

            FROM Ventas V
            LEFT JOIN VentaDetalles VD ON VD.VentaId = V.VentaId
            LEFT JOIN Clientes C ON V.ClienteId = C.ClienteId
            WHERE V.Fecha BETWEEN @desde AND @hasta
            GROUP BY V.VentaId, V.Fecha, C.Nombre, V.Total
            ORDER BY V.Fecha DESC;";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvHistorialVentas.DataSource = dt;
                gvHistorialVentas.DataBind();
            }
        }

        protected void gvHistorialVentas_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvHistorialVentas.PageIndex = e.NewPageIndex;
            CargarHistorialVentas();

            ScriptManager.RegisterStartupScript(this, GetType(), "activarTabVentas",
                "var tabVentas = new bootstrap.Tab(document.querySelector('#tabVentasBtn')); tabVentas.show();", true);
        }
        #endregion

        #region ESTADÍSTICAS
        private void CargarEstadisticas()
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                con.Open();
                DataTable dt = new DataTable();
                dt.Columns.Add("Indicador");
                dt.Columns.Add("Valor");

                SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1 C.Nombre, SUM(V.Total) AS Total
                    FROM Ventas V
                    INNER JOIN Clientes C ON V.ClienteId = C.ClienteId
                    GROUP BY C.Nombre
                    ORDER BY Total DESC", con);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                    dt.Rows.Add("Cliente que más compra", $"{dr["Nombre"]} (Q{Convert.ToDecimal(dr["Total"]):N2})");
                dr.Close();

                cmd = new SqlCommand(@"
                    SELECT TOP 1 L.Titulo, SUM(VD.Cantidad) AS Cantidad
                    FROM VentaDetalles VD
                    INNER JOIN Libros L ON VD.LibroId = L.LibroId
                    GROUP BY L.Titulo
                    ORDER BY Cantidad DESC", con);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                    dt.Rows.Add("Producto más vendido", $"{dr["Titulo"]} ({dr["Cantidad"]} unidades)");
                dr.Close();

                cmd = new SqlCommand(@"
                    SELECT TOP 1 P.Nombre, COUNT(*) AS Compras
                    FROM Compras C
                    INNER JOIN Proveedores P ON C.ProveedorId = P.ProveedorId
                    GROUP BY P.Nombre
                    ORDER BY Compras DESC", con);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                    dt.Rows.Add("Proveedor con más compras", $"{dr["Nombre"]} ({dr["Compras"]} compras)");
                dr.Close();

                cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(Total),0) FROM Ventas
                    WHERE MONTH(Fecha)=MONTH(GETDATE()) AND YEAR(Fecha)=YEAR(GETDATE())", con);
                decimal ventasMes = Convert.ToDecimal(cmd.ExecuteScalar());
                dt.Rows.Add("Ventas del mes actual", $"Q{ventasMes:N2}");

                cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(Total),0) FROM Compras
                    WHERE MONTH(Fecha)=MONTH(GETDATE()) AND YEAR(Fecha)=YEAR(GETDATE())", con);
                decimal comprasMes = Convert.ToDecimal(cmd.ExecuteScalar());
                dt.Rows.Add("Compras del mes actual", $"Q{comprasMes:N2}");

                dt.Rows.Add("Ganancia neta estimada", $"Q{(ventasMes - comprasMes):N2}");

                gvEstadisticas.DataSource = dt;
                gvEstadisticas.DataBind();
            }
        }
        #endregion

        private DateTime ParseOrDefaultDate(string text, DateTime defaultValue)
        {
            if (DateTime.TryParse(text, out DateTime result))
            {
                if (result < new DateTime(1753, 1, 1))
                    return new DateTime(1753, 1, 1);
                if (result > new DateTime(9999, 12, 31))
                    return new DateTime(9999, 12, 31);
                return result;
            }
            return defaultValue;
        }
    }
}
