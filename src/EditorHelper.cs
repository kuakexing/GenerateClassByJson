using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace KuFramework.EditorTools
{
    public class GenerateClass
    {
        public static void GenerateJsonClass(string json, string outputPath, string rootClassName) => new EditorGenerateJsonClass().GenerateJsonClass(json, outputPath, rootClassName);
        public static void GenerateJsonClassForGf(string json, string outputPath, string rootClassName) => new EditorGenerateJsonClassForGf().GenerateJsonClass(json, outputPath, rootClassName);
    }
    public partial class Utility
    {
        public static string GetRegularPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return path.Replace('\\', '/');
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
