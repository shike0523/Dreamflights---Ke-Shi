using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFlights.Data;
using DreamFlights.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DreamFlights.Areas.Identity.Pages.Account.Manage
{
    public class MyBookingsModel : PageModel
    {
        private readonly FlightContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public MyBookingsModel(FlightContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<BookingTransaction> BookingTransaction { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            BookingTransaction = await _context.BookingTransactions
                //使得Flight_Schedule的关联表(Route,City(FromCity,ToCity)等)可以随同MyBookingsModel一起传递到page上
                .Include(b => b.Flight_Schedule.Route.FromCity).Include(b => b.Flight_Schedule.Route.ToCity)
                .Where(b => b.CustomerEmail == user.UserName)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}