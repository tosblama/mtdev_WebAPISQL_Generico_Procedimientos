using Microsoft.AspNetCore.Mvc;

namespace WebAPISQL_Generico.Controllers
{
    public class GetDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("getDbData")]
        public async Task<ActionResult> GetDbData(string nombre, string parametros = "")
        {
            Clases.GetData gd = new Clases.GetData();
            string resultado = await gd.GetDbData(nombre, parametros);
            if (resultado == "NotFound")
            {
                return NotFound($"El método {nombre} no se ha encontrado");
            }
            else
            {
                return Ok(resultado);
            }
        }
    }
}
