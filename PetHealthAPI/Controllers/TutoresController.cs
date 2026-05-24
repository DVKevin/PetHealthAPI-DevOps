using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;
using PetHealthAPI.Models;

namespace PetHealthAPI.Controllers
{
    /// <summary>
    /// Gerenciamento de Tutores (donos de pets)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TutoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TutoresController(AppDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // GET api/tutores
        /// <summary>Lista todos os tutores cadastrados</summary>
        /// <returns>Lista de tutores</returns>
        // ─────────────────────────────────────────────
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Tutor>>> GetTodos()
        {
            var tutores = await _context.Tutores.Include(t => t.Pets).ToListAsync();
            return Ok(tutores);
        }

        // ─────────────────────────────────────────────
        // GET api/tutores/5
        /// <summary>Busca um tutor pelo ID</summary>
        /// <param name="id">ID do tutor</param>
        // ─────────────────────────────────────────────
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Tutor>> GetPorId(int id)
        {
            var tutor = await _context.Tutores.Include(t => t.Pets)
                                              .FirstOrDefaultAsync(t => t.Id == id);
            if (tutor == null)
                return NotFound(new { mensagem = $"Tutor com ID {id} não encontrado." });

            return Ok(tutor);
        }

        // ─────────────────────────────────────────────
        // GET api/tutores/email/joao@email.com
        /// <summary>Busca um tutor pelo email</summary>
        /// <param name="email">Email do tutor</param>
        // ─────────────────────────────────────────────
        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Tutor>> GetPorEmail(string email)
        {
            var tutor = await _context.Tutores.Include(t => t.Pets)
                                              .FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower());
            if (tutor == null)
                return NotFound(new { mensagem = $"Tutor com email '{email}' não encontrado." });

            return Ok(tutor);
        }

        // ─────────────────────────────────────────────
        // GET api/tutores/nome/Joao
        /// <summary>Busca tutores pelo nome (busca parcial, sem distinção de maiúsculas)</summary>
        /// <param name="nome">Nome ou parte do nome do tutor</param>
        // ─────────────────────────────────────────────
        [HttpGet("nome/{nome}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Tutor>>> GetPorNome(string nome)
        {
            var tutores = await _context.Tutores
                .Where(t => t.Nome.ToLower().Contains(nome.ToLower()))
                .Include(t => t.Pets)
                .ToListAsync();

            return Ok(tutores);
        }

        // ─────────────────────────────────────────────
        // GET api/tutores/5/pets
        /// <summary>Lista todos os pets de um tutor específico</summary>
        /// <param name="id">ID do tutor</param>
        // ─────────────────────────────────────────────
        [HttpGet("{id:int}/pets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPetsDeTutor(int id)
        {
            var tutor = await _context.Tutores.FindAsync(id);
            if (tutor == null)
                return NotFound(new { mensagem = $"Tutor com ID {id} não encontrado." });

            var pets = await _context.Pets.Where(p => p.TutorId == id).ToListAsync();
            return Ok(pets);
        }

        // ─────────────────────────────────────────────
        // POST api/tutores
        /// <summary>Cadastra um novo tutor</summary>
        /// <param name="tutor">Dados do tutor a ser cadastrado</param>
        // ─────────────────────────────────────────────
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Tutor>> Post([FromBody] Tutor tutor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica se email já existe
            var emailExiste = await _context.Tutores.AnyAsync(t => t.Email.ToLower() == tutor.Email.ToLower());
            if (emailExiste)
                return BadRequest(new { mensagem = "Já existe um tutor cadastrado com este email." });

            tutor.DataCadastro = DateTime.Now;
            _context.Tutores.Add(tutor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPorId), new { id = tutor.Id }, tutor);
        }

        // ─────────────────────────────────────────────
        // PUT api/tutores/5
        /// <summary>Atualiza os dados de um tutor</summary>
        /// <param name="id">ID do tutor a ser atualizado</param>
        /// <param name="tutor">Dados atualizados do tutor</param>
        // ─────────────────────────────────────────────
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Tutor tutor)
        {
            if (id != tutor.Id)
                return BadRequest(new { mensagem = "O ID da rota não corresponde ao ID do corpo da requisição." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existe = await _context.Tutores.AnyAsync(t => t.Id == id);
            if (!existe)
                return NotFound(new { mensagem = $"Tutor com ID {id} não encontrado." });

            _context.Entry(tutor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ─────────────────────────────────────────────
        // DELETE api/tutores/5
        /// <summary>Remove um tutor pelo ID</summary>
        /// <param name="id">ID do tutor a ser removido</param>
        // ─────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var tutor = await _context.Tutores.FindAsync(id);
            if (tutor == null)
                return NotFound(new { mensagem = $"Tutor com ID {id} não encontrado." });

            _context.Tutores.Remove(tutor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
