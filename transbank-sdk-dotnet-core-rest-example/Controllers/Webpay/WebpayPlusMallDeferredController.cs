using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;

namespace Controllers.Webpay
{

    [Route("webpay_plus_mall_deferred")]
    public class WebpayPlusMallDeferredController : BaseController
    {
        private MallTransaction tx;
        private String ctrlName = "webpay_plus_mall_deferred";
        private String viewBase = "Views/webpay_plus_mall_deferred/";
        public WebpayPlusMallDeferredController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
            tx = new MallTransaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_MALL_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
        }

        [Route("create")]
        public ActionResult Create()
        {
            var buyOrder = "buyOrder_" + GetRandomNumber();
            var sessionId = "sessionId_" + GetRandomNumber();
            var returnUrl = CreateUrl(ctrlName, "commit");

            var childCommerceCode1 = IntegrationCommerceCodes.WEBPAY_PLUS_MALL_DEFERRED_CHILD1;
            var childBuyOrder1 = "childBuyOrder_" + GetRandomNumber();
            decimal amount1 = 1000;

            var childCommerceCode2 = IntegrationCommerceCodes.WEBPAY_PLUS_MALL_DEFERRED_CHILD2;
            var childBuyOrder2 = "childBuyOrder_" + GetRandomNumber();
            decimal amount2 = 1000;

            var details = new List<TransactionDetail>();
            details.Add(new TransactionDetail(
                amount1,
                childCommerceCode1,
                childBuyOrder1
            ));
            details.Add(new TransactionDetail(
                amount2,
                childCommerceCode2,
                childBuyOrder2
            ));

            var response = tx.Create(buyOrder, sessionId, returnUrl, details);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);

            ViewBag.BuyOrder = buyOrder;
            ViewBag.SessionId = sessionId;

            ViewBag.ChildCommerceCode1 = childCommerceCode1;
            ViewBag.ChildBuyOrder1 = childBuyOrder1;
            ViewBag.amount1 = amount1;

            ViewBag.ChildCommerceCode2 = childCommerceCode2;
            ViewBag.ChildBuyOrder2 = childBuyOrder2;
            ViewBag.amount2 = amount2;

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Url = response.Url;
            ViewBag.Token = response.Token;

            return View($"{viewBase}create.cshtml");
        }
        [Route("commit")]
        public ActionResult Commit(String token_ws)
        {
            if (token_ws == null)
            {
                return View($"{viewBase}abort.cshtml");
            }
            var response = tx.Commit(token_ws);
            var detail = response.Details[0];
            AddDetailModelDeferred(response, detail.CommerceCode, token_ws, detail.BuyOrder, detail.AuthorizationCode, detail.Amount);
            return View($"{viewBase}commit.cshtml");
        }
        [Route("refund")]
        public ActionResult Refund()
        {
            var token = Request.Form["token_ws"];
            decimal amount = decimal.Parse(Request.Form["amount"]);
            var childBuyOrder = Request.Form["child_buy_order"];
            var childCommerceCode = Request.Form["child_commerce_code"];

            var response = tx.Refund(token, childBuyOrder, childCommerceCode, amount);

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
            decimal amount = decimal.Parse(Request.Form["amount"]);
            var childBuyOrder = Request.Form["child_buy_order"];
            var childCommerceCode = Request.Form["child_commerce_code"];
            var authorizationCode = Request.Form["authorization_code"];

            var response = tx.Capture(childCommerceCode, token, childBuyOrder, authorizationCode, amount);
            AddDetailModelDeferred(response, childCommerceCode, token, childBuyOrder, response.AuthorizationCode, response.CapturedAmount);
            ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");
            ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");
            return View($"{viewBase}capture.cshtml");
        }
        private void AddDetailModelDeferred(Object response, String childCommerceCode, String tokenWs, String childBuyOrder, String authorizationCode, decimal? amount)
        {
            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.TokenWs = tokenWs;
            ViewBag.ChildBuyOrder = childBuyOrder;
            ViewBag.ChildCommerceCode = childCommerceCode;
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