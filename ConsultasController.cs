using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/consultas")]
[ApiController]
public class ConsultasController : ControllerBase
{
    private readonly LocadoraContext _context;
    public ConsultasController(LocadoraContext context) => _context = context;

    [HttpGet("veiculos-por-tipo")]
    public async Task<IActionResult> GetByFabricanteECategoria(string fab, string cat)
    {
        var result = await _context.Veiculos
            .Include(v => v.fabricante)
            .Include(v => v.categoria)
            .Where(v => v.fabricante.nome.Contains(fab) && v.categoria.descricao.Contains(cat))
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("alugueis-cliente/{cpf}")]
    public async Task<IActionResult> GetAlugueisAtivos(string cpf)
    {
        var result = await _context.Alugueis
            .Include(a => a.cliente)
            .Include(a => a.veiculo)
            .Where(a => a.cliente.cpf == cpf && a.dataDevolucao == null)
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("veiculos-km")]
    public async Task<IActionResult> GetVeiculosPoucoRodados(int kmMax, int catId)
    {
        return Ok(await _context.Veiculos
            .Include(v => v.categoria)
            .Where(v => v.quilometragemAtual < kmMax && v.categoriaId == catId)
            .ToListAsync());
    }

    [HttpGet("historico-fabricante/{fabricanteId}")]
    public async Task<IActionResult> GetHistoricoByFabricante(int fabricanteId)
    {
        var result = await _context.Alugueis
            .Include(a => a.veiculo)
                .ThenInclude(v => v.fabricante)
            .Where(a => a.veiculo.fabricanteId == fabricanteId)
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("alugueis-premium")]
    public async Task<IActionResult> GetAlugueisCaros(decimal valorMin)
    {
        return Ok(await _context.Alugueis
            .Include(a => a.cliente)
            .Where(a => a.valorTotal > valorMin)
            .ToListAsync());
    }
}
