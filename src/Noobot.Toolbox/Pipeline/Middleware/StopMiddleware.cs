using System;
using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Toolbox.Plugins;
using FlatFile.Delimited.Attributes;
using Noobot.Core.Plugins;
using System.Linq;
using Noobot.Core.Plugins.StandardPlugins;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class StopMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;
        public StopMiddleware(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            _statsPlugin = statsPlugin;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{""},
                    Description = "Stop",
                    EvaluatorFunc = CommandHandler,
                    MessageShouldTargetBot = true,
                    ShouldContinueProcessing = true,
                    VisibleInHelp = true
                }
            };
        }

        private IEnumerable<ResponseMessage> CommandHandler(IncomingMessage message, string matchedHandle)
        {
            if (message.RawText.ToLower().Trim('.','?','!',' ').EndsWith("stop"))
            {
                yield return message.ReplyToChannel("HAMMERTIME!");
            }
            else if (message.RawText.ToLower().Trim('.','?','!',' ').EndsWith("stahp"))
            {
                yield return message.ReplyToChannel("HAMMAHTIME!");
            }
            else if (message.RawText.ToLower().Trim('.','?','!',' ').EndsWith("halt"))
            {
                yield return message.ReplyToChannel("HAMMERZEIT!");
            }
            else if (message.RawText.ToLower().Trim('.','?','!',' ').EndsWith("sterp"))
            {
                yield return message.ReplyToChannel("HERMMERDERP!");
            }
        }
    }
}