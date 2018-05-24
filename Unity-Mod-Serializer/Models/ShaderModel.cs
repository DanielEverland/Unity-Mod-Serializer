using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;
using ProtoBuf.Meta;

namespace UMS.Models
{
    public class ShaderModel : ModelBase<Shader>
    {
        public override void CreateModel(MetaType type)
        {
            type.AsReferenceDefault = true;
            type.SetSurrogate(typeof(ShaderSurrogate));
        }

        [ProtoContract(AsReferenceDefault = true)]
        private class ShaderSurrogate
        {
            public ShaderSurrogate() { }
            public ShaderSurrogate(Shader shader)
            {
                name = shader.name;
            }

            [ProtoMember(1)]
            private string name;

            public Shader Deserialize()
            {
                return Shader.Find(name);
            }

            public static implicit operator ShaderSurrogate (Shader shader)
            {
                return shader == null ? null : new ShaderSurrogate(shader);
            }
            public static implicit operator Shader (ShaderSurrogate surrogate)
            {
                return surrogate == null ? null : surrogate.Deserialize();
            }
        }
    }    
}
