using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFConFin.Models
{
    public class Conta
    {

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório")]
        [StringLength(200, ErrorMessage = "O campo Descrição pode ter até 200 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo Valor é obrigatório")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O campo Data de Vencimento é obrigatório")]
        [DataType(DataType.Date)]
        public DateTime DataVencimento { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataPagamento { get; set; }

        [Required(ErrorMessage = "O campo Situação é obrigatório")]
        public Situacao Situacao { get; set; }

        [Required(ErrorMessage = "O campo pessoa é obrigatório")]
        public Guid PessoaId { get; set; }

        public Conta()
        {
            Id = Guid.NewGuid();
        }

        // Relacionamento Entity Framework - indica que Conta tem um relacionamento com Pessoa
        public Pessoa Pessoa { get; set; }

    }
}
