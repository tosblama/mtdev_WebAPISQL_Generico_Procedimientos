using System.Data;
using System.Data.SqlClient;

namespace WebAPISQL_Generico.Clases
{
    public class GetData
    {
        public async Task<string> GetDbData(string nombre, string parametros = "")
        {
            // Buscar consulta asociada al nombre recibido
            DataTable dt = AccesoDatos.GetDataTable("SELECT * FROM WebAPIConsultas WHERE Nombre=@nombreConsulta", new string[] { "@nombreConsulta:" + nombre });
            if (dt.Rows.Count == 0) { return "NotFound"; }
            string query = dt.Rows[0]["Consulta"].ToString();

            // Si hay parámetros, asignarlos (separamos los parámetros por el caracter |)
            string[] listaParametros = new string[0];
            if (parametros.Length > 0)
            {
                listaParametros = parametros.Split('|');
            }

            // Ejecutar consulta
            return await AccesoDatos.JsonDataReader(query, listaParametros);
        }
    

        public async Task<string> ExecuteSP(string nombre, string parametros = "")
        {
            // Verificar que se puede ejecutar el procedimiento almacenado indicado
            DataTable dtProc = AccesoDatos.GetDataTable("SELECT Procedimiento FROM WebAPIProcedimientos WHERE Procedimiento=@nombreProcedimiento", new string[] { "@nombreProcedimiento:" + nombre });
            if (dtProc.Rows.Count == 0)
            {
                return "NotFound";
            }


            // Obtener los parámetros del procedimiento a ejecutar
            List<SqlParameter> SqlParams = new List<SqlParameter>();
            System.Text.StringBuilder QueryParametros = new System.Text.StringBuilder();
            QueryParametros.AppendLine("select  name,");
            QueryParametros.AppendLine("                   type_name(user_type_id) as type,");
            QueryParametros.AppendLine("                   max_length,");
            QueryParametros.AppendLine("                   precision,");
            QueryParametros.AppendLine("                   OdbcScale(system_type_id, scale) as scale,");
            QueryParametros.AppendLine("                   is_output,");
            QueryParametros.AppendLine("                   parameter_id");
            QueryParametros.AppendLine("from sys.parameters ");
            QueryParametros.AppendLine("where object_id = object_id('" + nombre + "')");
            QueryParametros.AppendLine("order by parameter_id");
            DataTable dtParametros = new DataTable();
            dtParametros = AccesoDatos.GetTmpDataTable(QueryParametros.ToString());

            SqlParameter p = new SqlParameter();
            for (int i = 0; i < dtParametros.Rows.Count; i++)
            {
                p = new SqlParameter();
                p.ParameterName = dtParametros.Rows[i]["name"].ToString();
                p.SqlDbType = TipoSQL(dtParametros.Rows[i]["type"].ToString());

                // Parámetros de entrada
                if (Convert.ToBoolean(dtParametros.Rows[i]["is_output"]) == false)
                {
                    if (parametros != null)
                    {
                        p.SqlValue = parametros.Split('|')[i];
                        if (p.SqlValue.ToString().ToLower() == "null") p.SqlValue = null;
                    }
                }
                // Parámetros de salida
                else
                {
                    p.Direction = ParameterDirection.Output;
                }

                // Añadir parámetro
                SqlParams.Add(p);
            }
            return await AccesoDatos.JsonStoredProcedure(nombre, SqlParams.ToArray());
        }

        private SqlDbType TipoSQL(string tipoSQL)
        {
            switch (tipoSQL.ToUpper())
            {
                case "INT": return SqlDbType.Int;
                case "BIT": return SqlDbType.Bit;
                case "VARCHAR": return SqlDbType.VarChar;
                case "CHAR": return SqlDbType.Char;
                case "DATETIME": return SqlDbType.DateTime;
                case "DATE": return SqlDbType.Date;
                default: return SqlDbType.VarChar;
            }
        }

    }
}
