using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Transbank.Common;
using Transbank.Patpass.PatpassComercio;

namespace transbanksdkdotnetrestexample.Controllers
{


    public class PatpassComercioController : Controller
    {

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PatpassComercioController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public ActionResult Start()
        {
            var random = new Random();
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("Start", "PatpassComercio", null, Request.Scheme);
            var url = urlHelper.Action("Finish", "PatpassComercio", null, Request.Scheme);

            var name = "Pepito";
            var f_lastname = "Perez";
            var s_lastname = "Perez";
            var rut = "18439979-2";
            var service_id = random.Next(999999999).ToString();
            var final_url = urlHelper.Action("Index", "Home",  null, Request.Scheme);
            var commerce_code = IntegrationCommerceCodes.PATPASS_COMERCIO;
            var max_amount = 1000;
            var phone_number = random.Next(999999999).ToString();
            var mobile_number = random.Next(999999999).ToString();
            var patpass_name = "nombre del patpass";
            var client_email = "persona@persona.cl";
            var commerce_email = "comercio@comercio.cl";
            var address = "huerfanos 101";
            var city = "Santiago";




            var result = (new Inscription()).Start(
                url: url,
                name: name,
                lastName: f_lastname,
                secondLastName: s_lastname,
                rut: rut,
                serviceId: service_id,
                finalUrl: final_url,
                maxAmount: max_amount,
                phone: phone_number,
                cellPhone: mobile_number,
                patpassName: patpass_name,
                personEmail: client_email,
                commerceEmail: commerce_email,
                address: address,
                city: city);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Action = result.Url;
            ViewBag.Token = result.Token;
            ViewBag.Result = result;
            
            ViewBag.Url = url;
            ViewBag.Name = name;
            ViewBag.F_Lastname = f_lastname;
            ViewBag.S_Lastname = s_lastname;
            ViewBag.Rut = rut;
            ViewBag.Service_id = service_id;
            ViewBag.Final_url = final_url;
            ViewBag.Commerce_code = commerce_code;
            ViewBag.Max_amount = max_amount;
            ViewBag.Phone_number = phone_number;
            ViewBag.Mobile_number = mobile_number;
            ViewBag.Patpass_name = patpass_name;
            ViewBag.Client_name = client_email;
            ViewBag.Commerce_email = commerce_email;
            ViewBag.Address = address;
            ViewBag.City = city;

            return View();
        }

        public ActionResult Status()
        {
            var token = Request.Form["token_ws"];
            var tokenComercio = Request.Form["tokenComercio"];
            var result = (new Inscription()).Status(tokenComercio);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            ViewBag.Action = result.UrlVoucher;
            ViewBag.Token = tokenComercio;
            ViewBag.Status = result.Status;

            return View();
        }

        public ActionResult Finish()
        {
            var token = Request.Form["j_token"];
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("Status", "PatpassComercio", null, Request.Scheme);
            ViewBag.Token = token;
            return View();
        }


    }
}