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
using AdaptiveCards;
using SimpleEchoBot.CustomCards;
using System.Threading;

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
            await DisplayActionsCard(context);
            context.Wait(ActionSelectionReceivedAsync);
        }

        private async Task DisplayActionsCard(IDialogContext context)
        {
            Attachment attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = CardFactory.GetProductActionsCard()
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply, CancellationToken.None);
        }




        private async Task ActionSelectionReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            dynamic value = message.Value;

            try
            {
                string submitType = value.Type.ToString();
                switch (submitType)
                {
                    case CardFactory.ProductSearch:
                        ProductQuery query = ProductQuery.Parse(value);
                        await ProductOptionsReceivedAsync(context, query);
                        break;
                    case CardFactory.ProductRemoval:
                        await context.PostAsync("Name of the product you want to remove:");
                        context.Wait(PromptProductRemoval);
                        break;
                    case CardFactory.ProductDialogCancellation:
                        context.Done(new MessageBag<Product>(null, MessageType.ProductDialogCancelled));
                        break;
                }
            }
            catch (InvalidCastException)
            {
                await context.PostAsync("Please complete all the search parameters");
                context.Wait(ActionSelectionReceivedAsync);
            }
        }




        private async Task ProductOptionsReceivedAsync(IDialogContext context, ProductQuery result)
        {
            var query = result;
            var products = AzureSearch.CreateClient()
                .WithIndex(AzureSearch.Products)
                .Sort(nameof(Product.ListPrice), query.Sort)
                .Limit(query.Limit)
                .Find<Product>(query.ProductName);

            if (products.Any())
            {
                PromptDialog.Choice(context, ProductSelectionReceivedAsync, products, "Add to basket:");
            }
            else
            {
                await context.PostAsync("No products found.");
                await DisplayActionsCard(context);
                context.Wait(ActionSelectionReceivedAsync);
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