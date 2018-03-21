using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;
using SimpleEchoBot;

namespace Microsoft.Bot.Sample.SimpleEchoBot.Dialogs
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.Contains("products"))
            {
                await context.Forward(new ProductDialog(), ResumeAfterProductDialog, message, CancellationToken.None);
            }
            else
            {
                await context.PostAsync("Welcome, how can we help you?");
                await RootActions(context);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterProductDialog(IDialogContext context, IAwaitable<object> result)
        {
            var messageObject = await result;
            MessageBag<string> message = (MessageBag<string>)messageObject;

            switch (message.Type)
            {
                case MessageType.ProductOrder:
                    await context.PostAsync($"The user ordered the product \"{message.Content}\"");
                    break;
                case MessageType.ProductRemoval:
                    await context.PostAsync($"The user removed the product {message.Content}");
                    break;
            }

            await RootActions(context);
            context.Wait(MessageReceivedAsync);
        }

        private static async Task RootActions(IDialogContext context)
        {
            await context.PostAsync(
                "Looking for products? Managing your Basket? Or want to checkout?");
        }
    }
}