//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Database.Abstractions.Data
{
    public class DatabaseOptions
    {
        public bool UseLocalDatabase { get; set; }
        public bool UseMostRecentDatabase { get; set; }
        public string LocalDatabaseName { get; set; }
        public string LocalConnectionString { get; set; }
        public bool DatabaseLoggingEnabled { get; set; }
    }
}
