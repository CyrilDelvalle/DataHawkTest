using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataHawkTest.Model;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DataHawkTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerReviewController : ControllerBase
    {
        private readonly ILogger<CustomerReviewController> _logger;
        private List<CustomerReview> _customerReviews = new List<CustomerReview>();
        private string _amazonASIN = "B082XY23D5";

        public CustomerReviewController(ILogger<CustomerReviewController> logger)
        {
            _logger = logger;
        }
        private HtmlDocument GetHtmlDocument()
        {
            string url = "https://www.amazon.com/product-reviews/" + _amazonASIN;
            HttpClient httpclient = new HttpClient();
            Task<string> html = httpclient.GetStringAsync(url);
            HtmlDocument htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(html.Result);
            return htmlDocument;
        }

        public void GetDataFromWebPage()
        {
            HtmlDocument AmazonWebPage = GetHtmlDocument();

            List<HtmlNode> ReviewList = AmazonWebPage.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("cm_cr-review_list")).ToList();

            List<HtmlNode> ProductListItems = ReviewList[0].Descendants("div")
                .Where(node => node.GetAttributeValue("id", "")
                .StartsWith("customer_review"))
                .ToList();

            foreach (HtmlNode ProductHtml in ProductListItems)
            {
                    string reviewAuthor = ProductHtml.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .StartsWith("a-profile-name"))
                        .FirstOrDefault()
                        .InnerText;

                    string reviewDate = ProductHtml.Descendants("span")
                        .Where(node => node.GetAttributeValue("data-hook", "")
                        .Contains("review-date"))
                        .FirstOrDefault()
                        .InnerText.Trim();

                    string reviewTitle = ProductHtml.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("a-letter-space"))
                        .FirstOrDefault()
                        .NextSibling.InnerText.Trim();

                    string reviewBody = ProductHtml.Descendants("span")
                        .Where(node => node.GetAttributeValue("data-hook", "")
                        .Contains("review-body"))
                        .FirstOrDefault()
                        .InnerText.Trim();

                    string reviewRate = ProductHtml.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .StartsWith("a-icon-alt"))
                        .FirstOrDefault().InnerText;

                    CustomerReview customerReview = SetCustomerReviewFromProductHtml(ProductHtml.Id, reviewAuthor, reviewDate, reviewTitle, reviewBody, reviewRate);

                    _customerReviews.Add(customerReview);
                }
        }

        private CustomerReview SetCustomerReviewFromProductHtml(string reviewId, string author, string date, string title, string body, string rate)
        {
            return new CustomerReview
            {
                AmazonASIN = _amazonASIN,
                Id = reviewId.Substring("customer_review-".Length),
                Author = author,
                Date = date,
                Rate = Convert.ToInt32(rate[0].ToString()),
                Title = title,
                Body = body
            };
        }

        [HttpGet]
        public IEnumerable<CustomerReview> Get()
        {
            GetDataFromWebPage();
            return this._customerReviews.ToArray();
        }
    }
}
