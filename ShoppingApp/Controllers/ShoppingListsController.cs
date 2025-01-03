using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Models;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public class ShoppingListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShoppingLists
        public async Task<IActionResult> Index()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var shoppingLists = await _context.ShoppingList
                .Include(s => s.User)
                .Include(s => s.Product)
                .Where(s => s.UserID == userID) 
                .ToListAsync();

            return View(shoppingLists);
        }


        // GET: ShoppingLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingList
                .Include(s => s.Product)
                .Where(s => s.Id == id && s.UserID == userID)
                .FirstOrDefaultAsync();
            if (shoppingList == null)
            {
                return NotFound();
            }

            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", shoppingList.ProductId);
            return View(shoppingList);
        }

        // GET: ShoppingLists/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name");
            return View();
        }

        // POST: ShoppingLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId")] ShoppingList shoppingList)
        {
            shoppingList.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                _context.Add(shoppingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", shoppingList.ProductId);
            return View(shoppingList);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var shoppingList = await _context.ShoppingList
                .Where(s => s.Id == id && s.UserID == userID)
                .FirstOrDefaultAsync();

            if (id == null)
            {
                ModelState.AddModelError("", "Błąd, dane o tym id nie są w bazie.");
                return View(shoppingList);  // Zwróć widok z komunikatem
            }


            if (shoppingList == null)
            {
                ModelState.AddModelError("", "Nie masz uprawnień do edytowania tej listy zakupów.");
                return View(shoppingList);  // Zwróć widok z komunikatem
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", shoppingList.ProductId);
            return View(shoppingList);
        }

        // POST: ShoppingLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId")] ShoppingList shoppingList)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != shoppingList.Id)
            {
                return NotFound();
            }

            var existingSession = await _context.ShoppingList
                .Where(s => s.Id == id && s.UserID == userID)
                .FirstOrDefaultAsync();

            if (existingSession == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingListExists(shoppingList.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", shoppingList.ProductId);
            return View(shoppingList);
        }

        // GET: ShoppingLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingList = await _context.ShoppingList
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", shoppingList.ProductId);
            return View(shoppingList);
        }

        // POST: ShoppingLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingList = await _context.ShoppingList.FindAsync(id);
            if (shoppingList != null)
            {
                _context.ShoppingList.Remove(shoppingList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingListExists(int id)
        {
            return _context.ShoppingList.Any(e => e.Id == id);
        }




        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GenerateFamilyList()
        {
            // Zbiorcza lista zakupów
            var familyShoppingList = await _context.ShoppingList
                .Include(s => s.Product)
                .GroupBy(s => s.Product.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Count(),
                    TotalValue = g.Sum(s => s.Product.Value)
                })
                .ToListAsync();

            // Całkowita wartość zakupów
            var totalValue = await _context.ShoppingList
                .Include(s => s.Product)
                .SumAsync(s => s.Product.Value);

            // Lista zakupów poszczególnych użytkowników
            var userShoppingLists = await _context.ShoppingList
                .Include(s => s.Product)
                .Include(s => s.User)
                .GroupBy(s => s.User.UserName)
                .Select(g => new
                {
                    UserName = g.Key,
                    Items = g.Select(i => i.Product.Name).ToList(),
                    TotalUserValue = g.Sum(i => i.Product.Value)
                })
                .ToListAsync();

            ViewBag.TotalValue = totalValue;
            ViewBag.UserShoppingLists = userShoppingLists;

            return View(familyShoppingList);
        }

        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<IActionResult> ClearShoppingLists()
        {
            var allShoppingLists = _context.ShoppingList;
            _context.ShoppingList.RemoveRange(allShoppingLists);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GenerateFamilyList));
        }



    }
}
