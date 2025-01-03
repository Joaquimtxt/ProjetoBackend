﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoBackend.Data;
using ProjetoBackend.Migrations;
using ProjetoBackend.Models;
using Rotativa.AspNetCore;

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
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "Nome");
            List<ItemVenda> itens = null;
            List<ServicoVenda> servicos = null;
            if (vendaAtual != null)
            {
                itens = _context.ItensVenda.Where(i => i.VendaId == vendaAtual.VendaId).Include(p => p.Produto).ToList();
                servicos = _context.ServicoVenda.Where(d => d.VendaId == vendaAtual.VendaId).Include(s => s.Servico).ToList();
            }
            ViewData["listaItens"] = itens;
            ViewData["listaServicos"] = servicos;
            List<Cliente> clients = _context.Clientes.ToList();
            ViewData["Clientes"] = clients;
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
                ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "Nome");
                List<ItemVenda> itensProdutos = await _context.ItensVenda.Where(i => i.VendaId == venda.VendaId).ToListAsync();
                List<ServicoVenda> itensServicos = await _context.ServicoVenda.Where(i => i.VendaId == venda.VendaId).ToListAsync();
                ViewData["listaItens"] = itensProdutos;
                ViewData["listaServicos"] = itensServicos;
                List<Cliente> clientes = _context.Clientes.ToList();
                ViewData["Clientes"] = clientes;
                return RedirectToAction("Create", new { id = venda.VendaId });
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome", venda.ClienteId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "Nome");
            List<ItemVenda> itens = await _context.ItensVenda.Where(i => i.VendaId == venda.VendaId).Include(p => p.Produto).ToListAsync();
            List<ServicoVenda> itensServic = await _context.ServicoVenda.Where(i => i.VendaId == venda.VendaId).Include(s => s.Servico).ToListAsync();
            ViewData["listaItens"] = itens;
            ViewData["listaServicos"] = itensServic; //Aqui o que está dando errado
            List<Cliente> clients = _context.Clientes.ToList();
            ViewData["Clientes"] = clients;
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
        public async Task<IActionResult> AddProduto(Guid VendaId, Guid ProdutoId, int Quantidade, Produto? produto)
        {

            var produtos = _context.Produtos.FindAsync(ProdutoId);
            ItemVenda itemVenda = new ItemVenda();
            itemVenda.VendaId = VendaId;
            itemVenda.Quantidade = Quantidade;
            itemVenda.ProdutoId = ProdutoId;

            itemVenda.ItemVendaId = Guid.NewGuid();
            itemVenda.ValorUnitario = produto.Preco;
            itemVenda.ValorTotal = Quantidade * produto.Preco;

            _context.ItensVenda.Add(itemVenda);
            await _context.SaveChangesAsync();

            var venda = await _context.Vendas.FindAsync(VendaId);
            venda.ValorTotal += itemVenda.ValorTotal;

            _context.Update(venda);
            await _context.SaveChangesAsync();

            List<ItemVenda> itens = await _context.ItensVenda.Where(i => i.VendaId == VendaId).Include(p => p.Produto).ToListAsync();
            ViewData["listaItens"] = itens;
            return RedirectToAction("Create", new { id = VendaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServico(Guid VendaIdS, Guid ServicoId, int QuantidadeS, string Observacao, decimal ValorServico)
        {

            var servicos = _context.Servicos.FindAsync(ServicoId).Result;
            ServicoVenda servicovenda = new ServicoVenda();
            servicovenda.VendaId = VendaIdS;
            servicovenda.Quantidade = QuantidadeS;
            servicovenda.Observacao = Observacao;
            servicovenda.ServicoId = ServicoId;

            servicovenda.ServicoVendaId = Guid.NewGuid();
            servicovenda.Servicocusto = ValorServico;
            servicovenda.ValorTotal = QuantidadeS * ValorServico;

            _context.ServicoVenda.Add(servicovenda);
            await _context.SaveChangesAsync();

            var venda = _context.Vendas.FindAsync(VendaIdS).Result;
            venda.ValorTotal += servicovenda.ValorTotal;


            _context.Update(venda);
            await _context.SaveChangesAsync();

            List<ServicoVenda> listaServicos = await _context.ServicoVenda.Where(i => i.VendaId == VendaIdS).Include(p => p.Servico).ToListAsync();
            ViewData["listaServicos"] = listaServicos;
            return RedirectToAction("Create", new { id = VendaIdS });
        }

        public async Task<IActionResult> BuscarClientes(string nome)
        {
            List<Cliente> clientes;

            if (string.IsNullOrEmpty(nome))
            {
                clientes = await _context.Clientes.OrderBy(c => c.Nome).ToListAsync();
            }
            else
            {
                clientes = await _context.Clientes
                                          .Where(c => c.Nome.Contains(nome))
                                          .OrderBy(c => c.Nome)
                                          .ToListAsync();
            }

            // Retorna apenas o conteúdo da tabela como PartialView
            return PartialView("_ClientesTable", clientes);
        }

        [HttpGet, ActionName("ImprimirVenda")]
        public IActionResult ImprimirVenda(Guid? id)
        {

            Venda vendaAtual = _context.Vendas.Where(v => v.VendaId == id).Include(c => c.Cliente).FirstOrDefaultAsync().Result;
            ViewData["vendaAtual"] = vendaAtual;
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Nome");
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "Nome");
            List<ItemVenda> itens = null;
            List<ServicoVenda> servicos = null;
            if (vendaAtual != null)
            {
                itens = _context.ItensVenda.Where(i => i.VendaId == vendaAtual.VendaId).Include(p => p.Produto).ToList();
                servicos = _context.ServicoVenda.Where(d => d.VendaId == vendaAtual.VendaId).Include(s => s.Servico).ToList();
            }
            ViewData["listaItens"] = itens;
            ViewData["listaServicos"] = servicos;
            List<Cliente> clients = _context.Clientes.ToList();
            ViewData["Clientes"] = clients;
            return View(vendaAtual);
        }


        private bool VendaExists(Guid id)
        {
            return _context.Vendas.Any(e => e.VendaId == id);
        }
    }
}
