using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetHealthAPI.Models
{
    [Table("TB_PH_PET")]
    public class Pet
    {
        [Key]
        [Column("ID_PET")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        [Column("NM_PET")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Espécie é obrigatória")]
        [StringLength(50)]
        [Column("DS_ESPECIE")]
        public string Especie { get; set; } = string.Empty;

        [StringLength(80)]
        [Column("DS_RACA")]
        public string? Raca { get; set; }

        [Range(0, 30, ErrorMessage = "Idade deve ser entre 0 e 30 anos")]
        [Column("NR_IDADE")]
        public int? Idade { get; set; }

        [Range(0.1, 200, ErrorMessage = "Peso deve ser entre 0.1kg e 200kg")]
        [Column("NR_PESO")]
        public decimal? Peso { get; set; }

        [StringLength(10)]
        [Column("DS_SEXO")]
        public string? Sexo { get; set; }

        [Column("FL_CASTRADO")]
        public bool Castrado { get; set; } = false;

        [StringLength(300)]
        [Column("DS_ALERGIAS")]
        public string? Alergias { get; set; }

        [Column("DT_NASCIMENTO")]
        public DateTime? DataNascimento { get; set; }

        [Column("DT_CADASTRO")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Chave estrangeira
        [Required(ErrorMessage = "TutorId é obrigatório")]
        [Column("ID_TUTOR")]
        public int TutorId { get; set; }

        // Navegação — [JsonIgnore] evita ciclo infinito na serialização JSON
        [JsonIgnore]
        public Tutor? Tutor { get; set; }
        public ICollection<Vacina> Vacinas { get; set; } = new List<Vacina>();
        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
        public ICollection<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
    }
}
