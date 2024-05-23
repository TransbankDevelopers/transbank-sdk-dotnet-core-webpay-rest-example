using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;

namespace Controllers.Webpay
{

    [Route("webpay_plus_mall")]
    public class WebpayPlusMallController : BaseController
    {
        private MallTransaction tx;
        private String ctrlName = "webpay_plus_mall";
        private String viewBase = "Views/webpay_plus_mall/";
        public WebpayPlusMallController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
          //   tx = new MallTransaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_MALL, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            tx = MallTransaction.buildForIntegration(IntegrationCommerceCodes.WEBPAY_PLUS_MALL, IntegrationApiKeys.WEBPAY);
          
        }
        [Route("create")]
        public ActionResult Create()
        {
            var buyOrder = "buyOrder_" + GetRandomNumber();
            var sessionId = "sessionId_" + GetRandomNumber();
            var returnUrl = CreateUrl(ctrlName, "commit");

            var childCommerceCode1 = IntegrationCommerceCodes.WEBPAY_PLUS_MALL_CHILD1;
            var childBuyOrder1 = "childBuyOrder_" + GetRandomNumber();
            decimal amount1 = 1000;

            var childCommerceCode2 = IntegrationCommerceCodes.WEBPAY_PLUS_MALL_CHILD2;
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
            TempData["Token"] = response.Token;
            return View($"{viewBase}create.cshtml");
        }
        [Route("commit")]
        public ActionResult Commit(String token_ws)
        {
            var token = TempData["Token"] as string;
            var status = tx.Status(token);
            if (status.CardDetail == null || token_ws == null && status.CardDetail != null)
            {

                return View($"{viewBase}abort.cshtml");
            }
            else
            {
                var response = tx.Commit(token);
                var detail = response.Details[0];
                ViewBag.Response = response;
                ViewBag.Resp = ToJson(response);
                ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");
                ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");
                ViewBag.Response = response;
                ViewBag.Resp = ToJson(response);
                ViewBag.TokenWs = token_ws;
                ViewBag.AuthorizationCode = detail.AuthorizationCode;
                ViewBag.Amount = detail.Amount;
                ViewBag.ChildBuyOrder = detail.BuyOrder;
                ViewBag.ChildCommerceCode = detail.CommerceCode;
                return View($"{viewBase}commit.cshtml");
            }
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
    }
}