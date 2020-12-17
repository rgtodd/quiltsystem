//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Drawing;
using System.IO;
using System.Text;

using Azure.Storage.Blobs;

using Microsoft.Extensions.Configuration;

namespace RichTodd.QuiltSystem.Utility
{
    public static class AzureUtility
    {
        //public static void AppendAzureStringBlob(IConfiguration configuration, string containerName, string blobName, string value)
        //{
        //    var connectionString = configuration.GetConnectionString(ConnectionStringNames.Storage);
        //    var container = new BlobContainerClient(connectionString, containerName);
        //    _ = container.CreateIfNotExists();

        //    container.

        //    var blob = container.GetBlobClient(blobName);
        //    blob.


        //    var appendBlob = container.GetAppendBlobReference(blobName);
        //    if (!appendBlob.Exists())
        //    {
        //        appendBlob.CreateOrReplace();
        //    }

        //    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));

        //    _ = appendBlob.AppendBlock(stream);
        //}

        public static Image LoadAzureImageBlob(IConfiguration configuration, string containerName, string blobName)
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringNames.Storage);
            var container = new BlobContainerClient(connectionString, containerName);

            var blob = container.GetBlobClient(blobName);

            using var stream = blob.OpenRead();

            var image = Image.FromStream(stream);

            return image;
        }

        public static string LoadAzureStringBlob(IConfiguration configuration, string containerName, string blobName)
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringNames.Storage);
            var container = new BlobContainerClient(connectionString, containerName);

            var blob = container.GetBlobClient(blobName);

            using var stream = blob.OpenRead();

            using var reader = new StreamReader(stream);

            string text = reader.ReadToEnd();

            return text;
        }

        public static void SaveAzureStringBlob(IConfiguration configuration, string containerName, string blobName, string value)
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringNames.Storage);
            var container = new BlobContainerClient(connectionString, containerName);

            var blob = container.GetBlobClient(blobName);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));

            var response = blob.Upload(stream);
        }
    }
}