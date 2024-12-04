using System.ComponentModel.DataAnnotations;

namespace ProjetoBackend.Models
{
    public class ServicoVenda
    {
        public Guid ServicoVendaId { get; set; }
        [Required(ErrorMessage = "Serviço")]
        [Display(Name = "Serviço")]
        public Guid? ServicoId { get; set; }
        [Display(Name = "Serviço")]
        public Servico? Servico { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [DataType(DataType.Currency)]
        public decimal Servicocusto { get; set; }
        [DataType(DataType.Currency)]
        public decimal ValorTotal { get; set; }

        [Required(ErrorMessage = "Venda")]
        [Display(Name = "Venda")]
        public Guid? VendaId { get; set; }
        [Display(Name = "Venda")]
        public Venda? Venda { get; set; }
    }
}
