using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFConFin.Data;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EstadoController : Controller
    {
        // nosso context de banco de dados
        private readonly WFConFinDbContext _context;
        public EstadoController(WFConFinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEstados()
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
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PostEstado([FromBody] Estado estado)
        {
            try
            {
                await _context.Estado.AddAsync(estado);
                var valor = await _context.SaveChangesAsync();
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
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PutEstado([FromBody] Estado estado)
        {
            try
            {
                _context.Estado.Update(estado);
                var valor = await _context.SaveChangesAsync();
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
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteEstado([FromRoute] string sigla)
        {
            try
            {
                var estado = await _context.Estado.FindAsync(sigla);

                if(estado.Sigla == sigla && !string.IsNullOrEmpty(estado.Sigla))
                {
                    _context.Estado.Remove(estado);
                    var valor = await _context.SaveChangesAsync();

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

        // Encontrar um único objeto estado
        [HttpGet("{sigla}")]
        public async Task<IActionResult> GetEstado([FromRoute] string sigla)
        {
            try
            {
                var estado = await _context.Estado.FindAsync(sigla);

                if (estado != null && !string.IsNullOrEmpty(estado.Sigla))
                {
                    return Ok(estado);
                }
                else
                {
                    return NotFound("Erro, estado não existe.");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, consulta de estado. Exceção: {e.Message}");
            }
        }

        // Encontrar por busca o objeto estado, tanto em sigla quanto em nome
        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetEstadoPesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto no campo de Sigla quanto no Nome
                var lista = from objeto in _context.Estado.ToList()
                            where objeto.Sigla.ToUpper().Contains(valor.ToUpper())
                            || objeto.Nome.ToUpper().Contains(valor.ToUpper())
                            select objeto;

                /*
                    outra forma com Entity:
                    
                    lista = _context.Estado
                            .Where(objetos => objetos.Sigla.ToUpper().Contains(valor.ToUpper())
                                || objetos.Nome.ToUpper().Contains(valor.ToUpper())                      
                             )
                            .ToList();
                */

                return Ok(lista);

                /*
                  No banco de dados seria assim abaixo...

                  select * from estado where upper(sigla) like upper('%valor%') or upper(nome) like upper('%valor%') 
                */
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de estado. Exceção: {e.Message}");
            }

        }

        // Encontrar por busca o objeto estado Com Paginação
        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetEstadoPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto no campo de Sigla quanto no Nome
                var lista = from objeto in _context.Estado.ToList()
                            where objeto.Sigla.ToUpper().Contains(valor.ToUpper())
                            || objeto.Nome.ToUpper().Contains(valor.ToUpper())
                            select objeto;

                if (ordemDesc)
                {
                    lista = from objeto in lista
                            orderby objeto.Nome descending
                            select objeto;
                }
                else
                {
                    lista = from objeto in lista
                            orderby objeto.Nome ascending
                            select objeto;
                }

                var quantidade = lista.Count();

                lista = lista.Skip(skip).Take(take).ToList();

                var paginacaoResponse = new PaginacaoResponse<Estado>(lista, quantidade, skip, take);

                    return Ok(paginacaoResponse);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de estado. Exceção: {e.Message}");
            }

        }
    }
}
