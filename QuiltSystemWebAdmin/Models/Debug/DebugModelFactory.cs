//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Debug
{
    public class DebugModelFactory : ApplicationModelFactory
    {
        public DebugModel CreateDebugModel(ApplicationOptions applicationOptions)
        {
            return new DebugModel()
            {
                ApplicationOptions = applicationOptions
            };
        }
    }
}