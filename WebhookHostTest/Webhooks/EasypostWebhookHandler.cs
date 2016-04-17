using Microsoft.AspNet.WebHooks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhookHostTest.Webhooks
{
    public class EasypostWebHookHandler : WebHookHandler
    {
        public EasypostWebHookHandler()
        {
            this.Receiver = EasyPostWebHookReceiver.ReceiverName;
        }

        public override Task ExecuteAsync(string generator, WebHookHandlerContext context)
        {
            // For more information about WebHook payloads, please see 
            //https://www.easypost.com/webhooks-guide
            //https://www.easypost.com/docs/api/curl#events
            JObject entry = context.GetDataOrDefault<JObject>();

            return Task.FromResult(true);
        }
    }
}
