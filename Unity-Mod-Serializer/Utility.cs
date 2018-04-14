using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS
{
    public static class Utility
    {
        static Utility()
        {
            _random = new System.Random();
        }

        public const int MENU_ITEM_PRIORITY = 100;
        public const string MOD_EXTENSION = ".mod";

        private static System.Random _random;

        public static uint GetRandomUnsignedInt()
        {
            //Random doesn't support uint
            byte[] uintData = new byte[4];

            _random.NextBytes(uintData);

            return System.BitConverter.ToUInt32(uintData, 0);
        }
    }
}
