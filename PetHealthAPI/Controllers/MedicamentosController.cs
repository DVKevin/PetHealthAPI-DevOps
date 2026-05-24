using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;
using PetHealthAPI.Models;

namespace PetHealthAPI.Controllers
{
    /// <summary>
    /// Registro e controle de medicamentos dos pets
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MedicamentosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicamentosController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/medicamentos
        /// <summary>Lista todos os medicamentos registrados</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Medicamento>>> GetTodos()
        {
            var medicamentos = await _context.Medicamentos.Include(m => m.Pet).ToListAsync();
            return Ok(medicamentos);
        }

        // GET api/medicamentos/5
        /// <summary>Busca um medicamento pelo ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Medicamento>> GetPorId(int id)
        {
            var med = await _context.Medicamentos.Include(m => m.Pet)
                                                 .FirstOrDefaultAsync(m => m.Id == id);
            if (med == null)
                return NotFound(new { mensagem = $"Medicamento com ID {id} não encontrado." });

            return Ok(med);
        }

        // GET api/medicamentos/pet/3
        /// <summary>Lista todos os medicamentos de um pet específico</summary>
        /// <param name="petId">ID do pet</param>
        [HttpGet("pet/{petId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Medicamento>>> GetPorPet(int petId)
        {
            var petExiste = await _context.Pets.AnyAsync(p => p.Id == petId);
            if (!petExiste)
                return NotFound(new { mensagem = $"Pet com ID {petId} não encontrado." });

            var medicamentos = await _context.Medicamentos.Where(m => m.PetId == petId).ToListAsync();
            return Ok(medicamentos);
        }

        // GET api/medicamentos/ativos
        /// <summary>Lista apenas os medicamentos ativos (em uso atualmente)</summary>
        [HttpGet("ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Medicamento>>> GetAtivos()
        {
            var medicamentos = await _context.Medicamentos
                .Where(m => m.Ativo)
                .Include(m => m.Pet)
                .ToListAsync();

            return Ok(medicamentos);
        }

        // POST api/medicamentos
        /// <summary>Registra um novo medicamento para um pet</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Medicamento>> Post([FromBody] Medicamento medicamento)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var petExiste = await _context.Pets.AnyAsync(p => p.Id == medicamento.PetId);
            if (!petExiste)
                return BadRequest(new { mensagem = $"Pet com ID {medicamento.PetId} não encontrado." });

            _context.Medicamentos.Add(medicamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPorId), new { id = medicamento.Id }, medicamento);
        }

        // PUT api/medicamentos/5
        /// <summary>Atualiza os dados de um medicamento</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Medicamento medicamento)
        {
            if (id != medicamento.Id)
                return BadRequest(new { mensagem = "O ID da rota não corresponde ao ID do corpo da requisição." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existe = await _context.Medicamentos.AnyAsync(m => m.Id == id);
            if (!existe)
                return NotFound(new { mensagem = $"Medicamento com ID {id} não encontrado." });

            _context.Entry(medicamento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/medicamentos/5
        /// <summary>Remove o registro de um medicamento</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var med = await _context.Medicamentos.FindAsync(id);
            if (med == null)
                return NotFound(new { mensagem = $"Medicamento com ID {id} não encontrado." });

            _context.Medicamentos.Remove(med);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
