using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SecuroteckWebApplication.Models;

namespace SecuroteckWebApplication.Controllers
{
    public class UserController : ApiController
    {
        UserDatabaseAccess accessor = new UserDatabaseAccess();

        [ActionName("New")]
        public string Get([FromUri]string username)
        {
            if (username == "" || username == null)
            {
                return "False - User Does Not Exist! Did you mean to do a POST to create a new user?";
            }
            else if (accessor.CheckUsernameExists(username))
            {
                return "True - User Does Exist! Did you mean to do a POST to create a new user?";
            }
            else
            {
                return "False - User Does Not Exist! Did you mean to do a POST to create a new user?";
            }
        }

        [ActionName("New")]
        public HttpResponseMessage Post([FromBody]string username)
        {
            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);

            if (username == "" || username == null)
            {
                message.StatusCode = HttpStatusCode.BadRequest;
                message.Content = new StringContent("Oops. Make sure your body contains a string" +
                      " with your username and your Content-Type is Content-Type:application/json");
            }
            else
            {
                Guid newKey = Guid.NewGuid();
                User newUser = new User()
                {
                    m_ApiKey = newKey.ToString(),
                    m_UserName = username
                };

                using (var context = new UserContext())
                {
                    context.Users.Add(newUser);
                    context.SaveChanges();
                    message.Content = new StringContent(newKey.ToString());
                }
            }
            return message;
        }

        [CustomAuthorise]
        [ActionName("RemoveUser")]
        public bool Delete([FromUri]string username)
        {
            string headerKey = Request.Headers.GetValues("ApiKey").First();
            User user = accessor.CheckandGetUserExists(headerKey);

            if (user != null && user.m_UserName == username)
            {
                accessor.DeleteUser(headerKey);
                accessor.CreateNewLogEntry(headerKey, "User request User/Delete");
                return true;
            }
            return false;
        }
    }
}
