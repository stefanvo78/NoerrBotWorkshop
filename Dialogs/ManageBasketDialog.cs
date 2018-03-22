using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using SimpleEchoBot.CustomCards;
using SimpleEchoBot.Repository;
using ShopBot.Models;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class ManageBasketDialog : IDialog<object>
    {
        private const string BasketContents = "Show products in basket";
        private const string ProductRemoval = "Remove single products";
        private const string EmptyBasket = "Clear basket";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptForBasketOptionSelection(context);
        }

        private void PromptForBasketOptionSelection(IDialogContext context)
        {
            var options = new List<string>
            {
                BasketContents,
                ProductRemoval,
                EmptyBasket
            };
            PromptDialog.Choice(context, AfterOptionSelection, options, "Manage basket:");
        }

        private async Task AfterOptionSelection(IDialogContext context, IAwaitable<string> result)
        {
            var optionSelected = await result;
            await ExecuteAction(context, optionSelected);
        }

        private async Task ExecuteAction(IDialogContext context, string optionSelected)
        {
            switch (optionSelected)
            {
                case BasketContents:
                    await ShowBasketContentsCard(context);
                    break;
                case ProductRemoval:
                    //TODO
                    break;
                case EmptyBasket:
                    //TODO
                    break;
            }
        }

        private async Task ShowBasketContentsCard(IDialogContext context)
        {
            IList<Product> products;
            context.ConversationData.TryGetValue(BotStateRepository.ProductsInBasket, out products);

            Attachment attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = CardFactory.GetProductsBasketCard(products)
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply, CancellationToken.None);
            context.Done("Basket contents viewed");
        }
    }
}