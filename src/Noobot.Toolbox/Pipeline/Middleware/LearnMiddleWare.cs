using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Toolbox.Plugins;
using FlatFile.Delimited.Attributes;
using Noobot.Core.Plugins;
using System.Linq;

namespace Noobot.Toolbox.Pipeline.Middleware
{
    public class LearnMiddleware : MiddlewareBase
    {
        private readonly LearnPlugin _learnPlugin;

        public LearnMiddleware(IMiddleware next, LearnPlugin learnPlugin) : base(next)
        {
            _learnPlugin = learnPlugin;

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new []{ "learn" },
                    Description = "Teach Me Things!",
                    EvaluatorFunc = LearningHandler
                },

                new HandlerMapping
                {
                    ValidHandles = new []{""},
                    Description = "Try to calculate mathematical expressions without the 'calc' prefix - usage: ((1+2)*3)/4",
                    EvaluatorFunc = CommandHandler,
                    MessageShouldTargetBot = true,
                    ShouldContinueProcessing = true,
                    VisibleInHelp = false
                }
            };
        }

        private IEnumerable<ResponseMessage> LearningHandler(IncomingMessage message, string matchedHandle)
        {
            message.TargetedText = message.TargetedText.Replace(message.TargetedText.Split(' ')[0] + " ", "");
            var command = message.TargetedText.Split(' ')[0];
            var result = message.TargetedText.Replace(command, "");
            command = command.ToLower();
            
            if( GetActiveCommands().FirstOrDefault(x => x.ToLower().Equals(command)) != null || _learnPlugin.KnowsCommand(command))
            {
                yield return message.ReplyToChannel($"Command {command} already exists");
                yield break;
            }

            _learnPlugin.Teach(command, result);
            yield return message.ReplyToChannel($"TIL: {command}");
        }

        private IEnumerable<ResponseMessage> CommandHandler(IncomingMessage message, string matchedHandle)
        {
            if (_learnPlugin.KnowsCommand(message.TargetedText.ToLower()))
            {
                yield return message.ReplyToChannel(_learnPlugin.GetOutput(message.TargetedText.ToLower()));
            }
        }
    }
}