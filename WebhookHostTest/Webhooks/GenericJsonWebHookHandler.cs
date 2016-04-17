using Microsoft.AspNet.WebHooks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebhookHostTest
{
    public class GenericJsonWebHookHandler : WebHookHandler
    {
        public GenericJsonWebHookHandler()
        {
            this.Receiver = "genericjson";
        }

        public override Task ExecuteAsync(string generator, WebHookHandlerContext context)
        {
            // Get JSON from WebHook
            try {
                JObject data = context.GetDataOrDefault<JObject>();
            }catch(Exception ex)
            {
                throw;
            }
            if (context.Id == "i")
            {
                // Received a WebHook from IFTTT!
            }
            else if (context.Id == "z")
            {
                // Received a WebHook from Zapier!
            }

            return Task.FromResult(true);
        }
    }
}