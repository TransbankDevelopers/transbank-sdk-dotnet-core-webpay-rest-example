using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.TransaccionCompleta;
using Transbank.Webpay.TransaccionCompletaMall;
using Transbank.Webpay.TransaccionCompletaMall.Common;
using Transbank.Webpay.WebpayPlus;

namespace transbanksdkdotnetrestexample.Controllers
{
    public class WebpayPlusController : Controller
    {

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public WebpayPlusController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        #region Webpay Plus

        public ActionResult NormalCreate()
        {
            var random = new Random();
            var buyOrder = random.Next(999999999).ToString();
            var sessionId = random.Next(999999999).ToString();
            var amount = random.Next(1000, 999999);
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("NormalReturn", "WebpayPlus", null, Request.Scheme);
            var result = (new Transaction()).Create(buyOrder, sessionId, amount, returnUrl);

            ViewBag.BuyOrder = buyOrder;
            ViewBag.SessionId = sessionId;
            ViewBag.Amount = amount;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Result = result;
            ViewBag.Action = result.Url;
            ViewBag.Token = result.Token;


            Prueba2();
            return View();
        }

        public ActionResult NormalReturn(String token_ws)
        {
            var result = (new Transaction()).Commit(token_ws);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            ViewBag.Token = token_ws;
            ViewBag.Action = urlHelper.Action("ExecuteRefund", "WebpayPlus", null, Request.Scheme);
            ViewBag.Result = result;
            ViewBag.SaveToken = token_ws;

            return View();
        }

        public ActionResult NormalRefund()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteRefund", "WebpayPlus", null, Request.Scheme);
            return View();
        }

        [HttpPost]
        public ActionResult ExecuteRefund()
        {
            var token = Request.Form["token_ws"];
            var refundAmount = 500;
            var result = (new Transaction()).Refund(token, refundAmount);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteStatus", "WebpayPlus", null, Request.Scheme);

            ViewBag.Token = token;
            ViewBag.Amount = refundAmount;
            ViewBag.Result = result;

            return View();
        }


