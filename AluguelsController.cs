using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AlugueisController : ControllerBase
{
    private readonly LocadoraContext _context;
    public AlugueisController(LocadoraContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult> PostAluguel(AluguelCreateDTO dto)
    {
        var cliente = await _context.Clientes.FindAsync(dto.clienteId);
        if (cliente == null) return NotFound("Cliente não encontrado.");

        var veiculo = await _context.Veiculos.FindAsync(dto.veiculoId);
        if (veiculo == null) return NotFound("Veículo não encontrado.");

        var jaAlugado = await _context.Alugueis.AnyAsync(a => a.veiculoId == dto.veiculoId && a.dataDevolucao == null);
        if (jaAlugado) return BadRequest("Este veículo já está com um aluguel ativo.");

        var aluguel = new Aluguel
        {
            clienteId = dto.clienteId,
            veiculoId = dto.veiculoId,
            dataSaida = DateTime.Now,
            kmInicial = veiculo.quilometragemAtual,
            valorDiaria = dto.valorDiaria,
            dataDevolucao = null,
            valorTotal = dto.valorDiaria
        };

        _context.Alugueis.Add(aluguel);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Aluguel registrado com sucesso!", id = aluguel.id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAluguel(int id)
    {
        var aluguel = await _context.Alugueis.FindAsync(id);
        
        if (aluguel == null)
        {
            return NotFound();
        }

        _context.Alugueis.Remove(aluguel);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
