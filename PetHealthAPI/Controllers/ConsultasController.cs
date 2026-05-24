using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;
using PetHealthAPI.Models;

namespace PetHealthAPI.Controllers
{
    /// <summary>
    /// Histórico clínico — consultas veterinárias dos pets
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ConsultasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsultasController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/consultas
        /// <summary>Lista todas as consultas registradas</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Consulta>>> GetTodas()
        {
            var consultas = await _context.Consultas.Include(c => c.Pet).ToListAsync();
            return Ok(consultas);
        }

        // GET api/consultas/5
        /// <summary>Busca uma consulta pelo ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Consulta>> GetPorId(int id)
        {
            var consulta = await _context.Consultas.Include(c => c.Pet)
                                                   .FirstOrDefaultAsync(c => c.Id == id);
            if (consulta == null)
                return NotFound(new { mensagem = $"Consulta com ID {id} não encontrada." });

            return Ok(consulta);
        }

        // GET api/consultas/pet/3
        /// <summary>Lista todo o histórico clínico de um pet específico</summary>
        /// <param name="petId">ID do pet</param>
        [HttpGet("pet/{petId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Consulta>>> GetPorPet(int petId)
        {
            var petExiste = await _context.Pets.AnyAsync(p => p.Id == petId);
            if (!petExiste)
                return NotFound(new { mensagem = $"Pet com ID {petId} não encontrado." });

            var consultas = await _context.Consultas
                .Where(c => c.PetId == petId)
                .OrderByDescending(c => c.DataConsulta)
                .ToListAsync();

            return Ok(consultas);
        }

        // GET api/consultas/retornos
        /// <summary>Lista consultas com retorno agendado nos próximos 30 dias</summary>
        [HttpGet("retornos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Consulta>>> GetRetornosPendentes()
        {
            var hoje = DateTime.Now;
            var limite = hoje.AddDays(30);

            var consultas = await _context.Consultas
                .Where(c => c.DataRetorno.HasValue &&
                            c.DataRetorno.Value >= hoje &&
                            c.DataRetorno.Value <= limite)
                .Include(c => c.Pet)
                .OrderBy(c => c.DataRetorno)
                .ToListAsync();

            return Ok(consultas);
        }

        // POST api/consultas
        /// <summary>Registra uma nova consulta para um pet</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Consulta>> Post([FromBody] Consulta consulta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var petExiste = await _context.Pets.AnyAsync(p => p.Id == consulta.PetId);
            if (!petExiste)
                return BadRequest(new { mensagem = $"Pet com ID {consulta.PetId} não encontrado." });

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPorId), new { id = consulta.Id }, consulta);
        }

        // PUT api/consultas/5
        /// <summary>Atualiza os dados de uma consulta</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Consulta consulta)
        {
            if (id != consulta.Id)
                return BadRequest(new { mensagem = "O ID da rota não corresponde ao ID do corpo da requisição." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existe = await _context.Consultas.AnyAsync(c => c.Id == id);
            if (!existe)
                return NotFound(new { mensagem = $"Consulta com ID {id} não encontrada." });

            _context.Entry(consulta).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/consultas/5
        /// <summary>Remove o registro de uma consulta</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null)
                return NotFound(new { mensagem = $"Consulta com ID {id} não encontrada." });

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
