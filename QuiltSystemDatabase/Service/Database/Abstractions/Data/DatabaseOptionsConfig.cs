//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Options;

namespace RichTodd.QuiltSystem.Service.Database.Abstractions.Data
{
    public class DatabaseOptionsConfig : IConfigureOptions<DatabaseOptions>
    {
        public void Configure(DatabaseOptions options)
        {
            if (options.UseLocalDatabase)
            {
                if (options.UseMostRecentDatabase)
                {
                    options.LocalDatabaseName = FindMostRecentDatabaseName();
                }

                var fileName = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\SQL\\{options.LocalDatabaseName}.mdf";

                options.LocalConnectionString = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={options.LocalDatabaseName};AttachDbFilename={fileName};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            }
        }

        private string FindMostRecentDatabaseName()
        {
            var files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SQL", @"Quilt-*.mdf");
            if (!files.Any())
            {
                throw new Exception("Database not found.");
            }

            Array.Sort(files);

            var file = files.Last();

            return Path.GetFileNameWithoutExtension(file);
        }
    }
}
