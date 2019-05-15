using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;


namespace personal_project.Controllers
{
    [Route("api/petfinder")]
    [ApiController]
    public class PetfinderController
    {
        [HttpGet]
        public string AccessToken()
        {
            try
            {

                var client = new RestClient("https://api.petfinder.com/v2/oauth2/token");
                var request = new RestRequest(Method.POST);
                request.AddParameter("client_id", "/.. API KEY ../");
                request.AddParameter("client_secret", "/.. SECRET KEY ../");
                request.AddParameter("grant_type", "client_credentials");

                IRestResponse accessToken = client.Execute(request);
                var data = accessToken.Content;
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("feed")]
        public string Petfinder(string token)
        {
            try
            {

                var client = new RestClient("https://api.petfinder.com/v2/animals?limit=10&status=adoptable");
                var request = new RestRequest(Method.GET);

                request.AddHeader("Authorization", "Bearer " + token);
                IRestResponse info = client.Execute(request);
                var data = info.Content;

                return data;
            }
           
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
