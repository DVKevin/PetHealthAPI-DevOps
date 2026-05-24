using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetHealthAPI.Models
{
    [Table("TB_PH_TUTOR")]
    public class Tutor
    {
        [Key]
        [Column("ID_TUTOR")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        [Column("NM_TUTOR")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150)]
        [Column("DS_EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20)]
        [Column("NR_TELEFONE")]
        public string Telefone { get; set; } = string.Empty;

        [StringLength(200)]
        [Column("DS_ENDERECO")]
        public string? Endereco { get; set; }

        [Column("DT_CADASTRO")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Navegação
        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
