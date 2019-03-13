using System;
using System.Collections.Generic;
using System.Compat.Web;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwittBlock.WebTwitter;

namespace TwittBlock
{
    public class Blocker
    {
        int blockCount = 0;
        string[] blockingLinks;
        Twitter twitter;
        List<string> searched = new List<string>();

        public Blocker(string[] blockingLinks)
        {
            twitter = new Twitter();
            this.blockingLinks = blockingLinks;
        }

        public async Task StartScan(string userName, string password)
        {
            if (!await twitter.Login(userName, password))
            {
                Console.WriteLine("Login failed");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            int previousBlockCount = 0;
            while (true)
            {
                List<string> rootKeywords = new List<string>();
                rootKeywords.AddRange(blockingLinks);
                rootKeywords.AddRange(blockingLinks.Select(l => l.Split('.')[0]));

                try
                {
                    foreach (var link in rootKeywords)
                    {
                        List<Tweet> tweets = Judge(await twitter.GetFeeds("https://twitter.com/search?f=tweets&vertical=default&q=" + HttpUtility.UrlEncode(link) + "&src=typd", 100));
                        do
                        {
                            Console.WriteLine($"Blocking ({link}): " + tweets.Count);
                            tweets = await BlockProcess(tweets, false);
                        }
                        while (tweets.Count > 0);
                    }

                    //if (blockCount > previousBlockCount)
                    //{
                    //    Console.WriteLine("Blocked " + blockCount);
                    //    previousBlockCount = blockCount;
                    //}
                    //else
                    Console.WriteLine("Waiting for next scan");

                    Thread.Sleep(60000);
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        private async Task<List<Tweet>> BlockProcess(List<Tweet> tweets, bool deepSearch)
        {
            List<Tweet> nextBlockTweets = new List<Tweet>();

            foreach (var tweet in tweets)
            {
                try
                {
                    if (deepSearch)
                    {
                        var accountBlockingTweets = await twitter.GetFeeds("https://twitter.com/" + tweet.UserScreenName, 100);
                        await twitter.Block(tweet);
                        blockCount++;

                        accountBlockingTweets = Judge(accountBlockingTweets);
                        foreach (var accountTweet in accountBlockingTweets)
                        {
                            string message = accountTweet.Message.Replace("\n", "");
                            if (!searched.Contains(message))
                            {
                                searched.Add(message);
                                nextBlockTweets.AddRange(Judge(await twitter.GetFeeds("https://twitter.com/search?f=tweets&vertical=default&q=" + HttpUtility.UrlEncode(message) + "&src=typd", 100)));
                            }
                        }
                    }
                    else
                    {
                        await twitter.Block(tweet);
                    }
                }
                catch(Exception exception)
                {

                }
            }

            return nextBlockTweets;
        }

        private List<Tweet> Judge(List<Tweet> tweets)
        {
            int no = 0;
            List<Tweet> targets = new List<Tweet>();
            foreach (var tweet in tweets)
            {
                bool found = false;
                string cause = "";
                foreach (var link in blockingLinks)
                {
                    if (tweet.Content.Contains(link))
                    {
                        found = true;
                        cause = link;
                        break;
                    }
                }

                if (!found && !string.IsNullOrEmpty(tweet.LinkUrl))
                {
                    foreach (var link in blockingLinks)
                    {
                        if (tweet.RedirectedUrl.Contains(link))
                        {
                            found = true;
                            cause = tweet.RedirectedUrl;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var link in blockingLinks)
                    {
                        if (tweet.Content.Contains(link))
                        {
                            found = true;
                            cause = link;
                            break;
                        }
                    }
                }

                if (found)
                {
                    //Console.WriteLine("No: " + ++no);
                    //Console.WriteLine("Name: " + tweet.UserScreenName);
                    //Console.WriteLine("Cause: " + cause);
                    //await twitter.Block(tweet);
                    targets.Add(tweet);
                }
            }

            return targets;
        }
    }
}
