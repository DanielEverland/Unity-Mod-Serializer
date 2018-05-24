using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf.Meta;
using ProtoBuf;

namespace UMS.Models
{
    public class MaterialModel : ModelBase<Material>
    {
        public override void CreateModel(MetaType type)
        {
            type.AsReferenceDefault = true;
            type.SetSurrogate(typeof(MaterialSurrogate));
        }

        [ProtoContract(AsReferenceDefault = true)]
        private class MaterialSurrogate
        {
            public MaterialSurrogate() { }
            public MaterialSurrogate(Material material)
            {
                name = material.name;
                doubleSidedGI = material.doubleSidedGI;
                globalIlluminationFlags = material.globalIlluminationFlags;
                renderQueue = material.renderQueue;
                mainTextureScale = material.mainTextureScale;
                mainTextureOffset = material.mainTextureOffset;
                color = material.color;
                shader = material.shader;
                shaderKeywords = material.shaderKeywords;
                enableInstancing = material.enableInstancing;
            }

            [ProtoMember(1)]
            private string name;
            [ProtoMember(2)]
            private bool doubleSidedGI;
            [ProtoMember(3)]
            private MaterialGlobalIlluminationFlags globalIlluminationFlags;
            [ProtoMember(4)]
            private int renderQueue;
            [ProtoMember(5)]
            private Vector2 mainTextureScale;
            [ProtoMember(6)]
            private Vector2 mainTextureOffset;
            [ProtoMember(7)]
            private Color color;
            [ProtoMember(8)]
            private Shader shader;
            [ProtoMember(9)]
            private string[] shaderKeywords;
            [ProtoMember(10)]
            private bool enableInstancing;

            public Material Deserialize()
            {
                Material material = new Material(shader);

                material.name = name;
                material.doubleSidedGI = doubleSidedGI;
                material.globalIlluminationFlags = globalIlluminationFlags;
                material.renderQueue = renderQueue;
                material.mainTextureScale = mainTextureScale;
                material.mainTextureOffset = mainTextureOffset;
                material.color = color;
                material.shaderKeywords = shaderKeywords;
                material.enableInstancing = enableInstancing;

                return material;
            }

            public static implicit operator MaterialSurrogate (Material material)
            {
                return material == null ? null : new MaterialSurrogate(material);
            }
            public static implicit operator Material (MaterialSurrogate surrogate)
            {
                return surrogate == null ? null : surrogate.Deserialize();
            }
        }
    }
}
