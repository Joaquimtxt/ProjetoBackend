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
    public class ComprasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComprasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Compras
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Compras.Include(v => v.Fornecedor);
            return View(await applicationDbContext.OrderBy(v => v.NotaFiscal).ToListAsync());
        }

        public async Task<IActionResult> Search(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                // Se o campo de pesquisa estiver vazio, redireciona para o Index com todas as vendas
                return RedirectToAction(nameof(Index));
            }

            // Normaliza o termo de pesquisa (removendo acentos e ignorando letras maiúsculas/minúsculas)
            var nomeNormalizado = searchString.RemoverAcentos().ToLower();

            // Carrega as compras do banco de dados, incluindo o Fornecedor
            var compras = await _context.Compras
                .Include(v => v.Fornecedor)
                .ToListAsync();

            // Filtra as vendas removendo acentos e ignorando maiúsculas/minúsculas
            var comprasFiltradas = compras
                .Where(v => v.Fornecedor.Nome.RemoverAcentos().ToLower().Contains(nomeNormalizado))
                .OrderByDescending(v => v.DataCompra)
                .ToList();

            // Adiciona o termo de pesquisa na ViewData para exibição
            ViewData["CurrentFilter"] = searchString;

            // Retorna a view Index com as vendas filtradas
            return View("Index", comprasFiltradas);
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .FirstOrDefaultAsync(m => m.CompraId == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // GET: Compras/Create
        public IActionResult Create(Guid? id)
        {
            Compra compraAtual = _context.Compras.Where(v => v.CompraId == id).Include(c => c.Fornecedor).FirstOrDefaultAsync().Result;
            ViewData["compraAtual"] = compraAtual;
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome");
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
            List<ItemCompra> itens = null;
            if (compraAtual != null)
            {
                itens = _context.ItensCompra.Where(i => i.CompraId == compraAtual.CompraId).Include(p => p.Produto).ToList();
            }
            ViewData["listaItens"] = itens;
            List<Fornecedor> supplier = _context.Fornecedores.ToList();
            ViewData["Fornecedor"] = supplier;
            return View();
        }

        // POST: Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompraId,NotaFiscal,FornecedorId,DataCompra,ValorTotal")] Compra compra)
        {
            if (ModelState.IsValid)
            {
                // Auto incremento da nota fiscal
                compra.NotaFiscal = _context.Compras.Count() + 1;
                compra.DataCompra = DateTime.Now;
                compra.CompraId = Guid.NewGuid();
                _context.Add(compra);
                await _context.SaveChangesAsync();

                ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome", compra.FornecedorId);
                ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
                List<ItemCompra> itensProdutos = await _context.ItensCompra.Where(i => i.CompraId == compra.CompraId).ToListAsync();
                ViewData["listaItens"] = itensProdutos;
                List<Fornecedor> fornecedores = _context.Fornecedores.ToList();
                ViewData["Fornecedor"] = fornecedores;
                ViewData["compraAtual"] = compra.CompraId;
                return RedirectToAction("Create", new { id = compra.CompraId });
            }
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome", compra.FornecedorId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
            List<ItemCompra> itens = await _context.ItensCompra.Where(i => i.CompraId == compra.CompraId).Include(p => p.Produto).ToListAsync();
            ViewData["listaItens"] = itens;
            List<Fornecedor> supplier = _context.Fornecedores.ToList();
            ViewData["Fornecedores"] = supplier;
            return View("Create", compra);
        }

        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome", compra.FornecedorId);
            return View(compra);
        }

        // POST: Compras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CompraId,NotaFiscal,FornecedorId,DataCompra,ValorTotal")] Compra compra)
        {
            if (id != compra.CompraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.CompraId))
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
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome", compra.FornecedorId);
            return View(compra);
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .FirstOrDefaultAsync(m => m.CompraId == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra != null)
            {
                _context.Compras.Remove(compra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProd(Guid id)
        {
            // Busca o item pelo ID
            var item = await _context.ItensCompra.FindAsync(id);

            if (item == null)
            {
                return NotFound(); // Retorna erro 404 se o item não for encontrado
            }

            // Subtrai o valor do item do total da venda
            var compra = await _context.Compras.FindAsync(item.CompraId);
            if (compra != null)
            {
                compra.ValorTotal -= item.ValorTotal;
                _context.Compras.Update(compra);
            }

            // Remove o item da venda
            _context.ItensCompra.Remove(item);
            await _context.SaveChangesAsync();

            // Redireciona de volta ao método Create com o ID da venda
            return RedirectToAction("Create", new { id = item.CompraId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduto(Guid CompraId, Guid ProdutoId, int Quantidade)
        {
            var produto = await _context.Produtos.FindAsync(ProdutoId); // Busca o produto pelo ID

            if (produto == null)
            {
                return NotFound(); // Retorna NotFound se o produto não for encontrado
            }

            ItemCompra itemCompra = new ItemCompra();
            itemCompra.CompraId = CompraId;
            itemCompra.Quantidade = Quantidade;
            itemCompra.ProdutoId = ProdutoId;
            itemCompra.ItemCompraId = Guid.NewGuid();
            itemCompra.ValorUnitario = produto.Preco; // Agora produto não será null
            itemCompra.ValorTotal = Quantidade * produto.Preco;

            _context.ItensCompra.Add(itemCompra);
            await _context.SaveChangesAsync();

            var compra = await _context.Compras.FindAsync(CompraId);
            compra.ValorTotal += itemCompra.ValorTotal;

            _context.Update(compra);
            await _context.SaveChangesAsync();

            List<ItemCompra> itens = await _context.ItensCompra.Where(i => i.CompraId == CompraId).Include(p => p.Compra).ToListAsync();
            ViewData["listaItens"] = itens;
            return RedirectToAction("Create", new { id = CompraId });
        }

        public async Task<IActionResult> BuscarFornecedores(string nome)
        {
            List<Fornecedor> fornecedores;

            if (string.IsNullOrEmpty(nome))
            {
                fornecedores = await _context.Fornecedores.OrderBy(c => c.Nome).ToListAsync();
            }
            else
            {
                fornecedores = await _context.Fornecedores.Where(c => c.Nome.Contains(nome)).OrderBy(c => c.Nome).ToListAsync();
            }

            // Retorna apenas o conteúdo da tabela como PartialView
            return PartialView("_FornecedoresTable", fornecedores);
        }

        private bool CompraExists(Guid id)
        {
            return _context.Compras.Any(e => e.CompraId == id);
        }
    }
}