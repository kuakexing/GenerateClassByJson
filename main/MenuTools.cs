using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
namespace KuFramework.EditorTools
{
    public class MenuTools
    {
        [MenuItem("Assets/GetSelectPaths", false, 91)]
        public static void Execute()
        {
            string[] strs = Selection.assetGUIDs;
            var curPath = Directory.GetCurrentDirectory();
            foreach (var item in strs)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                path = Utility.GetRegularPath(curPath + "/" + path);
                Debug.Log(path);
            }
        }
        [MenuItem(@"Tools/GenerateJsonClass", false, 24)]
        public static void CreateJsonClass()
        {
            string path = Application.streamingAssetsPath;
            if(!Directory.Exists(path))
            {
                Debug.LogError("No streamingAssets");
                return;
            }
            path = Path.Combine(path, "source.json");
            if (!File.Exists(path))
            {
                Debug.LogError("No .json file in streamingAssets");
                return;
            }
            try
            {
                string json = File.ReadAllText(path);
                //Debug.Log(json);
                GenerateClass.GenerateJsonClass(json, Application.streamingAssetsPath);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
