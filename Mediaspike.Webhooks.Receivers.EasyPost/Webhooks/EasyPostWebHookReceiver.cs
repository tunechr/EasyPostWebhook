﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.AspNet.WebHooks.Properties;
using Newtonsoft.Json.Linq;
using Mediaspike.Webhooks.Receivers.EasyPost;

namespace Microsoft.AspNet.WebHooks
{
    /// <summary>
    /// Provides an <see cref="IWebHookReceiver"/> implementation which supports WebHooks generated by EasyPost. 
    /// Set the '<c>MS_WebHookReceiverSecret_EasyPost</c>' application setting to the application secrets, optionally using IDs
    /// to differentiate between multiple WebHooks, for example '<c>secret0, id1=secret1, id2=secret2</c>'.
    /// The corresponding WebHook URI is of the form '<c>https://&lt;host&gt;/api/webhooks/incoming/easypost/{id}</c>'.
    /// For details about EasyPost WebHooks, see <c>https://www.easypost.com/account#/webhooks/</c>.
    /// </summary>
    public class EasyPostWebHookReceiver : WebHookReceiver
    {
        internal const string RecName = "easypost";
        internal const int SecretMinLength = 16;
        internal const int SecretMaxLength = 128;

        internal const string SignatureHeaderKey = "sha1";
        internal const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";
        internal const string SignatureHeaderName = "X-Hub-Signature";
        internal const string EventHeaderName = "X-easypost-Event";
        internal const string PingEvent = "ping";

        /// <summary>
        /// Gets the receiver name for this receiver.
        /// </summary>
        public static string ReceiverName
        {
            get { return RecName; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return RecName; }
        }

        /// <inheritdoc />
        public override async Task<HttpResponseMessage> ReceiveAsync(string id, HttpRequestContext context, HttpRequestMessage request)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Method == HttpMethod.Post)
            {
                //todo implement signiture verification
                //await VerifySignature(id, request);

                // Read the request entity body.
                JObject data = await ReadAsJsonAsync(request);

                // Pick out action from headers
                IEnumerable<string> actions;
                if (!request.Headers.TryGetValues(EventHeaderName, out actions))
                {
                    string msg = string.Format(CultureInfo.CurrentCulture, EasyPostReceiverResources.Receiver_NoEvent, EventHeaderName);
                    context.Configuration.DependencyResolver.GetLogger().Error(msg);
                    HttpResponseMessage noHeader = request.CreateErrorResponse(HttpStatusCode.BadRequest, msg);
                    return noHeader;
                }

                // If this is a ping request then just return. Otherwise call handlers.
                if (string.Equals(actions.FirstOrDefault(), PingEvent, StringComparison.OrdinalIgnoreCase))
                {
                    context.Configuration.DependencyResolver.GetLogger().Info(EasyPostReceiverResources.Receiver_PingEvent);
                    return request.CreateResponse();
                }
                return await ExecuteWebHookAsync(id, context, request, actions, data);
            }
            else
            {
                return CreateBadMethodResponse(request);
            }
        }

        /// <summary>
        /// Verifies that the signature header matches that of the actual body.
        /// </summary>
        protected virtual async Task VerifySignature(string id, HttpRequestMessage request)
        {
            string secretKey = await GetReceiverConfig(request, Name, id, SecretMinLength, SecretMaxLength);

            // Get the expected hash from the signature header
            string header = GetRequestHeader(request, SignatureHeaderName);
            string[] values = header.SplitAndTrim('=');
            if (values.Length != 2 || !string.Equals(values[0], SignatureHeaderKey, StringComparison.OrdinalIgnoreCase))
            {
                string msg = string.Format(CultureInfo.CurrentCulture, EasyPostReceiverResources.Receiver_BadHeaderValue, SignatureHeaderName, SignatureHeaderKey, "<value>");
                request.GetConfiguration().DependencyResolver.GetLogger().Error(msg);
                HttpResponseMessage invalidHeader = request.CreateErrorResponse(HttpStatusCode.BadRequest, msg);
                throw new HttpResponseException(invalidHeader);
            }

            byte[] expectedHash;
            try
            {
                expectedHash = EncodingUtilities.FromHex(values[1]);
            }
            catch (Exception ex)
            {
                string msg = string.Format(CultureInfo.CurrentCulture, EasyPostReceiverResources.Receiver_BadHeaderEncoding, SignatureHeaderName);
                request.GetConfiguration().DependencyResolver.GetLogger().Error(msg, ex);
                HttpResponseMessage invalidEncoding = request.CreateErrorResponse(HttpStatusCode.BadRequest, msg);
                throw new HttpResponseException(invalidEncoding);
            }

            // Get the actual hash of the request body
            byte[] actualHash;
            byte[] secret = Encoding.UTF8.GetBytes(secretKey);
            using (var hasher = new HMACSHA1(secret))
            {
                byte[] data = await request.Content.ReadAsByteArrayAsync();
                actualHash = hasher.ComputeHash(data);
            }

            // Now verify that the provided hash matches the expected hash.
            if (!WebHookReceiver.SecretEqual(expectedHash, actualHash))
            {
                HttpResponseMessage badSignature = CreateBadSignatureResponse(request, SignatureHeaderName);
                throw new HttpResponseException(badSignature);
            }
        }
    }
}