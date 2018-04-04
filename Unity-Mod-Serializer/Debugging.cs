using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS
{
    public static class Debugging
    {
        public static void Force(DebuggingLevels level, string message)
        {
            switch (level)
            {
                case DebuggingLevels.Verbose:
                    Debug.Log(message);
                    break;
                case DebuggingLevels.Info:
                    Debug.Log(message);
                    break;
                case DebuggingLevels.Warning:
                    Debug.LogWarning(message);
                    break;
                case DebuggingLevels.Error:
                    Debug.LogError(message);
                    break;
                default:
                    throw new ArgumentException("Unexpected level");
            }
        }
        public static void Verbose(DebuggingFlags flags, string message)
        {
            Output(flags, DebuggingLevels.Verbose, message, Debug.Log);
        }
        public static void Info(DebuggingFlags flags, string message)
        {
            Output(flags, DebuggingLevels.Info, message, Debug.Log);
        }
        public static void Warning(DebuggingFlags flags, string message)
        {
            Output(flags, DebuggingLevels.Warning, message, Debug.LogWarning);
        }
        public static void Error(DebuggingFlags flags, string message)
        {
            Output(flags, DebuggingLevels.Error, message, Debug.LogError);
        }
        private static void Output(DebuggingFlags flags, DebuggingLevels levels, string message, Action<string> action)
        {
            if (flags == DebuggingFlags.None || levels == DebuggingLevels.None)
                return;

#if !DEBUG
            if (!Application.isEditor && !Settings.DebugInBuiltVersion)
                return;
#endif

            if ((Settings.DebuggingLevels & levels) != levels)
                return;

            if ((Settings.DebuggingFlags & flags) != flags)
                return;

            action(message);
        }
    }
}
