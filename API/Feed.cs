using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Utility;

namespace API
{
    public class Feed
    {
        private string userName;
        private string password;
        private string server;
        private string token;
        private string feedWKCURL;
        private string feedPeachURL;

        public Feed(string server, string userName, string password)
        {
            this.userName = userName;
            this.password = password;
            this.server = server;

            this.feedWKCURL = string.Format(ConfigurationManager.AppSettings["feedWKCURL"], server);
            this.feedPeachURL = string.Format(ConfigurationManager.AppSettings["feedPeachURL"], server);

            this.UpdateToken();
        }

        public Feed(string server, string token)
        {
            this.token = token;

            this.feedWKCURL = string.Format(ConfigurationManager.AppSettings["feedWKCURL"], server);
            this.feedPeachURL = string.Format(ConfigurationManager.AppSettings["feedPeachURL"], server);
        }

        public bool FeedWKC(int id, double wkc)
        {
            var body = string.Format("coin={0}&monkeyId={1}", wkc, id);
            return Post(this.feedWKCURL, body);
        }

        public bool FeedPeach(int id, int count)
        {
            var body = string.Format("num={0}&monkeyId={1}", count, id);
            return Post(this.feedPeachURL, body);
        }

        private void UpdateToken()
        {
            int? code;
            do
            {
                Thread.Sleep(1000);
                this.token = Login.TryLogin(server, userName, password, out code);
                if (code != 200)
                {
                    Console.WriteLine();
                    Console.WriteLine("登陆失败,服务器返回消息: 【{0}】\n请输入正确的账号密码,原账号密码: {1}  {2}", this.token ?? "无", userName, password);
                    Console.Write("账号:");
                    userName = Console.ReadLine();
                    Console.Write("密码:");
                    password = Console.ReadLine();
                }
            }
            while (code != 200);
        }

        private bool Post(string url, string body)
        {
            do
            {
                Thread.Sleep(1000);
                var headers = new Dictionary<string, string>();
                headers["accessToken"] = this.token;
                var client = new RestClient(url, HttpVerb.POST, body, headers);
                client.ContentType = "application/x-www-form-urlencoded";
                var retry = false;
                string json = string.Empty;
                try
                {
                    json = client.MakeRequest();
                }
                catch (Exception)
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
                if (jsonData.code == 401)
                {
                    this.UpdateToken();
                    continue;
                }
                else if (jsonData.code == 200)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("请求失败,服务器返回消息: 【{0}】", jsonData.msg);
                    string input;
                    do
                    {
                        Console.WriteLine("按【1】并回车以重新请求,按【2】并回车以跳过并执行下一条请求");
                        input = Console.ReadLine();
                    }
                    while (input != "1" && input != "2");

                    if (input == "1")
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            while (true);
        }
    }
}
