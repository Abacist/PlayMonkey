using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedTool
{
    enum FeedType
    {
        WKC,
        Peach
    }

    class FeedInfo
    {
        public int MonkeyId;
        public FeedType FeedType;
        public double Amount;
        public int Time;
    }
}
