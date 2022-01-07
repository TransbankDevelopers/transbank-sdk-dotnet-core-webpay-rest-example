using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Transbank.Webpay.TransaccionCompleta;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace transbanksdkdotnetrestexample.Controllers
{
    public class TransaccionCompletaController : Controller
    {

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public TransaccionCompletaController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        #region Transaccion Completa
        public ActionResult Create()
        {
            var random = new Random();

            var buy_order = random.Next(999999999).ToString();
            var session_id = random.Next(9999999).ToString();
            var amount = 10000;
            var cvv = 123;
            var card_number = "4051885600446623";
            var card_expiration_date = "22/10";
            
            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("Installments", "TransaccionCompleta", null, Request.Scheme);


            var result = (new FullTransaction()).Create(
                buyOrder: buy_order,
                sessionId: session_id,
                amount: amount,
                cvv: cvv,
                cardNumber: card_number,
                cardExpirationDate: card_expiration_date);

            ViewBag.Action = returnUrl;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Token = result.Token;
            ViewBag.Result = result;

            ViewBag.BuyOrder = buy_order;
            ViewBag.SessionId = session_id;
            ViewBag.Amount = amount;
            ViewBag.Cvv = cvv;
            ViewBag.CardNumber = card_number;
            ViewBag.CardExpirationDate = card_expiration_date;

            return View();
        }
        
        [HttpPost]
        public ActionResult Installments()
        {
            var token = Request.Form["token_ws"];
            var installments_number = 10;
            ViewBag.Token = token;
            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.SaveToken = token;
            var returnUrl = urlHelper.Action("Commit", "TransaccionCompleta", null, Request.Scheme);
            ViewBag.Action = returnUrl;
            var result = (new FullTransaction()).Installments(
                token,
                installments_number);
            ViewBag.InstallmentsNumber = installments_number;
            ViewBag.SaveIdQueryInstallments = result.IdQueryInstallments.ToString();
            ViewBag.Result = result;
            ViewBag.ReturnUrl = returnUrl;

            return View();

        }
        [HttpPost]
        public ActionResult Commit()
        {
            var token = Request.Form["token_ws"];
            var idQueryInstallments = int.Parse(Request.Form["id_query_installments"]);
            var deferredPeriodsIndex = 10;
            var gracePeriods = false;

            var result = (new FullTransaction()).Commit(token, idQueryInstallments, deferredPeriodsIndex, gracePeriods);

            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("Status", "TransaccionCompleta", null, Request.Scheme);


            ViewBag.SaveIdQueryInstallments = idQueryInstallments;
            ViewBag.DeferredPeriodIndex = deferredPeriodsIndex;
            ViewBag.GracePrediods = gracePeriods;
            ViewBag.Action = returnUrl;
            ViewBag.Token = token;
            ViewBag.Result = result;

            return View();
        }
        
        [HttpPost]
        public ActionResult Status()
        {
            var token = Request.Form["token_ws"];

            var result = (new FullTransaction()).Status(token);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("Refund", "TransaccionCompleta", null, Request.Scheme);

            ViewBag.Action = returnUrl;
            ViewBag.Token = token;
            ViewBag.Result = result;

            return View();
        }

        [HttpPost]
        public ActionResult Refund()
        {
            var token = Request.Form["token_ws"];
            var amount = 10000;

            var result = (new FullTransaction()).Refund(token, amount);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("Status", "TransaccionCompleta", null, Request.Scheme);

            ViewBag.Amount = amount;
            ViewBag.Action = returnUrl;
            ViewBag.Token = token;
            ViewBag.Result = result;

            return View();
        }
        

        #endregion
        
        
        
        
        
    }
}