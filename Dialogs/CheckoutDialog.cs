using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class CheckoutDialog: IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync("You have these options: \n\n 1. Checkout \n\n 2. Cancel");
            context.Wait(ActionSelectionReceivedAsync);
        }

        private async Task ActionSelectionReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            switch (message.Text)
            {
                case "1":
                    context.Done(MessageBag.Of("User checked out!", MessageType.Checkout));
                    
                    break;
                case "2":
                    context.Done(MessageBag.Of("User canceled!", MessageType.Checkout));
                    break;
            }
        }
    }
}