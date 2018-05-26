using SecuroteckWebApplication.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;


namespace SecuroteckWebApplication.Controllers
{
    public class ProtectedController : ApiController
    {
        UserDatabaseAccess accessor = new UserDatabaseAccess();

        [CustomAuthorise]
        [ActionName("Hello")]
        public string Get()
        {
            string headerKey = Request.Headers.GetValues("ApiKey").First();
            User user = accessor.CheckandGetUserExists(headerKey);

            accessor.CreateNewLogEntry(headerKey, "User request Protected/Hello");
            return "Hello " + user.m_UserName;
        }

        [CustomAuthorise]
        [HttpGet]
        [ActionName("SHA1")]
        public HttpResponseMessage SHA1([FromUri]string message)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            if (message != "" && message != null)
            {
                byte[] asciiMessage = System.Text.Encoding.ASCII.GetBytes(message);
                SHA1 sha1Provider = new SHA1CryptoServiceProvider();
                byte[] SHA1Message = sha1Provider.ComputeHash(asciiMessage);

                string hexMessage = BytesArrayToString(SHA1Message).Replace("-", "");
                response.Content = new StringContent(hexMessage);
            }
            else
            {
                response.Content = new StringContent("Bad Request");
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            return response;
        }

        [CustomAuthorise]
        [HttpGet]
        [ActionName("SHA256")]
        public HttpResponseMessage SHA256([FromUri]string message)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            if (message != "" && message != null)
            {
                byte[] asciiMessage = System.Text.Encoding.ASCII.GetBytes(message);
                SHA256 sha256Provider = new SHA256CryptoServiceProvider();
                byte[] SHA256Message = sha256Provider.ComputeHash(asciiMessage);

                string hexMessage = BytesArrayToString(SHA256Message).Replace("-", "");
                response.Content = new StringContent(hexMessage);
            }
            else
            {
                response.Content = new StringContent("Bad Request");
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            return response;
        }

        [CustomAuthorise]
        [HttpGet]
        [ActionName("GetPublicKey")]
        public string GetPublicKey()
        {
            string headerKey = Request.Headers.GetValues("ApiKey").First();
            if (accessor.CheckUserExists(headerKey))
            {
                accessor.CreateNewLogEntry(headerKey, "User request Protected/GetPublicKey");
                return WebApiConfig.RSAProvider.ToXmlString(false);
            }
            return "";
        }

        [CustomAuthorise]
        [HttpGet]
        [ActionName("Sign")]
        public string Sign([FromUri]string message)
        {
            string headerKey = Request.Headers.GetValues("ApiKey").First();
            
            if (accessor.CheckUserExists(headerKey))
            {
                byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
                using (var sha1 = new SHA1CryptoServiceProvider())
                {
                    byte[] signed = WebApiConfig.RSAProvider.SignData(messageBytes, CryptoConfig.MapNameToOID("SHA1"));
                    accessor.CreateNewLogEntry(headerKey, "User request Protected/Sign");
                    return BytesArrayToString(signed);
                }
            }
            return "";
        }

        private string BytesArrayToString(byte[] byteArray)
        {
            return BitConverter.ToString(byteArray);
        }
    }
}
