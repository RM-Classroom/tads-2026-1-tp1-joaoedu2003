using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly LocadoraContext _context;
    public ClientesController(LocadoraContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult<Cliente>> PostCliente(ClienteCreateDTO dto)
    {
        if (await _context.Clientes.AnyAsync(c => c.cpf == dto.cpf))
            return BadRequest("Já existe um cliente cadastrado com este CPF.");

        var cliente = new Cliente
        {
            nome = dto.nome,
            cpf = dto.cpf,
            email = dto.email
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(PostCliente), new { id = cliente.id }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCliente(int id, ClienteCreateDTO dto)
    {
        var clienteIdBanco = await _context.Clientes.FindAsync(id);

        if (clienteIdBanco == null)
        {
            return NotFound($"Cliente com ID {id} não encontrado.");
        }

        clienteIdBanco.nome = dto.nome;
        clienteIdBanco.cpf = dto.cpf;
        clienteIdBanco.email = dto.email;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Clientes.Any(c => c.id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteReadDTO>>> GetClientes() 
    {
        var clientes = await _context.Clientes
            .Select(c => new ClienteReadDTO {
                id = c.id,
                nome = c.nome,
                cpf = c.cpf,
                email = c.email,
            })
            .ToListAsync();

        return Ok(clientes);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        
        if (cliente == null)
        {
            return NotFound();
        }

        var temAluguel = await _context.Alugueis.AnyAsync(a => a.clienteId == id);
        if (temAluguel)
        {
            return BadRequest("Não é possível excluir um cliente que possui histórico de aluguéis.");
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
