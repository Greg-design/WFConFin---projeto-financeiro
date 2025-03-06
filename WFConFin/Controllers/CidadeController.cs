using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFConFin.Data;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CidadeController : Controller
    {
        // nosso context de banco de dados
        private readonly WFConFinDbContext _context;

        public CidadeController(WFConFinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCidades()
        {
            try
            {
                var res = _context.Cidade.ToList();
                return Ok(res);
            }
            catch(Exception e)
            {
                return BadRequest($"Erro na listagem de cidades. Exceção: {e.Message}");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PostCidade([FromBody] Cidade cidade)
        {
            try
            {
                await _context.Cidade.AddAsync(cidade);
                var valor = await _context.SaveChangesAsync();

                if(valor == 1)
                {
                    return Ok("Sucesso, cidade incluída.");
                }
                else
                {
                    return BadRequest("Erro, cidade não incluída.");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro na inclusão de cidade. Exceção: {e.Message}. Inner Exception: {e.InnerException?.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PutCidade([FromBody] Cidade cidade)
        {
            try
            {
                _context.Cidade.Update(cidade);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso, cidade alterada.");
                }
                else
                {
                    return BadRequest("Erro, cidade não alterada.");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro na alterasão de cidade. Exceção: {e.Message}");
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteCidade([FromRoute] Guid id)
        {
            try
            {
                Cidade cidade = await _context.Cidade.FindAsync(id);

                if(cidade != null)
                {
                    _context.Cidade.Remove(cidade);

                    var valor = await _context.SaveChangesAsync();

                    if (valor == 1)
                    {
                        return Ok("Sucesso, cidade excluída.");
                    }
                    else
                    {
                        return BadRequest("Erro, cidade não excluída.");
                    }
                }
                else
                {
                    return NotFound("Erro, cidade não existe.");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro na exclusão de cidade. Exceção: {e.Message}");
            }

        }

        // pegar uma única cidade
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCidade([FromRoute] Guid id)
        {
            try
            {
                Cidade cidade = await _context.Cidade.FindAsync(id);

                if (cidade != null)
                {
                    return Ok(cidade);
                }
                else
                {
                    return NotFound("Erro, cidade não existe.");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro na consulta de cidade. Exceção: {e.Message}");
            }

        }

        // Encontrar por busca o objeto cidade
        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetCidadePesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto por Nome quanto por EstadoSigla
                var lista = from objeto in _context.Cidade.ToList()
                            where objeto.Nome.ToUpper().Contains(valor.ToUpper())
                            || objeto.EstadoSigla.ToUpper().Contains(valor.ToUpper())
                            select objeto;

                /*
                    outra forma com Entity:
                    
                    lista = _context.Estado
                            .Where(objetos => objetos.Nome.ToUpper().Contains(valor.ToUpper())
                                || objetos.EstadoSigla.ToUpper().Contains(valor.ToUpper())                      
                             )
                            .ToList();
                */

                return Ok(lista);

                /*
                  No banco de dados seria assim abaixo...

                  select * from cidade where upper(nome) like upper('%valor%') or upper(estadoSigla) like upper('%valor%') 
                */
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de cidade. Exceção: {e.Message}");
            }

        }

        // Encontrar por busca o objeto cidade Com Paginação
        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetCidadePaginacao([FromQuery] string? valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto no campo de Nome quanto no estadoSigla
                var lista = from objeto in _context.Cidade.ToList()
                            select objeto;
                if (!String.IsNullOrEmpty(valor)) {
                    lista = from objeto in lista
                         where objeto.Nome.ToUpper().Contains(valor.ToUpper())
                         || objeto.EstadoSigla.ToUpper().Contains(valor.ToUpper())
                             select objeto;
                }

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

                lista = lista.Skip((skip - 1) * take).Take(take).ToList();

                var paginacaoResponse = new PaginacaoResponse<Cidade>(lista, quantidade, skip, take);

                return Ok(paginacaoResponse);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de cidade. Exceção: {e.Message}");
            }

        }
    }
}
