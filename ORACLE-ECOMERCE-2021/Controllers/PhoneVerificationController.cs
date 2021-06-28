using System.Threading.Tasks;
using ORACLE_ECOMERCE_2021.Models.Authy;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace ORACLE_ECOMERCE_2021.Controllers {

    public class PhoneVerificationController : ApplicationController
    {
        public IAuthy authy;

        public PhoneVerificationController(IAuthy authy)
        {
            this.authy = authy;
        }

        public IActionResult Index(PhoneVerificationRequestModel model)
        {
            return View(model);
        }

        public IActionResult Verification(TokenVerificationModel model)
        {
            return View(model);
        }

        [HttpPost]
        [Route("/verification/start")]
        public async Task<ActionResult> Start(PhoneVerificationRequestModel verificationRequest)
        {
            string sessionStr = JsonConvert.SerializeObject(verificationRequest);
            HttpContext.Session.SetString(_2faVerificationModelSessionName, sessionStr);

            if (ModelState.IsValid)
            {
                string result;
                result = await authy.phoneVerificationRequestAsync(
                    verificationRequest.CountryCode,
                    verificationRequest.PhoneNumber
                );

                return RedirectToAction("Verification", "PhoneVerification");
                
            } else {
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route("/verification/verify")]
        public async Task<ActionResult> Verify(TokenVerificationModel tokenVerification)
        {
            string sessionStr = HttpContext.Session.GetString(_2faVerificationModelSessionName);
            var verificationRequest = JsonConvert
                .DeserializeObject<PhoneVerificationRequestModel>(sessionStr);

            if (ModelState.IsValid)
            {
                var validationResult = await authy.verifyPhoneTokenAsync(
                    verificationRequest.PhoneNumber,
                    verificationRequest.CountryCode,
                    tokenVerification.Token ?? "0000"
                );

                if (validationResult.Succeeded)
                {
                    return RedirectToAction("Authy_PhoneVerified", "Customer");
                }
                else
                {
                    var modelErrors = this.GetModelStateDictionary<TokenVerificationModel>();
                    modelErrors.Add("Summary", new ModelStateError()
                    {
                        ErrorMessages = $"Mã xác thực không chính xác!! Vui lòng kiểm tra lại"
                    });

                    ViewBag.ModelErrors = modelErrors;
                    return View("Verification", tokenVerification);
                }
            }

            return BadRequest();
        }
    }
}