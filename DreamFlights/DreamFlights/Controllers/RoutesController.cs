using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DreamFlights.Data;
using DreamFlights.Models;
using Microsoft.AspNetCore.Authorization;
using DreamFlights.Extensions;

namespace DreamFlights.Controllers
{
    [Authorize(Roles = "RouteManager,ScheduleManager")]    
    public class RoutesController : Controller
    {
        private readonly FlightContext _context;

        public RoutesController(FlightContext context)
        {
            _context = context;
        }

        // GET: Routes
        public async Task<IActionResult> Index()
        {
            var flightContext = _context.Routes.Include(r => r.FromCity).Include(r => r.ToCity);
            return View(await flightContext.ToListAsync());
        }

        // GET: Routes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes
                .Include(r => r.FromCity)
                .Include(r => r.ToCity)
                .FirstOrDefaultAsync(m => m.RouteID == id);
            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        // GET: Routes/Create
        public IActionResult Create()
        {
            ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name");
            ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name");
            return View();
        }

        // POST: Routes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteID,FromCityID,ToCityID,BasicFare,AveDuration")] Route route)
        {
            if (ModelState.IsValid)
            {
                _context.Add(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name", route.FromCityID);
            ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name", route.ToCityID);
            return View(route);
        }

        // GET: Routes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes.FindAsync(id);
            if (route == null)
            {
                return NotFound();
            }
            ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name", route.FromCityID);
            ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name", route.ToCityID);
            return View(route);
        }

        // POST: Routes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteID,FromCityID,ToCityID,BasicFare,AveDuration")] Route route)
        {
            if (id != route.RouteID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(route);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteExists(route.RouteID))
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
            ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name", route.FromCityID);
            ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name", route.ToCityID);
            return View(route);
        }

        // GET: Routes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes
                .Include(r => r.FromCity)
                .Include(r => r.ToCity)
                .FirstOrDefaultAsync(m => m.RouteID == id);
            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        // POST: Routes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (await _context.Flight_Schedules.AnyAsync(r => r.Route.RouteID == id))
            {
                var scheduleToBeDeleted = await _context.Routes.Where(c => c.RouteID == id).FirstOrDefaultAsync();
                ViewBag.constrainError = "Cannot delete because it has constraint with some schedules";
                //return RedirectToAction(nameof(Index));
                return View(scheduleToBeDeleted);
            }

            var route = await _context.Routes.FindAsync(id);
            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RouteExists(int id)
        {
            return _context.Routes.Any(e => e.RouteID == id);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> Validate()
        {
            //var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync("a"));
            await HttpContext.RefreshLoginAsync();
            return Json("");
        }
    }
}
