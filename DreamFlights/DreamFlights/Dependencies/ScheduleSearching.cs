using DreamFlights.Data;
using DreamFlights.Interface;
using DreamFlights.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DreamFlights.Dependencies
{
    public class ScheduleSearching : IScheduleSearching
    {
        private readonly FlightContext _context;

        public ScheduleSearching(FlightContext context)
        {
            _context = context;
        }

        public List<CandidateTrip> tripList = new List<CandidateTrip>();

        List<Flight_Schedule> flightContext;

        List<int> citiesNameInTrip = new List<int>();
        DateTime departTime;

        public async Task<List<CandidateTrip>> SearchAsync(int FromCityID, int ToCityID, string takeoffDate, int adults, int Youths, int Children, string cabin)
        {
            //reset the value of "tripList", preventing the tripList of depart and return in the same tripList
            //清空上一次计算(出发航班)的tripList,防止出发和返程的tripList放在同一个tripList里面
            tripList.Clear();
            string tempTrip = "";
            int searchCount = 0;

            int passengers = adults + Youths + Children;            

            switch (cabin)
            {
                case "Economy":
                    flightContext = await _context.Flight_Schedules
                .Where(x => x.DepartDateTime.Date.ToString("d") == takeoffDate)
                .Where(x => x.DepartDateTime > DateTime.Now)
                .Where(x => x.Economy > passengers)
                .Include(f => f.AirCraft).Include(f => f.Route).Include(f => f.Route.FromCity).ToListAsync();
                    break;
                case "Business":
                    flightContext = await _context.Flight_Schedules
                .Where(x => x.DepartDateTime.Date.ToString("d") == takeoffDate)
                .Where(x => x.DepartDateTime > DateTime.Now)
                .Where(x => x.Business > passengers)
                .Include(f => f.AirCraft).Include(f => f.Route).Include(f => f.Route.FromCity).ToListAsync();
                    break;
                case "PremEconomy":
                    flightContext = await _context.Flight_Schedules
                .Where(x => x.DepartDateTime.Date.ToString("d") == takeoffDate)
                .Where(x => x.DepartDateTime > DateTime.Now)
                .Where(x => x.PremEconomy > passengers)
                .Include(f => f.AirCraft).Include(f => f.Route).Include(f => f.Route.FromCity).ToListAsync();
                    break;
                case "First":
                    flightContext = await _context.Flight_Schedules
                .Where(x => x.DepartDateTime.Date.ToString("d") == takeoffDate)
                .Where(x => x.DepartDateTime > DateTime.Now)
                .Where(x => x.First > passengers)
                .Include(f => f.AirCraft).Include(f => f.Route).Include(f => f.Route.FromCity).ToListAsync();
                    break;
            }

            await SearchSchedules(FromCityID, ToCityID, tempTrip, searchCount, DateTime.Now);
            //return await Task.Run(() => tripList);
            CalculatePrice(adults, Youths, Children, cabin);

            return tripList;   //返回一个list
        }

        public async Task SearchSchedules(int FromCityID, int ToCityID, string trip, int searchCount, DateTime NextEarliestDepartLimit)
        {
            if (searchCount != 0)
            {
                Flight_Schedule flight_Schedule =  flightContext.Where(x => x.Route.FromCityID == FromCityID)
                    .Where(x => x.Route.ToCityID == ToCityID)
                    .Where(x => x.DepartDateTime > NextEarliestDepartLimit)  //(filter out the fights that too late to book)确保只获取晚于当前时间的航班
                    .Where(x => x.DepartDateTime.Date.ToString("d") == NextEarliestDepartLimit.Date.ToString("d"))
                    .OrderBy(x => x.DepartDateTime)  //(get the earliest flight from the candidate flights)取起飞时间最早的转机航班
                    .FirstOrDefault();               //(get the earliest flight from the candidate flights)取起飞时间最早的转机航班
                if (flight_Schedule != null)
                {
                    //trip.Add(route.RouteID);
                    trip = trip + " " + flight_Schedule.Flight_ScheduleID.ToString();
                    CandidateTrip cR2 = new CandidateTrip
                    {
                        //List<>类型一定要初始化
                        RouteList = new List<int>(),
                        stopList = new List<string>(),   
                        EntertainmentList = new List<bool>(),
                        WiFiList = new List<bool>(),
                        ACPowerList = new List<bool>(),
                        AirCraftModelList = new List<string>(),
                        AirlineCodeList = new List<string>(),
                    };

                    cR2.DepartTime = departTime;
                    cR2.ArrivetTime = flight_Schedule.ArriveDateTime;
                    cR2.RouteList = trip.Trim(' ').Split(' ').Select(Int32.Parse).ToList();
                    AddCities(cR2.stopList, cR2.RouteList, cR2.EntertainmentList, cR2.WiFiList, cR2.ACPowerList, cR2.AirCraftModelList,cR2.AirlineCodeList);  //将RouteList中一路经过并转机的城市放入stopList中
                    //cR2.Entertainment = flight_Schedule.AirCraft.Entertainment;
                    //cR2.ACPower = flight_Schedule.AirCraft.ACPower;

                    tripList.Add(cR2);
                    
                }
                else if (searchCount < 2)
                {
                    //”citiesNameInTrip“ prevents the searching route of recursion algorithm from being a dead loop(one route contains 2 identical cities)
                    //”citiesNameInTrip“防止搜索路线成为一个环(同一条路线中包含两个相同的城市)
                    citiesNameInTrip.Add(FromCityID);
                    //iterate all the adjacent cities, using recursion algorithm in each adjacent city
                    //依次遍历当前节点的所有相邻节点,并逐个进行递归(SearchSchedules)
                    foreach (var t in  flightContext.Where(x => x.Route.FromCityID == FromCityID)
                        .Where(x => !citiesNameInTrip.Contains(x.Route.ToCityID))
                        .Where(x => x.DepartDateTime > NextEarliestDepartLimit)
                        .Where(x => x.DepartDateTime.Date.ToString("d") == NextEarliestDepartLimit.Date.ToString("d"))  //(ensure all the results are in the same day)保证在同一天
                        .GroupBy(x => x.Route.FromCity)
                        .Select(x => x.OrderBy(y => y.DepartDateTime)).Select(x => x.First()))   //CAUTION: 取起飞时间最早的转机航班
                    {
                            //List<int> tripTemp = trip;
                            //tripTemp.Add(t.RouteID);
                            string tripTemp = trip + " " + t.Flight_ScheduleID.ToString();
                            searchCount++;
                            await SearchSchedules(t.Route.ToCityID, ToCityID, tripTemp, searchCount, t.ArriveDateTime.AddHours(1)); //(ensure there is at least 1 hour in a stop)确保转乘航班间有1个小时的间隔
                            searchCount--;
                        }

                    citiesNameInTrip.Remove(FromCityID);
                }
            }
            else
            {
                IEnumerable<Flight_Schedule> flight_Schedules =  flightContext.Where(x => x.Route.FromCityID == FromCityID).Where(x => x.Route.ToCityID == ToCityID);

                if (flight_Schedules != null)
                {
                    
                    foreach (var flight_Schedule in flight_Schedules)
                    {
                        CandidateTrip cR = new CandidateTrip
                        {
                            RouteList = new List<int>(),
                            stopList = new List<string>(),
                            EntertainmentList = new List<bool>(),
                            WiFiList = new List<bool>(),
                            ACPowerList = new List<bool>(),
                            AirCraftModelList = new List<string>(),
                            AirlineCodeList = new List<string>(),
                        };
                        cR.RouteList.Add(flight_Schedule.Flight_ScheduleID);
                        AddCities(cR.stopList, cR.RouteList, cR.EntertainmentList, cR.WiFiList, cR.ACPowerList, cR.AirCraftModelList, cR.AirlineCodeList);
                        cR.DepartTime = flight_Schedule.DepartDateTime;
                        cR.ArrivetTime = flight_Schedule.ArriveDateTime;
                        //cR.Entertainment = flight_Schedule.AirCraft.Entertainment;
                        //cR.ACPower = flight_Schedule.AirCraft.ACPower;
                        tripList.Add(cR);
                    }
                }

                citiesNameInTrip.Add(FromCityID);
                //iterate all the adjacent cities, using recursion algorithm in each adjacent city
                //依次遍历当前节点的所有相邻节点,并逐个进行递归(SearchSchedules)
                foreach (var t in  flightContext.Where(x => x.Route.FromCityID == FromCityID).Where(x => x.Route.ToCityID != ToCityID))
                {
                    //store the depart time of the 1st city as the depart time of a trip
                    //保存第一站的出发时间作为最终路线的出发时间
                    departTime = t.DepartDateTime;
                    string tripTemp = trip + " " + t.Flight_ScheduleID.ToString();
                    searchCount++;
                    await SearchSchedules(t.Route.ToCityID, ToCityID, tripTemp, searchCount, t.ArriveDateTime.AddHours(1));
                    searchCount--;
                }
                citiesNameInTrip.Remove(FromCityID);
            }
        }

        public void AddCities(List<string> stopList, List<int> routeNumbers, List<bool> EntertainmentList, List<bool> WiFiList, List<bool> ACPowerList, List<string> AirCraftModelList, List<string> AirlineCodeList)  //参数stopList是其物理地址;该函数如果返回List<string>类型,那么返回值也是其物理地址,函数调用完毕马上变成空值而无法使用
        {
            stopList.Add(flightContext.Where(r => r.Flight_ScheduleID == routeNumbers.First()).FirstOrDefault().Route.FromCity.Name);
            
            foreach (var rN in routeNumbers)
            {
                var flightScheduleTemp = flightContext.Where(r => r.Flight_ScheduleID == rN).FirstOrDefault();
                var cityName = flightScheduleTemp.Route.ToCity.Name;
                EntertainmentList.Add(flightScheduleTemp.AirCraft.Entertainment);
                WiFiList.Add(flightScheduleTemp.AirCraft.WiFi);
                ACPowerList.Add(flightScheduleTemp.AirCraft.ACPower);
                AirCraftModelList.Add(flightScheduleTemp.AirCraft.AirCraftModel);
                stopList.Add(cityName);
                AirlineCodeList.Add(flightScheduleTemp.AirlineCode);
            }
        }
        public void CalculatePrice(int adults, int Youths, int Children, string cabin)
        {
            double price = 0;
            foreach(var t in tripList)
            {
                if (t.RouteList.Count == 1)
                {
                    //Get a "NetFaire" of a specific Flight_ScheduleID to the variable "price"
                    price = flightContext.Where(f => f.Flight_ScheduleID == t.RouteList.First()).Select(x => x.NetFaire).FirstOrDefault();
                    price = price * adults + price * Youths * 0.8 + price * Children * 0.5;
                    switch (cabin)
                    {
                        case "eco":
                            price = price * 1;
                            break;
                        case "busi":
                            price = price * 1.3;
                            break;
                        case "preE":
                            price = price * 1.5;
                            break;
                        case "first":
                            price = price * 2;
                            break;
                    }
                    t.TotalPrice = Convert.ToInt32(price);
                    price = 0;
                }
                else
                {
                    foreach (var r in t.RouteList)
                    {
                        //Get a "NetFaire" of a specific Flight_ScheduleID to the variable "price"
                        price = price + flightContext.Where(f => f.Flight_ScheduleID == r).Select(x => x.NetFaire).FirstOrDefault();
                    }
                    price = price / t.RouteList.Count;
                    price = price * adults + price * Youths * 0.8 + price * Children * 0.5;

                    switch(cabin)
                    {
                        case "eco":
                            price = price * 1;
                            break;
                        case "busi":
                            price = price * 1.3;
                            break;
                        case "preE":
                            price = price * 1.5;
                            break;
                        case "first":
                            price = price * 2;
                            break;
                    }

                    t.TotalPrice = Convert.ToInt32(price);
                    price = 0;
                }
                

            }
        }
    }
}
