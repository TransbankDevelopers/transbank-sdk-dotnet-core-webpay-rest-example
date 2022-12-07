using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.Oneclick;
using System.Collections.Generic;

namespace Controllers.Oneclick
{

    [Route("oneclick_mall")]
    public class OneclickMallController : BaseController
    {
        private MallInscription inscription;
        private MallTransaction tx;
        private String ctrlName = "oneclick_mall";
        private String viewBase = "Views/oneclick_mall/";

        public OneclickMallController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
            inscription = new MallInscription(new Options(IntegrationCommerceCodes.ONECLICK_MALL, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            tx = new MallTransaction(new Options(IntegrationCommerceCodes.ONECLICK_MALL, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
        }

        [Route("start")]
        public ActionResult Start()
        {
            var username = "User-" + GetRandomNumber();
            HttpContext.Session.SetString("username", username);

            var email = "user." + GetRandomNumber() + "@example.cl";
            var returnUrl = CreateUrl(ctrlName, "finish");
            var response = inscription.Start(username, email, returnUrl);
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
            var response = inscription.Finish(tbk_token);
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
        [Route("delete")]
        public ActionResult Delete(String username, String tbk_user)
        {
            var response = inscription.Delete(tbk_user, username);
            ViewBag.Resp = ToJson(response);

            return View($"{viewBase}delete.cshtml");
        }
        [Route("authorize")]
        public ActionResult Authorize(String username, String tbk_user, Decimal amount1, Int16 installments1, Decimal amount2, Int16 installments2)
        {
            var buyOrder = "buyOrder_" + GetRandomNumber();
            var buyOrderMallOne = "childBuyOrder1_" + GetRandomNumber();
            var buyOrderMallTwo = "childBuyOrder2_" + GetRandomNumber();
            var childCommerceCode1 = IntegrationCommerceCodes.ONECLICK_MALL_CHILD1;
            var childCommerceCode2 = IntegrationCommerceCodes.ONECLICK_MALL_CHILD2;

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

            ViewBag.RefundEndpoint = CreateUrl(ctrlName, "refund");
            ViewBag.StatusEndpoint = CreateUrl(ctrlName, "status");

            return View($"{viewBase}authorize.cshtml");
        }
        [Route("refund")]
        public ActionResult Refund(String buy_order, String child_commerce_code, String child_buy_order, Decimal amount)
        {
            var response = tx.Refund(buy_order, child_commerce_code, child_buy_order, amount);
            ViewBag.Resp = ToJson(response);

            return View($"{viewBase}refund.cshtml");
        }
        [Route("status")]
        public ActionResult Status(String buy_order)
        {
            var response = tx.Status(buy_order);
            ViewBag.Response = response;
            ViewBag.Resp = ToJson(response);

            return View($"{viewBase}status.cshtml");
        }
    }
}