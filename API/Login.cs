using System;
using System.Configuration;
using System.Threading;
using Utility;

namespace API
{
    public class Login
    {
        public static string TryLogin(string server, string name, string password, out int? code)
        {
            do
            {
                var loginURL = string.Format(ConfigurationManager.AppSettings["loginURL"], server);
                //TODO: Fake headers
                var client = new RestClient(loginURL, HttpVerb.POST, string.Format("phone={0}&pwd={1}", name, password));
                client.ContentType = "application/x-www-form-urlencoded";
                var retry = false;
                string json = string.Empty;
                try
                {
                    json = client.MakeRequest();
                }
                catch(Exception)
                {
                    retry = true;
                }

                if (string.IsNullOrEmpty(json) || retry)
                {
                    var waitTime = 10;
                    Console.WriteLine("无法连接服务器,{0} 秒后重试...", waitTime);
                    Thread.Sleep(waitTime * 1000);
                    continue;
                }

                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                //Console.WriteLine(jsonData.msg);
                if (jsonData.code == 200)
                {
                    code = 200;
                    return jsonData.result.token;
                }
                else
                {
                    code = jsonData.code;
                    return jsonData.msg;
                }
            }
            while (true);
        }
    }
}
