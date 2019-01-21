
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using DreamFlights.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DreamFlights.Services
{
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        private readonly IServiceScopeFactory scopeFactory;

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;

            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(60));//(set the interval)设定时间间隔

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FlightContext>();
                //var city =  dbContext.Cities.Where(c => c.Name == "aaa").FirstOrDefault();
                //if(city != null)
                //{
                //    dbContext.Cities.Remove(city);
                //    dbContext.SaveChanges();
                //}     
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //get the suspended bookings(not finished) over 60 seconds, and reverse them
                        //获取寿命大于一分钟的临时订单,然后进行取消
                        foreach (var b in dbContext.BookingTransactions.Where(b => b.BookingDateTime.AddMinutes(1) < DateTime.Now).Where(b => b.Suspended == true ).GroupBy(x => new { x.Flight_ScheduleID, x.CabinType })
                        .Select(group => new {
                            Flight_ScheduleID = group.Key.Flight_ScheduleID,
                            CabinType = group.Key.CabinType,
                            Count = group.Count()
                        })
                    )
                        {
                            switch (b.CabinType.ToString())
                            {
                                case "Economy":
                                    dbContext.Flight_Schedules.Where(f => f.Flight_ScheduleID == b.Flight_ScheduleID).ToList().ForEach(f => f.Economy = f.Economy + b.Count);
                                    //remove multiple records in database
                                    //同时删除多个记录
                                    foreach (var bt in dbContext.BookingTransactions.Where(x => x.Flight_ScheduleID == b.Flight_ScheduleID))
                                    {
                                        dbContext.BookingTransactions.Remove(bt);
                                    }
                                    break;
                                case "Business":
                                    dbContext.Flight_Schedules.Where(f => f.Flight_ScheduleID == b.Flight_ScheduleID).ToList().ForEach(f => f.Business = f.Business + b.Count);
                                    foreach (var bt in dbContext.BookingTransactions.Where(x => x.Flight_ScheduleID == b.Flight_ScheduleID))
                                    {
                                        dbContext.BookingTransactions.Remove(bt);
                                    }
                                    break;
                                case "PremEconomy":
                                    dbContext.Flight_Schedules.Where(f => f.Flight_ScheduleID == b.Flight_ScheduleID).ToList().ForEach(f => f.PremEconomy = f.PremEconomy + b.Count);
                                    foreach (var bt in dbContext.BookingTransactions.Where(x => x.Flight_ScheduleID == b.Flight_ScheduleID))
                                    {
                                        dbContext.BookingTransactions.Remove(bt);
                                    }
                                    break;
                                case "First":
                                    dbContext.Flight_Schedules.Where(f => f.Flight_ScheduleID == b.Flight_ScheduleID).ToList().ForEach(f => f.First = f.First + b.Count);
                                    foreach (var bt in dbContext.BookingTransactions.Where(x => x.Flight_ScheduleID == b.Flight_ScheduleID))
                                    {
                                        dbContext.BookingTransactions.Remove(bt);
                                    }
                                    break;
                            }

                        }
                        //SaveChanges要在transaction.Complete()之前或者using (var transaction = new TransactionScope())之外
                        dbContext.SaveChanges();
                        transaction.Complete();                        
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }                
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
