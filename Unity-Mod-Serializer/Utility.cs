using System.Linq;
using System.Diagnostics;
using UnityEngine;
using System;

namespace UMS
{
    public static class Utility
    {
        static Utility()
        {
            _random = new System.Random();
        }

        public const int MENU_ITEM_PRIORITY = 100;
        public const string MOD_EXTENSION = "mod";
        public const string MANIFEST_NAME = ".manifest";

        private static System.Random _random;
        
        public static uint GetRandomID()
        {
            //Random doesn't support uint
            byte[] uintData = new byte[4];

            _random.NextBytes(uintData);

            return BitConverter.ToUInt32(uintData, 0);
        }
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
