using System.ComponentModel.DataAnnotations;

namespace WFConFin.Models
{
    public class Cidade
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo Nome deve ter entre 3 e 200 caracteres")]
        public string Nome { get; set; }

        // relacionamento entre as duas model, estado e cidade 
        [Required(ErrorMessage = "O campo Estado é obrigatório")]
        [StringLength(2, MinimumLength = 2 , ErrorMessage = "o campo Estado deve ter 2 caracteres")]
        public string EstadoSigla { get; set; }

        public Cidade()
        {
            Id = Guid.NewGuid();
        }

        // relacionamento entity framework
        public Estado Estado { get; set; }

    }
}
