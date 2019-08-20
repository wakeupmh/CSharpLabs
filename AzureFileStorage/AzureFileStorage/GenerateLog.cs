using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AzureFileStorage
{
    class GenerateLog
    {
        private CloudStorageAccount StorageAccount { get; set; }
        private CloudFileShare Share { get; set; }
        private CloudFile File { get; set; }
        public GenerateLog() {}
        public async Task Initialize(string owner, string connectionString)
        {
            if (string.IsNullOrEmpty(owner))
            {
                owner = "UNDEFINED";
            }
            await FileShareClient($"{owner.ToLower()}logs", connectionString);
        }
        private async Task FileShareClient(string shareReferenceName, string connectionString)
        {
            StorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudFileClient fileClient = StorageAccount.CreateCloudFileClient();
            Share = fileClient.GetShareReference(shareReferenceName);
            await Share.CreateIfNotExistsAsync();
        }
        public async Task<string> GetLogContent()
        {
            try
            {
                CloudFileDirectory root = Share.GetRootDirectoryReference();
                File = root.GetFileReference($"log_{DateTime.Now.ToString("dd_MM_yyyy")}.log");
                string content;
                if (!await File.ExistsAsync())
                {
                    content = "";
                }
                else
                {

                    using (var memoryStream = new MemoryStream())
                    {
                        await File.DownloadToStreamAsync(memoryStream);
                        content = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }


                }
                return content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task WriteLog(string text)
        {
            try
            {
                var actualContent = await GetLogContent();
                actualContent += text;
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(Encoding.UTF8.GetBytes(actualContent), 0, Encoding.UTF8.GetBytes(actualContent).Length);
                    memoryStream.Position = 0;
                    await File.UploadFromStreamAsync(memoryStream);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
