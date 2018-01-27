using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedTool
{
    class AuthInfo
    {
        public string Server;
        public string UserName;
        public string Password;
        public string Token;
        public string ServerFullName
        {
            get
            {
                switch (Server)
                {
                    case "0":
                        return "链克创世服";
                    case "wkc":
                        return "链克国际服";
                    default:
                        return "服务器" + Server;
                }
            }
        }
    }
}
