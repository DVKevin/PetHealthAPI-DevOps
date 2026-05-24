using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;
using PetHealthAPI.Models;

namespace PetHealthAPI.Controllers
{
    /// <summary>
    /// Gerenciamento de Pets cadastrados na plataforma
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetsController(AppDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // GET api/pets
        /// <summary>Lista todos os pets cadastrados</summary>
        // ─────────────────────────────────────────────
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetTodos()
        {
            var pets = await _context.Pets.Include(p => p.Tutor).ToListAsync();
            return Ok(pets);
        }

        // ─────────────────────────────────────────────
        // GET api/pets/5
        /// <summary>Busca um pet pelo ID com histórico completo</summary>
        /// <param name="id">ID do pet</param>
        // ─────────────────────────────────────────────
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pet>> GetPorId(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.Tutor)
                .Include(p => p.Vacinas)
                .Include(p => p.Consultas)
                .Include(p => p.Medicamentos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
                return NotFound(new { mensagem = $"Pet com ID {id} não encontrado." });

            return Ok(pet);
        }

        // ─────────────────────────────────────────────
        // GET api/pets/especie/Gato
        /// <summary>Lista pets filtrados por espécie</summary>
        /// <param name="especie">Espécie do pet (ex: Cão, Gato)</param>
        // ─────────────────────────────────────────────
        [HttpGet("especie/{especie}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPorEspecie(string especie)
        {
            var pets = await _context.Pets
                .Where(p => p.Especie.ToLower() == especie.ToLower())
                .Include(p => p.Tutor)
                .ToListAsync();

            return Ok(pets);
        }

        // ─────────────────────────────────────────────
        // GET api/pets/nome/Rex
        /// <summary>Busca pets pelo nome (busca parcial)</summary>
        /// <param name="nome">Nome ou parte do nome do pet</param>
        // ─────────────────────────────────────────────
        [HttpGet("nome/{nome}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPorNome(string nome)
        {
            var pets = await _context.Pets
                .Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
                .Include(p => p.Tutor)
                .ToListAsync();

            return Ok(pets);
        }

        // ─────────────────────────────────────────────
        // GET api/pets/castrados
        /// <summary>Lista apenas os pets castrados</summary>
        // ─────────────────────────────────────────────
        [HttpGet("castrados")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetCastrados()
        {
            var pets = await _context.Pets
                .Where(p => p.Castrado)
                .Include(p => p.Tutor)
                .ToListAsync();

            return Ok(pets);
        }

        // ─────────────────────────────────────────────
        // POST api/pets
        /// <summary>Cadastra um novo pet vinculado a um tutor</summary>
        /// <param name="pet">Dados do pet a ser cadastrado</param>
        // ─────────────────────────────────────────────
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pet>> Post([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tutorExiste = await _context.Tutores.AnyAsync(t => t.Id == pet.TutorId);
            if (!tutorExiste)
                return BadRequest(new { mensagem = $"Tutor com ID {pet.TutorId} não encontrado. Cadastre o tutor antes do pet." });

            pet.DataCadastro = DateTime.Now;
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPorId), new { id = pet.Id }, pet);
        }

        // ─────────────────────────────────────────────
        // PUT api/pets/5
        /// <summary>Atualiza os dados de um pet</summary>
        /// <param name="id">ID do pet a ser atualizado</param>
        /// <param name="pet">Dados atualizados do pet</param>
        // ─────────────────────────────────────────────
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Pet pet)
        {
            if (id != pet.Id)
                return BadRequest(new { mensagem = "O ID da rota não corresponde ao ID do corpo da requisição." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existe = await _context.Pets.AnyAsync(p => p.Id == id);
            if (!existe)
                return NotFound(new { mensagem = $"Pet com ID {id} não encontrado." });

            _context.Entry(pet).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ─────────────────────────────────────────────
        // DELETE api/pets/5
        /// <summary>Remove um pet pelo ID</summary>
        /// <param name="id">ID do pet a ser removido</param>
        // ─────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
                return NotFound(new { mensagem = $"Pet com ID {id} não encontrado." });

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
