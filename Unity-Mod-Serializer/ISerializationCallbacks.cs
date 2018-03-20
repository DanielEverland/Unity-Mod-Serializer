using System;
#if !NO_UNITY
using UnityEngine;
#endif

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace UMS
{
    /// <summary>
    /// Extend this interface on your type to receive notifications about
    /// serialization/deserialization events. If you don't have access to the
    /// type itself, then you can write an ObjectProcessor instead.
    /// </summary>
    public interface ISerializationCallbacks
    {
        /// <summary>
        /// Called before serialization.
        /// </summary>
        void OnBeforeSerialize(Type storageType);

        /// <summary>
        /// Called after serialization.
        /// </summary>
        /// <param name="storageType">
        /// The field/property type that is storing the instance.
        /// </param>
        /// <param name="data">The data that was serialized.</param>
        void OnAfterSerialize(Type storageType, ref Data data);

        /// <summary>
        /// Called before deserialization.
        /// </summary>
        /// <param name="storageType">
        /// The field/property type that is storing the instance.
        /// </param>
        /// <param name="data">
        /// The data that will be used for deserialization.
        /// </param>
        void OnBeforeDeserialize(Type storageType, ref Data data);

        /// <summary>
        /// Called after deserialization.
        /// </summary>
        /// <param name="storageType">
        /// The field/property type that is storing the instance.
        /// </param>
        /// <param name="instance">The type of the instance.</param>
        void OnAfterDeserialize(Type storageType);
    }
}

namespace UMS.Converters
{
    public class SerializationCallbackProcessor : ObjectProcessor
    {
        public override bool CanProcess(Type type)
        {
            return typeof(ISerializationCallbacks).IsAssignableFrom(type);
        }

        public override void OnBeforeSerialize(Type storageType, object instance)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((ISerializationCallbacks)instance).OnBeforeSerialize(storageType);
        }

        public override void OnAfterSerialize(Type storageType, object instance, ref Data data)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((ISerializationCallbacks)instance).OnAfterSerialize(storageType, ref data);
        }

        public override void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref Data data)
        {
            if (instance is ISerializationCallbacks == false)
            {
                throw new InvalidCastException("Please ensure the converter for " + storageType + " actually returns an instance of it, not an instance of " + instance.GetType());
            }

            ((ISerializationCallbacks)instance).OnBeforeDeserialize(storageType, ref data);
        }

        public override void OnAfterDeserialize(Type storageType, object instance)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((ISerializationCallbacks)instance).OnAfterDeserialize(storageType);
        }
    }

#if !NO_UNITY
    public class SerializationCallbackReceiverProcessor : ObjectProcessor
    {
        public override bool CanProcess(Type type)
        {
            return typeof(ISerializationCallbackReceiver).IsAssignableFrom(type);
        }

        public override void OnBeforeSerialize(Type storageType, object instance)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((ISerializationCallbackReceiver)instance).OnBeforeSerialize();
        }

        public override void OnAfterDeserialize(Type storageType, object instance)
        {
            // Don't call the callback on null instances.
            if (instance == null) return;
            ((ISerializationCallbackReceiver)instance).OnAfterDeserialize();
        }
    }
#endif
}