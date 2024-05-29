using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using Transbank.Common;
using Transbank.Patpass;
using Transbank.Webpay.Common;
using Transbank.Webpay.Oneclick;
using Transbank.Exceptions;
namespace Controllers.Oneclick
{

    [Route("oneclick_mall_deferred")]
    public class OneclickMallDeferredController : BaseController
    {
        private MallInscription ins;
        private MallTransaction tx;
        private String ctrlName = "oneclick_mall_deferred";
        private String viewBase = "Views/oneclick_mall_deferred/";
        public OneclickMallDeferredController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
      
            ins = MallInscription.buildForIntegration(IntegrationCommerceCodes.ONECLICK_MALL, IntegrationApiKeys.WEBPAY);
            tx = MallTransaction.buildForIntegration(IntegrationCommerceCodes.ONECLICK_MALL, IntegrationApiKeys.WEBPAY);
        }
        [Route("start")]
        public ActionResult Start()
        {
            var username = "User-" + GetRandomNumber();
            HttpContext.Session.SetString("username", username);

            var email = "user." + GetRandomNumber() + "@example.cl";
            var returnUrl = CreateUrl(ctrlName, "finish");
            var response = ins.Start(username, email, returnUrl);
            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);
            ViewBag.Url = response.Url;
            ViewBag.Token = response.Token;
            ViewBag.Username = username;
            ViewBag.Email = email;
            ViewBag.ReturnUrl = returnUrl;
            return View($"{viewBase}start.cshtml");
        }
        [Route("finish")]
        public ActionResult Finish(String tbk_token)
        {
            var response = ins.Finish(tbk_token);
            if (tbk_token != "" && response.TbkUser == null)
            {

                ViewBag.Response = response;
                ViewBag.Resp = ToJson(response);
                ViewBag.Token = tbk_token;
                return View($"{viewBase}abort.cshtml");

            }
            else
            {
                ViewBag.Response = response;
                ViewBag.Resp = ToJson(response);
                ViewBag.Token = tbk_token;
                ViewBag.Username = HttpContext.Session.GetString("username");
                ViewBag.TbkUser = response.TbkUser;
                ViewBag.Amount1 = 1000;
                ViewBag.installments1 = 4;
                ViewBag.Amount2 = 500;
                ViewBag.installments2 = 5;
                ViewBag.AuthorizeEndpoint = CreateUrl(ctrlName, "authorize");
                ViewBag.DeleteEndpoint = CreateUrl(ctrlName, "delete");

                return View($"{viewBase}finish.cshtml");
            }
        }
        [Route("delete")]
        public ActionResult Delete(String username, String tbk_user)
        {
            try
            {
                var response = ins.Delete(tbk_user, username);
                ViewBag.Resp = ToJson(response);
            }
            catch (InscriptionDeleteException e)
            {
                ViewBag.Resp = e.Code;
            }
            return View($"{viewBase}delete.cshtml");
        }
        [Route("authorize")]
        public ActionResult Authorize(String username, String tbk_user, Decimal amount1, Int16 installments1, Decimal amount2, Int16 installments2)
        {
            var buyOrder = "buyOrder_" + GetRandomNumber();
            var buyOrderMallOne = "childBuyOrder1_" + GetRandomNumber();
            var buyOrderMallTwo = "childBuyOrder2_" + GetRandomNumber();
            var childCommerceCode1 = IntegrationCommerceCodes.ONECLICK_MALL_DEFERRED_CHILD1;
            var childCommerceCode2 = IntegrationCommerceCodes.ONECLICK_MALL_DEFERRED_CHILD2;

            List<PaymentRequest> details = new List<PaymentRequest>();
            details.Add(new PaymentRequest(
              childCommerceCode1, buyOrderMallOne, amount1, installments1
            ));
            details.Add(new PaymentRequest(
              childCommerceCode2, buyOrderMallTwo, amount2, installments2
            ));

            var response = tx.Authorize(username, tbk_user, buyOrder, details);

            ViewBag.Resp = ToJson(response);
            ViewBag.BuyOrder = response.BuyOrder;
            ViewBag.ChildBuyOrder = response.Details[0].BuyOrder;
            ViewBag.ChildCommerceCode = response.Details[0].CommerceCode;
            ViewBag.Amount = response.Details[0].Amount;
            ViewBag.AuthorizationCode = response.Details[0].AuthorizationCode;

            ViewBag.CaptureEndpoint = CreateUrl(ctrlName, "capture");
            ViewBag.IncreaseEndpoint = CreateUrl(ctrlName, "increase_amount");
            ViewBag.IncreaseDateEndpoint = CreateUrl(ctrlName, "increase_date");
            ViewBag.ReverseEndpoint = CreateUrl(ctrlName, "reverse");
            ViewBag.HistoryEndpoint = CreateUrl(ctrlName, "history");

            return View($"{viewBase}authorize.cshtml");
        }
        [Route("capture")]
        public ActionResult Capture(String token_ws, String buy_order, String child_buy_order, String child_commerce_code, String authorization_code, Decimal amount)
        {

            var response = tx.Capture(child_commerce_code, child_buy_order, authorization_code, amount);

            ViewBag.Resp = ToJson(response);
            ViewBag.response = response;
            ViewBag.BuyOrder = buy_order;
            ViewBag.ChildBuyOrder = child_buy_order;
            ViewBag.ChildCommerceCode = child_commerce_code;
            ViewBag.Amount = amount;
            ViewBag.AuthorizationCode = authorization_code;

            ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");
            ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");

            return View($"{viewBase}capture.cshtml");
        }
        [Route("status")]
        public ActionResult Status(String buy_order)
        {

            var response = tx.Status(buy_order);

            ViewBag.Resp = ToJson(response);

            return View($"{viewBase}status.cshtml");
        }
        [Route("refund")]
        public ActionResult Refund(String token_ws, String buy_order, String child_buy_order, String child_commerce_code, String authorization_code, Decimal amount)
        {

            var response = tx.Refund(buy_order, child_commerce_code, child_buy_order, amount);

            ViewBag.Resp = ToJson(response);

            return View($"{viewBase}refund.cshtml");
        }
    }
}