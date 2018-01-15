using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedTool
{
    class TransationReader
    {
        public static List<Tuple<AuthInfo, List<FeedInfo>>> Read(string fileName)
        {
            do
            {
                if (File.Exists(fileName))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("文件 【{0}】 不存在,请检查后按回车键继续", fileName);
                    Console.ReadKey();
                }
            }
            while (true);

            do
            {
                using (var fs = new StreamReader(fileName))
                {
                    var line = string.Empty;
                    var lineNum = 0;
                    Tuple<AuthInfo, List<FeedInfo>> curInfoTuple = null;
                    var result = new List<Tuple<AuthInfo, List<FeedInfo>>>();
                    try
                    {
                        var allFeedInfo = new List<FeedInfo>();
                        while ((line = fs.ReadLine()) != null)
                        {
                            lineNum++;
                            if (line.Length == 0 || line[0] == '#')
                            {
                                continue;
                            }

                            var words = line.Split(" \t".ToCharArray()).Where(word => !string.IsNullOrWhiteSpace(word)).ToArray();
                            if (words[0].Length >= 11) //A phone number or token
                            {
                                var authInfo = new AuthInfo();
                                if (words[0].Length > 11) //A token
                                {
                                    authInfo.Token = words[0];
                                }
                                else //A phone number
                                {
                                    authInfo.UserName = words[0];
                                    authInfo.Password = words[1];
                                }
                                curInfoTuple = new Tuple<AuthInfo, List<FeedInfo>>(authInfo, new List<FeedInfo>());
                                result.Add(curInfoTuple);
                            }
                            else
                            {
                                var feedInfo = new FeedInfo();
                                feedInfo.MonkeyId = int.Parse(words[0]);
                                if (words.Length <= 2)
                                {
                                    feedInfo.FeedType = FeedType.Peach;
                                    feedInfo.Amount = int.Parse(words[1]);
                                    feedInfo.Time = 1;
                                }
                                else
                                {
                                    feedInfo.FeedType = FeedType.WKC;
                                    feedInfo.Amount = double.Parse(words[1]);
                                    feedInfo.Time = int.Parse(words[2]);
                                }
                                if (curInfoTuple == null)
                                {
                                    Console.WriteLine("未找到登陆信息");
                                    throw new InvalidDataException();
                                }
                                curInfoTuple.Item2.Add(feedInfo);
                            }

                        }
                        return result;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("格式错误,请修改后按回车键继续:");
                        Console.WriteLine("第 【{0}】行: {1}", lineNum, line);
                        Console.ReadKey();
                        continue;
                    }
                }
            }
            while (true);
        }
    }
}
