﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.Converters
{
    public class Texture2DConverter : BinaryConverter<Texture2D>
    {
        public override Result DoSerialize(Texture2D obj, out byte[] data)
        {
            data = Utility.EncodeToPNG(obj);
            return Result.Success;
        }
    }
}