using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Transbank.Patpass.PatpassByWebpay;

namespace transbanksdkdotnetrestexample.Controllers
{
    public class PatpassByWebpayController : Controller
    {

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PatpassByWebpayController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }
        public ActionResult Create()
        {
            var random = new Random();
            var buyOrder = random.Next(999999999).ToString();
            var sessionId = random.Next(999999999).ToString();
            var amount = random.Next(1000, 999999);

            var service_id = random.Next(999999999).ToString();
            var card_holder_id = random.Next(999999999).ToString();
            var card_holder_name = "Pepito";
            var card_holder_last_name1 = "Continuum";
            var card_holder_last_name2 = "Chile";
            var card_holder_mail = "pepito@continuum.cl";
            var cellphone_number = random.Next(999999999).ToString();
            var expiration_date = "2019-11-11";
            var commerce_mail = "Comercio@continuum.cl";
            var uf_flag = false;


            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("Commit", "PatpassByWebpay", null, Request.Scheme);
            var result = (new Transaction()).Create(buyOrder, sessionId, amount, returnUrl, service_id,
                card_holder_id, card_holder_name, card_holder_last_name1, card_holder_last_name2,
                card_holder_mail, cellphone_number, expiration_date, commerce_mail, uf_flag);


            ViewBag.BuyOrder = buyOrder;
            ViewBag.SessionId = sessionId;
            ViewBag.Amount = amount;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Result = result;
            ViewBag.Action = result.Url;
            ViewBag.Token = result.Token;

            ViewBag.service_id = service_id;
            ViewBag.card_holder_id = card_holder_id;
            ViewBag.card_holder_name = card_holder_name;
            ViewBag.card_holder_last_name1 = card_holder_last_name1;
            ViewBag.card_holder_last_name2 = card_holder_last_name2;
            ViewBag.card_holder_mail = card_holder_mail;
            ViewBag.cellphone_number = cellphone_number;
            ViewBag.expiration_date = expiration_date;
            ViewBag.commerce_mail = commerce_mail;
            ViewBag.uf_flag = uf_flag;


            return View();
        }


        public ActionResult Commit(String token_ws)
        {
            var result = (new Transaction()).Commit(token_ws);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            ViewBag.Token = token_ws;
            ViewBag.Action = urlHelper.Action("Status", "PatpassByWebpay", null, Request.Scheme);
            ViewBag.Result = result;
            ViewBag.SaveToken = token_ws;

            return View();
        }

        public ActionResult RequestStatus()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("Status", "PatpassByWebpay", null, Request.Scheme);
            return View();
        }

        [HttpPost]
        public ActionResult Status()
        {
            var token = Request.Form["token_ws"];
            var result = (new Transaction()).Status(token);

            ViewBag.Result = result;

            return View();
        }

    }
}