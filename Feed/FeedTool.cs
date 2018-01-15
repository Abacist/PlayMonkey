using API;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace FeedTool
{
    class FeedTool
    {
        static void Main(string[] args)
        {
            var fileName = ConfigurationManager.AppSettings["FeedFile"];
            var allInfo = TransationReader.Read(fileName);
            foreach (var info in allInfo)
            {
                var authInfo = info.Item1;
                Feed feed = null;
                if (!string.IsNullOrEmpty(authInfo.Token))
                {
                    feed = new Feed(authInfo.Token);
                }
                else
                {
                    feed = new Feed(authInfo.UserName, authInfo.Password);
                }

                foreach (var feedInfo in info.Item2)
                {
                    Console.Write("正在喂食 {0} 号猴子 {1} 个 ", feedInfo.MonkeyId, feedInfo.Amount);
                    switch (feedInfo.FeedType)
                    {
                        case FeedType.Peach:
                            Console.Write("蟠桃...");
                            feed.FeedPeach(feedInfo.MonkeyId, (int)feedInfo.Amount);
                            break;
                        case FeedType.WKC:
                            Console.Write("WKC * {0}...", feedInfo.Time);
                            for (int i = 0; i < feedInfo.Time; i++)
                            {
                                feed.FeedWKC(feedInfo.MonkeyId, feedInfo.Amount);
                            }
                            break;
                    }
                    Console.WriteLine("完成");
                }
            }

            Console.WriteLine();
            Console.WriteLine("全部完成");
            Console.WriteLine();
            Console.WriteLine("QQ:84065523. 申请好友后可以获得工具的最新更新");
            Console.WriteLine("如有其它需求也可联系");
            Console.WriteLine();
            Console.WriteLine("**********************************************************");
            Console.WriteLine("*    是否愿意向工具作者转账任意数量链克以支持后续发展?   *");
            Console.WriteLine("*   链克地址:0x227b583dbf826cb3c7cce26ce9acdc37001835a1  *");
            Console.WriteLine("*         若愿意,请按任意键,二维码图片将自动打开         *");
            Console.WriteLine("**********************************************************");
            Console.ReadKey();

            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream("FeedTool.Sponsor.bmp");
            var image = new Bitmap(imageStream);
            image.Save(@"..\Sponsor.bmp");
            Process.Start(@"..\Sponsor.bmp");
        }
    }
}
