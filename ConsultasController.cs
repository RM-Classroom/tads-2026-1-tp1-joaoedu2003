using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/consultas")]
[ApiController]
public class ConsultasController : ControllerBase
{
    private readonly LocadoraContext _context;
    public ConsultasController(LocadoraContext context) => _context = context;

    [HttpGet("veiculos-por-tipo")]
    public async Task<ActionResult<IEnumerable<VeiculoReadDTO>>> GetByFabricanteECategoria(string fab, string cat)
    {
        return Ok(await _context.Veiculos
            .Where(v => v.fabricante.nome.Contains(fab) && v.categoria.descricao.Contains(cat))
            .Select(v => new VeiculoReadDTO {
                id = v.id,
                modelo = v.modelo,
                anoFabricacao = v.anoFabricacao,
                nomeFabricante = v.fabricante.nome,
                descricaoCategoria = v.categoria.descricao
            })
            .ToListAsync());
    }

    [HttpGet("alugueis-cliente/{cpf}")]
    public async Task<ActionResult<IEnumerable<AluguelReadDTO>>> GetAlugueisAtivos(string cpf)
    {
        return Ok(await _context.Alugueis
            .Where(a => a.cliente.cpf == cpf && a.dataDevolucao == null)
            .Select(a => new AluguelReadDTO {
                id = a.id,
                nomeCliente = a.cliente.nome,
                cpfCliente = a.cliente.cpf,
                modeloVeiculo = a.veiculo.modelo,
                fabricanteVeiculo = a.veiculo.fabricante.nome,
                dataSaida = a.dataSaida,
                dataDevolucao = a.dataDevolucao,
                valorTotal = a.valorTotal
            })
            .ToListAsync());
    }

    [HttpGet("veiculos-km")]
    public async Task<ActionResult<IEnumerable<VeiculoReadDTO>>> GetVeiculosPoucoRodados(int kmMax, int catId)
    {
        return Ok(await _context.Veiculos
            .Where(v => v.quilometragemAtual < kmMax && v.categoriaId == catId)
            .Select(v => new VeiculoReadDTO {
                id = v.id,
                modelo = v.modelo,
                anoFabricacao = v.anoFabricacao,
                nomeFabricante = v.fabricante.nome,
                descricaoCategoria = v.categoria.descricao
            })
            .ToListAsync());
    }

    [HttpGet("historico-fabricante/{fabricanteId}")]
    public async Task<ActionResult<IEnumerable<AluguelReadDTO>>> GetHistoricoByFabricante(int fabricanteId)
    {
        return Ok(await _context.Alugueis
            .Where(a => a.veiculo.fabricanteId == fabricanteId)
            .Select(a => new AluguelReadDTO {
                id = a.id,
                nomeCliente = a.cliente.nome,
                modeloVeiculo = a.veiculo.modelo,
                dataSaida = a.dataSaida,
                valorTotal = a.valorTotal
            })
            .ToListAsync());
    }

    [HttpGet("alugueis-premium")]
    public async Task<ActionResult<IEnumerable<AluguelReadDTO>>> GetAlugueisCaros(decimal valorMin)
    {
        return Ok(await _context.Alugueis
            .Where(a => a.valorTotal > valorMin)
            .Select(a => new AluguelReadDTO {
                id = a.id,
                nomeCliente = a.cliente.nome,
                cpfCliente = a.cliente.cpf,
                modeloVeiculo = a.veiculo.modelo,
                fabricanteVeiculo = a.veiculo.fabricante.nome,
                valorTotal = a.valorTotal,
                dataSaida = a.dataSaida,
                dataDevolucao = a.dataDevolucao
            })
            .ToListAsync());
    }
}
