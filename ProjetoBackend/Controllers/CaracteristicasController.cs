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
    public class CaracteristicasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CaracteristicasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Caracteristicas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Caracteristicas.ToListAsync());
        }

        // GET: Caracteristicas/Search?nome={clientName} (New Search Action)
        public async Task<IActionResult> Search(string nome)
        {
            if (string.IsNullOrEmpty(nome)) // Handle empty search term
            {
                return RedirectToAction(nameof(Index)); // Redirect to main Index
            }

            var caracteristicas = await _context.Caracteristicas.Where(c => c.Nome.Contains(nome)).ToListAsync();
            return View("Index", caracteristicas.OrderBy(c => c.Nome)); // Reuse the existing Index view
        }
        // GET: Caracteristicas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caracteristica = await _context.Caracteristicas
                .FirstOrDefaultAsync(m => m.CaracteristicaId == id);
            if (caracteristica == null)
            {
                return NotFound();
            }

            return View(caracteristica);
        }

        // GET: Caracteristicas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Caracteristicas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaracteristicaId,Nome")] Caracteristica caracteristica)
        {
            if (ModelState.IsValid)
            {
                caracteristica.CaracteristicaId = Guid.NewGuid();
                _context.Add(caracteristica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(caracteristica);
        }

        // GET: Caracteristicas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caracteristica = await _context.Caracteristicas.FindAsync(id);
            if (caracteristica == null)
            {
                return NotFound();
            }
            return View(caracteristica);
        }

        // POST: Caracteristicas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CaracteristicaId,Nome")] Caracteristica caracteristica)
        {
            if (id != caracteristica.CaracteristicaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(caracteristica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaracteristicaExists(caracteristica.CaracteristicaId))
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
            return View(caracteristica);
        }

        // GET: Caracteristicas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caracteristica = await _context.Caracteristicas
                .FirstOrDefaultAsync(m => m.CaracteristicaId == id);
            if (caracteristica == null)
            {
                return NotFound();
            }

            return View(caracteristica);
        }

        // POST: Caracteristicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var caracteristica = await _context.Caracteristicas.FindAsync(id);
            if (caracteristica != null)
            {
                _context.Caracteristicas.Remove(caracteristica);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CaracteristicaExists(Guid id)
        {
            return _context.Caracteristicas.Any(e => e.CaracteristicaId == id);
        }
    }
}
