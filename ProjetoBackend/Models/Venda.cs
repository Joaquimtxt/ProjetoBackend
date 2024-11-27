using ProjetoBackend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetoBackend.Models
{
    public class Venda
    {
        public Guid VendaId { get; set; }

        public int? NotaFiscal { get; set; } = 0;

        [Required(ErrorMessage = "É necessário informar o Cliente")]
        [Display(Name = "Cliente")]
        public Guid ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [Display(Name = "Data da Venda")]
        public DateTime? DataVenda { get; set; } = DateTime.Now;

        [Display(Name = "Valor Total")]
        public decimal? ValorTotal { get; set; } = 0;

        // Propriedades de navegação
        public ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
        public ICollection<ServicoVenda> ServicosVenda { get; set; } = new List<ServicoVenda>();
        public decimal CalcularValorTotal()
        {
            decimal totalItens = ItensVenda.Sum(i => i.ValorTotal);
            decimal totalServicos = ServicosVenda.Sum(s => s.Servico.ValorServico);
            return totalItens + totalServicos;
        }
    }
}