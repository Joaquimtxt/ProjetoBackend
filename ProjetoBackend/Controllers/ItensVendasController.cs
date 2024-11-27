using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoBackend.Data;
using ProjetoBackend.Models;

namespace ProjetoBackend.Controllers
{
    public class ItensVendasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItensVendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ItensVendas
        public async Task<IActionResult> Index(Guid? id)
        {
            var listaItens = await _context.ItensVenda.Include(i => i.Produto).Include(i => i.Venda).ToListAsync();
            listaItens = listaItens.Where(i => i.VendaId == id).ToList();
            ViewData["idVendaAtual"] = id;
            return View("Index", listaItens);
        }

        // GET: ItensVendas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemVenda = await _context.ItensVenda
                .Include(i => i.Produto)
                .Include(v => v.Venda)
                .FirstOrDefaultAsync(m => m.ItemVendaId == id);
            if (itemVenda == null)
            {
                return NotFound();
            }

            return View(itemVenda);
        }

        // POST: ItensVendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemVendaId,VendaId,ProdutoId,ServicoId,Quantidade,ValorUnitario,ValorTotal")] ItemVenda itemVenda)
        {
            if (ModelState.IsValid)
            {
                // Se o item for um produto, obtém o preço do produto
                if (itemVenda.ProdutoId != Guid.Empty)
                {
                    var produto = await _context.Produtos.FindAsync(itemVenda.ProdutoId);
                    if (produto != null)
                    {
                        itemVenda.ValorUnitario = produto.Preco;
                        itemVenda.ValorTotal = itemVenda.Quantidade * itemVenda.ValorUnitario;
                    }
                }
                // Se o item for um serviço, obtém o preço do serviço
                else if (itemVenda.ServicoId != Guid.Empty)
                {
                    var servico = await _context.Servicos.FindAsync(itemVenda.ServicoId);
                    if (servico != null)
                    {
                        itemVenda.ValorUnitario = servico.ValorServico;
                        itemVenda.ValorTotal = itemVenda.Quantidade * itemVenda.ValorUnitario;
                    }
                }

                // Atribui um novo ID para o item de venda
                itemVenda.ItemVendaId = Guid.NewGuid();
                _context.Add(itemVenda);
                await _context.SaveChangesAsync();

                // Atualiza o valor total da venda
                var venda = await _context.Vendas.FindAsync(itemVenda.VendaId);
                venda.ValorTotal = await _context.ItensVenda
                    .Where(i => i.VendaId == itemVenda.VendaId)
                    .SumAsync(i => i.ValorTotal);

                _context.Update(venda);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { id = itemVenda.VendaId });
            }
            return View(itemVenda);
        }

        // GET: ItensVendas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemVenda = await _context.ItensVenda.FindAsync(id);
            if (itemVenda == null)
            {
                return NotFound();
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", itemVenda.ProdutoId);
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "NotaFiscal", itemVenda.VendaId);
            return View(itemVenda);
        }

        // POST: ItensVendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ItemVendaId,VendaId,ProdutoId,Quantidade,ValorUnitario,ValorTotal")] ItemVenda itemVenda)
        {
            if (id != itemVenda.ItemVendaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemVenda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemVendaExists(itemVenda.ItemVendaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", itemVenda.ProdutoId);
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "NotaFiscal", itemVenda.VendaId);
            return View(itemVenda);
        }

        // GET: ItensVendas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemVenda = await _context.ItensVenda
                .Include(i => i.Produto)
                .Include(i => i.Venda)
                .FirstOrDefaultAsync(m => m.ItemVendaId == id);
            if (itemVenda == null)
            {
                return NotFound();
            }

            return View(itemVenda);
        }

        // POST: ItensVendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var itemVenda = await _context.ItensVenda.FindAsync(id);
            if (itemVenda != null)
            {
                _context.ItensVenda.Remove(itemVenda);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ItemVendaExists(Guid id)
        {
            return _context.ItensVenda.Any(e => e.ItemVendaId == id);
        }

        public decimal PrecoProduto(Guid id)
        {
            var produto = _context.Produtos.Where(p => p.ProdutoId == id).FirstOrDefault();
            return produto.Preco;
        }
    }
}
