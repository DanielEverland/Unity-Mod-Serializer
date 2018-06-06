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
            type.SetSurrogate(typeof(ShaderSurrogate));
        }

        [ProtoContract()]
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
                Shader shader = Shader.Find(name);

                if (shader == null)
                    throw new System.NullReferenceException($"Couldn't find shader with name {name}");

                return shader;
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
