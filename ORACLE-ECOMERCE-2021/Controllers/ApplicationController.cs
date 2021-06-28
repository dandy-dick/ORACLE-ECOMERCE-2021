using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheAchEcom.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ORACLE_ECOMERCE_2021.Models;
using Repository.SqlDataProvider;

namespace ORACLE_ECOMERCE_2021.Controllers
{
    public class ModelStateError
    {
        public string State { get; private set; } = "invalid";
        public string ErrorMessages { get; set; }

        public void SetState(ModelValidationState state)
        {
            switch (state)
            {
                case ModelValidationState.Invalid:
                    this.State = "invalid";
                    break;
                case ModelValidationState.Skipped:
                    this.State = "skipped";
                    break;
                case ModelValidationState.Valid:
                    this.State = "valid";
                    break;
            }
        }
    }

    public class ApplicationController : Controller
    {
        protected const string _2faRegisterModelSessionName = "_2faRegisterModelSessionName";
        protected const string _2faVerificationModelSessionName = "_2faVerificationModelSessionName";

        protected const string _connString = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=orcl)));User ID=system;Password=123123;";

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /* Để custom validation thôi */
        public Dictionary<string, ModelStateError> GetModelStateDictionary<TModel>()
        {
            var result = new Dictionary<string, ModelStateError>();
            foreach (var item in ModelState)
            {
                var errorMessages = item.Value.Errors
                    .Select(p => "-" + p.ErrorMessage)
                    .ToArray<string>();

                var error = new ModelStateError
                {
                    ErrorMessages = String.Join("<br>", errorMessages)
                };
                error.SetState(item.Value.ValidationState);

                result.Add(item.Key, error);
            }
            return result;
        }

        internal void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public string GetConnectionString()
        {
            return _connString;
        }
    }
}
