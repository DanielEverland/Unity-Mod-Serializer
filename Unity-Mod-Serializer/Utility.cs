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

        public static byte[] EncodeToPNG(Texture2D texture)
        {
            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture temporaryTexture = RenderTexture.GetTemporary(
                                texture.width,
                                texture.height,
                                0,
                                RenderTextureFormat.Default,
                                RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, temporaryTexture);

            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;

            RenderTexture.active = temporaryTexture;

            Texture2D toReturn = new Texture2D(texture.width, texture.height);

            toReturn.ReadPixels(new Rect(0, 0, temporaryTexture.width, temporaryTexture.height), 0, 0);
            toReturn.Apply();

            // Reset the active RenderTexture
            RenderTexture.active = previous;
            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(temporaryTexture);

            return toReturn.EncodeToPNG();
        }
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
