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
    public class OneclickMallController : Controller
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public OneclickMallController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }
        public ActionResult InscriptionStart()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = urlHelper.Action("InscriptionFinish", "OneclickMall", null, Request.Scheme);
            var userName = "18152024-k";
            var email = $"{RandomString(5)}@{RandomString(5)}.com";

            ViewBag.UserName = userName;
            ViewBag.Email = email;
            ViewBag.ReturnUrl = returnUrl;

            var response = (new MallInscription()).Start(userName, email, returnUrl);
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

            var result = (new MallInscription()).Finish(tbk_token);

            ViewBag.AuthorizationCode = result.AuthorizationCode;
            ViewBag.ResponseCode = result.ResponseCode;
            ViewBag.TbkUser = result.TbkUser;
            ViewBag.CreditCardType = result.CardType;
            ViewBag.LastFourCardDigits = result.CardNumber;
            ViewBag.Result = result;
            
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("TransactionAuthorize", "OneclickMall", null, Request.Scheme);

            return View();
        }

        public ActionResult TransactionAuthorize()
        {
            var userName = Request.Form["user_name"];
            var tbkUser = Request.Form["tbk_user"];
            var buyOrder = RandomString(10);
            
            var childCommerceCode = "597055555542";
            var childBuyOrder = RandomString(10);
            var amount = Decimal.Parse(Request.Form["amount"]);
            var installmentsNumber = 1;

            List<PaymentRequest> details = new List<PaymentRequest>();
            details.Add(new PaymentRequest(childCommerceCode, childBuyOrder, amount, installmentsNumber));

            var result = (new MallTransaction()).Authorize(userName, tbkUser, buyOrder, details);
            Console.WriteLine(result);

            ViewBag.UserName = userName;
            ViewBag.TbkUser = tbkUser;
            ViewBag.BuyOrder = buyOrder;
            ViewBag.Details = details.First();
            ViewBag.Result = result;
            
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("TransactionRefund", "OneclickMall", null, Request.Scheme);

            return View();
        }
        
        public ActionResult InscriptionDelete()
        {
            var userName = Request.Form["user_name"];
            var tbkUser = Request.Form["TBK_TOKEN"];

            (new MallInscription()).Delete(tbkUser, userName);

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

            var result = (new MallTransaction()).Refund(buyOrder, childCommerceCode, childBuyOrder, amount);
            Console.WriteLine(result);

            ViewBag.BuyOrder = buyOrder;
            ViewBag.ChildCommerceCode = childCommerceCode;
            ViewBag.ChildBuyOrder = childBuyOrder;
            ViewBag.Amount = amount;
            ViewBag.Result = result;
            ViewBag.Token = token;
            ViewBag.UserName = userName;
            
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            ViewBag.Action = urlHelper.Action("InscriptionDelete", "OneclickMall", null, Request.Scheme);

            return View();
        }
    }
}