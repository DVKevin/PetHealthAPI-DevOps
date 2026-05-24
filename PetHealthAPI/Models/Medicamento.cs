using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetHealthAPI.Models
{
    [Table("TB_PH_MEDICAMENTO")]
    public class Medicamento
    {
        [Key]
        [Column("ID_MEDICAMENTO")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do medicamento é obrigatório")]
        [StringLength(150)]
        [Column("NM_MEDICAMENTO")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dosagem é obrigatória")]
        [StringLength(100)]
        [Column("DS_DOSAGEM")]
        public string Dosagem { get; set; } = string.Empty;

        [Required(ErrorMessage = "Frequência é obrigatória")]
        [StringLength(100)]
        [Column("DS_FREQUENCIA")]
        public string Frequencia { get; set; } = string.Empty;

        [Column("DT_INICIO")]
        public DateTime DataInicio { get; set; }

        [Column("DT_FIM")]
        public DateTime? DataFim { get; set; }

        [StringLength(150)]
        [Column("NM_VETERINARIO_PRESCREVEU")]
        public string? VeterinarioPrescreveu { get; set; }

        [Column("FL_ATIVO")]
        public bool Ativo { get; set; } = true;

        [StringLength(300)]
        [Column("DS_OBSERVACOES")]
        public string? Observacoes { get; set; }

        // Chave estrangeira
        [Required(ErrorMessage = "PetId é obrigatório")]
        [Column("ID_PET")]
        public int PetId { get; set; }

        // Navegação — [JsonIgnore] evita ciclo infinito na serialização JSON
        [JsonIgnore]
        public Pet? Pet { get; set; }
    }
}
