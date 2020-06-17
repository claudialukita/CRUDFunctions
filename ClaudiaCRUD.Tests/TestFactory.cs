using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ClaudiaCRUD.Tests
{
    public class TestFactory
    {

        public static IEnumerable<object[]> DataGetSong()
        {
            return new List<object[]>
            {
                new object[] { "id", "dad15a36-54cc-4c79-b8cf-ecdcc85e0dda", true },
                new object[] { "id", "eb050a69-f8b5-4ef9-8584-3c950602c0c1", false }
            };
        }

        public static IEnumerable<object[]> DataUpdateSong()
        {
            return new List<object[]>
            {
                new object[] { "id", "dad15a36-54cc-4c79-b8cf-ecdcc85e0dda", "{'singer': 'Rex Orange County', 'album': 'UntitledEdit', 'song': 'Untitled', 'genre': 'Indie', 'release': '2017'}", 200 },
                new object[] { "id", "eb050a69-f8b5-4ef9-8584-3c950602c0c1", "{'singer': 'Rex Orange County', 'album': 'UntitledEdit', 'song': 'Untitled', 'genre': 'Indie', 'release': '2017'}", 404 }
            };
        }

        public static IEnumerable<object[]> DataDeleteSong()
        {
            return new List<object[]>
            {
                new object[] { "id", "dad15a36-54cc-4c79-b8cf-ecdcc85e0dda", 200 },
                new object[] { "id", "eb050a69-f8b5-4ef9-8584-3c950602c0c1", 404 }
            };
        }

        public static Dictionary<string, StringValues> CreateDictionary(string key, string value)
        {
            var qs = new Dictionary<string, StringValues>
            {
                { key, value }
            };
            return qs;
        }

        public static DefaultHttpRequest CreateHttpRequest(string queryStringKey, string queryStringValue)
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(CreateDictionary(queryStringKey, queryStringValue))
            };
            return request;
        }
        
        public static DefaultHttpRequest CreateHttpRequestGetSong(string queryStringKey, string queryStringValue)
        {

            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(CreateDictionary(queryStringKey, queryStringValue))
            };
            return request;
        }

        public static DefaultHttpRequest CreateHttpRequestDeleteSong(string queryStringKey, string queryStringValue)
        {

            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(CreateDictionary(queryStringKey, queryStringValue))
            };
            return request;
        }

        public static DefaultHttpRequest CreateHttpRequestCreateSong(string inputBody)
        {

            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes(inputBody))
            };
            return request;
        }

        public static DefaultHttpRequest CreateHttpRequestUpdateSong(string queryStringKey, string queryStringValue, string inputBody)
        {

            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(CreateDictionary(queryStringKey, queryStringValue)),
                Body = new MemoryStream(Encoding.UTF8.GetBytes(inputBody))
            };
            return request;
        }

        public static DefaultHttpRequest CreateHttpRequestGetAllSong()
        {

            var request = new DefaultHttpRequest(new DefaultHttpContext());
            return request;
        }

        public static DocumentClient CreateClient()
        {
            return null;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
