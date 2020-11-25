using System;
using System.Collections.Generic;
using System.Text;

namespace KuFramework.EditorTools
{
    public class GenerateClass
    {
        public static void GenerateJsonClass(string json, string outputPath) => new EditorGenerateJsonClass().GenerateJsonClass(json, outputPath);
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
    }
    public static class ExpendFunc
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
    }

}
