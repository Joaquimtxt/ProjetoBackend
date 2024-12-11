using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoBackend.Models
{
    public class Compra
    {
        public Guid CompraId { get; set; }

        public int? NotaFiscal { get; set; } = 0;

        [Required(ErrorMessage = "É necessário informar o Fornecedor")]
        [Display(Name = "Cliente")]
        public Guid FornecedorId { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        [Display(Name = "Data da Compra")]
        public DateTime? DataCompra { get; set; } = DateTime.Now;

        [Display(Name = "Valor Total")]
        public decimal? ValorTotal { get; set; } = 0;

        // Propriedades de navegação
        public ICollection<ItemCompra> ItensCompra { get; set; } = new List<ItemCompra>();
        public decimal CalcularValorTotal()
        {
            decimal totalItens = ItensCompra.Sum(i => i.ValorTotal); ;
            return totalItens;
        }
    }

}

