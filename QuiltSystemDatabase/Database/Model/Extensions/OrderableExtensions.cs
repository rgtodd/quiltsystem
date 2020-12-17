//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Linq;

namespace RichTodd.QuiltSystem.Database.Model.Extensions
{
    public static class OrderableExtensions
    {
        public static OrderItem OrderItem(this Orderable orderable)
        {
            return orderable.OrderItems.Single();
        }
    }
}
