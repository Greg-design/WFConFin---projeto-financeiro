using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFConFin.Data;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PessoaController : Controller
    {

        // nosso context de banco de dados
        private readonly WFConFinDbContext _context;

        public PessoaController(WFConFinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPessoas()
        {
            try
            {
                var res = _context.Pessoa.ToList();
                return Ok(res);
            }
            catch (Exception e) { 
                return BadRequest($"Erro na listagem de pessoas. Exceçaõ: {e.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PostPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                await _context.Pessoa.AddAsync(pessoa);

                var valor = await _context.SaveChangesAsync();

                if(valor == 1)
                {
                    return Ok("Sucesso, pessoa incluída");
                }
                else
                {
                    return BadRequest("Erro, pessoa não incluída");
                }
            }
            catch (Exception e) {
                return BadRequest($"Erro na inclusão de pessoa. Exceção: {e.Message}. Inner Exception: {e.InnerException?.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PutPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                _context.Pessoa.Update(pessoa);

                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso, pessoa alterada");
                }
                else
                {
                    return BadRequest("Erro, pessoa não alterada");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro na alteração de pessoa. Exceçaõ: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeletePessoa([FromRoute] Guid id)
        {
            try
            {
                Pessoa pessoa = await _context.Pessoa.FindAsync(id);

                if(pessoa != null)
                {
                    _context.Pessoa.Remove(pessoa);
                    var valor = await _context.SaveChangesAsync();
                    if(valor == 1)
                    {
                        return Ok("Sucesso, pessoa excluida");
                    }
                    else
                    {
                        return BadRequest("Erro, pessoa não excluída");
                    }
                }
                else
                {
                    return NotFound("Erro, pessoa não existe.");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro na alteração de pessoa. Exceçaõ: {e.Message}");
            }
        }

        // pegar uma única pessoa
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPessoa([FromRoute] Guid id)
        {
            try
            {
                Pessoa pessoa = await _context.Pessoa.FindAsync(id);

                if (pessoa != null)
                {
                    return Ok(pessoa);
                }
                else
                {
                    return NotFound("Erro, pessoa não existe.");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro na consulta de pessoa. Exceção: {e.Message}");
            }

        }

        // Encontrar por busca o objeto pessoa
        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetPessoaPesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto por Nome, telefone e email
                var lista = from objeto in _context.Pessoa.ToList()
                            where objeto.Nome.ToUpper().Contains(valor.ToUpper())
                            || objeto.Telefone.ToUpper().Contains(valor.ToUpper())
                            || objeto.Email.ToUpper().Contains(valor.ToUpper())
                            select objeto;

                /*
                    outra forma com Entity:
                    
                    lista = _context.Estado
                            .Where(objetos => objetos.Nome.ToUpper().Contains(valor.ToUpper())
                                || objetos.Telefone.ToUpper().Contains(valor.ToUpper())
                                || objetos.Email.ToUpper().Contains(valor.ToUpper())
                             )
                            .ToList();
                */

                return Ok(lista);

                /*
                  No banco de dados seria assim abaixo...

                  select * from pessoa where upper(nome) like upper('%valor%') or upper(Telefone) like upper('%valor%') or upper(Email) like upper('%valor%')
                */
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de pessoa. Exceção: {e.Message}");
            }

        }

        // Encontrar por busca o objeto Pessoa Com Paginação
        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetPessoaPaginacao([FromQuery] string? valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto no campo de Nome quanto no Telefone e Email
                var lista = from objeto in _context.Pessoa.ToList()
                            select objeto;
                if (!String.IsNullOrEmpty(valor)) {
                    lista = from objeto in lista
                        where objeto.Nome.ToUpper().Contains(valor.ToUpper())
                        || objeto.Telefone.ToUpper().Contains(valor.ToUpper())
                        || objeto.Email.ToUpper().Contains(valor.ToUpper())
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

                var paginacaoResponse = new PaginacaoResponse<Pessoa>(lista, quantidade, skip, take);

                return Ok(paginacaoResponse);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de pessoa. Exceção: {e.Message}");
            }

        }
    }
}
