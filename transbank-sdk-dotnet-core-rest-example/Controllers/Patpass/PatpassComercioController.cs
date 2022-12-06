using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Transbank.Common;
using Transbank.Patpass.Common;
using Transbank.Patpass.PatpassComercio;

namespace Controllers.Patpass
{

    [Route("patpass_comercio")]
    public class PatpassComercioController : BaseController
    {
        private Inscription inscription;

        public PatpassComercioController(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) :
            base(urlHelperFactory, actionContextAccessor)
        {
            inscription = new Inscription(new Options(IntegrationCommerceCodes.PATPASS_COMERCIO, IntegrationApiKeys.PATPASS_COMERCIO, PatpassComercioIntegrationType.Live));
        }

        [Route("start")]
        public ActionResult Start()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            string returnUrl = "https://mvargas:5001/patpass_comercio/commit";//request.getRequestURL().toString().replace("start","commit"); // urlHelper.Action("start", "patpass_comercio", null, Request.Scheme);
            string name = "Isaac";
            string lastName = "Newton";
            string secondLastName = "Gonzales";
            string rut = "11111111-1";
            string serviceId = "Service_" + GetRandomNumber();
            string finalUrl = "https://mvargas:5001/patpass_comercio/voucher_return"; //request.getRequestURL().toString().replace("start","voucher_return");//urlHelper.Action("Finish", "PatpassComercio", null, Request.Scheme);
            decimal maxAmount = 100000;
            string phone = "123456734";
            string cellPhone = "123456723";
            string patpassName = "Membresia de cable";
            string personEmail = "developer@continuum.cl";
            string commerceEmail = "developer@continuum.cl";
            string address = "huerfanos 101";
            string city = "Santiago";


            var response = inscription.Start(
                url: returnUrl,
                name: name,
                lastName: lastName,
                secondLastName: secondLastName,
                rut: rut,
                serviceId: serviceId,
                finalUrl: finalUrl,
                maxAmount: maxAmount,
                phone: phone,
                cellPhone: cellPhone,
                patpassName: patpassName,
                personEmail: personEmail,
                commerceEmail: commerceEmail,
                address: address,
                city: city);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Name = name;
            ViewBag.LastName = lastName;
            ViewBag.SecondLastName = secondLastName;
            ViewBag.Rut = rut;
            ViewBag.ServiceId = serviceId;
            ViewBag.FinalUrl = finalUrl;
            ViewBag.MaxAmount = maxAmount;
            ViewBag.Phone = phone;
            ViewBag.CellPhone = cellPhone;
            ViewBag.PatpassName = patpassName;
            ViewBag.PersonEmail = personEmail;
            ViewBag.CommerceEmail = commerceEmail;
            ViewBag.Address = address;
            ViewBag.City = city;

            ViewBag.UrlWebpay = response.Url;
            ViewBag.TbkToken = response.Token;
            ViewBag.Resp = ToJson(response);

            return View("Views/patpass_comercio/start.cshtml");
        }

        [Route("commit")]
        public ActionResult Commit()
        {
            var token = Request.Form["j_token"];
            var response = inscription.Status(token);

            ViewBag.UrlVoucher = response.UrlVoucher;
            ViewBag.Token = token;
            ViewBag.Status = response.Status;

            ViewBag.Resp = ToJson(response);
            return View("Views/patpass_comercio/commit.cshtml");
        }

        [Route("voucher_return")]
        public ActionResult VoucherReturn()
        {
            var token = Request.Form["j_token"];
            ViewBag.UrlVoucher = "https://pagoautomaticocontarjetasint.transbank.cl/nuevo-ic-rest/tokenVoucherLogin";
            ViewBag.Token = token;
            return View("Views/patpass_comercio/voucher-return.cshtml");
        }


    }
}