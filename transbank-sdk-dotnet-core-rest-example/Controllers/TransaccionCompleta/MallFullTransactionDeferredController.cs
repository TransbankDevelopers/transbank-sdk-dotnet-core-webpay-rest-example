using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.TransaccionCompletaMall;

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
            tx = new MallFullTransaction(new Options(IntegrationCommerceCodes.TRANSACCION_COMPLETA_MALL_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
        }
    }
}