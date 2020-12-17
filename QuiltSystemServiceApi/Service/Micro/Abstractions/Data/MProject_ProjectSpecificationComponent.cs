//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MProject_ProjectSpecificationComponent
    {
        public MProject_ProjectSpecificationComponent(string sku, string unitOfMeasureCode, int quantity)
        {
            Sku = sku ?? throw new ArgumentNullException(nameof(sku));
            UnitOfMeasureCode = unitOfMeasureCode;
            Quantity = quantity;
        }

        public string Sku { get; }
        public string UnitOfMeasureCode { get; }
        public int Quantity { get; }
    }
}