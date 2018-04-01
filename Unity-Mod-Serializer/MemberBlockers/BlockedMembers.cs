using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.MemberBlockers
{
    public static class BlockedMembers
    {
        [MemberBlocker]
        private static readonly List<string> _defaultBlockedMembers = new List<string>()
        {
            //Shared mesh/material stuff
            "MeshFilter.mesh",
            "Renderer.material",
            "Renderer.materials"
        };
    }
}
