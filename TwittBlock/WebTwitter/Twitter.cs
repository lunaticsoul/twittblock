using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TwittBlock.WebUtils;

namespace TwittBlock.WebTwitter
{
    public class Twitter
    {
        CookieContainer cookieContainer;
        HttpClientHandler handler;
        HttpClient client;
        string authenticity_token;
        public Twitter()
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            client.DefaultRequestHeaders.Add("referer", "https://twitter.com");
        }

        public async Task<bool> Login(string userName, string password)
        {
            var loginPage = await client.GetStringAsync("https://twitter.com/login");
            var form = WebParser.Parse(loginPage).Where(f => f.Action == "https://twitter.com/sessions").First();

            if(form != null)
            {
                Dictionary<string, string> postData = new Dictionary<string, string>();
                foreach(var input in form.Inputs)
                {
                    postData[input.Name] = input.Value;
                }

                postData["session[username_or_email]"] = userName;
                postData["session[password]"] = password;
                postData["redirect_after_login"] = "/";

                authenticity_token = postData["authenticity_token"];

                HttpContent content = new FormUrlEncodedContent(postData);
                var result = await client.PostAsync(form.Action, content);
                return true;
            }

            return false;
        }

        public async Task<List<Tweet>> GetFeeds(string url, int limit)
        {
            List<Tweet> tweets = new List<Tweet>();
            var feed = await client.GetStringAsync(url);
            int start, end = 0;

            int timeLineIndex = feed.IndexOf("<div id=\"timeline\"");
            if (timeLineIndex == -1)
                return new List<Tweet>();

            end = timeLineIndex;
            do
            {
                string tweetHtml = feed.Substring("<li class=\"js-stream-item stream-item stream-item", "<div class=\"stream-item-footer\">", out start, out end, end);
                if(end != -1)
                {
                    Tweet tweet = new Tweet();
                    tweet.TweetId = tweetHtml.Substring("data-item-id=\"", "\"");
                    tweet.UserId = tweetHtml.Substring("data-user-id=\"", "\"");
                    tweet.UserScreenName = tweetHtml.Substring("data-screen-name=\"", "\"");
                    tweet.UserName = tweetHtml.Substring("data-name=\"", "\"");
                    tweet.Content = tweetHtml.Substring("<div class=\"js-tweet-text-container\">", "</div>");
                    tweet.LinkUrl = tweetHtml.Substring("data-card-url=\"", "\"");
                    if(!string.IsNullOrEmpty(tweet.LinkUrl))
                        tweet.RedirectedUrl = await GetRedirectLink(tweet.LinkUrl);

                    tweet.Message = tweet.Content.CleanseTwitterMessage();

                    tweet.Raw = tweetHtml;
                    tweets.Add(tweet);
                }
            }
            while(start != -1);

            return tweets;
        }

        public async Task<string> GetRedirectLink(string url)
        {
            string text = await client.GetStringAsync(url);
            return text.Substring("<title>", "</title>");
        }

        public async Task Block(Tweet tweet)
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData["authenticity_token"] = authenticity_token;
            postData["challenges_passed"] = "false";
            postData["handles_challenges"] = "1";
            postData["user_id"] = tweet.UserId;

            HttpContent content = new FormUrlEncodedContent(postData);
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            string url = "https://twitter.com/i/user/block";
            var result = await client.PostAsync(url, content);

            string responseContent = await result.Content.ReadAsStringAsync();
            //client.DefaultRequestHeaders.Remove("content-type");
        }
    }
}
