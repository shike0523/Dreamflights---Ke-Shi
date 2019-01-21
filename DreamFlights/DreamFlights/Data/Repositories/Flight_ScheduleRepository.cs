using DreamFlights.Models;
using DreamFlights.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DreamFlights.Data.Repositories
{
    public class Flight_ScheduleRepository : IFlight_ScheduleRepository, IDisposable
    {
        private readonly FlightContext _context;

        public Flight_ScheduleRepository(FlightContext context)
        {
            _context = context;
        }

        public async Task<List<Passenger>> CreatPassengers(string[] lastName, string[] firstName, string[] ID)
        {
            List<Passenger> passengerList = new List<Passenger>();

            for(int i = 0; i < lastName.Length; i++)
            {
                Passenger passenger = new Passenger { LastName = lastName[i], FirstName = firstName[i], PersonalID = ID[i] };
                await _context.AddAsync(passenger);
                passengerList.Add(passenger);
            }

            return passengerList;
        }

        public async Task UpdateSeatsAsync(List<int> tripList, string cabin, int passangers, string userName, List<Passenger> passengersList, int suspendedID)
        {
            foreach (var t in tripList)
            {
                //await _context.BookingTransactions.Where(b => b.Flight_ScheduleID == t).ForEachAsync(b => { b.Suspended = false; b.CustomerEmail = userName; });
                int i = 0;
               
                foreach (var bookingTrans in await _context.BookingTransactions.Where(b => b.Flight_ScheduleID == t).Where(b => b.SuspendedID == suspendedID).ToListAsync())
                {
                    bookingTrans.Suspended = false;
                    bookingTrans.CustomerEmail = userName;
                    bookingTrans.PersonalID = passengersList.ElementAt(i).PersonalID;
                    i++;
                }
            }
        }

        public int InitialBooking(List<int> tripList, string cabin, int passangers, DateTime bookingDateTime)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    int suspendedID;
                    if (_context.BookingTransactions.Any())
                    {
                        suspendedID = _context.BookingTransactions.OrderByDescending(b => b.SuspendedID).Select(b => b.SuspendedID).FirstOrDefault() + 1;
                    }
                    else
                    {
                        IncrementCounter incrementCounter = new IncrementCounter();
                        suspendedID = incrementCounter.NextValue(); //生成一个自动递增的数
                    }                    
                    
                    //var bookingDateTime = DateTime.Now;
                    foreach (int tripNo in tripList)
                    {
                        Flight_Schedule flight_Schedule = _context.Flight_Schedules.Where(x => x.Flight_ScheduleID == tripNo).FirstOrDefault();
                        
                        for (int i = 0; i< passangers; i++)
                        {
                            //generate a specific booking transaction(BookingTransaction) for every passanger
                            //每一个passanger都单独生成一比业务记录(BookingTransaction)
                            BookingTransaction bookingTransaction = new BookingTransaction { BookingDateTime = bookingDateTime, DepartDateTime = flight_Schedule.DepartDateTime, Flight_ScheduleID = flight_Schedule.Flight_ScheduleID, PersonalID = "", SuspendedID = suspendedID, CabinType = (CabinType)Enum.Parse(typeof(CabinType), cabin), Suspended = true };
                            _context.Add(bookingTransaction);
                        }                        
                        //_context.SaveChanges();
                    }

                    //change the booked seats number according the cabin and the nuumber of passengers
                    switch (cabin)
                    {
                        case "Economy":
                            foreach (var t in tripList)
                            {
                                _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ToList().ForEach(f => f.Economy = f.Economy - passangers);
                            }
                            break;
                        case "Business":
                            foreach (var t in tripList)
                            {
                                _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ToList().ForEach(f => f.Business = f.Business - passangers);
                            }
                            break;
                        case "PremEconomy":
                            foreach (var t in tripList)
                            {
                                _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ToList().ForEach(f => f.PremEconomy = f.PremEconomy - passangers);
                            }
                            break;
                        case "First":
                            foreach (var t in tripList)
                            {
                                _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ToList().ForEach(f => f.First = f.First - passangers);
                            }
                            break;
                    }
                    //_context.SaveChanges();

                    transaction.Complete();
                    return suspendedID;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
