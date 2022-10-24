using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.TransaccionCompleta;

namespace Controllers.TransaccionCompleta
{

    [Route("transaccion_completa_deferred")]
    public class FullTransactionDeferredController : BaseController
    {
        private FullTransaction tx;
        private String ctrlName = "transaccion_completa_deferred";
        private String viewBase = "Views/transaccion_completa_deferred/";
        public FullTransactionDeferredController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
            tx = new FullTransaction(new Options(IntegrationCommerceCodes.TRANSACCION_COMPLETA_DEFERRED, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
        }
    }
}