using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Net;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.Oneclick;

namespace Controllers.Oneclick
{

    [Route("oneclick_mall")]
    public class OneclickMallController : BaseController
    {
        private MallInscription ins;
        private MallTransaction tx;
        private String ctrlName = "oneclick_mall";
        private String viewBase = "Views/oneclick_mall/";
        public OneclickMallController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
            ins = new MallInscription(new Options(IntegrationCommerceCodes.ONECLICK_MALL, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            tx = new MallTransaction(new Options(IntegrationCommerceCodes.ONECLICK_MALL, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
        }
    }
}