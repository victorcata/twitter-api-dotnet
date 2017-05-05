using System.Linq;
using System.Web.Mvc;
using [namespace].Models;

namespace [namespace].Controllers
{
    public class ComponentsController : Controller
    {
        public PartialViewResult Tweets(int count = 12, string hashtag = "<HASHTAG>")
        {
            var twitter = new Twitter();
            return this.PartialView(twitter.Get(count, hashtag));
        }
    }
} 