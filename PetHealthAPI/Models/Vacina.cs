using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetHealthAPI.Models
{
    [Table("TB_PH_VACINA")]
    public class Vacina
    {
        [Key]
        [Column("ID_VACINA")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome da vacina é obrigatório")]
        [StringLength(150)]
        [Column("NM_VACINA")]
        public string Nome { get; set; } = string.Empty;

        [Column("DT_APLICACAO")]
        public DateTime DataAplicacao { get; set; }

        [Column("DT_PROXIMA_DOSE")]
        public DateTime? DataProximaDose { get; set; }

        [StringLength(100)]
        [Column("NM_FABRICANTE")]
        public string? Fabricante { get; set; }

        [StringLength(80)]
        [Column("NR_LOTE")]
        public string? Lote { get; set; }

        [StringLength(150)]
        [Column("NM_VETERINARIO")]
        public string? Veterinario { get; set; }

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
