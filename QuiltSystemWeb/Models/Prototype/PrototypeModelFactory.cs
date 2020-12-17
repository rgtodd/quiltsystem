//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Web.Models.Prototype
{
    public static class PrototypeModelFactory
    {
        private static int s_id;

        public static PrototypeItemModel CreatePrototypeItemModel()
        {
            return CreatePrototypeItemModel(null);
        }

        public static PrototypeItemModel CreatePrototypeItemModel(string id)
        {
            if (!int.TryParse(id, out int intId))
            {
                intId = GetNextId();
            }

            return new PrototypeItemModel()
            {
                Id = intId,
                Name = "Item " + intId
            };
        }

        public static PrototypeItemListModel CreatePrototypeItemListModel()
        {
            var items = new List<PrototypeItemModel>();
            for (int idx = 0; idx < 10; ++idx)
            {
                items.Add(CreatePrototypeItemModel());
            }

            return new PrototypeItemListModel()
            {
                Items = items
            };
        }

        private static int GetNextId()
        {
            return ++s_id;
        }
    }
}