using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.TransaccionCompletaMall;
using Transbank.Webpay.TransaccionCompletaMall.Common;

namespace Controllers.TransaccionCompleta
{

    [Route("transaccion_completa_mall_deferred")]
    public class MallFullTransactionDeferredController : BaseController
    {
        private MallFullTransaction tx;
        private String ctrlName = "transaccion_completa_mall_deferred";
        private String viewBase = "Views/transaccion_completa_mall_deferred/";
        public MallFullTransactionDeferredController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
          //  tx = new MallFullTransaction(new Options(IntegrationCommerceCodes.TRANSACCION_COMPLETA_MALL_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            tx = MallFullTransaction.buildForIntegration(IntegrationCommerceCodes.TRANSACCION_COMPLETA_MALL_DEFERRED, IntegrationApiKeys.WEBPAY);
        }
        [Route("form")]
        public ActionResult Form()
        {
            var amount = 1000;
            ViewBag.CreateEndpoint = CreateUrl(ctrlName, "create");
            ViewBag.Amount = amount;

            return View($"{viewBase}form.cshtml");
        }
        [Route("create")]
        public ActionResult Create(String card_number, Int16 cvv, String year, String month, Decimal amount)
        {

            var buyOrder = "buyOrder_" + GetRandomNumber();
            var sessionId = "sessionId_" + GetRandomNumber();
            var returnUrl = CreateUrl(ctrlName, "commit");
            var cardExpirationDate = year + "/" + month;

            var childCommerceCode = IntegrationCommerceCodes.TRANSACCION_COMPLETA_MALL_DEFERRED_CHILD1;
            var childBuyOrder = "childBuyOrder_" + GetRandomNumber();
            decimal amount1 = 1000;

            var details = new List<CreateDetails>();
            details.Add(new CreateDetails(
                amount1,
                childCommerceCode,
                childBuyOrder
            ));

            var response = tx.Create(buyOrder, sessionId, card_number, cardExpirationDate, details, cvv);

            ViewBag.Resp = response;
            ViewBag.Token = response.Token;
            ViewBag.BuyOrder = buyOrder;
            ViewBag.ChildCommerceCode = childCommerceCode;
            ViewBag.ChildBuyOrder = childBuyOrder;

            ViewBag.CommitEndpoint = CreateUrl(ctrlName, "commit");
            ViewBag.InstallmentsEndpoint = CreateUrl(ctrlName, "installments");


            return View($"{viewBase}create.cshtml");
        }
        [Route("commit")]
        public ActionResult Commit(String token, String child_commerce_code, String child_buy_order, Int32 idQueryInstallments, Int32 deferredPeriodIndex, String gracePeriodCheck = null)
        {
            Boolean gracePeriod = gracePeriodCheck != null ? true : false;

            var transactionDetails = new List<MallCommitDetails>();
            transactionDetails.Add(new MallCommitDetails(
                child_commerce_code,
                child_buy_order,
                null,
                null,
                gracePeriod
            ));

            var response = tx.Commit(token, transactionDetails);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.ChildCommerceCode = child_commerce_code;
            ViewBag.ChildBuyOrder = child_buy_order;
            ViewBag.Amount = 1000;
            ViewBag.TokenWs = token;
            ViewBag.BuyOrder = response.BuyOrder;
            ViewBag.AuthorizationCode = response.Details[0].AuthorizationCode;

            ViewBag.CaptureEndpoint = CreateUrl(ctrlName, "capture");
            ViewBag.IncreaseEndpoint = CreateUrl(ctrlName, "increase_amount");
            ViewBag.IncreaseDateEndpoint = CreateUrl(ctrlName, "increase_date");
            ViewBag.ReverseEndpoint = CreateUrl(ctrlName, "reverse");
            ViewBag.HistoryEndpoint = CreateUrl(ctrlName, "history");


            return View($"{viewBase}commit.cshtml");
        }
        [Route("capture")]
        public ActionResult Capture(String token_ws, String buy_order, String child_buy_order, String child_commerce_code, String authorization_code, Decimal amount)
        {

            var response = tx.Capture(token_ws, child_commerce_code, child_buy_order, authorization_code, amount);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.TokenWs = token_ws;
            ViewBag.Amount = amount;
            ViewBag.AuthorizationCode = authorization_code;
            ViewBag.BuyOrder = buy_order;
            ViewBag.ChildCommerceCode = child_commerce_code;
            ViewBag.ChildBuyOrder = child_buy_order;

            ViewBag.CaptureEndpoint = CreateUrl(ctrlName, "capture");
            ViewBag.IncreaseEndpoint = CreateUrl(ctrlName, "increase_amount");
            ViewBag.IncreaseDateEndpoint = CreateUrl(ctrlName, "increase_date");
            ViewBag.ReverseEndpoint = CreateUrl(ctrlName, "reverse");
            ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");
            ViewBag.HistoryEndpoint = CreateUrl(ctrlName, "history");
            ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");

            return View($"{viewBase}capture.cshtml");
        }
        [Route("status")]
        public ActionResult status(String token_ws)
        {

            var response = tx.Status(token_ws);

            ViewBag.Response = response;
            var originalResponse = response.OriginalResponse;
            JObject original_response = JObject.Parse(originalResponse);
            ViewBag.Resp = original_response;

            return View($"{viewBase}status.cshtml");
        }
        [Route("refund")]
        public ActionResult Refund(String token_ws, String buy_order, String child_buy_order, String child_commerce_code, String authorization_code, int amount)
        {

            var response = tx.Refund(token_ws, child_buy_order, child_commerce_code, amount);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.TokenWs = token_ws;

            return View($"{viewBase}refund.cshtml");
        }
        [Route("installments")]
        public ActionResult Installments(String token, String child_commerce_code, String child_buy_order, Int32 installments)
        {
            var transactionDetails = new List<MallInstallmentsDetails>();
            transactionDetails.Add(new MallInstallmentsDetails(
                child_commerce_code,
                child_buy_order,
                installments
            ));

            var response = tx.Installments(token, transactionDetails);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.ChildCommerceCode = child_commerce_code;
            ViewBag.ChildBuyOrder = child_buy_order;
            ViewBag.Amount = 1000;
            ViewBag.TokenWs = token;

            ViewBag.CaptureEndpoint = CreateUrl(ctrlName, "capture");
            ViewBag.IncreaseEndpoint = CreateUrl(ctrlName, "increase_amount");
            ViewBag.IncreaseDateEndpoint = CreateUrl(ctrlName, "increase_date");
            ViewBag.ReverseEndpoint = CreateUrl(ctrlName, "reverse");
            ViewBag.HistoryEndpoint = CreateUrl(ctrlName, "history");


            return View($"{viewBase}installments.cshtml");
        }

    }
}