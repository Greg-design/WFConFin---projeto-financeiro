using Microsoft.AspNetCore.Mvc;
using WFConFin.Data;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadoController : Controller
    {
        // nosso context de banco de dados
        private readonly WFConFinDbContext _context;
        public EstadoController(WFConFinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetEstados()
        {
            try
            {
                var res = _context.Estado.ToList();
                return Ok(res);
            }catch(Exception e)
            {
                return BadRequest($"Erro na listagem de estados. Exceção: {e.Message}");
            }
          
        }

        [HttpPost]
        public IActionResult PostEstado([FromBody] Estado estado)
        {
            try
            {
                _context.Estado.Add(estado);
                var valor = _context.SaveChanges();
                if(valor == 1)
                {
                    return Ok("Sucesso, estado incluído.");
                }else
                {
                    return BadRequest("Erro, estado não incluído.");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, estado não incluído. Exceção: {e.Message}");
            }

        }

        [HttpPut]
        public IActionResult PutEstado([FromBody] Estado estado)
        {
            try
            {
                _context.Estado.Update(estado);
                var valor = _context.SaveChanges();
                if (valor == 1)
                {
                    return Ok("Sucesso, estado alterado.");
                }
                else
                {
                    return BadRequest("Erro, estado não alterado.");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, estado não alterado. Exceção: {e.Message}");
            }

        }

        [HttpDelete("{sigla}")]
        public IActionResult DeleteEstado([FromRoute] string sigla)
        {
            try
            {
                var estado = _context.Estado.Find(sigla);

                if(estado.Sigla == sigla && !string.IsNullOrEmpty(estado.Sigla))
                {
                    _context.Estado.Remove(estado);
                    var valor = _context.SaveChanges();

                    if(valor == 1)
                    {
                        return Ok("Sucesso, estado excluído.");
                    }
                    else
                    {
                        return BadRequest("Erro, estado não excluído.");
                    }

                } else
                {
                    return NotFound("Erro, estado não existe.");
                }
                
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, estado não alterado. Exceção: {e.Message}");
            }

        }
    }
}
