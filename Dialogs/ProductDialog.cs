using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using SimpleEchoBot;

namespace Microsoft.Bot.Sample.SimpleEchoBot.Dialogs
{
    [Serializable]
    public class ProductDialog : IDialog<object>
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
                    await context.PostAsync("Name of the product you want to order:");
                    context.Wait(ProductSelectionReceivedAsync);
                    break;
                case "2":
                    await context.PostAsync("Name of the product you want to remove:");
                    context.Wait(PromptProductRemoval);
                    break;
            }
        }

        private async Task ProductSelectionReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            context.Done(MessageBag.Of(message.Text, MessageType.ProductOrder));
        }

        private async Task PromptProductRemoval(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            context.Done(new MessageBag<string>(message.Text, MessageType.ProductRemoval));
        }
    }
}