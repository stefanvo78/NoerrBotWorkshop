using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;
using SimpleEchoBot;
using SimpleEchoBot.Dialogs;
using ShopBot.Models;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Configuration;

namespace Microsoft.Bot.Sample.SimpleEchoBot.Dialogs
{
    //LuisAppId , LuisAPIKey, Domain
    [LuisModel("11bc201a-b47e-4672-b1cd-f80c36ad27be", "ba9d15d5afc942d899fbeb2fe897979b", domain: "westeurope.api.cognitive.microsoft.com")]
    [Serializable]
    public class EchoDialog : LuisDialog<object>
    {
        
        //public Task StartAsync(IDialogContext context)
        //{
        //    context.Wait(MessageReceivedAsync);
        //    return Task.CompletedTask;
        //}


        [LuisIntent("")]
        [LuisIntent("None")]
        [LuisIntent("Help")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Welcome, how can we help you?");
            await RootActions(context);

            context.Wait(MessageReceived);
        }



        private const string EntityProducts = "Products";
        private const string EntityBasket = "Basket";
        private const string EntityCheckout = "Checkout";
        [LuisIntent("SelectDialog")]
        public async Task SelectDialog(IDialogContext context, IAwaitable<IMessageActivity> messageActivity, LuisResult result)
        {
            var message = await messageActivity;
            EntityRecommendation entityRecommendation;
            if (result.TryFindEntity(EntityProducts, out entityRecommendation))
            {
                await context.Forward(new ProductDialog(), ResumeAfterProductDialog, message, CancellationToken.None);
            }
            else if (result.TryFindEntity(EntityBasket, out entityRecommendation))
            {
                await context.Forward(new ManageBasketDialog(), ResumeAfterManageBasketDialog, message,
                    CancellationToken.None);
            }
            else if (result.TryFindEntity(EntityCheckout, out entityRecommendation))
            {
                await context.Forward(new CheckoutDialog(), ResumeAfterCheckoutDialog, message, CancellationToken.None);
            }
        }

        private async Task ResumeAfterManageBasketDialog(IDialogContext context, IAwaitable<object> result)
        {
            await RootActions(context);
        }
        //private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        //{

        //    var message = await result;

        //    if (message.Text.Contains("products"))
        //    {
        //        await context.Forward(new ProductDialog(), ResumeAfterProductDialog, message, CancellationToken.None);
        //    }

        //    else if (message.Text.Contains("checkout"))
        //    {
        //        await context.Forward(new CheckoutDialog(), ResumeAfterCheckoutDialog, message, CancellationToken.None);
        //    }
        //    else
        //    {
        //        await context.PostAsync("Welcome, how can we help you?");
        //        await RootActions(context);
        //        context.Wait(MessageReceivedAsync);
        //    }
        //}

        private async Task ResumeAfterProductDialog(IDialogContext context, IAwaitable<object> result)
        {
            var messageObject = await result;
            MessageBag<Product> message = (MessageBag<Product>)messageObject;

            switch (message.Type)
            {
                case MessageType.ProductOrder:
                    await context.PostAsync($"The user ordered the product \"{message.Content}\"");
                    break;
            }

            await RootActions(context);
            context.Wait(MessageReceived);
        }

        private async Task ResumeAfterCheckoutDialog(IDialogContext context, IAwaitable<object> result)
        {
            var messageObject = await result;
            MessageBag<string> message = (MessageBag<string>)messageObject;

            switch (message.Type)
            {
                case MessageType.Checkout:
                    await context.PostAsync(message.Content);
                    break;
            }

            await RootActions(context);
            context.Wait(MessageReceived);
        }

        private static async Task RootActions(IDialogContext context)
        {
            await context.PostAsync(
                "Looking for products? Managing your Basket? Or want to checkout?");
        }
    }
}