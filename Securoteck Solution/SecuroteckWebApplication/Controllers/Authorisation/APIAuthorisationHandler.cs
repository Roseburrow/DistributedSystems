using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SecuroteckWebApplication.Models;

namespace SecuroteckWebApplication.Controllers
{
    public class APIAuthorisationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then authorise the principle on the current thread using a claim, claimidentity and claimsprinciple

            IEnumerable<string> headerValues;
            string headerKey = "";

            if (request.Headers.TryGetValues("ApiKey", out headerValues))
            {
                headerKey = headerValues.First();
                UserDatabaseAccess accessor = new UserDatabaseAccess();
                User existingUser = accessor.CheckandGetUserExists(headerKey);

                //Will check for user even if nothing exists as header key is "".
                //Maybe not the best idea for efficiency but it's ok for now.
                if (existingUser != null)
                {
                    //Then the key is valid.
                    Claim claim = new Claim(ClaimTypes.Name, existingUser.m_UserName);
                    ClaimsIdentity identity = new ClaimsIdentity(new[] { claim }, "ApiKey");
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    Thread.CurrentPrincipal = principal;
                }
            }
            #endregion
            return base.SendAsync(request, cancellationToken);
        }
    }
}