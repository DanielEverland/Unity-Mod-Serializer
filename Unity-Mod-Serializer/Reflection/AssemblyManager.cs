﻿using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UMS;

namespace UMS.Reflection
{
    /// <summary>
    /// Handles loading of all assemblies that may contain UMS runtime hooks
    /// </summary>
    public static class AssemblyManager
    {
        /* The loading schema is as follows
         * 
         * 1) Gather all .dll files wihtin the mods folder,
         * and add them to a LinkedList. We expose the lists
         * enumerator through a public property
         * 
         * 2) We then go through all the types in every
         * assembly and add them to a LinkedList. We're
         * using LinkedList instead of Queue, since
         * Enqueue is an O(n) operation when it has to re-
         * allocate more space internally.
         * LinkedList.AddLast() is always an O(0) operation
         * 
         * 2b) During the above process we also check types
         * for static functions with the LoadTypeAttribute,
         * and assign the functions to a delegate
         * 
         * 3) We go through every Type again and raise them
         * through the delegate. This allows any assembly to
         * analyse types themselves, extending the flexibility
         * of UMS significantly.
         * 
         * 4) Run cleanup to de-allocate memory
         */

        public static void Initialize()
        {
#if DEBUG
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
#endif
            for (int i = 0; i < _executionFlow.Count; i++)
            {
                _executionFlow[i]();
            }

#if DEBUG
            stopWatch.Stop();
            UnityEngine.Debug.Log("Reflection flow elapsed: " + stopWatch.Elapsed);
#endif
        }

        public static IEnumerable<Assembly> LoadedAssemblies { get { return _loadedAssemblies; } }

        /// <summary>
        /// Quick overview of the order in which functions are called
        /// </summary>
        private static List<Action> _executionFlow = new List<Action>()
        {
            GatherAssemblies,
            GatherTypes,
            ExecuteReflection,
            Cleanup,
        };

        private static Action<Type> _typeAnalysers;

        private static LinkedList<Assembly> _loadedAssemblies;
        private static LinkedList<Type> _loadedTypes;
        
        private static void GatherAssemblies()
        {
            _loadedAssemblies = new LinkedList<Assembly>();

            if (UnityEngine.Application.isEditor)
            {
                LoadAssembliesInEditor();
            }
            else
            {
                LoadAssembliesInBuiltGame();
            }
        }
        private static void LoadAssembliesInEditor()
        {
            foreach (string assemblyName in Settings.PredefinedAssemblies)
            {
                _loadedAssemblies.AddLast(Assembly.Load(assemblyName));
            }
        }
        private static void LoadAssembliesInBuiltGame()
        {
            Queue<string> directories = new Queue<string>();
            directories.Enqueue(Settings.ModsDirectory);

            while (directories.Count > 0)
            {
                string currentDirectory = directories.Dequeue();

                foreach (string file in Directory.GetFiles(currentDirectory))
                {
                    if(Path.GetExtension(file) == ".dll")
                    {
                        _loadedAssemblies.AddLast(Assembly.LoadFile(file));
                    }
                }

                foreach (string subDirectory in Directory.GetDirectories(currentDirectory))
                {
                    directories.Enqueue(subDirectory);
                }
            }
        }
        private static void GatherTypes()
        {
            _loadedTypes = new LinkedList<Type>();

            foreach (Assembly assembly in LoadedAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    _loadedTypes.AddLast(type);

                    PollForHook(type);
                }
            }
        }
        private static void PollForHook(Type type)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (method.GetCustomAttributes().Any(x => x is LoadTypeAttribute))
                {
                    if (LoadTypeAttribute.IsValid(method))
                    {
                        //We cache the function as a delegate because it's much faster than calling MethodInfo.Invoke
                        _typeAnalysers += (Action<Type>)Delegate.CreateDelegate(typeof(Action<Type>), method);
                    }
                }
            }
        }
        private static void ExecuteReflection()
        {
            if (_typeAnalysers == null)
                return;

            foreach (Type type in _loadedTypes)
            {
                _typeAnalysers(type);
            }
        }
        private static void Cleanup()
        {
            _loadedTypes = null;
        }
    }
}
