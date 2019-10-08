using Lab03_TMDT.common;
using Lab03_TMDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Lab03_TMDT.Controllers
{
    public class BlogController : Controller
    {

        TINTUCEntities db = new TINTUCEntities();

        public ActionResult PostFeed(string type)
        {
            CATEGORY category = db.CATEGORies.Where(s => s.alias.Contains(type)).FirstOrDefault();
            if (category == null)
            {
                return HttpNotFound();
            }
            IEnumerable<ARTICLE> posts = (from s in db.ARTICLEs where s.CATEGORY.alias.Contains(type) select s).ToList();
            var feed = new SyndicationFeed(category.name, "RSS Feed",
                     new Uri("http://localhost:1699/RSS"),
                     Guid.NewGuid().ToString(),
                     DateTime.Now);
            var items = new List<SyndicationItem>();
            foreach (ARTICLE art in posts)
            {
                string postUrl = String.Format("http://localhost:1699/" + art.alias + "-{0}", art.id);
                var item =
                    new SyndicationItem(Helper.RemoveIllegalCharacters(art.title),
                    Helper.RemoveIllegalCharacters(art.description),
                    new Uri(postUrl),
                    art.id.ToString(),
                    art.datepublished.Value);
                items.Add(item);
            }
            feed.Items = items;
            return new RSSActionResult { Feed = feed };
        }


        // GET: Blog
        public ActionResult ReadRSS()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult ReadRSS(string url)
        {
            WebClient client = new WebClient();
            client.Encoding = ASCIIEncoding.UTF8;

            string RSSData = client.DownloadString(url);

            XDocument xml = XDocument.Parse(RSSData, LoadOptions.PreserveWhitespace);
            var RSSFeedData = (from x in xml.Descendants("item")
                               select new RSSFeed {
                                   Title = ((string)x.Element("title")),
                                   Link = ((string)x.Element("link")),
                                   Description = ((string)x.Element("description")),
                                   PubDate = ((string)x.Element("pubDate"))
                               });
            ViewBag.RSSFeed = RSSFeedData;
            ViewBag.URL = url;

            return View();
        }
    }
}