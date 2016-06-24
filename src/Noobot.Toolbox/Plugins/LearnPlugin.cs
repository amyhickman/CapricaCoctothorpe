using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlatFile.Delimited.Attributes;
using Noobot.Core;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins;

namespace Noobot.Toolbox.Plugins
{
    public class LearnPlugin : IPlugin
    {
        private readonly object _lock = new object();
        private bool _isRunning;
        private readonly INoobotCore _noobotCore;
        private readonly StoragePlugin _storagePlugin;
        private readonly Dictionary<string,string> _commands = new Dictionary<string, string>();
        private const string _learnFilename = "Learny";

        public LearnPlugin(INoobotCore noobotCore, StoragePlugin storagePlugin)
        {
            _noobotCore = noobotCore;
            _storagePlugin = storagePlugin;
        }

        public void Teach(string Command, string OutputPattern)
        {
            _commands.Add(Command, OutputPattern);

            lock (_lock)
            {
                var commands = _commands.Select(x => new LearnModel { Command = x.Key, OutputPattern = x.Value }).ToArray();
                _storagePlugin.SaveFile(_learnFilename, commands);
            }
        }

        public string GetOutput(string Command)
        {
            return _commands[Command];
        }

        public bool KnowsCommand(string Command)
        {
            return _commands.ContainsKey(Command);
        }

        public void Start()
        {
            _isRunning = true;

            lock (_lock)
            {
                var commands = _storagePlugin.ReadFile<LearnModel>(_learnFilename);
                foreach (LearnModel command in commands)
                {
                    _commands.Add(command.Command, command.OutputPattern);
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;

            lock (_lock)
            {
                var commands = _commands.Select(x => new LearnModel { Command = x.Key, OutputPattern = x.Value }).ToArray();
                _storagePlugin.SaveFile(_learnFilename, commands);
            }
        }

        [DelimitedFile(Delimiter = ";", Quotes = "\"")]
        private class LearnModel
        {
            [DelimitedField(1)]
            public string Command { get; set; }

            [DelimitedField(2)]
            public string OutputPattern { get; set; }
        }
    }
}