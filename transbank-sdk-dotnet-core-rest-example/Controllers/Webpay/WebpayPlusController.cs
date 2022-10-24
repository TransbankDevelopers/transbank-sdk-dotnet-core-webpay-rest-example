using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;

namespace Controllers.Webpay
{

    [Route("webpay_plus")]
    public class WebpayPlusController : BaseController
    {
        private Transaction tx;
        private String ctrlName = "webpay_plus";
        private String viewBase = "~/Views/webpay_plus/";

        public WebpayPlusController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) : 
            base(urlHelperFactory, actionContextAccessor)
        {
            tx = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
        }
        [Route("create")]
        public ActionResult Create()
        {
            var buyOrder = "buyOrder_" + GetRandomNumber();
            var sessionId = "sessionId_" + GetRandomNumber();
            var amount = 1000;
            var returnUrl = CreateUrl(ctrlName, "commit"); 
            var response = tx.Create(buyOrder, sessionId, amount, returnUrl);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);

            ViewBag.BuyOrder = buyOrder;
            ViewBag.SessionId = sessionId;
            ViewBag.Amount = amount;
            ViewBag.ReturnUrl = returnUrl;
          
            ViewBag.Url = response.Url;
            ViewBag.Token = response.Token;

            return View($"{viewBase}create.cshtml");
        }
        [Route("commit")]
        public ActionResult Commit(String token_ws)
        {
            var response = tx.Commit(token_ws);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);

            ViewBag.Amount = response.Amount;
            ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");
            ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");
            ViewBag.TokenWs = token_ws;

            return View($"{viewBase}commit.cshtml");
        }
        [Route("refund")]
        public ActionResult Refund()
        {
            var token = Request.Form["token_ws"];
            decimal amount = decimal.Parse(Request.Form["amount"]);

            var response = tx.Refund(token, amount);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.TokenWs = token;

            return View($"{viewBase}refund.cshtml");
        }

        [Route("status")]
        public ActionResult Status()
        {
            var token = Request.Form["token_ws"];
            var response = tx.Status(token);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.TokenWs = token;

            return View($"{viewBase}status.cshtml");
        }


    }
}