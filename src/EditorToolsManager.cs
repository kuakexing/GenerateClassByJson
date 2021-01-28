using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace KuFramework.EditorTools
{
    public partial class EditorToolsManager
    {
        [MenuItem("Assets/Tools/GenerateJsonClass",false,92)]
        public static void GenerateJsonScript()
        {
            string[] strs = Selection.assetGUIDs;
            var curPath = Directory.GetCurrentDirectory();
            string[] path = GetPath(strs, curPath);
            for (int i = 0; i < path.Length; i++)
            {
                string outputpath = Path.GetFileNameWithoutExtension(path[i]);
                string filename = Path.GetFileName(path[i]);
                GenerateJsonClass(path[i], GenerateClass.GenerateJsonClass);
            }
        }

        [MenuItem("Assets/Tools/GenerateJsonClassForGf",false,93)]
        public static void GenerateJsonScriptForGf()
        {
            string[] strs = Selection.assetGUIDs;
            var curPath = Directory.GetCurrentDirectory();
            foreach (var item in strs)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                path = GetRegularPath(curPath + "/" + path);
                //Debug.Log(path);
                string outputpath = Path.GetFileNameWithoutExtension(path);
                string filename = Path.GetFileName(path);
                GenerateJsonClass(path, GenerateClass.GenerateJsonClassForGf);
            }
        }
        private static string[] GetPath(string[] strs, string curPath)
        {
            string[] results = new string[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(strs[i]);
                path = GetRegularPath(curPath + "/" + path);
                //Debug.Log(path);
                results[i] = path;
            }
            return results;
        }

        private static void GenerateJsonClass(string sourcePath, System.Action<string, string, string> GenerateClass)
        {
            string dir = Path.GetDirectoryName(sourcePath);
            string filename = Path.GetFileName(sourcePath);
            filename = Path.GetFileNameWithoutExtension(filename);
            if (!Directory.Exists(dir))
            {
                Debug.LogError("No streamingAssets");
                return;
            }
            if (!File.Exists(sourcePath))
            {
                Debug.LogError("No file in: " + sourcePath);
                return;
            }
            try
            {
                string json = File.ReadAllText(sourcePath);
                //Debug.Log(json);
                GenerateClass.Invoke(json, dir, filename);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        public static string GetRegularPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return path.Replace('\\', '/');
        }

    }
}