using FS.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FS.Infrastructure.Services
{
    public class HttpService : IHttpService
    {
        public T Get<T>(string url, params object[] vals)
        {
            if (vals != null)
            {
                url = string.Format(url, vals);
            }
            var request = GetWebRequest(url);
            var response = GetWebResponse(request);
            var content = GetResponseData(response);
            var returnType = typeof(T);
            return returnType.IsPrimitive || returnType == typeof(decimal) || returnType == typeof(string)
                ? (T)Convert.ChangeType(content, typeof(T))
                : JsonConvert.DeserializeObject<T>(content);
        }
        private static HttpWebRequest GetWebRequest(string path, object data = null, string httpMethod = "GET")
        {
            var uri = new Uri(path);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "application/json";
            request.KeepAlive = false;
            request.Timeout = 120000;   // 2 minutes
            request.Method = (data != null && httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) ?
                "POST" : httpMethod;
            if (data == null) return request;
            // Create POST data and bind it as bytes to request body
            var postData = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
            return request;
        }

        private static HttpWebResponse GetWebResponse(HttpWebRequest request)
        {
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            return response;
        }

        private static string GetResponseData(HttpWebResponse response)
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = true } }
                };

                var stream = response.GetResponseStream();
                if (stream == null) return "";
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch
            {
                return "";
            }
        }
    }
}
