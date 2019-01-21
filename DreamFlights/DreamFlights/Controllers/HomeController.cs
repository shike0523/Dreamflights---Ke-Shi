using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DreamFlights.Models;
using DreamFlights.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using DreamFlights.ViewModels;
using Microsoft.EntityFrameworkCore;
using DreamFlights.Dependencies;
using DreamFlights.Interface;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Localization;
using DreamFlights.Services;
using Newtonsoft.Json;
using DreamFlights.Extensions;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Transactions;

namespace DreamFlights.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlightContext _context;
        private readonly IScheduleSearching _ScheduleSearching;

        private readonly CookieControl _cookieControl;

        private readonly IEmailSender _emailSender;
        //引用Repository
        private readonly IFlight_ScheduleRepository _flight_ScheduleRepository;

        //用于登陆相关
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public List<candidateTrip> tripList = new List<candidateTrip>();

        public const string SessionKeyAdults = "_Adults";
        public const string SessionKeyYouths = "_Youth";
        public const string SessionKeyChildren = "_Children";
        public const string SessionKeyCabin = "_Cabin";
        public const string SessionKeyonewayOrReturn = "_onewayOrReturn";
        public const string SessionKeyTripList = "_TripList";
        public const string SessionKeyTripListReturn = "_TripListReturn";
        //Put total amount of candidate trips in oneway trip
        public const string SessionKeyTripTagTempDataOnewayTrip = "_TripTagTempDataOnewayTrip";
        //Put total amount of candidate trips in round trips
        public const string SessionKeyTripTagTempDataRoundTrip = "_TripTagTempDataRoundTrip";
        public const string SessionKeySuspendedIDDepart = "_SuspendedIDDepart";
        public const string SessionKeySuspendedIDReturn = "_SuspendedIDReturn";
        public const string SessionKeyBookingDateTime = "_BookingDateTime";

        public HomeController(FlightContext context, IScheduleSearching ScheduleSearching, IHttpContextAccessor httpContextAccessor, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IEmailSender emailSender, IFlight_ScheduleRepository flight_ScheduleRepository)
        {
            _context = context;
            _ScheduleSearching = ScheduleSearching;

            _cookieControl = new CookieControl(httpContextAccessor);

            _signInManager = signInManager;
            _userManager = userManager;

            _emailSender = emailSender;

            _flight_ScheduleRepository = flight_ScheduleRepository;
        }

        //for matching browser type
        private static readonly Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public async Task<IActionResult> Index(string userAgent)
        {
            await _emailSender.SendEmailAsync(
                    "shike0523@gmail.com",
                    "Visit alert!",
                    $"Some one open your dreamflights");

            //把cookie的值放入页面
            ViewBag.FromCityName = _cookieControl.Get("FromCityName");
            ViewBag.ToCityName = _cookieControl.Get("ToCityName");
            if(_cookieControl.Get("FromCityID") != null && _cookieControl.Get("ToCityID") != null)
            {
                ViewBag.FromCityID = int.Parse(_cookieControl.Get("FromCityID"));
                ViewBag.ToCityID = int.Parse(_cookieControl.Get("ToCityID"));
            }
            ViewBag.adults = _cookieControl.Get("adults");
            ViewBag.youths = _cookieControl.Get("youths");
            ViewBag.children = _cookieControl.Get("children");
            ViewBag.cabin= _cookieControl.Get("cabin");

            //如果session过期就返回主页,并以TempData["SessionTimeout"]作为session过期的信号
            if (TempData["SessionTimeout"] != null)
            {
                ViewBag.SessionTimeout = TempData["SessionTimeout"].ToString();
            }            

            //用于登陆的panel
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewBag.ExternalLogins = ExternalLogins;

            //用于登陆的panel
            string returnUrl = null;
            returnUrl = returnUrl ?? Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;

            //detect the brower type and redirect to the corresponding view
            userAgent = Request.Headers["User-Agent"];
            UserAgent.UserAgent ua = new UserAgent.UserAgent(userAgent);
            ViewBag.userAgent = ua.Browser.IsMobileDevice.ToString();
            if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
            {
                return View("IndexMobile");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Specification of this demo website";

            //这个函数"SendEmailAsync"有Azure和本地服务器两个版本的设定
            await _emailSender.SendEmailAsync(
                    "shike0523@gmail.com",
                    "Reset Password",
                    $"Please reset your password by");

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";           

            return View();
        }

        public IActionResult ExternalLoginSuceed()
        {
            return View();
        }

        public async Task<IActionResult> FaceBookLoginChildWindow()
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewBag.ExternalLogins = ExternalLogins;

            string returnUrl = null;
            returnUrl = returnUrl ?? Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        public async Task<IActionResult> FaceBookLoginChildWindow2()
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewBag.ExternalLogins = ExternalLogins;

            string returnUrl = null;
            returnUrl = returnUrl ?? Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        public IActionResult SearchWaiting()
        {
            return View();
        }

        public async Task<IActionResult> Search(string userAgent, string FromCityName, string ToCityName, int FromCityID, int ToCityID, string startTime, string returnTime, int adults, int youths, int children, string cabin, string onewayOrReturn)
        {
            //防止在这一步时切换语言文化后出现报错(表单的值为空)
            if(FromCityName != null)
            {
                //把页面表单提交传过来的值放入cookie长期保存
                _cookieControl.Set("FromCityName", FromCityName, 1000);
                _cookieControl.Set("ToCityName", ToCityName, 1000);
                _cookieControl.Set("FromCityID", FromCityID.ToString(), 1000);
                _cookieControl.Set("ToCityID", ToCityID.ToString(), 1000);
                _cookieControl.Set("adults", adults.ToString(), 1000);
                _cookieControl.Set("youths", youths.ToString(), 1000);
                _cookieControl.Set("children", children.ToString(), 1000);
                _cookieControl.Set("cabin", cabin, 1000);
                _cookieControl.Set("startTime", startTime, 1000);
                if(returnTime != null)
                _cookieControl.Set("returnTime", returnTime, 1000);
            }
            else
            {
                FromCityName = _cookieControl.Get("FromCityName");
                ToCityName = _cookieControl.Get("ToCityName");
                FromCityID = Int32.Parse(_cookieControl.Get("FromCityID"));
                ToCityID = Int32.Parse(_cookieControl.Get("ToCityID"));
                adults = Int32.Parse(_cookieControl.Get("adults"));
                youths = Int32.Parse(_cookieControl.Get("youths"));
                children = Int32.Parse(_cookieControl.Get("children"));
                cabin = _cookieControl.Get("cabin");
                onewayOrReturn = "return";
                startTime = _cookieControl.Get("startTime");
                returnTime = _cookieControl.Get("returnTime");
            }           

            //清除上一次的请求时遗留下的session,防止数据错乱
            HttpContext.Session.Clear();

            var departViewModel = new List<CandidateTripVM>();  //传给view的
            var returnViewModel = new List<CandidateTripVM>();  //传给view的

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyAdults)))
            {
                HttpContext.Session.SetInt32(SessionKeyAdults, adults);
                HttpContext.Session.SetInt32(SessionKeyYouths, youths);
                HttpContext.Session.SetInt32(SessionKeyChildren, children);
                HttpContext.Session.SetString(SessionKeyCabin, cabin);
                HttpContext.Session.SetString(SessionKeyonewayOrReturn, onewayOrReturn);
            }

            ViewBag.cabin = cabin;
            //传递oneway或者return的信号
            ViewBag.onewayOrReturn = onewayOrReturn;

            ViewBag.stops0 = 0;
            ViewBag.stops1 = 1;
            ViewBag.stops2 = 2;

            //slider的初始值
            ViewBag.timeEarliestFromDepart = 0;
            ViewBag.timeLatiestFromDepart = 1439;
            ViewBag.timeEarliestFromArrive = 0;
            ViewBag.timeLatiestFromArrive = 1439;
            ViewBag.timeEarliestToDepart = 0;
            ViewBag.timeLatiestToDepart = 1439;
            ViewBag.timeEarliestToArrive = 0;
            ViewBag.timeLatiestToArrive = 1439;

            //_ScheduleSearching = new ScheduleSearching(_context);
            var departCandidateTrip = new List<CandidateTrip>();            
            departCandidateTrip = await _ScheduleSearching.SearchAsync(FromCityID, ToCityID, startTime, adults, youths, children, cabin);
            PopulateViewModel(departViewModel, departCandidateTrip);

            if (onewayOrReturn == "return")
            {
                var returnCandidateTrip = new List<CandidateTrip>();
                returnCandidateTrip = await _ScheduleSearching.SearchAsync(ToCityID, FromCityID, returnTime, adults, youths, children, cabin);
                PopulateViewModel(returnViewModel, returnCandidateTrip);
            }
            
            //整个List<CandidateTripVM>的ViewModel放入ViewBag中
            ViewBag.Trips = departViewModel;
            ViewBag.TripsReturn = returnViewModel;
            //return RedirectToAction(nameof(Index));

            //存储slider filter标题(Take off:/Landing:)
            ViewBag.FromCityName = _cookieControl.Get("FromCityName");
            ViewBag.ToCityName = _cookieControl.Get("ToCityName");

            userAgent = Request.Headers["User-Agent"];
            UserAgent.UserAgent ua = new UserAgent.UserAgent(userAgent);
            ViewBag.userAgent = ua.Browser.IsMobileDevice.ToString();
            if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
            {
                return View("SearchMobile");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Book(string r, string Return, int totalPriceDepart, int totalPriceReturn)
        {
            int suspendedID;

            //如果session过期就返回主页,并以TempData["SessionTimeout"]作为session过期的信号,if中的条件是某个session(随机选取)是否还存在
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyAdults)))
            {
                TempData["SessionTimeout"] = "SessionTimeout";
                return RedirectToAction("Index");
            }

            //if culture is changed at this page
            if(r != null)
            {
                HttpContext.Session.SetString(SessionKeyTripList, r);
                if(HttpContext.Session.GetString(SessionKeyonewayOrReturn) == "return")
                {
                    HttpContext.Session.SetString(SessionKeyTripListReturn, Return);
                }               
            }
            else
            {
                r = HttpContext.Session.GetString(SessionKeyTripList);
                if (HttpContext.Session.GetString(SessionKeyonewayOrReturn) == "return")
                {
                    Return = HttpContext.Session.GetString(SessionKeyTripListReturn);
                }                
            }

            var passengerTatal = HttpContext.Session.GetInt32(SessionKeyAdults) + HttpContext.Session.GetInt32(SessionKeyYouths) + HttpContext.Session.GetInt32(SessionKeyChildren);           

            string cabin = HttpContext.Session.GetString(SessionKeyCabin);
            //ViewBag.schedules = tripListDepart;
            ViewBag.adults = HttpContext.Session.GetInt32(SessionKeyAdults);
            ViewBag.youths = HttpContext.Session.GetInt32(SessionKeyYouths);
            ViewBag.children = HttpContext.Session.GetInt32(SessionKeyChildren);
            ViewBag.cabin = HttpContext.Session.GetString(SessionKeyCabin);
            //传递oneway或者return的信号
            ViewBag.onewayOrReturn = HttpContext.Session.GetString(SessionKeyonewayOrReturn);
            

            DateTime bookingDateTime = DateTime.Now;
            //string bookingDateTimeString = bookingDateTime.ToString();
            //HttpContext.Session.SetString(SessionKeyBookingDateTime, bookingDateTime.ToString());
            

            List<int> tripListDepart = r.Trim().Split(' ').Select(Int32.Parse).ToList();
            //返回ID被包含在"tripList"里面的Flight_Schedules的记录(".Where(x => tripList.Contains(x.Flight_ScheduleID))")
            ViewBag.departSchedules = await _context.Flight_Schedules.Where(x => tripListDepart.Contains(x.Flight_ScheduleID)).Include(x => x.Route.FromCity).Include(x => x.Route.ToCity).OrderBy(x => x.DepartDateTime).ToListAsync();
            suspendedID = _flight_ScheduleRepository.InitialBooking(tripListDepart, cabin, passengerTatal.Value, bookingDateTime);
            HttpContext.Session.SetInt32(SessionKeySuspendedIDDepart, suspendedID);
            await _flight_ScheduleRepository.SaveAsync();

            //对return的订票数据进行处理
            if (HttpContext.Session.GetString(SessionKeyonewayOrReturn) == "return")
            {
                List<int> tripListReturn = Return.Trim().Split(' ').Select(Int32.Parse).ToList();
                ViewBag.returnSchedules = await _context.Flight_Schedules.Where(x => tripListReturn.Contains(x.Flight_ScheduleID)).Include(x => x.Route.FromCity).Include(x => x.Route.ToCity).OrderBy(x => x.DepartDateTime).ToListAsync();
                suspendedID = _flight_ScheduleRepository.InitialBooking(tripListReturn, cabin, passengerTatal.Value, bookingDateTime);
                HttpContext.Session.SetInt32(SessionKeySuspendedIDReturn, suspendedID);
            }

            if (HttpContext.Session.GetString(SessionKeyonewayOrReturn) == "return")
            {
                ViewBag.totalPrice = totalPriceDepart + totalPriceReturn;
            }
            else
            {
                ViewBag.totalPrice = totalPriceDepart;
            }
                                                
            await _flight_ScheduleRepository.SaveAsync();
                       
            return View();
        }

        public async Task<IActionResult> ConfirmBook(string[] lastName, string[] firstName, string[] ID, string[] email)
        {
            //如果session过期就返回主页,并以TempData["SessionTimeout"]作为session过期的信号
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyAdults)))
            {
                TempData["SessionTimeout"] = "SessionTimeout";
                return RedirectToAction("Index");
            }

            if (HttpContext.Session.GetString(SessionKeyCabin) == null)
            {
                return NotFound();
            }

            string cabin = HttpContext.Session.GetString(SessionKeyCabin);

            //get passanger total amount
            var passangers = HttpContext.Session.GetInt32(SessionKeyAdults) + HttpContext.Session.GetInt32(SessionKeyYouths) + HttpContext.Session.GetInt32(SessionKeyChildren);

            ViewBag.adults = HttpContext.Session.GetInt32(SessionKeyAdults);
            ViewBag.youths = HttpContext.Session.GetInt32(SessionKeyYouths);
            ViewBag.children = HttpContext.Session.GetInt32(SessionKeyChildren);

            ViewBag.tripList = HttpContext.Session.GetString(SessionKeyTripList);
            ViewBag.tripListReturn = HttpContext.Session.GetString(SessionKeyTripListReturn);
            List<int> tripList = HttpContext.Session.GetString(SessionKeyTripList).Trim().Split(' ').Select(Int32.Parse).ToList();
            List<int> tripListReturn = HttpContext.Session.GetString(SessionKeyTripListReturn).Trim().Split(' ').Select(Int32.Parse).ToList();

            //get depart and arrive city
            ViewBag.depart = await _context.Flight_Schedules.Include(x => x.Route.FromCity).Where(f => f.Flight_ScheduleID == tripList.First()).Select(f => f.Route.FromCity.Name).FirstOrDefaultAsync();
            ViewBag.arrive = await _context.Flight_Schedules.Include(x => x.Route.FromCity).Where(f => f.Flight_ScheduleID == tripList.Last()).Select(f => f.Route.ToCity.Name).FirstOrDefaultAsync();

            string depart = await _context.Flight_Schedules.Include(x => x.Route.FromCity).Where(f => f.Flight_ScheduleID == tripList.First()).Select(f => f.Route.FromCity.Name).FirstOrDefaultAsync();
            string arrive = await _context.Flight_Schedules.Include(x => x.Route.FromCity).Where(f => f.Flight_ScheduleID == tripList.Last()).Select(f => f.Route.ToCity.Name).FirstOrDefaultAsync();

            DateTime departTime = await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == tripList.First()).Select(f => f.DepartDateTime).FirstOrDefaultAsync();
            string departTimeString = departTime.ToString();
            DateTime arriveTime = await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == tripList.First()).Select(f => f.ArriveDateTime).FirstOrDefaultAsync();
            string arriveTimeString = arriveTime.ToString();

            

            ////change the booked seats number according the cabin
            //switch (cabin)
            //{
            //    case "Economy":
            //        foreach (var t in tripList)
            //        {
            //            await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ForEachAsync(f => f.Economy = f.Economy - passangers.Value);
            //        }
            //        break;
            //    case "Business":
            //        foreach (var t in tripList)
            //        {
            //            await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ForEachAsync(f => f.Business = f.Business - passangers.Value);
            //        }
            //        break;
            //    case "PremEconomy":
            //        foreach (var t in tripList)
            //        {
            //            await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ForEachAsync(f => f.PremEconomy = f.PremEconomy - passangers.Value);
            //        }
            //        break;
            //    case "First":
            //        foreach (var t in tripList)
            //        {
            //            await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ForEachAsync(f => f.First = f.First - passangers.Value);
            //        }
            //        break;
            //}

            List<Passenger> passengerList = await _flight_ScheduleRepository.CreatPassengers(lastName, firstName, ID);
            await _flight_ScheduleRepository.SaveAsync();

            int suspendedIDDepart = HttpContext.Session.GetInt32(SessionKeySuspendedIDDepart).HasValue ? HttpContext.Session.GetInt32(SessionKeySuspendedIDDepart).Value : 0;
            int suspendedIDReturn = HttpContext.Session.GetInt32(SessionKeySuspendedIDReturn).HasValue ? HttpContext.Session.GetInt32(SessionKeySuspendedIDReturn).Value : 0;
            //DateTime bookingDateTime = DateTime.Parse(HttpContext.Session.GetString(SessionKeyBookingDateTime));

            //如果用户没有登陆,那么不执行"_userManager.GetUserAsync(User)"语句,不然系统会报错
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                await _flight_ScheduleRepository.UpdateSeatsAsync(tripList, cabin, passangers.Value, user.UserName, passengerList, suspendedIDDepart);
                await _flight_ScheduleRepository.UpdateSeatsAsync(tripListReturn, cabin, passangers.Value, user.UserName, passengerList, suspendedIDReturn);
            }
            else
            {
                await _flight_ScheduleRepository.UpdateSeatsAsync(tripList, cabin, passangers.Value, null, passengerList, suspendedIDDepart);
                await _flight_ScheduleRepository.UpdateSeatsAsync(tripListReturn, cabin, passangers.Value, null, passengerList, suspendedIDReturn);
            }
            
            //await _flight_ScheduleRepository.SaveAsync();
            try
            {
                await _flight_ScheduleRepository.SaveAsync();
                //await _context.SaveChangesAsync();
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }

            string emailMessage = "<div>" + depart + ": " + departTimeString + "; " + arrive + ": " + arriveTimeString + "</div>";
            int i = 0;
            foreach (var n in lastName)
            {
                emailMessage = emailMessage + "<div>" + "Passenger " + i + 1 + " Name: " + n + " " + firstName[i] + "ID:" + ID[i] + ". " + "</div>";
                    i++;
            }

            foreach (var e in email)
            {
                await _emailSender.SendEmailAsync(
                    e,
                    "Booking success",
                    emailMessage);
            }

            HttpContext.Session.Clear();
            return View();
        }

        //AutoComplete in the index page
        public async Task<IActionResult> GetCityNameAuto(string Prefix)
        {
            var c = await _context.Cities.Where(p => p.Name.StartsWith(Prefix)).ToArrayAsync();
            return Json(c);
        }

        public IActionResult FilterForm(string userAgent, int timeEarliestFromDepart, int timeLatiestFromDepart, int timeEarliestToDepart, int timeLatiestToDepart, int timeEarliestFromArrive, int timeLatiestFromArrive, int timeEarliestToArrive, int timeLatiestToArrive, int stops0, int stops1, int stops2)
        {
            ViewBag.stops0 = stops0;
            ViewBag.stops1 = stops1;
            ViewBag.stops2 = stops2;

            //设置页面刷新后slider的值(<input id="timeEarliestFromDepart" name="timeEarliestFromDepart" value="@ViewBag.timeEarliestFromDepart">)(values: [$("#timeEarliestFromDepart").val(), $("#timeLatiestFromDepart").val()],)
            ViewBag.timeEarliestFromDepart = timeEarliestFromDepart;
            ViewBag.timeLatiestFromDepart = timeLatiestFromDepart;
            ViewBag.timeEarliestFromArrive = timeEarliestFromArrive;
            ViewBag.timeLatiestFromArrive = timeLatiestFromArrive;
            ViewBag.timeEarliestToDepart = timeEarliestToDepart;
            ViewBag.timeLatiestToDepart = timeLatiestToDepart;
            ViewBag.timeEarliestToArrive = timeEarliestToArrive;
            ViewBag.timeLatiestToArrive = timeLatiestToArrive;

            //Convert parameters(timeEarliestFromDepart...) passed from sliders to Time Type 
            var timeEarliestFromDepartConverted = GetTime(timeEarliestFromDepart);
            var timeLatiestFromDepartConverted = GetTime(timeLatiestFromDepart);
            var timeEarliestFromArriveConverted = GetTime(timeEarliestFromArrive);
            var timeLatiestFromArriveConverted = GetTime(timeLatiestFromArrive);
            var timeEarliestToDepartConverted = GetTime(timeEarliestToDepart);
            var timeLatiestToDepartConverted = GetTime(timeLatiestToDepart);
            var timeEarliestToArriveConverted = GetTime(timeEarliestToArrive);
            var timeLatiestToArriveConverted = GetTime(timeLatiestToArrive);

            //Get the total amount of candidate trips(tripTagTempData) from session
            var tripTagTempData = HttpContext.Session.GetInt32(SessionKeyTripTagTempDataOnewayTrip);

            var ViewModel = new List<CandidateTripVM>();  //存储出发航班
            var ViewModelReturn = new List<CandidateTripVM>();  //存储返程航班

            bool timeEarliest;
            bool timeLatiest;
            bool stops;

            for (var tt = 0; tt <= HttpContext.Session.GetInt32(SessionKeyTripTagTempDataOnewayTrip); tt++)
            {
                //Get all the candidate trips from TempData without deleting them(Peek)
                var tempCTVMSerialized = (string)TempData.Peek(tt.ToString());
                var tempCTVM = JsonConvert.DeserializeObject<CandidateTripVM>(tempCTVMSerialized);

                //将表单提交过来的时间数据(timeEarliestFromDepartConverted...)与日期合并后方便与TempData中的时间数据(tempCTVM.DepartTime)进行大小对比
                timeEarliest = tempCTVM.DepartTime >= DateTime.Parse(tempCTVM.DepartTime.Date.ToString("d") + " " + timeEarliestFromDepartConverted) && tempCTVM.ArrivetTime >= DateTime.Parse(tempCTVM.ArrivetTime.Date.ToString("d") + " " + timeEarliestFromArriveConverted);
                timeLatiest = tempCTVM.DepartTime <= DateTime.Parse(tempCTVM.DepartTime.Date.ToString("d") + " " + timeLatiestFromDepartConverted) && tempCTVM.ArrivetTime <= DateTime.Parse(tempCTVM.ArrivetTime.Date.ToString("d") + " " + timeLatiestFromArriveConverted);
                stops = tempCTVM.stopList.Count() - 2 == stops0 || tempCTVM.stopList.Count() - 2 == stops1 || tempCTVM.stopList.Count() - 2 == stops2;

                //只对满足以上3组条件的数据进行输出(放入ViewModel/ViewModelReturn中)
                if (timeEarliest && timeLatiest && stops)
                {
                    ViewModel.Add(tempCTVM);
                }
                
            }
            //viewbag存储的只是地址,所以要分成Trips和TripsReturn,同时此处不要用clear函数来清除ViewModel,不然前者会被后者TripsReturn覆盖掉
            ViewBag.Trips = ViewModel;

            
            for (var tt = HttpContext.Session.GetInt32(SessionKeyTripTagTempDataOnewayTrip) + 1; tt <= HttpContext.Session.GetInt32(SessionKeyTripTagTempDataRoundTrip); tt++)
            {
                //Get all the candidate trips from TempData without deleting them(Peek)
                var tempCTVMSerialized = (string)TempData.Peek(tt.ToString());
                var tempCTVM = JsonConvert.DeserializeObject<CandidateTripVM>(tempCTVMSerialized);

                timeEarliest = tempCTVM.DepartTime >= DateTime.Parse(tempCTVM.DepartTime.Date.ToString("d") + " " + timeEarliestToDepartConverted) && tempCTVM.ArrivetTime >= DateTime.Parse(tempCTVM.ArrivetTime.Date.ToString("d") + " " + timeEarliestToArriveConverted);
                timeLatiest = tempCTVM.DepartTime <= DateTime.Parse(tempCTVM.DepartTime.Date.ToString("d") + " " + timeLatiestToDepartConverted) && tempCTVM.ArrivetTime <= DateTime.Parse(tempCTVM.ArrivetTime.Date.ToString("d") + " " + timeLatiestToArriveConverted);
                stops = tempCTVM.stopList.Count() - 2 == stops0 || tempCTVM.stopList.Count() - 2 == stops1 || tempCTVM.stopList.Count() - 2 == stops2;

                if (timeEarliest && timeLatiest && stops)
                {
                    ViewModelReturn.Add(tempCTVM);
                }

            }

            ViewBag.TripsReturn = ViewModelReturn;

            //存储slider filter标题(Take off:/Landing:)
            ViewBag.FromCityName = _cookieControl.Get("FromCityName");
            ViewBag.ToCityName = _cookieControl.Get("ToCityName");

            //return View("Search");
            userAgent = Request.Headers["User-Agent"];
            UserAgent.UserAgent ua = new UserAgent.UserAgent(userAgent);
            ViewBag.userAgent = ua.Browser.IsMobileDevice.ToString();
            if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
            {
                return View("SearchMobile");
            }
            else
            {
                return View("Search");
            }
        }


        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public class candidateTrip
        {
            public List<int> RouteList { get; set; }
            public List<string> stopList { get; set; }
        }

        //time converter, whhich converts a number from time slider bar to a time format
        public string GetTime(int timeLimit)
        {
            var minutes = timeLimit % 60;
            var hours = timeLimit / 60 % 24;

            var hoursS = hours.ToString();
            var minutesS = minutes.ToString();

            var time = "";
            minutesS = minutesS + "";
            if (hours < 12)
            {
                time = "AM";
            }
            else
            {
                time = "PM";
            }
            if (hours == 0)
            {
                hours = 12;
            }
            if (hours > 12)
            {
                hours = hours - 12;
            }
            if (minutesS.Length == 1)
            {
                minutesS = "0" + minutesS;
            }
            hoursS = hours.ToString();
            return hoursS + ":" + minutesS + " " + time;
        }

        public void PopulateViewModel(List<CandidateTripVM> departViewModel, List<CandidateTrip> departCandidateTrip)
        {
            //tripTagTempData serves as an index in storing candidate trips in TempData;SessionKeyTripTagTempDataOnewayTrip存储备选出发航班表的的总数,SessionKeyTripTagTempDataRoundTrip-SessionKeyTripTagTempDataOnewayTrip为备选返程航班表的的总数
            int tripTagTempData = HttpContext.Session.GetInt32(SessionKeyTripTagTempDataOnewayTrip).HasValue ? HttpContext.Session.GetInt32(SessionKeyTripTagTempDataOnewayTrip).Value + 1 : 0;
            int viewModelIndex = 0;
            foreach (var trip in departCandidateTrip)
            {
                departViewModel.Add(new CandidateTripVM   //ViewModel只能添加对应类型的(某个ViewModel类型)成员，这里是将tripList的成员转给ViewModel的CandidateTrip
                {
                    RouteList = trip.RouteList,
                    stopList = trip.stopList,
                    DepartTime = trip.DepartTime,
                    ArrivetTime = trip.ArrivetTime,
                    TotalPrice = trip.TotalPrice,
                    EntertainmentList = trip.EntertainmentList,
                    WiFiList = trip.WiFiList,
                    ACPowerList = trip.ACPowerList,
                    AirCraftModelList = trip.AirCraftModelList,
                    AirlineCodeList = trip.AirlineCodeList
                });
                //Put candidate trips in temp data, using method in "TempDataExtensions.cs, each trip TempData has a tag 'tripTagTempData.ToString()' to be identified"
                TempData.Put(tripTagTempData.ToString(), departViewModel.ElementAt(viewModelIndex));
                tripTagTempData++;
                viewModelIndex++;
            }
            if (HttpContext.Session.GetInt32(SessionKeyTripTagTempDataOnewayTrip).HasValue)
            {
                //Put total amount of candidate round trips(tripTagTempData) in session
                HttpContext.Session.SetInt32(SessionKeyTripTagTempDataRoundTrip, tripTagTempData - 1);
            }
            else
            {
                //Put total amount of candidate oneway trips(tripTagTempData) in session
                HttpContext.Session.SetInt32(SessionKeyTripTagTempDataOnewayTrip, tripTagTempData - 1);
            }
            
        }

        public async void UpdateSeatsAsync(List<int> tripList, string cabin, int passangers)
        {
            switch (cabin)
            {
                case "Economy":
                    foreach (var t in tripList)
                    {
                        await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ForEachAsync(f => f.Economy = f.Economy - passangers);                        
                    }
                    break;
                case "Business":
                    foreach (var t in tripList)
                    {
                        await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ForEachAsync(f => f.Business = f.Business - passangers);
                    }
                    break;
                case "PremEconomy":
                    foreach (var t in tripList)
                    {
                        await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ForEachAsync(f => f.PremEconomy = f.PremEconomy - passangers);
                    }
                    break;
                case "First":
                    foreach (var t in tripList)
                    {
                        await _context.Flight_Schedules.Where(f => f.Flight_ScheduleID == t).ForEachAsync(f => f.First = f.First - passangers);
                    }
                    break;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }
        }

        ///// <summary>  
        ///// Get the cookie  
        ///// </summary>  
        ///// <param name="key">Key </param>  
        ///// <returns>string value</returns>  
        //public string Get(string key)
        //{
        //    return Request.Cookies[key];
        //}
        ///// <summary>  
        ///// set the cookie  
        ///// </summary>  
        ///// <param name="key">key (unique indentifier)</param>  
        ///// <param name="value">value to store in cookie object</param>  
        ///// <param name="expireTime">expiration time</param>  
        //public void Set(string key, string value, int? expireTime)
        //{
        //    CookieOptions option = new CookieOptions();
        //    if (expireTime.HasValue)
        //        option.Expires = DateTime.Now.AddHours(expireTime.Value);
        //    else
        //        option.Expires = DateTime.Now.AddMilliseconds(10);
        //    Response.Cookies.Append(key, value, option);
        //}
        ///// <summary>  
        ///// Delete the key  
        ///// </summary>  
        ///// <param name="key">Key</param>  
        //public void Remove(string key)
        //{
        //    Response.Cookies.Delete(key);
        //}
    }
}
