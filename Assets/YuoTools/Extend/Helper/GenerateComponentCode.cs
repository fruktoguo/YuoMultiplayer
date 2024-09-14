using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YuoTools.Extend.Helper;
using YuoTools.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YuoTools
{
    public class GenerateComponentCode
    {
        public static SpawnCodeConfig SpawnConfig
        {
            get
            {
#if UNITY_EDITOR
                if (_spawnConfig == null)
                {
                    _spawnConfig = new();
                    ReflexHelper.InvokeMethodByPrefix(_spawnConfig, "Init");
                }
#endif
                return _spawnConfig;
            }
        }

        private static SpawnCodeConfig _spawnConfig;

        public static void SpawnCode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string ComponentName = gameObject.name;
            string strDlgName = $"View_{ComponentName}Component";

            string strFilePath = Application.dataPath + "/YuoComponent/View";

            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBComponentlder = new StringBuilder();

            Dictionary<string, string> allTypes = new();

            strBComponentlder.AppendLine(CodeSpawn_AddAllComponent(SpawnCodeConfig.ComponentTag, allTypes,
                gameObject.transform));

            strBComponentlder.AppendLine(CodeSpawn_FindAll(allTypes));

            strBComponentlder.AppendLine("\n\t\t}");

            strBComponentlder.AppendLine("\t}\r}");

            //引入命名空间
            StringBuilder final = new StringBuilder();

            final.AppendLine(CodeSpawn_AddNameSpace(strDlgName, allTypes));


            final.AppendLine("\tpublic static partial class ViewType");
            final.AppendLine("\t{");
            final.AppendLine($"\t\tpublic const string {ComponentName} = \"{ComponentName}\";");
            final.AppendLine("\t}");

            final.AppendLine($"\tpublic partial class {strDlgName} : ComponentComponent \n\t{{");

            final.AppendLine("");
            final.AppendLine(
                $"\t\tpublic static {strDlgName} GetView() => ComponentManagerComponent.Get.GetComponentView<{strDlgName}>();");
            final.AppendLine("");

            final.Append(strBComponentlder);

            sw.Write(final.ToString());

            sw.Close();

            SpawnSystemCode(ComponentName);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void SpawnGeneralComponentCode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string ComponentName = gameObject.name.Replace(SpawnCodeConfig.GeneralTag, "");

            string strDlgName = $"View_{ComponentName}Component";

            string strFilePath = Application.dataPath + "/YuoComponent/View/General";

            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBComponentlder = new StringBuilder();
            Dictionary<string, string> allTypes = new();

            strBComponentlder.AppendLine(CodeSpawn_AddAllComponent(SpawnCodeConfig.GeneralTag, allTypes,
                gameObject.transform));

            strBComponentlder.AppendLine(CodeSpawn_FindAll(allTypes));

            strBComponentlder.AppendLine("\n\t\t}");

            strBComponentlder.AppendLine("\t}\r}");

            //引入命名空间
            StringBuilder final = new StringBuilder();

            final.AppendLine(CodeSpawn_AddNameSpace(strDlgName, allTypes));
            final.AppendLine($"\tpublic partial class {strDlgName} : ComponentComponent \n\t{{");

            final.Append(strBComponentlder);
            sw.Close();
            SpawnSystemCode(ComponentName, ComponentType.General);
        }

        private static void SpawnChildComponentCode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string ComponentName = gameObject.name.Replace(SpawnCodeConfig.VariantChildComponentTag, SpawnCodeConfig.ChildTag);
            ComponentName = ComponentName.Replace(SpawnCodeConfig.ChildTag, "");
            if (ComponentName.Contains("_"))
            {
                ComponentName = ComponentName.Split('_')[0];
            }

            string strDlgName = $"View_{ComponentName}Component";

            string strFilePath = Application.dataPath + "/YuoComponent/View/Child";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBComponentlder = new StringBuilder();

            Dictionary<string, string> allTypes = new();

            strBComponentlder.AppendLine(CodeSpawn_AddAllComponent(SpawnCodeConfig.ComponentTag, allTypes,
                gameObject.transform, findThis: true));

            strBComponentlder.AppendLine(CodeSpawn_FindAll(allTypes));

            strBComponentlder.AppendLine("\n\t\t}");

            strBComponentlder.AppendLine("\t}\r}");

            //引入命名空间
            StringBuilder final = new StringBuilder();

            final.AppendLine(CodeSpawn_AddNameSpace(strDlgName, allTypes));
            final.AppendLine($"\tpublic partial class {strDlgName} : ComponentComponent \n\t{{");

            final.Append(strBComponentlder);

            sw.Write(final.ToString());

            sw.Close();

            SpawnSystemCode(ComponentName, ComponentType.Child);
        }

        private static void SpawnVariantChildComponentCode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string ComponentName = gameObject.name.Replace(SpawnCodeConfig.VariantChildComponentTag, "");

            string strDlgName = $"View_{ComponentName}VariantComponent";

            string strFilePath = Application.dataPath + "/YuoComponent/View/Child";

            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBComponentlder = new StringBuilder();

            Dictionary<string, string> allTypes = new();

            strBComponentlder.AppendLine(CodeSpawn_AddAllComponent(SpawnCodeConfig.VariantChildComponentTag, allTypes,
                gameObject.transform));

            strBComponentlder.AppendLine(CodeSpawn_FindAll(allTypes));

            strBComponentlder.AppendLine("\n\t\t}");

            strBComponentlder.AppendLine("\t}\r}");

            //引入命名空间
            StringBuilder final = new StringBuilder();

            final.AppendLine(CodeSpawn_AddNameSpace(strDlgName, allTypes));
            final.AppendLine($"\tpublic partial class {strDlgName} : ComponentComponent \n\t{{");

            final.Append(strBComponentlder);

            sw.Write(final.ToString());

            sw.Close();

            SpawnSystemCode(ComponentName + "Variant", ComponentType.Child);
        }

        public static string CodeSpawn_AddNameSpace(string strDlgName, Dictionary<string, string> allTypes)
        {
            StringBuilder strBComponentlder = new StringBuilder();
            foreach (var space in SpawnConfig.ComponentNameSpace)
            {
                strBComponentlder.AppendLine(space);
            }

            foreach (var type in allTypes.Keys)
            {
                foreach (var item in SpawnConfig.ComponentAddNameSpace)
                {
                    if (item.Key.Name == type)
                        strBComponentlder.AppendLine(item.Value);
                }
            }

            strBComponentlder.AppendLine();
            //设置命名空间
            strBComponentlder.AppendLine("namespace YuoTools.Component\n{");

            return strBComponentlder.ToString();
        }

        public static string CodeSpawn_AddAllComponent(string tag, Dictionary<string, string> allTypes,
            Transform transform, bool findThis = false)
        {
            StringBuilder strBComponentlder = new StringBuilder();

            //获取所有组件
            // List<Transform> trans = FindAll(transform);
            List<Transform> trans = new();
            TransformInfo tranInfo = new TransformInfo(transform);

            FindAll(tranInfo);

            //剔除自己
            trans.Remove(transform);

            #region 初始化额外组件

            //获取通用组件
            List<Transform> generalTemps = new List<Transform>();
            List<Transform> generals = new List<Transform>();

            //获取子组件
            List<Transform> childTemps = new List<Transform>();
            List<Transform> children = new List<Transform>();

            //获取变体子组件
            List<Transform> variantChildren = new List<Transform>();
            List<Transform> variantChildTemps = new List<Transform>();

            void GetAll(TransformInfo info)
            {
                foreach (var tran in info.children)
                {
                    if (tran.transform.name.StartsWith(SpawnCodeConfig.GeneralTag))
                    {
                        generals.Add(tran.transform);
                    }
                    else if (tran.transform.name.StartsWith(SpawnCodeConfig.ChildTag))
                    {
                        children.Add(tran.transform);
                    }
                    else if (tran.transform.name.StartsWith(SpawnCodeConfig.VariantChildComponentTag))
                    {
                        variantChildren.Add(tran.transform);
                    }
                    else
                    {
                        trans.Add(tran.transform);
                        GetAll(tran);
                    }
                }
            }

            GetAll(tranInfo);

            #endregion

            if (findThis)
            {
                var types = GetTypes(transform);
                foreach (var type in types)
                {
                    if (!allTypes.ContainsKey(type)) allTypes.Add(type, "");
                    allTypes[type] += $"\n\t\t\tall_{type}.Add(Main{type});";
                    string get = "{\n\t\t\tget\n\t\t\t{\n\t\t\t\tif (" + $"main{type}" +
                                 $" == null)\n\t\t\t\t\tmain{type} = rectTransform.GetComponent<{type}>();\n\t\t\t\treturn " +
                                 $"main{type}" +
                                 ";\n\t\t\t}\n\t\t}";

                    strBComponentlder.AppendLine($"\n\t\tprivate {type} main{type};");

                    strBComponentlder.AppendLine($"\n\t\tpublic {type} Main{type}\n\t\t{get}");
                }
            }

            foreach (var item in trans)
            {
                if (item.name.StartsWith(tag))
                {
                    string name = item.name.Replace(tag, "");
                    var types = GetTypes(item);

                    if (types.Count == 0)
                    {
                        types.Add("RectTransform");
                    }

                    foreach (var type in types)
                    {
                        CodeSpawn_AddFindAll(allTypes, type, name);
                        strBComponentlder.AppendLine(CodeSpawn_AddComponent(type, name,
                            GetRelativePath(item, transform)));
                    }
                }
            }

            #region 生成额外组件

            foreach (var general in generals)
            {
                Debug.Log(general);
                tag = SpawnCodeConfig.ChildTag;
                SpawnGeneralComponentCode(general.gameObject);
                string name = general.name.Replace(tag, "");
                string type = $"View_{name}Component";
                string get = "{\n\t\t\tget" +
                             "\n\t\t\t{" +
                             "\n\t\t\t\tif (" + $"mGeneral_{name} == null)" +
                             "\n\t\t\t\t{" +
                             $"\n\t\t\t\t\tmGeneral_{name} = Entity.AddChild<{type}>();" +
                             $"\n\t\t\t\t\tmGeneral_{name}.rectTransform = rectTransform.Find(" +
                             $"\"{GetRelativePath(general, transform)}\"" + ") as RectTransform;" +
                             "\n\t\t\t\t}" +
                             $"\n\t\t\t\treturn " + $"mGeneral_{name}" + ";\n\t\t\t}\n\t\t}";

                strBComponentlder.AppendLine($"\n\t\tprivate {type} mGeneral_{name};");

                strBComponentlder.AppendLine($"\n\t\tpublic {type} General_{name}\n\t\t{get}");
            }

            foreach (var child in children)
            {
                tag = SpawnCodeConfig.ChildTag;
                SpawnChildComponentCode(child.gameObject);
                string name = child.name.Replace(tag, "");
                string typeName = child.name.Replace(tag, "");
                if (typeName.Contains("_"))
                {
                    typeName = typeName.Split('_')[0];
                }

                string type = $"View_{typeName}Component";
                Debug.Log($"检索到子物体 [ {child} ]  _ 类型为 [ {type} ]");
                if (!allTypes.ContainsKey(type))
                {
                    allTypes.Add(type, "");
                }

                allTypes[type] += $"\n\t\t\tall_{type}.Add(Child_{name});";

                string get = "{\n\t\t\tget" +
                             "\n\t\t\t{" +
                             "\n\t\t\t\tif (" + $"mChild_{name} == null)" +
                             "\n\t\t\t\t{" +
                             $"\n\t\t\t\t\tmChild_{name} = Entity.AddChild<{type}>();" +
                             "\n\t\t\t\t\t" + $@"mChild_{name}.Entity.EntityName = ""{name}"";" +
                             $"\n\t\t\t\t\tmChild_{name}.rectTransform = rectTransform.Find(" +
                             $"\"{GetRelativePath(child, transform)}\"" + ") as RectTransform;" +
                             $"\n\t\t\t\t\tmChild_{name}.RunSystem<IComponentCreate>();" +
                             "\n\t\t\t\t}" +
                             $"\n\t\t\t\treturn " + $"mChild_{name}" + ";\n\t\t\t}\n\t\t}";

                strBComponentlder.AppendLine($"\n\t\tprivate {type} mChild_{name};");

                strBComponentlder.AppendLine($"\n\t\tpublic {type} Child_{name}\n\t\t{get}");
            }

            foreach (var variantChild in variantChildren)
            {
                tag = SpawnCodeConfig.VariantChildComponentTag;
                SpawnChildComponentCode(variantChild.gameObject);
                SpawnVariantChildComponentCode(variantChild.gameObject);
                string name = variantChild.name.Replace(tag, "");
                string typeName = variantChild.name.Replace(tag, "");
                string typeVariantName = variantChild.name.Replace(tag, "");
                if (typeName.Contains("_"))
                {
                    typeName = typeName.Split('_')[0];
                }

                string type = $"View_{typeName}Component";
                string typeVariant = $"View_{typeVariantName}VariantComponent";
                Debug.Log($"检索到子物体变体 [ {variantChild} ]  _ 类型为 [ {type} ]");
                allTypes.TryAdd(type, "");

                allTypes[type] += $"\n\t\t\tall_{type}.Add(Child_{name});";

                string get = "{\n\t\t\tget" +
                             "\n\t\t\t{" +
                             "\n\t\t\t\tif (" + $"mChild_{name} == null)" +
                             "\n\t\t\t\t{" +
                             $"\n\t\t\t\t\tmChild_{name} = Entity.AddChild<{type}>();" +
                             "\n\t\t\t\t\t" + $@"mChild_{name}.Entity.EntityName = ""{name}"";" +
                             $"\n\t\t\t\t\tmChild_{name}Variant = mChild_{name}.AddComponent<{typeVariant}>();" +
                             $"\n\t\t\t\t\tmChild_{name}.rectTransform = rectTransform.Find(" +
                             $"\"{GetRelativePath(variantChild, transform)}\"" + ") as RectTransform;" +
                             $"\n\t\t\t\t\tmChild_{name}Variant.rectTransform = mChild_{name}.rectTransform;" +
                             $"\n\t\t\t\t\tmChild_{name}.Entity.RunSystem<IComponentCreate>();;" +
                             "\n\t\t\t\t}" +
                             $"\n\t\t\t\treturn " + $"mChild_{name}" + ";\n\t\t\t}\n\t\t}";

                strBComponentlder.AppendLine($"\n\t\tprivate {type} mChild_{name};");

                strBComponentlder.AppendLine($"\n\t\tpublic {type} Child_{name}\n\t\t{get}");

                string getVariant = "{\n\t\t\tget" +
                                    "\n\t\t\t{" +
                                    $"\n\t\t\t\treturn " + $"mChild_{name}Variant" + ";\n\t\t\t}\n\t\t}";

                strBComponentlder.AppendLine($"\n\t\tprivate {typeVariant} mChild_{name}Variant;");

                strBComponentlder.AppendLine($"\n\t\tpublic {typeVariant} Child_{name}Variant\n\t\t{getVariant}");
            }

            #endregion

            return strBComponentlder.ToString();
        }

        public static string CodeSpawn_AddComponent(string type, string name, string relativePath)
        {
            StringBuilder strBComponentlder = new StringBuilder();

            string get = "{\n\t\t\tget\n\t\t\t{\n\t\t\t\tif (" + $"m{type}_{name}" +
                         $" == null)\n\t\t\t\t\tm{type}_{name} = rectTransform.Find(\"" +
                         relativePath +
                         $"\").GetComponent<{type}>();\n\t\t\t\treturn " + $"m{type}_{name}" +
                         ";\n\t\t\t}\n\t\t}";

            strBComponentlder.AppendLine($"\n\t\tprivate {type} m{type}_{name};");

            strBComponentlder.AppendLine($"\n\t\tpublic {type} {type}_{name}\n\t\t{get}");
            return strBComponentlder.ToString();
        }

        public static void CodeSpawn_AddFindAll(Dictionary<string, string> allTypes, string type, string name)
        {
            if (!allTypes.ContainsKey(type)) allTypes.Add(type, "");
            allTypes[type] += $"\n\t\t\tall_{type}.Add({type}_{name});";
        }

        public static string CodeSpawn_FindAll(Dictionary<string, string> allTypes)
        {
            StringBuilder strBComponentlder = new StringBuilder();
            foreach (var item in allTypes.Keys)
            {
                strBComponentlder.AppendLine("\n\t\t[FoldoutGroup(\"ALL\")]");
                strBComponentlder.AppendLine($"\n\t\tpublic List<{item}> all_{item} = new();");
            }

            strBComponentlder.AppendLine("\n\t\tpublic void FindAll()\n\t\t{");
            foreach (var item in allTypes)
            {
                strBComponentlder.AppendLine($"\t\t\t\t{item.Value};");
            }

            return strBComponentlder.ToString();
        }

        public enum ComponentType
        {
            Window,
            Child,
            General
        }

        public static void SpawnSystemCode(string name, ComponentType ComponentType = ComponentType.Window)
        {
            string strFilePath = Application.dataPath + "/YuoComponent/System";
            if (ComponentType != ComponentType.Window) strFilePath += "/Child";
            if (!Directory.Exists(strFilePath))
            {
                Directory.CreateDirectory(strFilePath);
            }

            string strDlgName = $"View_{name}System";
            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            if (File.Exists(strFilePath))
            {
                Debug.LogWarning($"{strDlgName}已存在");
                return;
            }

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBComponentlder = new StringBuilder();
            foreach (var space in SpawnConfig.SystemNameSpace)
            {
                strBComponentlder.AppendLine(space);
            }

            //设置命名空间
            strBComponentlder.AppendLine("namespace YuoTools.Component");
            strBComponentlder.AppendLine("{");
            switch (ComponentType)
            {
                case ComponentType.Window:
                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}CreateSystem :YuoSystem<View_{name}Component>, IComponentCreate");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine($"\t\tprotected override void Run(View_{name}Component view)");
                    strBComponentlder.AppendLine($"\t\t{{");
                    strBComponentlder.AppendLine("\t\t\tview.FindAll();");
                    strBComponentlder.AppendLine("\t\t\t//关闭窗口的事件注册,名字不同请自行更");
                    strBComponentlder.AppendLine("\t\t\tview.Button_Close.SetComponentClose(view.ViewName);");
                    strBComponentlder.AppendLine(" \t\t\tview.Button_Mask.SetComponentClose(view.ViewName);");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");

                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}OpenSystem :YuoSystem<View_{name}Component>, IComponentOpen");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine(
                        $"\t\tprotected override void Run(View_{name}Component view)");
                    strBComponentlder.AppendLine($"\t\t{{");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");

                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}CloseSystem :YuoSystem<View_{name}Component>, IComponentClose");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine(
                        $"\t\tprotected override void Run(View_{name}Component view)");
                    strBComponentlder.AppendLine($"\t\t{{");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");

                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}OpenAnimaSystem :YuoSystem<View_{name}Component,ComponentAnimaComponent>, IComponentOpen");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine(
                        $"\t\tprotected override void Run(View_{name}Component view, ComponentAnimaComponent anima)");
                    strBComponentlder.AppendLine($"\t\t{{");
                    strBComponentlder.AppendLine("\t\t\tview.Button_Mask.image.SetColorA(0);\n");
                    strBComponentlder.AppendLine("\t\t\tview.Button_Mask.image.DOFade(0.6f, anima.AnimaDuration);");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");

                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}CloseAnimaSystem :YuoSystem<View_{name}Component,ComponentAnimaComponent>, IComponentClose");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine(
                        $"\t\tprotected override void Run(View_{name}Component view, ComponentAnimaComponent anima)");
                    strBComponentlder.AppendLine("\t\t{");
                    strBComponentlder.AppendLine("\t\t\tview.Button_Mask.image.DOFade(0f, anima.AnimaDuration);");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");
                    break;
                case ComponentType.General:
                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}ActiveSystem :YuoSystem<View_{name}Component>, IComponentActive");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine($"\t\tprotected override void Run(View_{name}Component view)");
                    strBComponentlder.AppendLine($"\t\t{{");
                    strBComponentlder.AppendLine("\t\t\tview.FindAll();");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");
                    break;
                case ComponentType.Child:
                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}CreateSystem :YuoSystem<View_{name}Component>, IComponentCreate");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine($"\t\tprotected override void Run(View_{name}Component view)");
                    strBComponentlder.AppendLine($"\t\t{{");
                    strBComponentlder.AppendLine("\t\t\tview.FindAll();");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");

                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}OpenSystem :YuoSystem<View_{name}Component>, IComponentOpen");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine(
                        $"\t\tprotected override void Run(View_{name}Component view)");
                    strBComponentlder.AppendLine($"\t\t{{");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");

                    strBComponentlder.AppendLine(
                        $"\tpublic class View{name}CloseSystem :YuoSystem<View_{name}Component>, IComponentClose");
                    strBComponentlder.AppendLine("\t{");
                    strBComponentlder.Append("\t\t");
                    strBComponentlder.Append(@$"public override string Group =>""Component/{name}"";");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine("");
                    strBComponentlder.AppendLine(
                        $"\t\tprotected override void Run(View_{name}Component view)");
                    strBComponentlder.AppendLine($"\t\t{{");
                    strBComponentlder.AppendLine("\t\t}");
                    strBComponentlder.AppendLine("\t}");
                    break;
                default:
                    break;
            }

            strBComponentlder.AppendLine("}");

            sw.Write(strBComponentlder.ToString());
            sw.Close();
        }

        private static List<string> GetTypes(Transform transform)
        {
            List<string> ts = new List<string>();

            foreach (var item in SpawnConfig.SpawnType)
            {
                Get(item);
            }

            foreach (var item in SpawnConfig.RemoveType)
            {
                if (ts.Contains(item.Key.Name)) ts.Remove(item.Value.Name);
            }

            if (ts.Count == 0)
            {
                ts.Add(nameof(RectTransform));
            }

            return ts;

            void Get(Type type)
            {
                var t = transform.GetComponent(type);
                if (t != null)
                {
                    ts.Add(t.GetType().Name);
                }

                foreach (var item in transform.GetComponents<IGenerateCode>())
                {
                    ts.Add(item.GetType().Name);
                }
            }
        }

        private static string GetRelativePath(Transform child, Transform parent)
        {
            if (child == parent)
            {
                return parent.name;
            }

            var path = child.name;
            Transform nowParent = child.parent;
            while (nowParent != parent && nowParent != null)
            {
                path = nowParent.name + "/" + path;
                nowParent = nowParent.parent;
            }

            return path;
        }

        public static List<T> GetAllSelectComponent<T>() where T : Component
        {
            List<T> list = new List<T>();

#if UNITY_EDITOR
            foreach (var item in Selection.transforms)
            {
                foreach (var item_1 in FindAll(item))
                {
                    T t = item_1.GetComponent<T>();
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }
            }
#endif
            return list;
        }

        private static List<Transform> FindAll(Transform transform)
        {
            List<Transform> list = new List<Transform>();
            list.Add(transform);
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).childCount > 0)
                {
                    list.AddRange(FindAll(transform.GetChild(i)));
                }
                else
                {
                    list.Add(transform.GetChild(i));
                }
            }

            return list;
        }

        class TransformInfo
        {
            public readonly Transform transform;
            public readonly List<TransformInfo> children = new List<TransformInfo>();

            public TransformInfo(Transform transform)
            {
                this.transform = transform;
            }
        }

        private static void FindAll(TransformInfo transform)
        {
            for (int i = 0; i < transform.transform.childCount; i++)
            {
                if (transform.transform.GetChild(i).childCount > 0)
                {
                    var child = new TransformInfo(transform.transform.GetChild(i));
                    transform.children.Add(child);
                    FindAll(child);
                }
                else
                {
                    var child = new TransformInfo(transform.transform.GetChild(i));
                    transform.children.Add(child);
                }
            }
        }


        public partial class SpawnCodeConfig
        {
            public const string ComponentTag = "C_";
            public const string GeneralTag = "G_";
            public const string ChildTag = "D_";
            public const string VariantChildTag = "DV_";
            public const string VariantChildComponentTag = "CV_";

            public static List<string> AllTag = new List<string>()
            {
                ComponentTag,
                GeneralTag,
                ChildTag,
                VariantChildTag,
                VariantChildComponentTag,
            };

            [LabelText("会被检索到的组件类型")] public List<Type> SpawnType = new()
            {
                typeof(Button),
            };

            [LabelText("相斥组件")] public Dictionary<Type, Type> RemoveType = new()
            {
                { typeof(Button), typeof(Image) },
            };

            [LabelText("组件的默认命名空间")] public List<string> ComponentNameSpace = new()
            {
                "using UnityEngine;",
                "using System.Collections;",
                "using System.Collections.Generic;",
                "using UnityEngine.;",
                "using YuoTools.Main.Ecs;",
                "using Sirenix.OdinInspector;",
            };

            [LabelText("需要额外添加命名空间的组件")] public Dictionary<Type, string> ComponentAddNameSpace = new()
            {
                { typeof(TMP_InputField), "using TMPro;" },
                { typeof(TMP_Dropdown), "using TMPro;" },
                { typeof(EventTrigger), "using UnityEngine.EventSystems;" },
            };

            [LabelText("System的默认命名空间")] public List<string> SystemNameSpace = new()
            {
                "using YuoTools.Extend.Helper;",
                "using YuoTools.Main.Ecs;",
            };
        }
    }

    public interface IGenerateCode
    {
    }
}