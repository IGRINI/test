using Game.Utils.Console;

namespace Game.Utils
{
    public static class Dev
    {
        private static ConsoleView _consoleView;
        
        public static void Log(object log)
        {
            _consoleView.AddLine(log);
        }
    }
}