using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using LitJson;
using System.Reflection;
using UnityEditor;
using System.Text.RegularExpressions;
namespace KuFramework.EditorTools
{
    internal class EditorGenerateJsonClass
    {
        private GenerateClassTemplete mClassTemplete;
        private ClassItem mClassItem;
        private string mOutputPath;
        private string mFileName;
        private readonly Dictionary<JsonType,string> mTypeDic;
        public EditorGenerateJsonClass()
        {
            mOutputPath = "";
            mFileName = "";
            mClassTemplete = new GenerateClassTemplete();
            mTypeDic = new Dictionary<JsonType, string>()
            {
                {JsonType.Boolean,"bool"},
                {JsonType.Double,"double"},
                {JsonType.Int,"int"},
                {JsonType.Long,"long"},
                {JsonType.String,"string"},
                {JsonType.Array,"List"},
                {JsonType.Object,"class"}
            };
        }
        public void GenerateJsonClass(string json,string outputPath,string rootClassName)
        {
            mOutputPath = outputPath;
            mFileName = string.Format("{0}.cs",rootClassName);
            JsonData jsondata = JsonMapper.ToObject(json);
            Iteration(jsondata, rootClassName, rootClassName, JsonType.Array);
            CreateScript(mClassTemplete);
        }
        private void CreateScript(GenerateClassTemplete templete)
        {
            StringBuilder classstr = new StringBuilder("using System.Collections;\r\nusing System.Collections.Generic;\r\nusing GameFramework.DataTable;\r\nusing LitJson;\r\n\r\n");
            classstr.AppendFormat("public class JsonDataRow : IDataRow\r\n{{\r\n\tpublic int Id {{ get; protected set; }}\r\n\tpublic Root JsonData {{ get; protected set; }}\r\n\tpublic bool ParseDataRow(string dataRowString, object userData)\r\n\t{{\r\n\t\tJsonData = JsonMapper.ToObject<Root>(dataRowString);\r\n\t\treturn true;\r\n\t}}\r\n\tpublic bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)\r\n\t{{\r\n\t\tJsonData = JsonMapper.ToObject<Root>(dataRowBytes.ToString());\r\n\t\treturn true;\r\n\t}}\r\n}}\r\n");
            foreach (var item in templete.classDic)
            {
                classstr.Append(CreateClass(item));
            }
            if (!Directory.Exists(mOutputPath))
                Debug.LogErrorFormat("No directory : {0}", mOutputPath);
            else
            {
                string path = Utility.GetRegularPath(Path.Combine(mOutputPath, mFileName));
                if (File.Exists(path))
                {
                    int choice = EditorUtility.DisplayDialogComplex("Warning", string.Format("File {0} has already exist, confirm to overwrite?", mFileName), "ok", "cancel", "no");
                    if (choice != 0)
                    {
                        Debug.Log("Canceled");
                        return;
                    }
                }
                File.WriteAllText(path, classstr.ToString());
                Debug.LogFormat("Generate successful!！ path: {0}", path);
                AssetDatabase.Refresh();
            }
        }
        private StringBuilder CreateClass(KeyValuePair<string,SubClass> subClassInfo)
        {
            StringBuilder classstr = new StringBuilder();
            Dictionary<string,ClassItem> classitemdic = subClassInfo.Value.itemDic;
            List<ClassItem> itemlist = new List<ClassItem>();
            foreach (var item in classitemdic.Keys)
            {
                itemlist.Add(classitemdic[item]);
            }
            StringBuilder tempstr = new StringBuilder();
            for (int i = 0; i < itemlist.Count; i++)
            {
                tempstr.Append("#\r\n\t");
            }
            classstr.AppendFormat("public struct {0}\r\n{{\r\n\t#\r\n}}", subClassInfo.Key);
            classstr.Replace("#", tempstr.ToString());
            tempstr.Clear();
            int index = 0;
            foreach (var item in itemlist)
            {
                tempstr.AppendFormat("public readonly {0} {1};",item.type,item.name);
                index = classstr.IndexOf("#");
                classstr.Replace("#",tempstr.ToString(),index,1);
                tempstr.Clear();
            }
            classstr.Append("\r\n");
            return classstr;
        }
        private void Iteration(JsonData data,string rootName,string parentName,JsonType parentType)
        {
            JsonType jsontype = data.GetJsonType();
            switch (jsontype)
            {
                case JsonType.Array:
                    ArrayIteration(data, "inst_array", rootName, parentName);
                    break;
                case JsonType.Object:
                    ObjectIteration(data, "inst_object", rootName, parentName, parentType);
                    break;
                default:
                    CreateItem(data,rootName,jsontype,parentName);
                    break;
            }
        }
        private void ArrayIteration(JsonData data, string property, string rootName,string parentName)
        {
            if (!mClassTemplete.classDic.ContainsKey(parentName))
                mClassTemplete.classDic[parentName] = new SubClass();
            string uppername =  "Sub" + Upper(rootName);
            if (!mClassTemplete.classDic[parentName].itemDic.ContainsKey(rootName))
                mClassTemplete.classDic[parentName].itemDic[rootName] = new ClassItem(rootName,string.Format("List<{0}>", uppername),"");
            List<JsonData> list = GetStructList(data, property) as List<JsonData>;
            if(list.Count == 0)
            {
                if (!mClassTemplete.classDic.ContainsKey(uppername))
                    mClassTemplete.classDic[uppername] = new SubClass();
            }
            foreach (var item in list)
            {
                Iteration(item, uppername, parentName,JsonType.Array);
            }
        }
        private void ObjectIteration(JsonData data, string property,string rootName,string parentName,JsonType parentType)
        {
            string upppername = Upper(rootName);
            if (!mClassTemplete.classDic.ContainsKey(upppername))
                mClassTemplete.classDic[upppername] = new SubClass();
            if(parentType.Equals(JsonType.Object))
                if (!mClassTemplete.classDic[parentName].itemDic.ContainsKey(upppername))
                    mClassTemplete.classDic[parentName].itemDic[rootName] = new ClassItem(rootName, upppername, "");
            Dictionary<string, JsonData> dic = GetStructList(data, property) as Dictionary<string, JsonData>;
            foreach (var item in dic.Keys)
            {
                Iteration(dic[item], item, upppername, JsonType.Object);
            }
        }
        private void CreateItem(JsonData data, string rootName, JsonType jsontype, string parentName)
        {
            mClassItem.name = rootName;
            mClassItem.type = mTypeDic[jsontype];
            mClassItem.value = data.ToString();
            ClassItem item = new ClassItem(mClassItem);
            mClassTemplete.classDic[parentName].itemDic[rootName] = item;
            //Debug.LogFormat("{0} {1} = {2}", mClassItem.type, mClassItem.name, mClassItem.value);
        }
        private string Upper(string str)
        {
            StringBuilder sb = new StringBuilder(str);
            Match collection = Regex.Match(sb[0].ToString(), "a-z");
            if (collection != null)
            {
                string upper = sb[0].ToString().ToUpperInvariant();
                sb = sb.Remove(0,1).Insert(0, upper);
            }
            return sb.ToString();
        }
        private object GetStructList(JsonData jsonData, string property) => jsonData.GetType().GetField(property, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(jsonData);
    }
    internal class GenerateClassTemplete
    {
        public Dictionary<string,SubClass> classDic = new Dictionary<string, SubClass>();
    }
    internal class SubClass
    {
        public Dictionary<string,ClassItem> itemDic = new Dictionary<string, ClassItem>();
    }
    internal struct ClassItem
    {
        public string  name;
        public string  type;
        public string  value;

        public ClassItem(string name, string type, string value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
        public ClassItem(ClassItem classItem)
        {
            name = classItem.name;
            type = classItem.type;
            value = classItem.value;
        }
    }
}


