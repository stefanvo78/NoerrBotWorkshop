using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Bot.Builder.FormFlow;

namespace ShopBot.Models
{
    public enum Sort
    {
        Ascending = 1,
        Descending
    }

    [Serializable]
    public class ProductQuery
    {
        public string ProductName { get; set; }

        [Prompt("How do you want to sort? {||}")]
        public Sort Sort { get; set; }

        [Numeric(1, 60)]
        public int Limit { get; set; } = 1;

        public string GetSort()
        {
            return Sort == Sort.Ascending ? "asc" : "desc";
        }

        public static IForm<ProductQuery> BuildForm()
        {
            return new FormBuilder<ProductQuery>()
                .Message("Provide the required parameters for your search:")
                .Build();
        }
    }
}