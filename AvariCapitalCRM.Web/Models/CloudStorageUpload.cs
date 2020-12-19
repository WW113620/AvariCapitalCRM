using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AvariCapitalCRM.Web.Models
{
    public class CloudStorageUpload
    {
        public static string storageConnectionString = @"DefaultEndpointsProtocol=https;AccountName=avariblob;AccountKey=0Cp8yXr4xEZBTtuzDH0yA/slcARIVMhv/Ih/fBjHJEwARuOpT2MRhNFdXFYBJ+kTG/Npc8keeFGTKspwTBPNPA==;EndpointSuffix=core.windows.net";
        public static string UploadFileAsBlob(Stream stream, string filename)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("avaricapitalpartners");
                //container.CreateIfNotExistsAsync().Wait();

                CloudBlockBlob blob = container.GetBlockBlobReference(filename);
                blob.UploadFromStream(stream);
                stream.Dispose();
                return blob?.Uri.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static string DeleteFileAsBlob(string filename)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("avaricapitalcontainer");
                //container.CreateIfNotExistsAsync().Wait();

                CloudBlockBlob blob = container.GetBlockBlobReference(filename);
                blob.Delete();
                return blob?.Uri.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }


    }
}