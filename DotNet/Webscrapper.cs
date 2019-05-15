using AngleSharp.Html.Parser;
using personal_project.Domain.Model;
using personal_project.Services.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;

namespace personal_project.Services
{
    public class WebScrapeService:IWebScrapeService
    {
        public IPetService _petService;

        public WebScrapeService(IPetService petService)
        {
            _petService = petService;
        }



       public void WebScraping()
        {

            List<Pets> result = new List<Pets>();

            var webClient = new WebClient();
            var html = webClient.DownloadString("https://www.adoptapet.com/index.html");

            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            var data = document.QuerySelector(".results_wrapper");

            var eachData = data.QuerySelectorAll(".pet_results");

            foreach (var onedata in eachData)
            {
                Pets pet = new Pets();
                pet.ImageUrl = onedata.QuerySelector("img").GetAttribute("src");
                pet.Title = onedata.QuerySelector("p:nth-child(2)").TextContent.Trim();
                pet.Location = onedata.QuerySelector("p:nth-child(3)").TextContent.Trim();
                pet.Description = onedata.QuerySelector("a:nth-child(4)").TextContent;


                //  calling insert service to insert to db
                int addPet = _petService.Create(pet);

            }

            
        }
    }
}
