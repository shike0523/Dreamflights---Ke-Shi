using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DreamFlights.Data;
using DreamFlights.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using DreamFlights.Extensions;

namespace DreamFlights.Controllers
{
    [Authorize(Roles = "ScheduleManager,SuperAdmin")]
    public class Flight_ScheduleController : Controller
    {
        private readonly FlightContext _context;

        public Flight_ScheduleController(FlightContext context)
        {
            _context = context;
        }

        // GET: Flight_Schedule
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilterDepartFrom,
            string currentFilterArriveTo,
            string currentFilterDepartDate,
            string currentFilterID,
            string searchStringDepartFrom,
            string searchStringArriveTo,
            string searchStringDepartDate,
            int searchID,
            int? page)
        {
            if (searchStringDepartFrom != null || searchStringArriveTo != null || searchStringDepartDate != null)
            {
                page = 1;
            }
            else
            {
                if (currentFilterDepartFrom == null) currentFilterDepartFrom = "";  
                if (currentFilterArriveTo == null) currentFilterArriveTo = "";
                if (currentFilterDepartDate == null) currentFilterDepartDate = "";
                searchStringDepartFrom = currentFilterDepartFrom;
                searchStringArriveTo = currentFilterArriveTo;
                searchStringDepartDate = currentFilterDepartDate;
                if(currentFilterID != null)searchID = int.Parse(currentFilterID);
            }

            ViewData["currentFilterDepartFrom"] = searchStringDepartFrom;  //这里的ViewBag储存的是当前的过滤器
            ViewData["currentFilterArriveTo"] = searchStringArriveTo;
            ViewData["currentFilterDepartDate"] = searchStringDepartDate;
            if(searchID != 0)ViewData["currentFilterID"] = searchID;

            //var schedules = _context.Flight_Schedules.Include(f => f.AirCraft).Include(f => f.Route).Include(f => f.Route.FromCity).Include(f => f.Route.ToCity).Take(300);
            var schedules = from s in _context.Flight_Schedules.Include(x => x.AirCraft).Include(f => f.Route).Include(f => f.Route.FromCity).Include(f => f.Route.ToCity)
                            select s;

            if(searchID != 0)
            {
                schedules = schedules.Where(f => f.Flight_ScheduleID == searchID);
            }else if (!String.IsNullOrEmpty(searchStringDepartFrom) || !String.IsNullOrEmpty(searchStringArriveTo) || !String.IsNullOrEmpty(searchStringDepartDate))
            {
                if (searchStringDepartFrom == null) searchStringDepartFrom = "";
                if (searchStringArriveTo == null) searchStringArriveTo = "";
                if (searchStringDepartDate == null)
                {
                    schedules = schedules
                    .Where(f => f.Route.FromCity.Name.Contains(searchStringDepartFrom) && f.Route.ToCity.Name.Contains(searchStringArriveTo)).Distinct().Take(300);
                }
                else
                {
                    schedules = schedules
                    .Where(f => f.Route.FromCity.Name.Contains(searchStringDepartFrom) && f.Route.ToCity.Name.Contains(searchStringArriveTo) && f.DepartDateTime.Date.ToString("d") == searchStringDepartDate).Distinct().Take(300);
                }
            }

            schedules = schedules.OrderBy(f => f.Flight_ScheduleID);
            //return View(await flightContext.OrderBy(f => f.Flight_ScheduleID).ToListAsync());
            int pageSize = 20;
            return View(await PaginatedList<Flight_Schedule>.CreateAsync(schedules.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Flight_Schedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight_Schedule = await _context.Flight_Schedules
                .Include(f => f.AirCraft)
                .Include(f => f.Route)
                .Include(f => f.Route.FromCity).Include(f => f.Route.ToCity)
                .FirstOrDefaultAsync(m => m.Flight_ScheduleID == id);
            if (flight_Schedule == null)
            {
                return NotFound();
            }

            ViewBag.Date = flight_Schedule.DepartDateTime.Date.ToString("d");

            return View(flight_Schedule);
        }

        // GET: Flight_Schedule/Create
        public IActionResult Create(int? id)
        {
            ViewData["AirCraftID"] = new SelectList(_context.AirCrafts, "AirCraftID", "AirCraftID");
            ViewData["RouteID"] = new SelectList(_context.Routes, "RouteID", "RouteID");

            ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name");
            ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name");
            return View();
        }

        // POST: Flight_Schedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int FromCityID, int ToCityID)
        {                       
            var flightSchedule = new Flight_Schedule { };
            if (await TryUpdateModelAsync<Flight_Schedule>(flightSchedule,
                "",
                c => c.Flight_ScheduleID, c => c.RouteID, c => c.DepartDateTime, c => c.ArriveDateTime, c => c.AirCraftID, c => c.Economy, c => c.Business, c => c.PremEconomy, c => c.First))
            {
                //if the route in this schedule does not exist, then reject to create
                if (!await _context.Routes.AnyAsync(r => r.FromCity.CityID == FromCityID && r.ToCity.CityID == ToCityID))  //get rid of the route that does not exist
                {
                    ViewBag.ErrorNoRoute = "No such route exists!";
                    ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name");
                    ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name");
                    return View(flightSchedule);
                }
                int RouteID = await _context.Routes.Where(r => r.FromCity.CityID == FromCityID && r.ToCity.CityID == ToCityID).Select(r => r.RouteID).FirstAsync();  //get the ID of route between2 cities
                flightSchedule.Route = await _context.Routes.Where(r => r.RouteID == RouteID).FirstAsync();  //set the route in the new Flight_Schedule
                try
                {
                    _context.Add(flightSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name");
            ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name");
            return View(flightSchedule);
        }

        //public async Task<IActionResult> Create([Bind("Flight_ScheduleID,RouteID,DepartDateTime,ArriveDateTime,AirCraftID,Economy,Business,PremEconomy,First")] Flight_Schedule flight_Schedule)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(flight_Schedule);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["AirCraftID"] = new SelectList(_context.AirCrafts, "AirCraftID", "AirCraftID", flight_Schedule.AirCraftID);
        //    ViewData["RouteID"] = new SelectList(_context.Routes, "RouteID", "RouteID", flight_Schedule.RouteID);
        //    return View(flight_Schedule);
        //}

        // GET: Flight_Schedule/Edit/5
        public async Task<IActionResult> Edit(int? id, int FromCityID, int ToCityID)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var flight_Schedule = await _context.Flight_Schedules.FindAsync(id);  //Find并不会赋值,
            var flight_Schedule = await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == id).Include(r => r.Route).FirstOrDefaultAsync();
            if (flight_Schedule == null)
            {
                return NotFound();
            }
            ViewData["AirCraftID"] = new SelectList(_context.AirCrafts, "AirCraftID", "AirCraftID", flight_Schedule.AirCraftID);
            ViewData["RouteID"] = new SelectList(_context.Routes, "RouteID", "RouteID", flight_Schedule.RouteID);

            ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name", flight_Schedule.Route.FromCityID);
            ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name", flight_Schedule.Route.ToCityID);

            return View(flight_Schedule);
        }

        // POST: Flight_Schedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int FromCityID, int ToCityID)
        {
            var flightSchedule = await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == id).Include(r => r.Route).FirstOrDefaultAsync();

            if (await TryUpdateModelAsync<Flight_Schedule>(flightSchedule,
                "",
                c => c.Flight_ScheduleID, c => c.RouteID, c => c.DepartDateTime, c => c.ArriveDateTime, c => c.AirCraftID, c => c.Economy, c => c.Business, c => c.PremEconomy, c => c.First))
            {
                //if the route in this schedule does not exist, then reject to edit
                if (!await _context.Routes.AnyAsync(r => r.FromCity.CityID == FromCityID && r.ToCity.CityID == ToCityID))
                {
                    ViewBag.ErrorNoRoute = "No such route exists!";
                    ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name");
                    ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name");
                    return View(flightSchedule);
                }
                int RouteID = await _context.Routes.Where(r => r.FromCity.CityID == FromCityID && r.ToCity.CityID == ToCityID).Select(r => r.RouteID).FirstAsync();

                flightSchedule.Route = await _context.Routes.Where(r => r.RouteID == RouteID).FirstAsync();
                try
                {
                    //_context.Add(flightSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FromCityID"] = new SelectList(_context.Cities, "CityID", "Name");
            ViewData["ToCityID"] = new SelectList(_context.Cities, "CityID", "Name");
            return View(flightSchedule);
        }
        //public async Task<IActionResult> Edit(int id, [Bind("Flight_ScheduleID,RouteID,DepartDateTime,ArriveDateTime,AirCraftID,Economy,Business,PremEconomy,First")] Flight_Schedule flight_Schedule)
        //{
        //    if (id != flight_Schedule.Flight_ScheduleID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(flight_Schedule);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!Flight_ScheduleExists(flight_Schedule.Flight_ScheduleID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["AirCraftID"] = new SelectList(_context.AirCrafts, "AirCraftID", "AirCraftID", flight_Schedule.AirCraftID);
        //    ViewData["RouteID"] = new SelectList(_context.Routes, "RouteID", "RouteID", flight_Schedule.RouteID);
        //    return View(flight_Schedule);
        //}

        // GET: Flight_Schedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight_Schedule = await _context.Flight_Schedules
                .Include(f => f.AirCraft)
                .Include(f => f.Route)
                .FirstOrDefaultAsync(m => m.Flight_ScheduleID == id);
            if (flight_Schedule == null)
            {
                return NotFound();
            }

            return View(flight_Schedule);
        }

        // POST: Flight_Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight_Schedule = await _context.Flight_Schedules.FindAsync(id);
            _context.Flight_Schedules.Remove(flight_Schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddSchedules()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task AddSchedules(string scheduleFromDate, string scheduleToDate)
        {
            TimeSpan timeAccessory = new TimeSpan(0, 0, 0);
            DateTime fromDate = Convert.ToDateTime(scheduleFromDate + " " + timeAccessory);
            DateTime toDate = Convert.ToDateTime(scheduleToDate + " " + timeAccessory);

            AirCraft airCraft = new AirCraft();
            for (var dt = fromDate; dt <= toDate; dt = dt.AddDays(1))
            {
                //var dt = DateTime.Now;
                foreach (var r in _context.Routes)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        DateTime tempDepartDateTime = Convert.ToDateTime(dt.Date.ToString("d") + " " + GenerateDepartTime());
                        DateTime tempArriveDateTime = tempDepartDateTime.Add(r.AveDuration);
                        int ramID = GenerateAirCraftID();
                        airCraft = _context.AirCrafts.Where(a => a.AirCraftID == ramID).FirstOrDefault();  //随机选取一个飞机
                        await _context.Flight_Schedules.AddAsync(new Flight_Schedule { RouteID = r.RouteID, DepartDateTime = tempDepartDateTime, ArriveDateTime = tempArriveDateTime, AirlineCode = RandomString(2) + RandomInt(3), AirCraftID = airCraft.AirCraftID, NetFaire = r.BasicFare, Economy = airCraft.EconomyCap, Business = airCraft.BusinessCap, PremEconomy = airCraft.PremEconomyCap, First = airCraft.FirstCap });
                    }
                    //_context.SaveChanges();
                }
            }
            _context.SaveChanges();
        }

        private bool Flight_ScheduleExists(int id)
        {
            return _context.Flight_Schedules.Any(e => e.Flight_ScheduleID == id);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> Validate()
        {
            //var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync("a"));
            await HttpContext.RefreshLoginAsync();
            return Json("");
        }

        TimeSpan GenerateDuration()  //随机生成飞行时间的函数
        {
            Random random = new Random();
            TimeSpan start = TimeSpan.FromHours(0.5);
            TimeSpan end = TimeSpan.FromHours(1.8);
            int maxMinutes = (int)((end - start).TotalMinutes);
            int minutes = random.Next(maxMinutes);
            return start.Add(TimeSpan.FromMinutes(minutes));
        }

        TimeSpan GenerateDepartTime()  //随机生成起飞时间的函数
        {
            Random random = new Random();
            TimeSpan start = TimeSpan.FromHours(6.5);
            TimeSpan end = TimeSpan.FromHours(21.75);
            int maxMinutes = (int)((end - start).TotalMinutes);
            int minutes = random.Next(maxMinutes);
            return start.Add(TimeSpan.FromMinutes(minutes));
        }

        int GenerateAirCraftID()
        {
            Random rand = new Random();
            int toSkip = rand.Next(0, _context.AirCrafts.Count());
            //直接返回一个属性(collumn)
            return _context.AirCrafts.Skip(toSkip).Take(1).First().AirCraftID;
        }

        //随机生成字母组合
        string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //随机生成数字组合
        string RandomInt(int length)
        {
            Random random = new Random();

            const string chars = "1234567890";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
