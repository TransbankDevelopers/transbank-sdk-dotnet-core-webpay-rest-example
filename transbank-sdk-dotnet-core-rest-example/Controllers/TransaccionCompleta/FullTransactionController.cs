using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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
            tx = new FullTransaction(new Options(IntegrationCommerceCodes.TRANSACCION_COMPLETA, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
        }
    }
}