using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwittBlock.WebTwitter
{
    public class Tweet
    {
        public string TweetId { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string Message { get; set; }
        public string UserScreenName { get; set; }

        public string LinkUrl { get; set; }
        public string RedirectedUrl { get; set; }

        public string Raw { get; set; }
    }
}
