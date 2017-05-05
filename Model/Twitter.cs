using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

namespace [namespace].Models
{
    /// <summary>
    /// Tweet Model Object Sample
    /// </summary>
    public class Tweet
    {
        public string Author { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// OAuth Authentication
    /// </summary>
    public class TweetAuthenticateResponse
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
    }

    /// <summary>
    /// Twitter Class
    /// </summary>
    public class Twitter
    {
        const string OAUTH_URL = "https://api.twitter.com/oauth2/token";
        const string API_URL = "https://api.twitter.com/1.1/search/tweets.json?q=%23";

        const string OAUTH_CONSUMER_KEY = "<YOUR_CONSUMER_KEY>";
        const string OAUT_CONSUMER_SECRET = "<YOUR_CONSUMER_SECRET>";
        const string SCREEN_NAME = "<SCREEN_NAME>";

        public static bool IsAuthenticated { get; set; }
        public static TwitterAuthenticateResponse Token { get; set; }

        /// <summary>
        /// Authenticates in Twitter
        /// </summary>
        public List<Tweet> Twitter()
        {
            if (!IsAuthenticated)
            {
                Token = Authenticate();
            }
        }

        /// <summary>
        /// Get the tweets
        /// </summary>
        public List<Tweet> Tweets(int count, string hashtag)
        {
            var tweets = string.Format("{0}{1}{2}{3}", "https://api.twitter.com/1.1/search/tweets.json?q=%23", hashtag, "&result_type=recent&count=", count);
            HttpWebRequest tweetsRequest = (HttpWebRequest)WebRequest.Create(tweets);

            tweetsRequest.Headers.Add("Authorization", string.Format("{0} {1}", Token.token_type, Token.access_token));
            tweetsRequest.Method = "Get";

            WebResponse tweetsResponse = tweetsRequest.GetResponse();
            using (tweetsResponse)
            {
                using (var reader = new StreamReader(tweetsResponse.GetResponseStream()))
                {
                    return Parse(reader.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// Authenticates in Twitter
        /// </summary>
        private TwitterAuthenticateResponse Authenticate()
        {
            // Authenticate
            var authHeaderFormat = "Basic {0}";
            var authHeader = string.Format(authHeaderFormat,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(OAUTH_CONSUMER_KEY) + ":" +
                Uri.EscapeDataString((OAUT_CONSUMER_SECRET)))
            ));
            var postBody = "grant_type=client_credentials";

            // Request
            HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(OAUTH_URL);
            authRequest.Headers.Add("Authorization", authHeader);
            authRequest.Method = "POST";
            authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            authRequest.Headers.Add("Accept-Encoding", "gzip");
            using (Stream stream = authRequest.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            // Response
            WebResponse authResponse = authRequest.GetResponse();
            using (authResponse)
            {
                using (var reader = new StreamReader(authResponse.GetResponseStream()))
                {
                    var js = new JavaScriptSerializer();
                    var objectText = reader.ReadToEnd();
                    IsAuthenticated = true;

                    return JsonConvert.DeserializeObject<TwitterAuthenticateResponse>(objectText);
                }
            }
        }

        /// <summary>
        /// Parsing from a JSON to a Tweet Object
        /// </summary>
        private List<Tweet> Parse(string json)
        {
            var output = new List<Tweet>();

            JObject jsonObj = JObject.Parse(json);

            var tweets = from t in jsonObj["statuses"]
                         select t;

            foreach (var t in tweets)
            {
                output.Add(new Tweet
                {
                    Author = t["user"]["name"].ToString(),
                    UserName = t["user"]["screen_name"].ToString(),
                    Message = t["text"].ToString()
                });
            }

            return output;
        }
    }
}
