using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SecuroteckWebApplication.Controllers
{
    public class TalkBackController : ApiController
    {
        [ActionName("Hello")]
        public string Get()
        {
            #region TASK1
            return "Hello World!";
            #endregion
        }

        [ActionName("Sort")]
        public IHttpActionResult Get([FromUri]string[] integers)
        {
            #region TASK1
            if (integers.Length == 0 || integers[0] == null)
            {
                return Ok("[]");
            }
            else
            {
                int r;
                int[] numbers = new int[integers.Length];
                for (int i = 0; i < integers.Length; i++)
                {     
                    if (int.TryParse(integers[i], out r))
                    {
                        numbers[i] = r;
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

                Array.Sort(numbers);
                return Ok(numbers);
            }
            #endregion
        }
    }
}
