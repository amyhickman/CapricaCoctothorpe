using System;
using System.Collections.Generic;
using System.Threading;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class PaniniMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;

        public PaniniMiddleware(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            _statsPlugin = statsPlugin;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{"panini"},
                    Description = "Panini is love, Panini is life",
                    EvaluatorFunc = WelcomeHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> WelcomeHandler(IncomingMessage message, string matchedHandle)
        {
            _statsPlugin.IncrementState("Panini");

            yield return message.ReplyToChannel($"To all stupid put all \"Panini\" in your Asshole");
        }
    }
}