using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFConFin.Data;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : Controller
    {
        private readonly WFConFinDbContext _context;

        public ContaController(WFConFinDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetContas()
        {
            try
            {
                var res = _context.Conta.ToList();
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro na listagem de contas. Exceçaõ: {e.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostConta([FromBody] Conta conta)
        {
            try
            {
                await _context.Conta.AddAsync(conta);

                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso, conta incluída");
                }
                else
                {
                    return BadRequest("Erro, conta não incluída");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro na inclusão de conta. Exceção: {e.Message}. Inner Exception: {e.InnerException?.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutConta([FromBody] Conta conta)
        {
            try
            {
                _context.Conta.Update(conta);

                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso, conta alterada");
                }
                else
                {
                    return BadRequest("Erro, conta não alterada");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro na alteração da conta. Exceçaõ: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConta([FromRoute] Guid id)
        {
            try
            {
                Conta conta = await _context.Conta.FindAsync(id);

                if (conta != null)
                {
                    _context.Conta.Remove(conta);
                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1)
                    {
                        return Ok("Sucesso, conta excluida");
                    }
                    else
                    {
                        return BadRequest("Erro, conta não excluída");
                    }
                }
                else
                {
                    return NotFound("Erro, conta não existe.");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Erro na alteração da conta. Exceção: {e.Message}");
            }
        }

        // pegar uma única conta
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConta([FromRoute] Guid id)
        {
            try
            {
                Conta conta = await _context.Conta.FindAsync(id);

                if (conta != null)
                {
                    return Ok(conta);
                }
                else
                {
                    return NotFound("Erro, conta não existe.");
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Erro na consulta da conta. Exceção: {e.Message}");
            }

        }

        // Encontrar por busca o objeto conta
        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetContaPesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto por Nome e descrição
                var lista = from objeto in _context.Conta.Include(o => o.Pessoa).ToList()
                            where objeto.Descricao.ToUpper().Contains(valor.ToUpper())
                            || objeto.Pessoa.Nome.ToUpper().Contains(valor.ToUpper())                         
                            select objeto;

                return Ok(lista);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de conta. Exceção: {e.Message}");
            }

        }

        // Encontrar por busca o objeto Conta Com Paginação
        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetContaPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto no campo de Nome quanto no Descrição
                var lista = from objeto in _context.Conta.Include(o => o.Pessoa).ToList()
                            where objeto.Descricao.ToUpper().Contains(valor.ToUpper())
                            || objeto.Pessoa.Nome.ToUpper().Contains(valor.ToUpper())
                            select objeto;

                if (ordemDesc)
                {
                    lista = from objeto in lista
                            orderby objeto.Descricao descending
                            select objeto;
                }
                else
                {
                    lista = from objeto in lista
                            orderby objeto.Descricao ascending
                            select objeto;
                }

                var quantidade = lista.Count();

                lista = lista.Skip(skip).Take(take).ToList();

                var paginacaoResponse = new PaginacaoResponse<Conta>(lista, quantidade, skip, take);

                return Ok(paginacaoResponse);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa da conta. Exceção: {e.Message}");
            }

        }

        // Encontrar as contas de determinada pessoa, através do Id.
        [HttpGet("Pessoa/{pessoaId}")]
        public async Task<IActionResult> GetContasPessoa([FromRoute] Guid pessoaId)
        {
            try
            {
                //Query Criteria - uma Query pra filtrar o conteúdo tanto por Nome e descrição
                var lista = from objeto in _context.Conta.Include(o => o.Pessoa).ToList()
                            where objeto.PessoaId == pessoaId
                            select objeto;

                return Ok(lista);

            }
            catch (Exception e)
            {
                return BadRequest($"Erro, pesquisa de conta por pessoa. Exceção: {e.Message}");
            }

        }
    }
}
