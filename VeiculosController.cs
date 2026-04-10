using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class VeiculosController : ControllerBase
{
    private readonly LocadoraContext _context;

    public VeiculosController(LocadoraContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VeiculoReadDTO>>> GetVeiculos() 
    {
        var veiculos = await _context.Veiculos
            .Include(v => v.fabricante)
            .Include(v => v.categoria)
            .Select(v => new VeiculoReadDTO {
                id = v.id,
                modelo = v.modelo,
                anoFabricacao = v.anoFabricacao,
                nomeFabricante = v.fabricante.nome,
                descricaoCategoria = v.categoria.descricao
            })
            .ToListAsync();

        return Ok(veiculos);
    }

    [HttpPost]
    public async Task<ActionResult<Veiculo>> PostVeiculo(VeiculoCreateDTO dto)
    {
        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.id == dto.categoriaId);
        if (!categoriaExiste) 
            return BadRequest($"A categoria com ID {dto.categoriaId} não foi encontrada.");

        var fabricanteExiste = await _context.Fabricantes.AnyAsync(f => f.id == dto.fabricanteId);
        if (!fabricanteExiste) 
            return BadRequest($"O fabricante com ID {dto.fabricanteId} não foi encontrado.");

        var veiculo = new Veiculo {
            modelo = dto.modelo,
            anoFabricacao = dto.anoFabricacao,
            quilometragemAtual = dto.quilometragemAtual,
            fabricanteId = dto.fabricanteId,
            categoriaId = dto.categoriaId
        };

        _context.Veiculos.Add(veiculo);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetVeiculos), new { id = veiculo.id }, veiculo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutVeiculo(int id, VeiculoCreateDTO dto)
    {
        var veiculoIdBanco = await _context.Veiculos.FindAsync(id);

        if (veiculoIdBanco == null)
        {
            return NotFound($"Veículo com ID {id} não encontrado.");
        }

        if (!await _context.Categorias.AnyAsync(c => c.id == dto.categoriaId))
            return BadRequest($"A categoria com ID {dto.categoriaId} não existe.");

        if (!await _context.Fabricantes.AnyAsync(f => f.id == dto.fabricanteId))
            return BadRequest($"O fabricante com ID {dto.fabricanteId} não existe.");

        veiculoIdBanco.modelo = dto.modelo;
        veiculoIdBanco.anoFabricacao = dto.anoFabricacao;
        veiculoIdBanco.quilometragemAtual = dto.quilometragemAtual;
        veiculoIdBanco.fabricanteId = dto.fabricanteId;
        veiculoIdBanco.categoriaId = dto.categoriaId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Veiculos.Any(e => e.id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVeiculo(int id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        
        if (veiculo == null)
        {
            return NotFound();
        }

        var temAluguel = await _context.Alugueis.AnyAsync(a => a.veiculoId == id);
        if (temAluguel)
        {
            return BadRequest("Não é possível excluir um veículo que possui histórico de aluguéis.");
        }

        _context.Veiculos.Remove(veiculo);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
