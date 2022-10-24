using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using Transbank.Common;
using Transbank.Webpay.Common;

namespace Controllers
{
    public class BaseController : Controller
    {
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        public BaseController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }
        protected String GetRandomNumber()
        {
            var random = new Random();
            return random.Next(999999999).ToString();
        }

        protected String ToJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        protected String CreateUrl(String ctrl, String method)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return urlHelper.Action(method, ctrl, null, Request.Scheme);
        }
    }
}
