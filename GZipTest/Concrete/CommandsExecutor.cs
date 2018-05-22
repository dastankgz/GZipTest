using System.Collections.Generic;
using GZipTest.Abstract;

namespace GZipTest.Concrete
{
    public class CommandsExecutor : ICommand
    {
        private readonly List<ICommand> _commands;

        public CommandsExecutor(List<ICommand> commands)
        {
            _commands = commands;
        }

        public void Execute()
        {
            foreach (var command in _commands)
            {
                command.Execute();
            }
        }

        public void Cancel()
        {
            foreach (var command in _commands)
            {
                command.Cancel();
            }
        }

        public bool IsCompleted
        {
            get { return _commands.TrueForAll(x => x.IsCompleted); }
        }
    }
}