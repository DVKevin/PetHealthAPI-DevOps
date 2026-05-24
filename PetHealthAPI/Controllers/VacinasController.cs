using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;
using PetHealthAPI.Models;

namespace PetHealthAPI.Controllers
{
    /// <summary>
    /// Carteira de vacinação digital dos pets
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VacinasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VacinasController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/vacinas
        /// <summary>Lista todas as vacinas registradas</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Vacina>>> GetTodas()
        {
            var vacinas = await _context.Vacinas.Include(v => v.Pet).ToListAsync();
            return Ok(vacinas);
        }

        // GET api/vacinas/5
        /// <summary>Busca uma vacina pelo ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Vacina>> GetPorId(int id)
        {
            var vacina = await _context.Vacinas.Include(v => v.Pet)
                                               .FirstOrDefaultAsync(v => v.Id == id);
            if (vacina == null)
                return NotFound(new { mensagem = $"Vacina com ID {id} não encontrada." });

            return Ok(vacina);
        }

        // GET api/vacinas/pet/3
        /// <summary>Lista todas as vacinas de um pet específico</summary>
        /// <param name="petId">ID do pet</param>
        [HttpGet("pet/{petId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Vacina>>> GetPorPet(int petId)
        {
            var petExiste = await _context.Pets.AnyAsync(p => p.Id == petId);
            if (!petExiste)
                return NotFound(new { mensagem = $"Pet com ID {petId} não encontrado." });

            var vacinas = await _context.Vacinas.Where(v => v.PetId == petId).ToListAsync();
            return Ok(vacinas);
        }

        // GET api/vacinas/vencendo
        /// <summary>Lista vacinas com próxima dose nos próximos 30 dias</summary>
        [HttpGet("vencendo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Vacina>>> GetVencendo()
        {
            var hoje = DateTime.Now;
            var limite = hoje.AddDays(30);

            var vacinas = await _context.Vacinas
                .Where(v => v.DataProximaDose.HasValue &&
                            v.DataProximaDose.Value >= hoje &&
                            v.DataProximaDose.Value <= limite)
                .Include(v => v.Pet)
                .OrderBy(v => v.DataProximaDose)
                .ToListAsync();

            return Ok(vacinas);
        }

        // POST api/vacinas
        /// <summary>Registra uma nova vacina para um pet</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Vacina>> Post([FromBody] Vacina vacina)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var petExiste = await _context.Pets.AnyAsync(p => p.Id == vacina.PetId);
            if (!petExiste)
                return BadRequest(new { mensagem = $"Pet com ID {vacina.PetId} não encontrado." });

            _context.Vacinas.Add(vacina);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPorId), new { id = vacina.Id }, vacina);
        }

        // PUT api/vacinas/5
        /// <summary>Atualiza os dados de uma vacina</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Vacina vacina)
        {
            if (id != vacina.Id)
                return BadRequest(new { mensagem = "O ID da rota não corresponde ao ID do corpo da requisição." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existe = await _context.Vacinas.AnyAsync(v => v.Id == id);
            if (!existe)
                return NotFound(new { mensagem = $"Vacina com ID {id} não encontrada." });

            _context.Entry(vacina).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/vacinas/5
        /// <summary>Remove o registro de uma vacina</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var vacina = await _context.Vacinas.FindAsync(id);
            if (vacina == null)
                return NotFound(new { mensagem = $"Vacina com ID {id} não encontrada." });

            _context.Vacinas.Remove(vacina);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