        public ActionResult NormalStatus()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteStatus", "WebpayPlus", null, Request.Scheme);
            return View();
        }

        [HttpPost]
        public ActionResult ExecuteStatus()
        {
            var token = Request.Form["token_ws"];
            var result = (new Transaction()).Status(token);

            ViewBag.Result = result;

            return View();
        }

        #endregion Webpay Plus

        #region Webpay Plus Deferred

        public ActionResult DeferredCreate()
        {
            var random = new Random();
            var buyOrder = random.Next(999999999).ToString();
            var sessionId = random.Next(999999999).ToString();
            var amount = random.Next(1000, 999999);
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("DeferredReturn", "WebpayPlus", null, Request.Scheme);
            var tx = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Create(buyOrder, sessionId, amount, returnUrl);

            ViewBag.BuyOrder = buyOrder;
            ViewBag.SessionId = sessionId;
            ViewBag.Amount = amount;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Result = result;
            ViewBag.Action = result.Url;
            ViewBag.Token = result.Token;
            return View();
        }


        public ActionResult DeferredReturn(String token_ws)
        {
            var tx = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Commit(token_ws);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            ViewBag.Token = token_ws;
            ViewBag.Action = urlHelper.Action("ExecuteDeferredCapture", "WebpayPlus", null, Request.Scheme);
            ViewBag.Result = result;
            ViewBag.SaveToken = token_ws;
            ViewBag.SaveAmount = result.Amount.ToString();
            ViewBag.SaveAuthorizationCode = result.AuthorizationCode;
            /*
            var r1 = tx.IncreaseAmount(token_ws, result.BuyOrder, result.AuthorizationCode, 1000);
            var r2 = tx.IncreaseAuthorizationDate(token_ws, result.BuyOrder, result.AuthorizationCode);
            var r3 = tx.ReversePreAuthorizedAmount(token_ws, result.BuyOrder, result.AuthorizationCode, 1000);
            var r4 = tx.DeferredCaptureHistory(token_ws);*/
            /*
            IncreaseAmountResponse r1 = tx.increaseAmount(tokenWs, resp.getBuyOrder(), resp.getAuthorizationCode(), 1000);
            log.info(toJson(r1));
            ReversePreAuthorizedAmountResponse r2 = tx.reversePreAuthorizedAmount(tokenWs, resp.getBuyOrder(), resp.getAuthorizationCode(), 1000);
            log.info(toJson(r2));
            IncreaseAuthorizationDateResponse r3 = tx.increaseAuthorizationDate(tokenWs, resp.getBuyOrder(), resp.getAuthorizationCode());
            log.info(toJson(r3));
            List<DeferredCaptureHistoryResponse> r4 = tx.deferredCaptureHistory(tokenWs);
            log.info(toJson(r4));*/

            return View();
        }

        [HttpPost]
        public ActionResult ExecuteDeferredCapture()
        {
            var token = Request.Form["token_ws"];
            var buyOrder = Request.Form["buy_order"];
            var authorizationCode = Request.Form["authorization_code"];
            var captureAmount = decimal.Parse(Request.Form["capture_amount"]);
            var tx = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Capture(token, buyOrder, authorizationCode, captureAmount);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteDeferredRefund", "WebpayPlus", null, Request.Scheme);
            ViewBag.Token = token;
            ViewBag.SaveToken = token;

            ViewBag.BuyOrder = buyOrder;
            ViewBag.AuthorizationCode = authorizationCode;
            ViewBag.CaptureAmount = captureAmount;
            ViewBag.Result = result;

            return View();
        }

        public ActionResult DeferredRefund()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteDeferredRefund", "WebpayPlus", null, Request.Scheme);
            return View();
        }

        [HttpPost]
        public ActionResult ExecuteDeferredRefund()
        {
            var token = Request.Form["token_ws"];
            var amount = decimal.Parse(Request.Form["amount"]);
            var tx = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Refund(token, amount);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteDeferredStatus", "WebpayPlus", null, Request.Scheme);
            ViewBag.Token = token;

            ViewBag.Result = result;

            return View();
        }

        public ActionResult DeferredStatus()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteDeferredStatus", "WebpayPlus", null, Request.Scheme);
            return View();
        }

        [HttpPost]
        public ActionResult ExecuteDeferredStatus()
        {
            var token = Request.Form["token_ws"];
            var tx = new Transaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Status(token);

            ViewBag.Result = result;

            return View();
        }

        #endregion Webpay Plus Deferred

        #region Webpay Plus Mall

        public ActionResult MallCreate()
        {
            var random = new Random();
            var buyOrder = random.Next(9999999).ToString();
            var sessionId = random.Next(9999999).ToString();
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("MallReturn", "WebpayPlus", null, Request.Scheme);

            var transactions = new List<TransactionDetail>();
            transactions.Add(new TransactionDetail(
                random.Next(9999999),
                "597055555536",
                random.Next(9999999).ToString()
            ));
            transactions.Add(new TransactionDetail(
                random.Next(9999999),
                "597055555537",
                random.Next(9999999).ToString()
            ));

            var result = (new MallTransaction()).Create(buyOrder, sessionId, returnUrl, transactions);

            ViewBag.Result = result;
            ViewBag.BuyOrder = buyOrder;
            ViewBag.SessionId = sessionId;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Transactions = transactions;
            ViewBag.Action = result.Url;
            ViewBag.Token = result.Token;

            return View();
        }

        public ActionResult MallReturn(String token_ws)
        {
            var result = (new MallTransaction()).Commit(token_ws);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            ViewBag.Token = token_ws;
            ViewBag.Action = urlHelper.Action("ExecuteMallRefund", "WebpayPlus", null, Request.Scheme);
            ViewBag.Result = result;
            ViewBag.SaveToken = token_ws;
            ViewBag.SaveAmount = result.Details.First().Amount;
            ViewBag.SaveCommerceCode = result.Details.First().CommerceCode;
            ViewBag.SaveBuyOrder = result.Details.First().BuyOrder;

            ViewBag.Success = result.Details.All(detail => detail.ResponseCode == 0);
            return View();
        }

        public ActionResult MallRefund()
        {
            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteMallRefund", "WebpayPlus", null, Request.Scheme);
            return View();
        }

        [HttpPost]
        public ActionResult ExecuteMallRefund()
        {
            var token = Request.Form["token_ws"];
            var buyOrder = Request.Form["buy_order"];
            var commerceCode = Request.Form["commerce_code"];
            var amount = decimal.Parse(Request.Form["amount"]);
            ViewBag.Token = token;
            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteMallStatus", "WebpayPlus", null, Request.Scheme);

            ViewBag.SaveAmount = amount;
            ViewBag.SaveCommerceCode = commerceCode;
            ViewBag.SaveToken = token;
            ViewBag.SaveBuyOrder = buyOrder;

            var result = (new MallTransaction()).Refund(token, buyOrder, commerceCode, amount);

            ViewBag.Result = result;

            return View();
        }

        public ActionResult MallStatus()
        {
            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteMallStatus", "WebpayPlus", null, Request.Scheme);
            return View();
        }

        public ActionResult ExecuteMallStatus()
        {
            var token = Request.Form["token_ws"];

            var result = (new MallTransaction()).Status(token);

            ViewBag.Result = result;
            ViewBag.SaveToke = token;

            return View();
        }

        #endregion Webpay Plus Mall

        public ActionResult MallDeferredCreate()
        {
            var random = new Random();
            var buyOrder = random.Next(9999999).ToString();
            var sessionId = random.Next(9999999).ToString();
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("MallDeferredCommit", "WebpayPlus", null, Request.Scheme);

            var transactions = new List<TransactionDetail>();
            transactions.Add(new TransactionDetail(
                random.Next(9999999),
                "597055555582",
                random.Next(9999999).ToString()
            ));
            transactions.Add(new TransactionDetail(
                random.Next(9999999),
                "597055555583",
                random.Next(9999999).ToString()
            ));

            var tx = new MallTransaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_MALL_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Create(buyOrder, sessionId, returnUrl, transactions);

            ViewBag.Result = result;
            ViewBag.BuyOrder = buyOrder;
            ViewBag.SessionId = sessionId;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Transactions = transactions;
            ViewBag.Action = result.Url;
            ViewBag.Token = result.Token;

            return View();
        }
        
        public ActionResult MallDeferredCommit(String token_ws)
        {
            var tx = new MallTransaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_MALL_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Commit(token_ws);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            ViewBag.Token = token_ws;
            ViewBag.Action = urlHelper.Action("ExecuteMallDeferredCapture", "WebpayPlus", null, Request.Scheme);
            ViewBag.Result = result;
            ViewBag.ResponseCode = result.Details.First().ResponseCode;
            ViewBag.SaveToken = token_ws;
            ViewBag.SaveAmount = result.Details.First().Amount;
            ViewBag.SaveCommerceCode = result.Details.First().CommerceCode;
            ViewBag.SaveBuyOrder = result.Details.First().BuyOrder;
            ViewBag.SaveAuthorizationCode = result.Details.First().AuthorizationCode;

            ViewBag.Success = result.Details.All(detail => detail.ResponseCode == 0);
            /*
            var detail = result.Details[0];
            var r1 = tx.IncreaseAmount(token_ws, detail.CommerceCode, detail.BuyOrder, detail.AuthorizationCode, 1000);
            var r2 = tx.IncreaseAuthorizationDate(token_ws, detail.CommerceCode, detail.BuyOrder, detail.AuthorizationCode);
            var r3 = tx.ReversePreAuthorizedAmount(token_ws, detail.CommerceCode, detail.BuyOrder, detail.AuthorizationCode, 1000);
            var r4 = tx.DeferredCaptureHistory(token_ws, detail.CommerceCode, detail.BuyOrder);*/


            return View();
        }
        
        public ActionResult ExecuteMallDeferredCapture()
        {
            var token = Request.Form["token_ws"];
            var buyOrder = Request.Form["buy_order"];
            var authorizationCode = Request.Form["authorization_code"];
            var captureAmount = decimal.Parse(Request.Form["capture_amount"]);
            string commerceCode = Request.Form["commerce_code"];

            var tx = new MallTransaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_MALL_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Capture(token, commerceCode, buyOrder, authorizationCode, captureAmount);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteMallDeferredRefund", "WebpayPlus", null, Request.Scheme);
            ViewBag.Token = token;
            ViewBag.SaveToken = token;

            ViewBag.BuyOrder = buyOrder;
            ViewBag.AuthorizationCode = authorizationCode;
            ViewBag.CaptureAmount = captureAmount;
            ViewBag.CommerceCode = commerceCode;
            ViewBag.Result = result;

            return View();
        }

        public ActionResult ExecuteMallDeferredRefund()
        {
            var token = Request.Form["token_ws"];
            var buyOrder = Request.Form["buy_order"];
            var commerceCode = Request.Form["commerce_code"];
            var amount = decimal.Parse(Request.Form["amount"]);
            ViewBag.Token = token;
            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("ExecuteMallDeferredStatus", "WebpayPlus", null, Request.Scheme);

            ViewBag.SaveAmount = amount;
            ViewBag.SaveCommerceCode = commerceCode;
            ViewBag.SaveToken = token;
            ViewBag.SaveBuyOrder = buyOrder;

            var tx = new MallTransaction(new Options(IntegrationCommerceCodes.WEBPAY_PLUS_MALL_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var result = tx.Refund(token, buyOrder, commerceCode, amount);

            ViewBag.Result = result;

            return View();
        }

        public ActionResult ExecuteMallDeferredStatus()
        {
            var token = Request.Form["token_ws"];

            var result = (new MallTransaction()).Status(token);

            ViewBag.Result = result;
            ViewBag.SaveToke = token;

            return View();
        }


        private void Prueba()
        {
            var random = new Random();

            var buy_order = random.Next(999999999).ToString();
            var session_id = random.Next(9999999).ToString();
            var amount = 10000;
            var cvv = 123;
            var card_number = "4051885600446623";
            var card_expiration_date = "22/10";

            var tx = new FullTransaction(new Options(IntegrationCommerceCodes.TRANSACCION_COMPLETA_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));

            var result = tx.Create(
                buyOrder: buy_order,
                sessionId: session_id,
                amount: amount,
                cvv: cvv,
                cardNumber: card_number,
                cardExpirationDate: card_expiration_date);
            /*
            var result2 = tx.Commit(result.Token, null, null, false);

            var r1 = tx.IncreaseAmount(result.Token, result2.BuyOrder, result2.AuthorizationCode, 1000);
            var r2 = tx.IncreaseAuthorizationDate(result.Token, result2.BuyOrder, result2.AuthorizationCode);
            var r3 = tx.ReversePreAuthorizedAmount(result.Token, result2.BuyOrder, result2.AuthorizationCode, 1000);
            var r4 = tx.DeferredCaptureHistory(result.Token);*/



            var random2 = new Random();
        }

        private void Prueba2()
        {
            var random = new Random();

            var buy_order = random.Next(999999999).ToString();
            var session_id = random.Next(9999999).ToString();
            var amount = 10000;
            var card_number = "4051885600446623";
            var card_expiration_date = "22/10";

            var tx = new MallFullTransaction(new Options(IntegrationCommerceCodes.TRANSACCION_COMPLETA_MALL_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));

            var details = new List<CreateDetails>();
            details.Add(new CreateDetails(amount, IntegrationCommerceCodes.TRANSACCION_COMPLETA_MALL_DEFERRED_CHILD1, buy_order + "_1"));

            var result = tx.Create(
                buyOrder: buy_order,
                sessionId: session_id,
                cardNumber: card_number,
                cardExpirationDate: card_expiration_date,
                details,
                123
            );

            var details2 = new List<MallCommitDetails>();
            details2.Add(new MallCommitDetails(IntegrationCommerceCodes.TRANSACCION_COMPLETA_MALL_DEFERRED_CHILD1, buy_order + "_1", 0, 0, false));
            var result2 = tx.Commit(result.Token, details2);
            /*
            var detail = result2.Details[0];
            var r1 = tx.IncreaseAmount(result.Token, detail.CommerceCode, detail.BuyOrder, detail.AuthorizationCode, 1000);
            var r2 = tx.IncreaseAuthorizationDate(result.Token, detail.CommerceCode, detail.BuyOrder, detail.AuthorizationCode);
            var r3 = tx.ReversePreAuthorizedAmount(result.Token, detail.CommerceCode, detail.BuyOrder, detail.AuthorizationCode, 1000);
            var r4 = tx.DeferredCaptureHistory(result.Token, detail.CommerceCode, detail.BuyOrder);*/

            var random2 = new Random();
        }
    }
}
