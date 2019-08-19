using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Text;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureFileStorage
{
    public static class StoreFile
    {
        [FunctionName("StoreFile")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req, ILogger log)
        {
            HttpStatusCode result;
            string contentType;

            result = HttpStatusCode.BadRequest;

            contentType = req.Content.Headers?.ContentType?.MediaType;

            if (contentType == "application/json")
            {
                string body;

                body = await req.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(body))
                {
                    string name;

                    name = Guid.NewGuid().ToString("n");

                    await CreateFile(log);

                    result = HttpStatusCode.OK;
                }
            }

            return req.CreateResponse(result, string.Empty);
        }
        private async static Task CreateFile(ILogger log)
        {
            string connectionString;
            CloudStorageAccount storageAccount;
            connectionString = "DefaultEndpointsProtocol=https;AccountName=<ShareReference>AccountKey=<yourKey>;EndpointSuffix=core.windows.net";
            storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare share = fileClient.GetShareReference("yourStoreReference");
            await share.CreateIfNotExistsAsync();
            try
            {
                CloudFileDirectory root = share.GetRootDirectoryReference();
                await root.GetFileReference("MyFile.txt").UploadTextAsync("This is a test!");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
