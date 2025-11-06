using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Web.Script.Serialization;

public class SearchHandler : IHttpHandler
{
    private readonly string conexion = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        string q = (context.Request.QueryString["q"] ?? "").Trim();
        string type = (context.Request.QueryString["type"] ?? "").ToLower();

        if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(q))
        {
            // devolver lista vacía
            var empty = new { results = new object[0] };
            context.Response.Write(new JavaScriptSerializer().Serialize(empty));
            return;
        }

        try
        {
            if (type == "cliente")
            {
                var results = BuscarClientes(q);
                context.Response.Write(new JavaScriptSerializer().Serialize(new { results }));
                return;
            }
            else if (type == "libro")
            {
                var results = BuscarLibros(q);
                context.Response.Write(new JavaScriptSerializer().Serialize(new { results }));
                return;
            }
            else
            {
                context.Response.Write(new JavaScriptSerializer().Serialize(new { results = new object[0] }));
                return;
            }
        }
        catch (Exception ex)
        {
            // En caso de error devolvemos vacío (y opcionalmente podrías loggear)
            context.Response.Write(new JavaScriptSerializer().Serialize(new { results = new object[0], error = ex.Message }));
        }
    }

    // Buscar clientes por nombre (o NIT si quieres)
    private List<object> BuscarClientes(string q)
    {
        var list = new List<object>();
        using (SqlConnection con = new SqlConnection(conexion))
        {
            string sql = @"SELECT TOP 20 ClienteId, Nombre, ISNULL(NIT,'') AS NIT
                           FROM Clientes
                           WHERE Nombre LIKE @p OR ISNULL(NIT,'') LIKE @p
                           ORDER BY Nombre";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@p", "%" + q + "%");
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new
                        {
                            id = dr["ClienteId"].ToString(),
                            text = dr["Nombre"].ToString(),
                            nit = dr["NIT"].ToString()
                        });
                    }
                }
            }
        }
        return list;
    }

    // Buscar libros por título
    private List<object> BuscarLibros(string q)
    {
        var list = new List<object>();
        using (SqlConnection con = new SqlConnection(conexion))
        {
            string sql = @"SELECT TOP 20 LibroId, Titulo
                           FROM Libros
                           WHERE Titulo LIKE @p
                           AND ISNULL(Activo,0) = 1
                           ORDER BY Titulo";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@p", "%" + q + "%");
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new
                        {
                            id = dr["LibroId"].ToString(),
                            text = dr["Titulo"].ToString()
                        });
                    }
                }
            }
        }
        return list;
    }

    public bool IsReusable { get { return false; } }
}
