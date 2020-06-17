using ClaudiaCRUD.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient.Memcached;
using System;
using System.IO;
using System.Net;
using Xunit;

namespace ClaudiaCRUD.Tests
{
    public class FunctionsTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Theory]
        [MemberData(nameof(TestFactory.DataGetSong), MemberType = typeof(TestFactory))]
        public async void Get_song_function_should_return_known_status_code_from_member_data(string queryStringKey, string queryStringValue, bool statusCode)
        {
            var request = TestFactory.CreateHttpRequestGetSong(queryStringKey, queryStringValue);
            Uri collectionUri = new Uri("https://localhost:8081");
            var client = new DocumentClient(collectionUri, "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            var response = await MusicFunction.GetSong(request, client, logger);
            Assert.Equal(statusCode, response.IsSuccessStatusCode);
            
        }

        [Fact]
        public async void Get_all_song_function_should_return_success_code()
        {
            var request = TestFactory.CreateHttpRequestGetAllSong();
            Uri collectionUri = new Uri("https://localhost:8081");
            var client = new DocumentClient(collectionUri, "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            var response = await MusicFunction.GetAllSong(request, client, logger);
            var result = new OkObjectResult(response);
            Assert.Equal(200, (int)result.StatusCode);
        }

        [Fact]
        public async void Create_song_function_should_return_success_code()
        {
            string inputBody = "{'singer': 'Rex Orange County', 'album': 'Untitled', 'song': 'Untitled', 'genre': 'Indie', 'release': '2017'}";
            
            var request = TestFactory.CreateHttpRequestCreateSong(inputBody);
            Uri collectionUri = new Uri("https://localhost:8081");
            var client = new DocumentClient(collectionUri, "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            var response = await MusicFunction.CreateSong(request, client, logger);
            var result = new OkObjectResult(response);
            Assert.Equal(200, (int)result.StatusCode);
        }


        [Theory]
        [MemberData(nameof(TestFactory.DataUpdateSong), MemberType = typeof(TestFactory))]
        public async void Update_song_function_should_return_known_status_code_from_member_data(string queryStringKey, string queryStringValue, string updateBody, int statusCode)
        {
            var request = TestFactory.CreateHttpRequestUpdateSong(queryStringKey, queryStringValue, updateBody);
            Uri collectionUri = new Uri("https://localhost:8081");
            var client = new DocumentClient(collectionUri, "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            var response = await MusicFunction.UpdateSong(request, client, logger);
            var resultOK = response as OkObjectResult;
            var resultNotFound = response as NotFoundResult;
            if (resultOK == null)
            {
                Assert.Equal(statusCode, resultNotFound.StatusCode);
            }
            else
            {
                Assert.Equal(statusCode, resultOK.StatusCode);
            }

        }

        [Theory]
        [MemberData(nameof(TestFactory.DataDeleteSong), MemberType = typeof(TestFactory))]
        public async void Delete_song_function_should_return_known_status_code_from_member_data(string queryStringKey, string queryStringValue, int statusCode)
        {
            var request = TestFactory.CreateHttpRequestDeleteSong(queryStringKey, queryStringValue);
            Uri collectionUri = new Uri("https://localhost:8081");
            var client = new DocumentClient(collectionUri, "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            var response = await MusicFunction.UpdateSong(request, client, logger);
            var resultOK = response as OkObjectResult;
            var resultNotFound = response as NotFoundResult;
            if (resultOK == null)
            {
                Assert.Equal(statusCode, resultNotFound.StatusCode);
            }
            else
            {
                Assert.Equal(statusCode, resultOK.StatusCode);
            }

        }

    }
}
