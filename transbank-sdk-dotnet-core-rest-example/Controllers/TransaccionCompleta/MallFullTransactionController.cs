using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.TransaccionCompletaMall;

namespace Controllers.TransaccionCompleta
{

    [Route("transaccion_completa_mall")]
    public class MallFullTransactionController : BaseController
    {
        private MallFullTransaction tx;
        private String ctrlName = "transaccion_completa_mall";
        private String viewBase = "Views/transaccion_completa_mall/";
        public MallFullTransactionController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
            tx = new MallFullTransaction(new Options(IntegrationCommerceCodes.TRANSACCION_COMPLETA_MALL, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
        }
    }
}