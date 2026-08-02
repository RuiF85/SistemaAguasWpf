using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace WebApiBackEnd.Controllers
{
    public class EstadoFaturasController : ApiController
    {
        DataClassesDataContext dc = new DataClassesDataContext(
            ConfigurationManager.ConnectionStrings["SistemaAguasConnectionString"].ConnectionString);

        public List<EstadoFatura> Get()
        {
            return dc.EstadoFaturas.ToList();
        }
    }
}