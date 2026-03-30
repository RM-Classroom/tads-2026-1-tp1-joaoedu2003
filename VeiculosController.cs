using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class VeiculosController : ControllerBase
{
    private readonly LocadoraContext _context;

    public VeiculosController(LocadoraContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculos() 
        => await _context.Veiculos.Include(v => v.fabricante).Include(v => v.categoria).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Veiculo>> PostVeiculo(Veiculo veiculo)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _context.Veiculos.Add(veiculo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVeiculos), new { id = veiculo.id }, veiculo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutVeiculo(int id, Veiculo veiculo)
    {
        if (id != veiculo.id) return BadRequest();
        _context.Entry(veiculo).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { if (!_context.Veiculos.Any(e => e.id == id)) return NotFound(); throw; }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVeiculo(int id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo == null) return NotFound();
        _context.Veiculos.Remove(veiculo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
