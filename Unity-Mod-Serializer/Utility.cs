using System.Linq;
using System.Diagnostics;
using UnityEngine;

namespace UMS
{
    public static class Utility
    {
        public const string MOD_EXTENSION = "mod";
        public const string MANIFEST_NAME = ".manifest";

        public static void KillZipReaders()
        {
            if (!Application.isEditor)
                return;
            
            foreach (Process process in Process.GetProcesses().Where(x => x.ProcessName.Contains("7z")))
            {
                process.Kill();
            }
        }
    }
}
