#pragma checksum "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e1f526870a8168e8d0ea8734d0dff927678bac98"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_ConfirmBook), @"mvc.1.0.view", @"/Views/Home/ConfirmBook.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/ConfirmBook.cshtml", typeof(AspNetCore.Views_Home_ConfirmBook))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\_ViewImports.cshtml"
using DreamFlights;

#line default
#line hidden
#line 2 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\_ViewImports.cshtml"
using DreamFlights.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e1f526870a8168e8d0ea8734d0dff927678bac98", @"/Views/Home/ConfirmBook.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"765f2d1d6d65898ef58b0c959c718b41b2fffc74", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_ConfirmBook : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
  
    ViewData["Title"] = "ConfirmBook";

#line default
#line hidden
            BeginContext(49, 193, true);
            WriteLiteral("\r\n<div class=\"alert alert-success\">\r\n    <strong>Success!</strong> Your booking:\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-3\">\r\n            <h3>From: <span class=\"label label-default\">");
            EndContext();
            BeginContext(243, 14, false);
#line 10 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
                                                   Write(ViewBag.depart);

#line default
#line hidden
            EndContext();
            BeginContext(257, 116, true);
            WriteLiteral("</span></h3>\r\n        </div>\r\n        <div class=\"col-sm-3\">\r\n            <h3>To: <span class=\"label label-default\">");
            EndContext();
            BeginContext(374, 14, false);
#line 13 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
                                                 Write(ViewBag.arrive);

#line default
#line hidden
            EndContext();
            BeginContext(388, 145, true);
            WriteLiteral("</span></h3>\r\n        </div>\r\n    </div>\r\n    <h3>Passangers:</h3>\r\n    <button type=\"button\" class=\"btn btn-primary\">Adults <span class=\"badge\">");
            EndContext();
            BeginContext(534, 14, false);
#line 17 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
                                                                        Write(ViewBag.adults);

#line default
#line hidden
            EndContext();
            BeginContext(548, 95, true);
            WriteLiteral("</span></button>\r\n    <button type=\"button\" class=\"btn btn-success\">Youths <span class=\"badge\">");
            EndContext();
            BeginContext(644, 14, false);
#line 18 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
                                                                        Write(ViewBag.youths);

#line default
#line hidden
            EndContext();
            BeginContext(658, 96, true);
            WriteLiteral("</span></button>\r\n    <button type=\"button\" class=\"btn btn-danger\">Children <span class=\"badge\">");
            EndContext();
            BeginContext(755, 16, false);
#line 19 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
                                                                         Write(ViewBag.children);

#line default
#line hidden
            EndContext();
            BeginContext(771, 53, true);
            WriteLiteral("</span></button>\r\n</div>\r\n\r\n<h2>ConfirmBook</h2>\r\n<p>");
            EndContext();
            BeginContext(825, 14, false);
#line 23 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
Write(ViewBag.depart);

#line default
#line hidden
            EndContext();
            BeginContext(839, 232, true);
            WriteLiteral("</p>\r\n<h3>Dear employers:</h3>\r\n<h3>below are the IDs of schedules in this juerny, please use \"ScheduleManager@gmail.com\" to check the changes of database of Flight_Schedules(click the Flight_Schedules tab in the navi bar)</h3>\r\n<p>");
            EndContext();
            BeginContext(1072, 16, false);
#line 26 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
Write(ViewBag.tripList);

#line default
#line hidden
            EndContext();
            BeginContext(1088, 9, true);
            WriteLiteral("</p>\r\n<p>");
            EndContext();
            BeginContext(1098, 22, false);
#line 27 "C:\Users\lenovo\Documents\Visual Studio 2017\Projects\DreamFlights\DreamFlights\Views\Home\ConfirmBook.cshtml"
Write(ViewBag.tripListReturn);

#line default
#line hidden
            EndContext();
            BeginContext(1120, 4, true);
            WriteLiteral("</p>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
