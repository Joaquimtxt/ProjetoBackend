using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoBackend.Data;
using ProjetoBackend.Models;

namespace ProjetoBackend.Controllers
{
    [Authorize]
    public class ProdutosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos.ToListAsync();
            var applicationDbContext = _context.Produtos.Include(p => p.Categoria).Include(c => c.Caracteristica);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clientes/Search?nome={clientName} (New Search Action)
        public async Task<IActionResult> Search(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return RedirectToAction(nameof(Index));
            }

            // Remove acentos e converte para minúsculas no termo de pesquisa
            var nomeNormalizado = nome.RemoverAcentos().ToLower();

            // Recupera todos os produtos do banco
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(c => c.Caracteristica)
                .ToListAsync();

            // Filtra os produtos na memória removendo acentos e ignorando case
            var produtosFiltrados = produtos
                .Where(p => p.Nome.RemoverAcentos().ToLower().Contains(nomeNormalizado))
                .OrderBy(p => p.Nome)
                .ToList();

            return View("Index", produtosFiltrados);
        }


        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Categoria).Include(c => c.Caracteristica)
                .FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }


        // GET: Produtos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nome");
            ViewData["CaracteristicaId"] = new SelectList(_context.Caracteristicas, "CaracteristicaId", "Nome");
            return View();
        }

        // POST: Produtos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProdutoId,Nome,CodigoProduto,Preco,Estoque,tipoProduto,CategoriaId,CaracteristicaId")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                produto.ProdutoId = Guid.NewGuid();
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nome", produto.CategoriaId);
            ViewData["CaracteristicaId"] = new SelectList(_context.Caracteristicas, "CaracteristicaId", "Nome", produto.CaracteristicaId);
            return View(produto);
        }
        //Pesquisa a categoria
        public async Task<IActionResult> SearchByCategory(string nome)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria).Include(c => c.Caracteristica)
                .Where(p => p.Categoria.Nome == nome)
                .ToListAsync();
            return View("Index", produtos);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nome");
            ViewData["CaracteristicaId"] = new SelectList(_context.Caracteristicas, "CaracteristicaId", "Nome");
            return View(produto);
        }

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProdutoId,Nome,CodigoProduto,Preco,Estoque,tipoProduto,CategoriaId,CaracteristicaId")] Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.ProdutoId))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nome", produto.CategoriaId);
            ViewData["CaracteristicaId"] = new SelectList(_context.Caracteristicas, "CaracteristicaId", "Nome", produto.CaracteristicaId);
            return View(produto);
        }


        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(Guid id)
        {
            return _context.Produtos.Any(e => e.ProdutoId == id);
        }
        [HttpGet]
        public async Task<IActionResult> GetProdutoPorCodigo(Guid codigo)
        {
            if (codigo == null)
            {
                return BadRequest("Código do produto é necessário.");
            }

            var produto = await _context.Produtos
                .Where(cd => cd.ProdutoId == codigo)
                .Select(cd => new { cd.Nome, cd.Preco })
                .FirstOrDefaultAsync();

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            return Json(produto);
        }
    }
}
