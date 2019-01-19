using DreamFlights.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Data
{
    public class DbInitializer
    {
        public static void Initialize(FlightContext context)
        {
            context.Database.EnsureCreated();

            if (context.Routes.Any())
            {
                ////Adjusted for Deploying to Azure 
                //context.Database.EnsureCreated();
                context.SaveChanges();

                return;   // DB has been seeded
            }

            var cities = new City[]
            {
                new City{Name="Sydney", AirportName="Kingsford Smith", PostCode = 0000, StateOrTerritory = StateOrTerritory.NSW, State = "NSW" },
                new City{Name="Melbourne", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.VIC, State = "VIC" },
                new City{Name="Brisbane", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD, State = "QLD" },
                new City{Name="Perth", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.WA, State = "WA" },
                new City{Name="Adelaide", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.SA, State = "SA" },
                new City{Name="Gold Coast", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD, State = "QLD" },
                new City{Name="Cairns", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD, State = "QLD" },
                new City{Name="Canberra", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.ACT, State = "ACT" },
                new City{Name="Hobart", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.TAS, State = "TAS" },
                new City{Name="Darwin", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NT, State = "NT" },
                new City{Name="Townsville", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD, State = "QLD" },
                new City{Name="Launceston", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.TAS, State = "TAS" },
                new City{Name="Newcastle", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NSW, State = "NSW" },
                new City{Name="Mackay", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD , State = "QLD"},
                new City{Name="Sunshine Coast", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD, State = "QLD" },
                new City{Name="Rockhampton", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD, State = "QLD" },
                new City{Name="Alice Springs", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NT, State = "NT" },
                new City{Name="Karratha", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.WA, State = "WA" },
                new City{Name="Hamilton Island", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD, State = "QLD" },
                new City{Name="Broome", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.WA, State = "WA" },
                new City{Name="Coffs Harbour", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NSW, State = "NSW" },
                new City{Name="Port Hedland", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.WA, State = "WA" },
                new City{Name="Albury", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NSW, State = "NSW" },
                new City{Name="Ayers Rock", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NT, State = "NT" },
                new City{Name="Ballina", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NSW, State = "NSW" },
                new City{Name="Kalgoorlie	", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.WA, State = "WA" },
                new City{Name="Proserpine", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.QLD, State = "QLD" },
                new City{Name="Port Macquarie", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NSW, State = "NSW" },
                new City{Name="Wagga Wagga", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.NSW, State = "NSW" },
                new City{Name="Mildura", AirportName="", PostCode = 0000, StateOrTerritory = StateOrTerritory.VIC, State = "VIC" }
            };

            foreach (City city in cities)
            {
                context.Cities.Add(city);
            }
            context.SaveChanges();

            //var routes = new Route[]
            //{
            //    new Route{FromCityID=cities.Single(c => c.Name == "Sydney").CityID, ToCityID=cities.Single(c => c.Name == "Melbourne").CityID },
            //    new Route{FromCityID=cities.Single(c => c.Name == "Melbourne").CityID, ToCityID=cities.Single(c => c.Name == "Hobart").CityID },
            //    new Route{FromCityID=cities.Single(c => c.Name == "Sydney").CityID, ToCityID=cities.Single(c => c.Name == "Hobart").CityID },
            //    new Route{FromCityID=cities.Single(c => c.Name == "Melbourne").CityID, ToCityID=cities.Single(c => c.Name == "Perth").CityID },
            //    new Route{FromCityID=cities.Single(c => c.Name == "Perth").CityID, ToCityID=cities.Single(c => c.Name == "Hobart").CityID },
            //    new Route{FromCityID=cities.Single(c => c.Name == "Sydney").CityID, ToCityID=cities.Single(c => c.Name == "Perth").CityID }
            //    //new Route{FromCityID=cities.Single(c => c.Name == "Melbourne").CityID, ToCityID=cities.Single(c => c.Name == "Hobart").CityID }
            //};

            //foreach (Route route in routes)
            //{
            //    context.Routes.Add(route);
            //}


            //foreach(var FromCity in cities)
            //    foreach(var ToCity in cities)
            //    {
            //        if(FromCity != ToCity)
            //        {
            //            context.Routes.Add(new Route { FromCityID = FromCity.CityID, ToCityID = ToCity.CityID });
            //        }
            //    }

            //context.SaveChanges();    

            context.AirCrafts.Add(new AirCraft { AirCraftModel = "ATR 72-500", EconomyCap = 60, BusinessCap = 20, PremEconomyCap = 20, FirstCap = 10, Entertainment = false, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Airbus A320", EconomyCap = 120, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 20, Entertainment = true, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Boeing 777-300ER", EconomyCap = 250, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 30, Entertainment = true, ACPower = true, WiFi = true });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "ATR 72-500", EconomyCap = 60, BusinessCap = 20, PremEconomyCap = 20, FirstCap = 10, Entertainment = false, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Airbus A320", EconomyCap = 120, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 20, Entertainment = true, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Boeing 777-300ER", EconomyCap = 250, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 30, Entertainment = true, ACPower = true, WiFi = true });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "ATR 72-500", EconomyCap = 60, BusinessCap = 20, PremEconomyCap = 20, FirstCap = 10, Entertainment = false, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Airbus A320", EconomyCap = 120, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 20, Entertainment = true, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Boeing 777-300ER", EconomyCap = 250, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 30, Entertainment = true, ACPower = true, WiFi = true });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "ATR 72-500", EconomyCap = 60, BusinessCap = 20, PremEconomyCap = 20, FirstCap = 10, Entertainment = false, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Airbus A320", EconomyCap = 120, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 20, Entertainment = true, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Boeing 777-300ER", EconomyCap = 250, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 30, Entertainment = true, ACPower = true, WiFi = true });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "ATR 72-500", EconomyCap = 60, BusinessCap = 20, PremEconomyCap = 20, FirstCap = 10, Entertainment = false, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Airbus A320", EconomyCap = 120, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 20, Entertainment = true, ACPower = true, WiFi = false });
            context.AirCrafts.Add(new AirCraft { AirCraftModel = "Boeing 777-300ER", EconomyCap = 250, BusinessCap = 40, PremEconomyCap = 40, FirstCap = 30, Entertainment = true, ACPower = true, WiFi = true });
            context.SaveChanges();

            Random rnd = new Random();
            foreach (var c in cities)
            {
                if (c.Name != "Sydney")
                {
                    context.Routes.Add(new Route { FromCityID = cities.Single(t => t.Name == "Sydney").CityID, ToCityID = c.CityID,AveDuration = GenerateDuration(), BasicFare = rnd.Next(200, 301)});
                    context.Routes.Add(new Route { FromCityID = c.CityID, ToCityID = cities.Single(t => t.Name == "Sydney").CityID, AveDuration = GenerateDuration(), BasicFare = rnd.Next(200, 301) });
                }

                if (c.Name != "Melbourne" && c.Name != "Sydney")
                {
                    {
                        context.Routes.Add(new Route { FromCityID = cities.Single(t => t.Name == "Melbourne").CityID, ToCityID = c.CityID, AveDuration = GenerateDuration(), BasicFare = rnd.Next(200, 301) });
                        context.Routes.Add(new Route { FromCityID = c.CityID, ToCityID = cities.Single(t => t.Name == "Melbourne").CityID, AveDuration = GenerateDuration(), BasicFare = rnd.Next(200, 301) });
                    }
                }
            }
            context.SaveChanges();

            //int AirCraftsTotal = context.AirCrafts.Count();
            AirCraft airCraft = new AirCraft();
            //DateTime类型的加减
            for (var dt = DateTime.Now; dt < DateTime.Now.AddDays(5); dt = dt.AddDays(1))
            {
                
                //var dt = DateTime.Now;
                foreach (var r in context.Routes)
                {
                    for(var i = 0; i<4; i++)
                    {
                        DateTime tempDepartDateTime = Convert.ToDateTime(dt.Date.ToString("d") + " " + GenerateDepartTime());
                        DateTime tempArriveDateTime = tempDepartDateTime.Add(r.AveDuration);
                        int ramID = GenerateAirCraftID();
                        airCraft = context.AirCrafts.Where(a => a.AirCraftID == ramID).FirstOrDefault();  //随机选取一个飞机
                        context.Flight_Schedules.Add(new Flight_Schedule { RouteID = r.RouteID, DepartDateTime = tempDepartDateTime, ArriveDateTime = tempArriveDateTime, AirlineCode = RandomString(2) + RandomInt(3), AirCraftID = airCraft.AirCraftID, NetFaire = r.BasicFare, Economy = airCraft.EconomyCap, Business = airCraft.BusinessCap, PremEconomy = airCraft.PremEconomyCap, First = airCraft.FirstCap});
                    }
                }
            }
            context.SaveChanges();

            

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
                int toSkip = rand.Next(0, context.AirCrafts.Count());
                //直接返回一个属性(collumn)
                return context.AirCrafts.Skip(toSkip).Take(1).First().AirCraftID;
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

        public static void InitializeRoles(ApplicationDbContext context)  //初始化管理员角色
        {
            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}
