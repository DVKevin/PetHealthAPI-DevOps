using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetHealthAPI.Models
{
    [Table("TB_PH_CONSULTA")]
    public class Consulta
    {
        [Key]
        [Column("ID_CONSULTA")]
        public int Id { get; set; }

        [Column("DT_CONSULTA")]
        public DateTime DataConsulta { get; set; }

        [Required(ErrorMessage = "Motivo é obrigatório")]
        [StringLength(200)]
        [Column("DS_MOTIVO")]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(150)]
        [Column("NM_VETERINARIO")]
        public string? Veterinario { get; set; }

        [StringLength(150)]
        [Column("NM_CLINICA")]
        public string? Clinica { get; set; }

        [StringLength(500)]
        [Column("DS_DIAGNOSTICO")]
        public string? Diagnostico { get; set; }

        [StringLength(500)]
        [Column("DS_TRATAMENTO")]
        public string? Tratamento { get; set; }

        [Column("VL_CUSTO")]
        public decimal? Custo { get; set; }

        [Column("DT_RETORNO")]
        public DateTime? DataRetorno { get; set; }

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
