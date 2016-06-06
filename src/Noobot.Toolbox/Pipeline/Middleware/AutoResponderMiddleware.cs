﻿using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class AutoResponderMiddleware : MiddlewareBase
    {
        public AutoResponderMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new []
            {
                new HandlerMapping
                {
                    ValidHandles = new [] { ""},
                    Description = "Annoys the heck out of everyone",
                    EvaluatorFunc = AutoResponseHandler,
                    MessageShouldTargetBot = false,
                    ShouldContinueProcessing = true
                }
            };
        }

        private IEnumerable<ResponseMessage> AutoResponseHandler(IncomingMessage message, string matchedHandle)
        {
            yield return message.ReplyDirectlyToUser(message.FullText);
        }
    }
}