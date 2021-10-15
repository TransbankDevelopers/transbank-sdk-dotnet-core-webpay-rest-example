using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Transbank.Webpay.Common;
using Transbank.Webpay.Oneclick;

namespace transbanksdkdotnetrestexample.Controllers
{
    public class OneclickMallDeferredController : Controller
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public OneclickMallDeferredController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public ActionResult InscriptionStart()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("InscriptionFinish", "OneclickMallDeferred", null, Request.Scheme);
            var userName = "goncafa";
            var email = $"{RandomString(5)}@{RandomString(5)}.com";

            ViewBag.UserName = userName;
            ViewBag.Email = email;
            ViewBag.ReturnUrl = returnUrl;

            var response = MallDeferredInscription.Start(userName, email, returnUrl);
            ViewBag.Result = response;
            
            ViewBag.Action = response.Url;
            ViewBag.Token = response.Token;
            
            return View();
        }
        
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public ActionResult InscriptionFinish(String tbk_token)
        {
            ViewBag.Token = tbk_token;

            var result = MallDeferredInscription.Finish(tbk_token);

            ViewBag.AuthorizationCode = result.AuthorizationCode;
            ViewBag.ResponseCode = result.ResponseCode;
            ViewBag.TbkUser = result.TbkUser;
            ViewBag.CreditCardType = result.CardType;
            ViewBag.LastFourCardDigits = result.CardNumber;
            ViewBag.Result = result;

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("TransactionAuthorize", "OneclickMallDeferred", null, Request.Scheme);

            return View();
        }

        public ActionResult TransactionAuthorize()
        {
            var userName = Request.Form["user_name"];
            var tbkUser = Request.Form["tbk_user"];
            var buyOrder = RandomString(10);

            string childCommerceCode = "597055555548";
            string childBuyOrder = RandomString(10);
            decimal amount = decimal.Parse(Request.Form["amount"]);
            int installmentsNumber = 1;

            List<PaymentRequest> details = new List<PaymentRequest>
            {
                new PaymentRequest(childCommerceCode, childBuyOrder, amount, installmentsNumber)
            };

            Transbank.Webpay.Oneclick.Responses.MallAuthorizeResponse result = MallDeferredTransaction.Authorize(userName, tbkUser, buyOrder, details);
            Console.WriteLine(result);

            ViewBag.UserName = userName;
            ViewBag.TbkUser = tbkUser;
            ViewBag.BuyOrder = buyOrder;
            ViewBag.Details = details;
            ViewBag.Result = result;

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("TransactionCapture", "OneclickMallDeferred", null, Request.Scheme);

            return View();
        }

        public ActionResult TransactionCapture()
        {
            var ChildcommerceCode = Request.Form["child_commerce_code"];
            var ChildbuyOrder = Request.Form["child_buy_order"];
            decimal.TryParse(Request.Form["capture_amount"], out decimal amount);
            var authorizationCode = Request.Form["authorization_code"];
            var result = MallDeferredTransaction.Capture(ChildcommerceCode, ChildbuyOrder, amount, authorizationCode);

            ViewBag.UserName = Request.Form["userName"];
            ViewBag.ChildCommerceCode = ChildcommerceCode.ToString();
            ViewBag.ChildbuyOrder = ChildbuyOrder;
            ViewBag.CaptureAmount = amount;
            ViewBag.AuthorizationCode = authorizationCode;
            ViewBag.Result = result;

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("TransactionRefund", "OneclickMallDeferred", null, Request.Scheme);

            return View();
        }
        
        public ActionResult InscriptionDelete()
        {
            var userName = Request.Form["user_name"];
            var tbkUser = Request.Form["TBK_TOKEN"];

            MallDeferredInscription.Delete(userName, tbkUser);

            ViewBag.UserName = userName;
            ViewBag.TbkUser = tbkUser;

            return View();
        }
        
        public ActionResult TransactionRefund()
        {
            var buyOrder = Request.Form["buy_order"];
            var childCommerceCode = Request.Form["child_commerce_code"];
            var childBuyOrder = Request.Form["child_buy_order"];
            var amount = decimal.Parse(Request.Form["amount"]);
            var token = Request.Form["TBK_TOKEN"];
            var userName = Request.Form["user_name"];

            var result = MallDeferredTransaction.Refund(buyOrder, childCommerceCode,childBuyOrder,amount);
            Console.WriteLine(result);

            ViewBag.BuyOrder = buyOrder;
            ViewBag.ChildCommerceCode = childCommerceCode;
            ViewBag.ChildBuyOrder = childBuyOrder;
            ViewBag.Amount = amount;
            ViewBag.Result = result;
            ViewBag.Token = token;
            ViewBag.UserName = userName;
            
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("InscriptionDelete", "OneclickMallDeferred", null, Request.Scheme);

            return View();
        }
    }
}