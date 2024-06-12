using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;
using System;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;

namespace Controllers.Webpay
{

    [Route("webpay_plus_deferred")]
    public class WebpayPlusDeferredController : BaseController
    {
        private Transaction tx;
        private String ctrlName = "webpay_plus_deferred";
        private String viewBase = "~/Views/webpay_plus_deferred/";

        public WebpayPlusDeferredController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) : 
            base(urlHelperFactory, actionContextAccessor)
        {
         
            tx = Transaction.buildForIntegration(IntegrationCommerceCodes.WEBPAY_PLUS_DEFERRED, IntegrationApiKeys.WEBPAY);
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
			TempData["Token"] = response.Token;
			ViewBag.Url = response.Url;
            ViewBag.Token = response.Token;

            return View($"{viewBase}create.cshtml");
        }
        [Route("commit")]
        public ActionResult Commit(String token_ws)
        {
			var token = TempData["Token"] as string;
			var status = tx.Status(token);
            if (token_ws == null)
            {
                var sessionId = status.SessionId;
                var buyOrder = status.BuyOrder;
                var tbkId = token;
                var data = new
                {
                    SessionId = sessionId,
                    BuyOrder = buyOrder,
                    TBK_ID = tbkId
                };

                ViewBag.Status = ToJson(data);
                return View($"{viewBase}abort.cshtml");
            }
            else
            {
                var response = tx.Commit(token_ws);
                AddDetailModelDeferred(response, token_ws, response.BuyOrder, response.AuthorizationCode, response.Amount);
                return View($"{viewBase}commit.cshtml");
            }
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
        [Route("capture")]
        public ActionResult Capture()
        {
            var token = Request.Form["token_ws"];
            var buyOrder = Request.Form["buy_order"];
            var authorizationCode = Request.Form["authorization_code"];
            decimal amount = decimal.Parse(Request.Form["amount"]);
            var response = tx.Capture(token, buyOrder, authorizationCode, amount);
            AddDetailModelDeferred(response, token, buyOrder, response.AuthorizationCode, response.CapturedAmount);
            ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");
            ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");
            return View($"{viewBase}capture.cshtml");
        }
        private void AddDetailModelDeferred(Object response, String tokenWs, String buyOrder, String authorizationCode, decimal? amount)
        {
            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.TokenWs = tokenWs;
            ViewBag.BuyOrder = buyOrder;
            ViewBag.AuthorizationCode = authorizationCode;
            ViewBag.Amount = amount;

            ViewBag.CaptureEndpoint = CreateUrl(ctrlName, "capture");
            ViewBag.IncreaseEndpoint = CreateUrl(ctrlName, "increase_amount");
            ViewBag.IncreaseDateEndpoint = CreateUrl(ctrlName, "increase_date");
            ViewBag.ReverseEndpoint = CreateUrl(ctrlName, "reverse");
            ViewBag.HistoryEndpoint = CreateUrl(ctrlName, "history");
        }

    }
}