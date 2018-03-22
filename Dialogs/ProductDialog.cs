using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ShopBot.Models;
using ShopBot.Services;
using SimpleEchoBot;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;

namespace Microsoft.Bot.Sample.SimpleEchoBot.Dialogs
{
    
    [Serializable]
    public class ProductDialog : IDialog<MessageBag<Product>>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(DisplayActionsToUser);
        }

        private async Task DisplayActionsToUser(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync("You have these options: \n\n 1. Order products \n\n 2. Remove products from Basket");
            context.Wait(ActionSelectionReceivedAsync);
        }

        

        private async Task ActionSelectionReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            switch (message.Text)
            {
                case "1":
                    await ProductQueryFormFlow(context, message);
                    break;
                case "2":
                    await context.PostAsync("Name of the product you want to remove:");
                    context.Wait(PromptProductRemoval);
                    break;
            }
        }


        private Task ProductQueryFormFlow(IDialogContext context, IMessageActivity message)
        {
            return context.Forward(FormDialog.FromForm(ProductQuery.BuildForm), ProductOptionsReceivedAsync,
                message);
        }

        private async Task ProductOptionsReceivedAsync(IDialogContext context, IAwaitable<ProductQuery> result)
        {
            var query = await result;
            var products = AzureSearch.CreateClient()
                .WithIndex(AzureSearch.Products)
                .Sort(nameof(Product.ListPrice), query.GetSort())
                .Limit(query.Limit)
                .Find<Product>(query.ProductName);

            if (products.Any())
            {
                PromptDialog.Choice(context, ProductSelectionReceivedAsync, products, "Add to basket:");
            }
            else
            {
                await context.PostAsync("No products found please try with another query.");
                await ProductQueryFormFlow(context, context.MakeMessage());
            }
        }

        private async Task ProductSelectionReceivedAsync(IDialogContext context, IAwaitable<Product> result)
        {
            var product = await result;
            context.Done(MessageBag.Of(product, MessageType.ProductOrder));
        }

        private async Task PromptProductRemoval(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            context.Done(new MessageBag<string>(message.Text, MessageType.ProductRemoval));
        }
    }
}