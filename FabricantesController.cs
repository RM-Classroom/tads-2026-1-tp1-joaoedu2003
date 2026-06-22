using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class FabricantesController : ControllerBase
{
    private readonly LocadoraContext _context;
    public FabricantesController(LocadoraContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult> GetFabricantes()
    {
        var fabricantes = await _context.Fabricantes
            .Select(f => new { f.id, f.nome })
            .ToListAsync();

        return Ok(fabricantes);
    }
}
