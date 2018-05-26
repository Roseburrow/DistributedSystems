using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;

namespace SecuroteckClient
{
    #region Task 8 and beyond
    class Client
    {
        static HttpClient client = new HttpClient();
        static private string storedUsername = "";
        static private string storedApiKey = "";
        static private string storedPublicKey = "";

        static void Main(string[] args)
        {
            client.BaseAddress = new Uri("http://localhost:24702/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add
                (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            bool firstRun = true;
            while (true)
            {
                if (firstRun)
                {
                    Console.WriteLine("Hello. What would you like to do?");
                    firstRun = false;
                }
                else
                {
                    Console.WriteLine("What would you like to do next?");
                }
                string[] input = Console.ReadLine().TrimEnd(' ').Split();
                Console.Clear();

                if (input[0] == "Exit" && input.Length == 1)
                {
                    Environment.Exit(0);
                }
                else if (input.Length <= 1)
                {
                    Console.WriteLine("Invalid Command!");
                }
                else if (input[0] == "TalkBack")
                {
                    TalkBackActions(input);
                }
                else if (input[0] == "User")
                {
                    UserActions(input);
                }
                else if (input[0] == "Protected")
                {
                    ProtectedActions(input);
                }
                else
                {
                    Console.WriteLine("Command Not Found!");
                }
            }
        }

        static async Task<string> CreateGetRequest(string path, bool asString)
        {
            string serverMsg = "";
            Console.WriteLine("...please wait...");

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return "Un-Authorised User";

            if (!asString)
            {
                return serverMsg = await response.Content.ReadAsAsync<string>();
            }
            else
            {
                return serverMsg = await response.Content.ReadAsStringAsync();
            }

        }

        static async Task<string> CreatePostRequest(string path, string content)
        {
            string serverMsg = "";
            Console.WriteLine("...please wait...");

            HttpResponseMessage response = await client.PostAsJsonAsync(path, content);
            serverMsg = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                storedApiKey = serverMsg;
                storedUsername = content;
                return "Got API Key";
            }
            return serverMsg;
        }

        static async Task<string> CreateDeleteRequest(string path)
        {
            string serverMsg = "";
            Console.WriteLine("...please wait...");

            HttpResponseMessage response = await client.DeleteAsync(path);
            serverMsg = await response.Content.ReadAsStringAsync();
            return serverMsg;
        }

        public static void TalkBackActions(string[] input)
        {
            if (input[1] == "Hello" && input.Length == 2)
            {
                TalkBackHello();
            }
            else if (input[1] == "Sort" && input.Length == 3)
            {
                TalkBackSort(input[2]);
            }
            else
            {
                Console.WriteLine("Invalid TalkBack Command!");
            }
        }

        public static void TalkBackHello()
        {
            string request = "talkback/Hello";
            Task<string> task = CreateGetRequest(request, false);
            Console.WriteLine(task.Result);
        }

        public static void TalkBackSort(string numbers)
        {
            string[] n = numbers.Trim('[',']').Split(',');
            string request = "talkback/sort?integers=" + n[0];

            for (int i = 1; i < n.Length; i ++)
            {
                request += "&integers=" + n[i];
            }
            Task<string> task = CreateGetRequest(request, true);
            Console.WriteLine(task.Result);
        }

        public static void UserActions(string[] input)
        {
            if (input[1] == "Get" && input.Length > 2)
            {
                UserGet(input[2]);
            }
            else if (input[1] == "Get" && input.Length == 2)
            {
                UserGet("");
            }
            else if (input[1] == "Post" && input.Length == 3)
            {
                UserPost(input[2]);
            }
            else if (input[1] == "Post" && input.Length == 2)
            {
                UserPost("");
            }
            else if (input[1] == "Delete" && input.Length == 2)
            {
                UserDelete();
            }
            else if (input[1] == "Set" && input.Length == 4)
            {
                UserSet(input[2], input[3]);
            }
            else
            {
                Console.WriteLine("Invalid User Command!");
            }
        }

        public static void UserGet(string username)
        {
            string request = "user/new?username=" + username;
            Task<string> task = CreateGetRequest(request, false);
            Console.WriteLine(task.Result);
        }

        public static void UserPost(string username)
        {
            string request = "user/new";
            Task<string> task = CreatePostRequest(request, username);
            Console.WriteLine(task.Result);
        }

        public static void UserDelete()
        {
            if (!CheckStoredInfo())
            {
                Console.WriteLine("You need to do a User Post or User Set first");
            }
            else
            {
                string request = "user/removeuser?username=" + storedUsername;
                client.DefaultRequestHeaders.Add("ApiKey", storedApiKey);

                Task<string> task = CreateDeleteRequest(request);
                client.DefaultRequestHeaders.Remove("ApiKey");

                if (task.Result == "true")
                {
                    Console.WriteLine("True");
                }
                else
                {
                    Console.WriteLine("False");
                }
            }
        }

        public static void ProtectedActions(string[] input)
        {
            string message;

            if (input[1] == "Hello" && input.Length == 2)
            {
                ProtectedHello();
            }
            else if (input[1] == "SHA1" && input.Length > 2)
            {
                message = string.Join(" ", input.Skip(1)).Replace("SHA1 ", "");
                ProtectedSHA1(message);
            }
            else if (input[1] == "SHA1" && input.Length == 2)
            {
                ProtectedSHA1("");
            }
            else if (input[1] == "SHA256" && input.Length > 2)
            {
                message = string.Join(" ", input.Skip(1)).Replace("SHA256 ", "");
                ProtectedSHA256(message);
            }
            else if (input[1] == "SHA256" && input.Length == 2)
            {
                ProtectedSHA256("");
            }
            else if (input[1] == "Get" && input[2] == "PublicKey" && input.Length == 3)
            {
                ProtectedGetPublicKey();
            }
            else if (input[1] == "Sign" && input.Length > 2)
            {
                message = string.Join(" ", input.Skip(1)).Replace("Sign ", "");
                ProtectedSign(message);
            }
            else
            {
                Console.WriteLine("Invalid Protected Command!");
            }
        }

        public static void ProtectedHello()
        {
            if (!CheckStoredInfo())
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }
            string request = "protected/hello";

            client.DefaultRequestHeaders.Add("ApiKey", storedApiKey);
            Task<string> task = CreateGetRequest(request, false);
            client.DefaultRequestHeaders.Remove("ApiKey");
            Console.WriteLine(task.Result);
        }

