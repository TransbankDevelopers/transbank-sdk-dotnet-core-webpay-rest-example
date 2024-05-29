using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;
using System;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.TransaccionCompleta;

namespace Controllers.TransaccionCompleta
{

    [Route("transaccion_completa")]
    public class FullTransactionController : BaseController
    {
        private FullTransaction tx;
        private String ctrlName = "transaccion_completa";
        private String viewBase = "Views/transaccion_completa/";
        public FullTransactionController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
           
            tx = FullTransaction.buildForIntegration(IntegrationCommerceCodes.TRANSACCION_COMPLETA, IntegrationApiKeys.WEBPAY);
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
            var response = tx.Create(buyOrder, sessionId, amount, cvv, card_number, cardExpirationDate);

            ViewBag.Resp = response;
            ViewBag.Token = response.Token;
            ViewBag.CommitEndpoint = CreateUrl(ctrlName, "commit");
            ViewBag.CommitInstallments = CreateUrl(ctrlName, "commitInstallments");
            ViewBag.InstallmentsEndpoint = CreateUrl(ctrlName, "installments");
            ViewBag.CreateEndpoint = CreateUrl(ctrlName, "create");


            return View($"{viewBase}create.cshtml");
        }
        [Route("commit")]
        public ActionResult Commit(String token, Int32 idQueryInstallments, Int32 deferredPeriodIndex, String gracePeriodCheck = null)
        {
            Boolean gracePeriod = gracePeriodCheck != null ? true : false;
            ViewBag.CommitEndpoint = CreateUrl(ctrlName, "commit");
            ViewBag.InstallmentsEndpoint = CreateUrl(ctrlName, "installments");
            ViewBag.CreateEndpoint = CreateUrl(ctrlName, "create");
            var response = tx.Commit(token, null, null, gracePeriod);

            ViewBag.Response = response;
            ViewBag.Amount = response.Amount;
            ViewBag.TokenWs = token;
            ViewBag.Resp = ToJson(response);
            ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");
            ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");


            return View($"{viewBase}commit.cshtml");
        }
        [Route("commitInstallments")]
        public ActionResult CommitInstallments(String token, int id_query_installments, int deferred_period_index, Boolean grace_period)
        {
            ViewBag.CommitEndpoint = CreateUrl(ctrlName, "commit");
            ViewBag.InstallmentsEndpoint = CreateUrl(ctrlName, "installments");
            ViewBag.CreateEndpoint = CreateUrl(ctrlName, "create");
            var response = tx.Commit(token, id_query_installments, deferred_period_index, grace_period);

            ViewBag.Response = response;
            ViewBag.Amount = response.Amount;
            ViewBag.TokenWs = token;
            ViewBag.Resp = ToJson(response);
            ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");
            ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");


            return View($"{viewBase}commit.cshtml");
        }
        [Route("installments")]
        public ActionResult Installments(String token, Byte installments)
        {
            
            var response = tx.Installments(token, installments);

            ViewBag.Response = response;
            ViewBag.token = token;
            ViewBag.id_query_installments = response.IdQueryInstallments;
            ViewBag.Resp = ToJson(response);

            ViewBag.CreateEndpoint = CreateUrl(ctrlName, "create");
            ViewBag.CommitEndpoint = CreateUrl(ctrlName, "commitInstallments");

            return View($"{viewBase}installments.cshtml");
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
        public ActionResult Refund(String token_ws, Decimal amount)
        {

            var response = tx.Refund(token_ws, amount);

            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);

            return View($"{viewBase}refund.cshtml");
        }
    }
}