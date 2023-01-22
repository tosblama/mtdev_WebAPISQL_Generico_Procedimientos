using System.Data;

namespace WebAPISQL_Generico.Clases
{
    public class GetData
    {
        public async Task<string> GetDbData(string nombre, string parametros = "")
        {
            // Buscar consulta asociada al nombre recibido
            DataTable dt = AccesoDatos.GetTmpDataTable($"SELECT * FROM WebAPIConsultas WHERE Nombre='{nombre}'");
            if (dt.Rows.Count == 0) { return "NotFound"; }
            string query = dt.Rows[0]["Consulta"].ToString();

            // Si hay parámetros, asignarlos (separamos los parámetros por el caracter |)
            if (parametros.Length > 0)
            {
                string[] listaParametros = parametros.Split('|');
                for (int i = 0; i < listaParametros.Length; i++)
                {
                    query = query.Replace("@p" + (i + 1).ToString(), listaParametros[i]);
                }
            }

            // Ejecutar consulta
            return await AccesoDatos.JsonDataReader(query);
        }

    }
}
