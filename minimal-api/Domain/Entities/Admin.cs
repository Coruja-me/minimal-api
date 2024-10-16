using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Domain.Entities
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = default!;

        [StringLength(32)]
        public string Senha { get; set; } = default!;

        [StringLength(10)]
        public string Perfil { get; set; } = default!;
    }
}