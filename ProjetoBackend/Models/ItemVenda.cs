using System.ComponentModel.DataAnnotations;

namespace ProjetoBackend.Models
{
    public class ItemVenda
    {
        public Guid ItemVendaId { get; set; }

        public Guid VendaId { get; set; }  // Relacionamento com Venda
        public Venda Venda { get; set; }

        // Propriedades para Produto
        public Guid? ProdutoId { get; set; }  // Pode ser nulo caso seja um serviço
        public Produto? Produto { get; set; }  // Relacionamento com Produto

        // Propriedades para Serviço
        public Guid? ServicoId { get; set; }  // Pode ser nulo caso seja um produto
        public Servico? Servico { get; set; }  // Relacionamento com Serviço

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [DataType(DataType.Currency)]
        public decimal ValorUnitario { get; set; }

        [DataType(DataType.Currency)]
        public decimal ValorTotal { get; set; }

        // Método para calcular o valor total (se necessário)
        public void CalcularValorTotal()
        {
            if (ProdutoId.HasValue)
            {
                // Se o item é um produto
                ValorUnitario = Produto?.Preco ?? 0;
                ValorTotal = Quantidade * ValorUnitario;
            }
            else if (ServicoId.HasValue)
            {
                // Se o item é um serviço
                ValorUnitario = Servico?.ValorServico ?? 0;
                ValorTotal = Quantidade * ValorUnitario;
            }
        }
    }
}
