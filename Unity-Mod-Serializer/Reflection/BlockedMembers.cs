using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Reflection
{
    public static class BlockedMembers
    {
        [MemberBlocker]
        private static readonly List<string> _defaultBlockedMembers = new List<string>()
        {
            //Shared mesh/material stuff
            "MeshFilter.mesh",
            "Renderer.material",
            "Renderer.materials",
            "Renderer.sharedMaterial", //We want to serialize the entire list of materials, not just the first index
            "Renderer.lightmapTilingOffset",

            "MonoBehaviour.runInEditMode",
        };
    }
}
