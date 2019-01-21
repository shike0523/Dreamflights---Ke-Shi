# Dreamflights---Ke-Shi
The major techniques I applied:
1. Dependency injection & recursion algorithm. 
Interface path: \DreamFlights\Interface\IScheduleSearching.cs
Dependency path: \DreamFlights\Dependencies

2. Back ground tasks that handle SQL records.
path: \DreamFlights\Services\TimedHostedService.cs

3. Data repository.
path: \DreamFlights\Data\Repositories\Flight_ScheduleRepository.cs,
\DreamFlights\Data\IFlight_ScheduleRepository.cs

4.Identity sign in management.
method path：\DreamFlights\Extensions\HttpContextExtensions.cs 
calling path: \DreamFlights\Views\Flight_Schedule\Index.cshtml

5. Temp data management.
method path: \DreamFlights\Extensions\TempDataExtensions.cs
calling path: \DreamFlights\Controllers\HomeController.cs (method "PopulateViewModel")

6.Internationaization(multilanguages):
path: \DreamFlights\Resources\

7. Ajax & json (Jquery)
calling path: \DreamFlights\Views\Shared\_LoginPartialNonChildWindow.cshtml
method path: \DreamFlights\Areas\Identity\Pages\Account\LoginNonChildWindow.cshtml.cs

8. Brower detection in C#
method path: \DreamFlights\UserAgent\
calling path: \DreamFlights\Areas\Identity\Pages\Account\Login.cshtml.cs

9. .NET Core Razor Page Authentication
path: \DreamFlights\Areas\
