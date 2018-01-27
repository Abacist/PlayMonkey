using API;
using System;
using System.Collections.Generic;
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
            var di = new DirectoryInfo("..");
            var validFileNames = new List<string>();
            do
            {
                foreach (var next in di.GetFiles())
                {
                    if (next.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        var valid = true;
                        foreach (var c in next.Name.Substring(0, next.Name.LastIndexOf(".txt", StringComparison.OrdinalIgnoreCase)))
                        {
                            if (!char.IsLetterOrDigit(c))
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (valid)
                        {
                            validFileNames.Add(next.FullName);
                        }
                    }
                }

                if (validFileNames.Count == 0)
                {
                    Console.WriteLine("未能找到喂食文件,请确保喂食文件位于本工具上级目录且格式正确后按回车键继续");
                    Console.ReadLine();
                }
            }
            while (validFileNames.Count == 0);

            var allFeedInfo = new List<Tuple<AuthInfo, List<FeedInfo>>>();

            foreach (var fileName in validFileNames)
            {
                var feedInfo = TransationReader.Read(fileName);
                if (feedInfo != null)
                {
                    Console.WriteLine("喂食文件 {0} 读取成功", fileName);
                    allFeedInfo.AddRange(feedInfo);
                }
            }

            Console.Write("信息读取完毕,按回车键开始自动喂猴");
            Console.ReadLine();

            //TODO: list all feed info
            Work(allFeedInfo);

            Console.WriteLine();
            Console.WriteLine("全部完成!");
            Console.WriteLine();
            Console.WriteLine("QQ:84065523. 申请好友后可以获得工具的最新更新");
            Console.WriteLine("如有其它需求也可联系");
            Console.WriteLine();
            Console.WriteLine("**********************************************************");
            Console.WriteLine("*    是否愿意向工具作者转账任意数量链克以支持后续发展?   *");
            Console.WriteLine("*   链克地址:0x227b583dbf826cb3c7cce26ce9acdc37001835a1  *");
            Console.WriteLine("*         若愿意,请按回车键,二维码图片将自动打开         *");
            Console.WriteLine("**********************************************************");
            Console.ReadLine();

            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream("FeedTool.Sponsor.bmp");
            var image = new Bitmap(imageStream);
            image.Save(@"..\赞助作者.bmp");
            Process.Start(@"..\赞助作者.bmp");
        }

        static void Work(List<Tuple<AuthInfo, List<FeedInfo>>> allInfo)
        {
            foreach (var info in allInfo)
            {
                var authInfo = info.Item1;
                Feed feed = null;
                if (!string.IsNullOrEmpty(authInfo.Token))
                {
                    feed = new Feed(authInfo.Server, authInfo.Token);
                }
                else
                {
                    feed = new Feed(authInfo.Server, authInfo.UserName, authInfo.Password);
                }

                foreach (var feedInfo in info.Item2)
                {
                    Console.Write("正在喂食 {0} {1} 号猴子 {2} 个 ", authInfo.ServerFullName, feedInfo.MonkeyId, feedInfo.Amount);
                    bool success = false;
                    switch (feedInfo.FeedType)
                    {
                        case FeedType.Peach:
                            Console.Write("蟠桃...");
                            success = feed.FeedPeach(feedInfo.MonkeyId, (int)feedInfo.Amount);
                            break;
                        case FeedType.WKC:
                            Console.Write("WKC * {0}...", feedInfo.Time);
                            for (int i = 0; i < feedInfo.Time; i++)
                            {
                                success = feed.FeedWKC(feedInfo.MonkeyId, feedInfo.Amount);
                                if (!success)
                                {
                                    break;
                                }
                            }
                            break;
                    }

                    if (success)
                    {
                        Console.WriteLine("\t完成");
                    }
                }
            }
        }
    }
}
