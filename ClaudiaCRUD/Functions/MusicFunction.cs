using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using ClaudiaCRUD.Models;
using System.Net;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents;

namespace ClaudiaCRUD
{
    public static class MusicFunction
    {
        [FunctionName("GetSong")]
        public static async Task<HttpResponseMessage> GetSong(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "music/song")]HttpRequest req,
            [CosmosDB(
                databaseName: "musicDatabase",
                collectionName: "albumContainer",
                ConnectionStringSetting = "databaseConnection")] DocumentClient client,
            ILogger log)
        {
            var ids = req.Query["id"];
            if (string.IsNullOrWhiteSpace(ids))
            {
                return HttpResponseMessage(HttpStatusCode.NotFound);
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("albumDatabase", "albumContainer");

            var options = new FeedOptions { EnableCrossPartitionQuery = true };

            IDocumentQuery<MusicModel> query = client.CreateDocumentQuery<MusicModel>(collectionUri, options)
                .Where(p => p.Id.Contains(ids))
                .AsDocumentQuery();

            MusicModel musicModel = new MusicModel();

            while (query.HasMoreResults)
            {
                foreach (MusicModel result in await query.ExecuteNextAsync())
                {
                    musicModel.Id         = result.Id;
                    musicModel.Singer     = result.Singer;
                    musicModel.Album      = result.Album;
                    musicModel.Song       = result.Song;
                    musicModel.Genre      = result.Genre;
                    musicModel.Release    = result.Release;
                    musicModel.CreateDate = result.CreateDate;
                }
            }

            try
            {
                var json = JsonConvert.SerializeObject(musicModel, Formatting.Indented);
                return new HttpResponseMessage(HttpStatusCode.OK)
                { 
                   Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

            }
            catch (Exception e)
            {
                log.LogError($"Error Result:{e.Message}");
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            }

            

        }

        [FunctionName("GetAllSong")]
        public static async Task<IActionResult> GetAllSong(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "music/allsong")] HttpRequest req,
            [CosmosDB(
                databaseName: "musicDatabase",
                collectionName: "albumContainer",
                ConnectionStringSetting = "databaseConnection")] DocumentClient client,
            ILogger log)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("albumDatabase", "albumContainer");

            var options = new FeedOptions { EnableCrossPartitionQuery = true };

            IDocumentQuery<MusicModel> query = client.CreateDocumentQuery<MusicModel>(collectionUri, options)
                .Where(p => p.Id.Contains(""))
                .AsDocumentQuery();

            FeedResponse<MusicModel> result = await query.ExecuteNextAsync<MusicModel>();

            return new OkObjectResult(result);
        }

        [FunctionName("CreateSong")]
        public static async Task<IActionResult> CreateSong(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "music/song")] HttpRequest req,
            [CosmosDB(
                databaseName: "musicDatabase",
                collectionName: "albumContainer",
                ConnectionStringSetting = "databaseConnection")] DocumentClient client,
            ILogger log)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("albumDatabase", "albumContainer");
            
            MusicModel musicModel = new MusicModel();
            
            string requestBody      = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data            = JsonConvert.DeserializeObject(requestBody);
            musicModel.Id           = Guid.NewGuid().ToString();
            musicModel.Singer       = data?.singer;
            musicModel.Album        = data?.album;
            musicModel.Song         = data?.song;
            musicModel.Genre        = data?.genre;
            musicModel.Release      = data?.release;
            musicModel.CreateDate   = DateTime.Now;

            await client.CreateDocumentAsync(collectionUri, musicModel);
            return new OkObjectResult(musicModel);

        }

        [FunctionName("UpdateSong")]
        public static async Task<IActionResult> UpdateSong(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "music/song")] HttpRequest req,
            [CosmosDB(
                databaseName: "musicDatabase",
                collectionName: "albumContainer",
                ConnectionStringSetting = "databaseConnection")] DocumentClient client,
            ILogger log)
        {

            var id = req.Query["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return new NotFoundResult();
            }
           
            Uri collectionUri = UriFactory.CreateDocumentUri("albumDatabase", "albumContainer", id);

            MusicModel musicModel = new MusicModel();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            musicModel.Id = id;
            musicModel.Singer = data?.singer;
            musicModel.Album = data?.album;
            musicModel.Song = data?.song;
            musicModel.Genre = data?.genre;
            musicModel.Release = data?.release;
            musicModel.CreateDate = DateTime.Now;

            try
            {
                var updateResponse = await client.ReplaceDocumentAsync(collectionUri, musicModel);
                var updateDocument = updateResponse.Resource;
                log.LogInformation($"Update song with id: {updateDocument.Id}");

                return new OkObjectResult(musicModel);
            }
            catch (Exception e)
            {
                log.LogError($"Error:{e.Message}");
                return new NotFoundResult();
            }

        }

        [FunctionName("DeleteSong")]
        public static async Task<IActionResult> DeleteSong(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "music/song")] HttpRequest req,
            [CosmosDB(
                databaseName: "musicDatabase",
                collectionName: "albumContainer",
                PartitionKey = "/rexOrange",
                ConnectionStringSetting = "databaseConnection")] DocumentClient client,
            ILogger log)
        {

            var id = req.Query["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return new NotFoundResult();
            }

            Uri collectionUri = UriFactory.CreateDocumentUri("albumDatabase", "albumContainer", id);
            try
            {
                var deleteResponse = await client.DeleteDocumentAsync(collectionUri, new RequestOptions() { PartitionKey = new PartitionKey(Undefined.Value) });
                var deleteDocument = deleteResponse.Resource;
                return new OkResult();

            }
            catch (Exception e)
            {
                log.LogError($"Error:{e.Message}");
                return new NotFoundResult();
            }


        }

        private static HttpResponseMessage HttpResponseMessage(object notFound)
        {
            throw new NotImplementedException();
        }
    }
}
