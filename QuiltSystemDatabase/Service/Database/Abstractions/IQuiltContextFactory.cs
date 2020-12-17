//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Data.SqlClient;

using RichTodd.QuiltSystem.Database.Model;

namespace RichTodd.QuiltSystem.Service.Database.Abstractions
{
    public interface IQuiltContextFactory
    {
        string ConnectionString { get; }

        QuiltContext Create();

        SqlConnection CreateConnection();
    }
}
