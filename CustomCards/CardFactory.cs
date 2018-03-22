using System;
using System.Collections.Generic;
using System.Linq;
using AdaptiveCards;
using ShopBot.Models;

namespace SimpleEchoBot.CustomCards
{
    public static class CardFactory
    {
        public const string ProductSearch = "ProductSearch";
        public const string ProductRemoval = "ProductRemoval";
        public const string ProductDialogCancellation = "CancelProductDialog";

        public static AdaptiveCard GetProductActionsCard()
        {
            return new AdaptiveCard
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveContainer
                    {
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveColumnSet
                            {
                                Columns = new List<AdaptiveColumn>
                                {
                                    new AdaptiveColumn
                                    {
                                        Width = AdaptiveColumnWidth.Auto,
                                        Items = new List<AdaptiveElement>
                                        {
                                            new AdaptiveImage
                                            {
                                                Url = new Uri( $"https://robohash.org/{new Random().NextDouble()}?size=150x150"),
                                                Size = AdaptiveImageSize.Medium,
                                                Style = AdaptiveImageStyle.Person
                                            }
                                        }
                                    },
                                    new AdaptiveColumn
                                    {
                                        Width = AdaptiveColumnWidth.Stretch,
                                        Items = new List<AdaptiveElement>
                                        {
                                            new AdaptiveTextBlock
                                            {
                                                Text = "Hey there!",
                                                Weight = AdaptiveTextWeight.Bolder
                                            },
                                            new AdaptiveTextBlock
                                            {
                                                Text = "How can we help you?",
                                                Wrap = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveShowCardAction
                    {
                        Title = "Order Products",
                        Card = GetProductsSearchCard()
                    },
                    new AdaptiveSubmitAction
                    {
                        Title = "Cancel",
                        DataJson = $"{{ \"Type\": \"{ProductDialogCancellation}\" }}"
                    }
                }
            };
        }

        public static AdaptiveCard GetProductsBasketCard(IList<Product> products)
        {
            var productCards = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock
                {
                    Text = $"You have **{products?.Count ?? 0}** products in your Basket."
                }
            };
            productCards.AddRange((products ?? new List<Product>()).Select(TransformToProductCard).ToList<AdaptiveElement>());

            return new AdaptiveCard
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveContainer
                    {
                        Items = productCards
                    }
                }
            };
        }

        private static AdaptiveColumnSet TransformToProductCard(Product product)
        {
            return new AdaptiveColumnSet
            {
                Columns = new List<AdaptiveColumn>
                {
                    new AdaptiveColumn
                    {
                        Width = AdaptiveColumnWidth.Auto,
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveImage
                            {
                                Url = new Uri ( $"https://robohash.org/bob{product.Name + new Random().Next()}?size=75x75"),
                                Size = AdaptiveImageSize.Small,
                                Style = AdaptiveImageStyle.Default
                            }
                        }
                    },
                    new AdaptiveColumn
                    {
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Text = product.Name
                            },
                            new AdaptiveTextBlock
                            {
                                Text = $"**${product.ListPrice}**",
                                Wrap = true
                            }
                        }
                    }
                }
            };
        }

        private static AdaptiveCard GetProductsSearchCard()
        {
            return new AdaptiveCard
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = "Welcome to the Products finder!",
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large
                    },
                    new AdaptiveTextBlock {Text = "Please enter the Product you are looking for."},
                    new AdaptiveTextInput
                    {
                        Id = "ProductName",
                        Placeholder = "Bike",
                        Style = AdaptiveTextInputStyle.Text
                    },
                    new AdaptiveTextBlock {Text = "How do you want to sort?"},
                    new  AdaptiveChoiceSetInput
                    {
                        Id = "Sort",
                        Style = AdaptiveChoiceInputStyle.Compact,
                        Choices = new List<AdaptiveChoice>
                        {
                            new AdaptiveChoice
                            {
                                //IsSelected = true,
                                Title = "Cheapest first",
                                Value = "asc"
                            },
                            new AdaptiveChoice
                            {
                                //IsSelected = true,
                                Title = "Most expensive first",
                                Value = "desc"
                            }
                        }
                    },
                    new AdaptiveTextBlock {Text = "Limit search to:"},
                    new AdaptiveNumberInput
                    {
                        Id = "Limit",
                        Min = 1,
                        Max = 60
                    }
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = "Search",
                        DataJson = $"{{ \"Type\": \"{ProductSearch}\" }}"
                    }
                }
            };
        }
    }
}