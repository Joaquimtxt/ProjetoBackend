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
    public class ServicosVendasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicosVendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServicosVendas
        public async Task<IActionResult> Index(Guid? id)
        {
            var listaServicos = await _context.ServicoVenda.Include(s => s.Servico).Include(s => s.Venda).ToListAsync();
            listaServicos = listaServicos.Where(s => s.VendaId == id).ToList();
            ViewData["idVendaAtual"] = id;
            return View("Index", listaServicos);
        }



        // GET: ServicosVendas/Create
        public IActionResult Create()
        {
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "Nome");
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "VendaId");
            return View();
        }

        // POST: ServicosVendas/Create
        // POST: ServicosVendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicoVendaId,ServicoId,VendaId,Quantidade,Servicocusto,ValorTotal")] ServicoVenda servicoVenda)
        {
            if (ModelState.IsValid)
            {
                //Se o item for um serviço, obtém o preço do serviço
                if (servicoVenda.ServicoId != Guid.Empty)
                {
                    var servicop = await _context.Servicos.FindAsync(servicoVenda.ServicoId);
                    if (servicop != null)
                    {
                        servicoVenda.Servicocusto = servicop.ValorServico;
                        servicoVenda.ValorTotal = servicoVenda.Quantidade * servicoVenda.Servicocusto;
                    }
                    servicoVenda.ServicoVendaId = Guid.NewGuid();
                    _context.Add(servicoVenda);
                    await _context.SaveChangesAsync();

                    // Atualizar o valor total da venda
                    var servico = await _context.Servicos.FindAsync(servicoVenda.ServicoId);
                    var venda = await _context.Vendas.FindAsync(servicoVenda.VendaId);
                    venda.ValorTotal += servico.ValorServico;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            // Se o ModelState não for válido ou se o ServicoId for Guid.Empty
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "Nome", servicoVenda.ServicoId);
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "VendaId", servicoVenda.VendaId);
            return View(servicoVenda);
        }

        // GET: ServicosVendas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicoVenda = await _context.ServicoVenda
                .Include(s => s.Servico)
                .Include(s => s.Venda)
                .FirstOrDefaultAsync(m => m.ServicoVendaId == id);
            if (servicoVenda == null)
            {
                return NotFound();
            }

            return View(servicoVenda);
        }



        // GET: ServicosVendas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicoVenda = await _context.ServicoVenda.FindAsync(id);
            if (servicoVenda == null)
            {
                return NotFound();
            }
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "Nome", servicoVenda.ServicoId);
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "VendaId", servicoVenda.VendaId);
            return View(servicoVenda);
        }

        // POST: ServicosVendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ServicoVendaId,ServicoId,VendaId")] ServicoVenda servicoVenda)
        {
            if (id != servicoVenda.ServicoVendaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicoVenda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicoVendaExists(servicoVenda.ServicoVendaId))
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
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "Nome", servicoVenda.ServicoId);
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "VendaId", servicoVenda.VendaId);
            return View(servicoVenda);
        }

        // GET: ServicosVendas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicoVenda = await _context.ServicoVenda
                .Include(s => s.Servico)
                .Include(s => s.Venda)
                .FirstOrDefaultAsync(m => m.ServicoVendaId == id);
            if (servicoVenda == null)
            {
                return NotFound();
            }

            return View(servicoVenda);
        }

        // POST: ServicosVendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var servicoVenda = await _context.ServicoVenda.FindAsync(id);
            if (servicoVenda != null)
            {
                _context.ServicoVenda.Remove(servicoVenda);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicoVendaExists(Guid id)
        {
            return _context.ServicoVenda.Any(e => e.ServicoVendaId == id);
        }
    }
}
