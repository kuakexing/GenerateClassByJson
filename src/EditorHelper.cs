using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
namespace KuFramework.EditorTools
{
    public class GenerateClass
    {
        public static void GenerateJsonClass(string json, string outputPath, string rootClassName) => new EditorGenerateJsonClass().GenerateJsonClass(json, outputPath, rootClassName);
        public static void GenerateJsonClassForGf(string json, string outputPath, string rootClassName) => new EditorGenerateJsonClassForGf().GenerateJsonClass(json, outputPath, rootClassName);
    }
    public class Utility
    {
        public static string GetRegularPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return path.Replace('\\', '/');
        }
        public static void DebugCurWindow()
        {
            //StageHandle handle = StageUtility.GetCurrentStageHandle();
            //Debug.Log(handle.ToString());
            string focus = EditorWindow.focusedWindow.GetType().ToString();
            Debug.Log(focus);

        }
        public static string[] GetTypeNames(Type typeBase, string[] assemblyNames)
        {
            List<string> typeNames = new List<string>();
            foreach (string assemblyName in assemblyNames)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    continue;
                }

                if (assembly == null)
                {
                    continue;
                }

                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                    {
                        typeNames.Add(type.FullName);
                    }
                }
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }
        /// <summary>
        /// 获取子类型名字，不包括基类
        /// </summary>
        /// <param name="typeBase"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static string[] GetSubTypeNames(Type typeBase, string assemblyName)
        {
            List<string> typeNames = new List<string>();

            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch
            {
                throw new Exception("no assembly");
            }

            if (assembly == null)
            {
                throw new Exception("no assembly");
            }

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                {
                    if(type.FullName != typeBase.Name)
                        typeNames.Add(type.FullName);
                }
            }
            typeNames.Sort();
            return typeNames.ToArray();
        }

    }
    public static class FuncExtension
    {
        public static int IndexOf(this StringBuilder sb, string s)
        {
            int contains = 0;
            for (int i = 0; i < sb.Length; i++)
            {
                contains = string.Compare(sb[i].ToString(), s, StringComparison.Ordinal);
                if (contains == 0)
                    return i;
            }
            return -1;
        }
        public static bool HasComponent(this GameObject obj, string componentName)
        {
            return obj.GetComponent(componentName) == null;
        }
        public static bool HasComponent<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T>() == null;
        }
    }
}
