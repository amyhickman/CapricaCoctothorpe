using Noobot.Core.Configuration;
using Noobot.Toolbox.Pipeline.Middleware;
using Noobot.Toolbox.Plugins;

namespace Noobot.Toolbox
{
    public class Configuration : ConfigurationBase
    {
        public Configuration()
        {
            //UseMiddleware<AutoResponderMiddleware>();
            UseMiddleware<WelcomeMiddleware>();
            UseMiddleware<AdminMiddleware>();
            UseMiddleware<ScheduleMiddleware>();
            UseMiddleware<JokeMiddleware>();
            UseMiddleware<YieldTestMiddleware>();
            UseMiddleware<PingMiddleware>();
            UseMiddleware<FlickrMiddleware>();
            UseMiddleware<CalculatorMiddleware>();
            UseMiddleware<PaniniMiddleware>();
            UseMiddleware<WebQueryMiddleware>();
            UseMiddleware<LearnMiddleware>();
            UseMiddleware<StopMiddleware>();
            UsePlugin<StoragePlugin>();
            UsePlugin<SchedulePlugin>();
            UsePlugin<AdminPlugin>();
            UsePlugin<PingPlugin>();
            UsePlugin<LearnPlugin>();
        }
    }
}