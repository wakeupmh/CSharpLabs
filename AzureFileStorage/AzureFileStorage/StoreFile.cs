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
            await CreateFile(log);

            return req.CreateResponse(HttpStatusCode.OK, string.Empty);
        }
        private async static Task CreateFile(ILogger log)
        {
            string connectionString;
            CloudStorageAccount storageAccount;
            connectionString = "DefaultEndpointsProtocol=https;AccountName=atgeologs;AccountKey=nOL6CTcURztrP/qu31pc/PZTZffyrb+TlFNtZqaCF7fePQzGE9r6EpjcdDY8QdSYLbrxaIdT/QWcKnoxGVaQ3w==;EndpointSuffix=core.windows.net";
            storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare share = fileClient.GetShareReference("atfilelogs");
            await share.CreateIfNotExistsAsync();
            try
            {
                CloudFileDirectory root = share.GetRootDirectoryReference();
                var file = root.GetFileReference("teste.txt");
                string oldContent;
                if (!await file.ExistsAsync())
                {
                    oldContent = "";
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.DownloadToStreamAsync(memoryStream);
                        oldContent = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
                oldContent += "Novo conteudo\n";
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(Encoding.UTF8.GetBytes(oldContent), 0, Encoding.UTF8.GetBytes(oldContent).Length);
                    memoryStream.Position = 0;
                    await file.UploadFromStreamAsync(memoryStream);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
