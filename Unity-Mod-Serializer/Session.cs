using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMS.Reflection;

namespace UMS
{
    /// <summary>
    /// A session contains all data that we want to be able to reset,
    /// allowing us to serialize mod packages sequentially.
    /// 
    /// An example would be how we want to clear the SerializationQueue
    /// between every mod package we serialize. This is because we use
    /// the queue to determine, whether or not an object has been 
    /// serialized into its own entry. If we didn't clear the queue, we
    /// wouldn't be able to serialize a ModPackage twice in the editor
    /// </summary>
    public static class Session
    {
        public static void Initialize()
        {
            Serializer.Initialize();
        }
    }
}
