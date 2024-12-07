﻿using System;
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
    public class VendasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vendas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vendas.Include(v => v.Cliente);
            return View(await applicationDbContext.OrderBy(v => v.NotaFiscal).ToListAsync());
        }

        // GET: Vendas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.VendaId == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // GET: Vendas/Create
        public IActionResult Create(Guid? id)
        {

            Venda vendaAtual = _context.Vendas.Where(v => v.VendaId == id).Include(c => c.Cliente).FirstOrDefaultAsync().Result;
            ViewData["vendaAtual"] = vendaAtual;
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome");
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
            List<ItemVenda> itens = null;
            if (vendaAtual != null)
            {
                itens = _context.ItensVenda.Where(i => i.VendaId == vendaAtual.VendaId).Include(p => p.Produto).ToList();
            }
            ViewData["listaItens"] = itens;
            return View();
        }

        // POST: Vendas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VendaId,NotaFiscal,ClienteId,DataVenda,ValorTotal")] Venda venda)
        {
            if (ModelState.IsValid)
            {
                // Auto incremento da nota fiscal
                venda.NotaFiscal = _context.Vendas.Count() + 1;
                venda.DataVenda = DateTime.Now;
                venda.VendaId = Guid.NewGuid();
                _context.Add(venda);
                await _context.SaveChangesAsync();

                ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", venda.ClienteId);
                ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
                List<ItemVenda> itensProdutos = await _context.ItensVenda.Where(i => i.VendaId == venda.VendaId).ToListAsync();
                ViewData["listaItens"] = itensProdutos;
                return RedirectToAction("Create", new { id = venda.VendaId });
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", venda.ClienteId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
            List<ItemVenda> itens = await _context.ItensVenda.Where(i => i.VendaId == venda.VendaId).Include(p => p.Produto).ToListAsync();
            ViewData["listaItens"] = itens;
            return View("Create", venda);
        }

        // GET: Vendas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas.FindAsync(id);
            if (venda == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", venda.ClienteId);
            return View(venda);
        }

        // POST: Vendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("VendaId,NotaFiscal,ClienteId,DataVenda,ValorTotal")] Venda venda)
        {
            if (id != venda.VendaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendaExists(venda.VendaId))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", venda.ClienteId);
            return View(venda);
        }

        // GET: Vendas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.VendaId == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // POST: Vendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var venda = await _context.Vendas.FindAsync(id);
            if (venda != null)
            {
                _context.Vendas.Remove(venda);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduto(Guid VendaId, Guid ProdutoId, int Quantidade)
        {

            var produto = _context.Produtos.FindAsync(ProdutoId).Result;
            ItemVenda itemVenda = new ItemVenda();
            itemVenda.VendaId = VendaId;
            itemVenda.Quantidade = Quantidade;
            itemVenda.ProdutoId = ProdutoId;

            itemVenda.ItemVendaId = Guid.NewGuid();
            itemVenda.ValorUnitario = produto.Preco;
            itemVenda.ValorTotal = Quantidade * produto.Preco;

            _context.ItensVenda.Add(itemVenda);
            await _context.SaveChangesAsync();

            var venda = _context.Vendas.FindAsync(VendaId).Result;
            venda.ValorTotal += itemVenda.ValorTotal;

            _context.Update(venda);
            await _context.SaveChangesAsync();

            List<ItemVenda> itens = await _context.ItensVenda.Where(i => i.VendaId == VendaId).Include(p => p.Produto).ToListAsync();
            ViewData["listaItens"] = itens;
            return RedirectToAction("Create", new { id = VendaId });
        }



        private bool VendaExists(Guid id)
        {
            return _context.Vendas.Any(e => e.VendaId == id);
        }
    }
}