        public static void ProtectedSHA1(string message)
        {
            if (!CheckStoredInfo())
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }
            string request = "protected/sha1?message=" + message;

            client.DefaultRequestHeaders.Add("ApiKey", storedApiKey);
            Task<string> task = CreateGetRequest(request, true);
            client.DefaultRequestHeaders.Remove("ApiKey");
            Console.WriteLine(task.Result);
        }

        public static void ProtectedSHA256(string message)
        {
            if (!CheckStoredInfo())
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }
            string request = "protected/sha256?message=" + message;

            client.DefaultRequestHeaders.Add("ApiKey", storedApiKey);
            Task<string> task = CreateGetRequest(request, true);
            client.DefaultRequestHeaders.Remove("ApiKey");
            Console.WriteLine(task.Result);
        }

        public static void ProtectedGetPublicKey()
        {
            if (!CheckStoredInfo())
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }

            string request = "protected/getpublickey";
            client.DefaultRequestHeaders.Add("ApiKey", storedApiKey);
            Task<string> task = CreateGetRequest(request, false);

            client.DefaultRequestHeaders.Remove("ApiKey");
            if (task.Result != "" && task.Result != null && task.Result != "Un-Authorised User")
            {
                storedPublicKey = task.Result;
                Console.WriteLine("Got Public Key");
            }
            else
            {
                Console.WriteLine("Couldn't Get the Public Key");
            }
        }

        public static void ProtectedSign(string message)
        {
            if (message == "" || message == null)
            {
                Console.WriteLine("No message was given");
                return;
            }
            else if (!CheckStoredInfo())
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }

            string request = "protected/sign?message=" + message;

            client.DefaultRequestHeaders.Add("ApiKey", storedApiKey);
            Task<string> task = CreateGetRequest(request, false);
            client.DefaultRequestHeaders.Remove("ApiKey");

            if (storedPublicKey == "" || storedPublicKey == null)
            {
                Console.WriteLine("Client doesn't yet have the public key");
            }
            else if (VerifySign(message, task.Result))
            {
                Console.WriteLine("Message was successfully signed");
            }
            else
            {
                Console.WriteLine("Message was not successfully signed");
            }
        }

        private static bool VerifySign(string baseMsg, string serverResponse)
        {
            byte[] baseMsgBytes;
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                baseMsgBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(baseMsg));
            }

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(storedPublicKey);

            byte[] serverBytes = HexToBytes(serverResponse);
            if (provider.VerifyHash(baseMsgBytes, CryptoConfig.MapNameToOID("SHA1"), serverBytes))
            {
                return true;
            }
            return false;
        }

        public static void UserSet(string username, string apiKey)
        {
            storedApiKey = apiKey;
            storedUsername = username;
            Console.WriteLine("Stored");
        }

        public static bool CheckStoredInfo()
        {
            if (storedUsername == "" || storedUsername == null ||
                    storedApiKey == "" || storedApiKey == null)
            {
                return false;
            }
            return true;
        }

        public static byte[] HexToBytes(string hexString)
        {
            //Sourced From:
            //https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
            //By JaredPar and Edited by Robert Harvey.

            hexString = hexString.Replace("-", "");
            return Enumerable.Range(0, hexString.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                     .ToArray();
        }
    }
    #endregion
}