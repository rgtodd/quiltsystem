//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data;

namespace RichTodd.QuiltSystem.Test
{
    public static class Factory
    {
        public static XDesign_Design CreateDesign()
        {
            return new XDesign_Design()
            {
                designName = "New",
                width = "48\"",
                height = "48\"",
                layout = new XDesign_DesignLayout()
                {
                    layoutName = "Quilt Layout",
                    fabricStyles = new XDesign_FabricStyle[]
                    {
                        new XDesign_FabricStyle()
                        {
                            sku = "UNKNOWN",
                            color = new XDesign_Color()
                            {
                                webColor = "#FF0000"
                            }
                        },
                        new XDesign_FabricStyle()
                        {
                            sku = "UNKNOWN",
                            color = new XDesign_Color()
                            {
                                webColor = "#00FF00"
                            }
                        },
                        new XDesign_FabricStyle()
                        {
                            sku = "UNKNOWN",
                            color = new XDesign_Color()
                            {
                                webColor = "#0000FF"
                            }
                        }
                    },
                    rowCount = 5,
                    columnCount = 5,
                    blockCount = 2
                },
                blocks = new XDesign_DesignBlock[]
                {
                    new XDesign_DesignBlock()
                    {
                        blockName = "Half Square Triangle - Solid - Pinwheel/Block 0021",
                        fabricStyles = new XDesign_FabricStyle[]
                        {
                            new XDesign_FabricStyle()
                            {
                                sku = "K001-1161",
                                color = new XDesign_Color()
                                {
                                    webColor = "#047B5D"
                                }
                            },
                            new XDesign_FabricStyle()
                            {
                                sku = "K001-1010",
                                color = new XDesign_Color()
                                {
                                    webColor = "#BED6E4"
                                }
                            },
                            new XDesign_FabricStyle()
                            {
                                sku = "K001-0026",
                                color = new XDesign_Color()
                                {
                                    webColor = "#FACB0F"
                                }
                            }
                        }
                    },
                    new XDesign_DesignBlock()
                    {
                        blockName = "Solid - Split Rectangle - Pinwheel/Block 0048",
                        fabricStyles = new XDesign_FabricStyle[]
                        {
                            new XDesign_FabricStyle()
                            {
                                sku = "K001-1161",
                                color = new XDesign_Color()
                                {
                                    webColor = "#047B5D"
                                }
                            },
                            new XDesign_FabricStyle()
                            {
                                sku = "K001-1010",
                                color = new XDesign_Color()
                                {
                                    webColor = "#BED6E4"
                                }
                            },
                            new XDesign_FabricStyle()
                            {
                                sku = "K001-0026",
                                color = new XDesign_Color()
                                {
                                    webColor = "#FACB0F"
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
