using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMS.Reflection;
using ProtoBuf.Meta;
using System.IO;

namespace UMS
{
    public class Serializer
    {
        public Serializer()
        {
            _cachedModels = new Dictionary<System.Type, IModel>();
        }
        public static void Initialize()
        {
            Debugging.Info(DebuggingFlags.Serializer, "Initializing Serializer");

            _instance = new Serializer();            
            AssemblyManager.Initialize();
            ObjectHandler.Initialize();

            _instance.CreateModel();
        }
        
        public static RuntimeTypeModel Model
        {
            get
            {
                if (_instance == null)
                    Session.Initialize();

                return _instance._model;
            }
        }

        private static Serializer _instance;

        private Dictionary<System.Type, IModel> _cachedModels;
        private RuntimeTypeModel _model;
        
        public static void AddModel(IModel model)
        {
            _instance._cachedModels.Set(model.ModelType, model);
        }
        private void CreateModel()
        {
            _model = TypeModel.Create();

            foreach (IModel model in _cachedModels.Values)
            {
                model.CreateModel(_model.Add(model.ModelType, false));
            }
        }

        #region Public Deserialize Functions
        public static object Deserialize(byte[] data, System.Type type)
        {
            return InternalDeserialize(data, type);
        }
        public static T Deserialize<T>(byte[] data)
        {
            return (T)InternalDeserialize(data, typeof(T));
        }
        #endregion

        #region Public Serialize Functions
        public static byte[] Serialize(object obj)
        {
            return InternalSerialize(obj);
        }
        #endregion

        #region Internal Serialize Functions
        internal static byte[] InternalSerialize(object obj)
        {
            if (obj == null)
                return null;

            if (!Model.CanSerialize(obj.GetType()))
                throw new System.NotImplementedException("Cannot serialize " + obj.GetType());
            
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Model.Serialize(stream, obj);
                    return stream.ToArray();
                }
            }
            catch (System.Exception e)
            {
                Debugging.Error(DebuggingFlags.Serializer, $"Issue serializing {obj}");
                UnityEngine.Debug.LogException(e);
                return null;
            }
        }
        #endregion

        #region Internal Deserialize Functions
        internal static object InternalDeserialize(byte[] data, System.Type type)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    return Model.Deserialize(stream, null, type);
                }
            }
            catch (System.Exception e)
            {
                Debugging.Error(DebuggingFlags.Serializer, $"Issue deserializing {type.Name} ({data.Length.ToString("N0")})");
                UnityEngine.Debug.LogException(e);
                return null;
            }
        }
        #endregion
    }
}
