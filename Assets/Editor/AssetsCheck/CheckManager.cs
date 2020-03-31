using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class CheckManager
{
    private static Dictionary<string, List<ICheck>> mDictionaryCheck = new Dictionary<string, List<ICheck>>();
    
    public static List<string> initCheckList(AssetsCheckSettings _settings)
    {
        
        mDictionaryCheck = new Dictionary<string, List<ICheck>>();
        Type[] types = typeof(ICheck).Assembly.GetTypes();
        List<string> typeNameList = new List<string>();
        foreach (var item in types)
        { if (item.GetInterface("ICheck") != null)
            {
                typeNameList.Add(item.Name);
                mDictionaryCheck.Add(item.Name,new List<ICheck>());
                //mDictionaryCheck.Add(item,(ICheck)Activator.CreateInstance(item));
            }
        }
        
        return typeNameList;
    }

    public static void StartChecks(AssetsCheckSettings _settings)
    {
        foreach (var check in mDictionaryCheck)
        {
            check.Value.Clear();
        }
        foreach (var filter in _settings.filters)
        {
            if (filter.valid)
            {
                
                ItemCheck(filter);
            }
        }
    }
    public static void StartChecksAndSaveData(AssetsCheckSettings _settings)
    {
        StartChecks(_settings);
        saveToExcel(Application.dataPath+"/Editor/AssetsCheck/AssetsCheck.xlsx",_settings);
    }
    public static void ItemCheck(AssetFilter filter)
    {
        ICheck checkTypd = (ICheck) Activator.CreateInstance(Type.GetType(filter.CheckTagString));
        string[] guids = AssetDatabase.FindAssets(checkTypd.SearchTag,new string[]{filter.path});
        float count = guids.Length;
        float index = 1;
        List<string> fixList = new List<string>();
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string info = String.Format("{0}/{1} {2}",index,count,assetPath);
            ICheck _check = (ICheck) Activator.CreateInstance(Type.GetType(filter.CheckTagString));
            EditorUtility.DisplayProgressBar(_check.GetType().Name + " Check", info, (float)(index / count));
            
            if (!string.IsNullOrEmpty(assetPath))
            {
                if (!_check.Check(AssetDatabase.GUIDToAssetPath(guid)))
                {
                    fixList.Add(assetPath);
                    mDictionaryCheck[filter.CheckTagString].Add(_check);
                    if (!_check.CanFix)
                    {
                        Debug.LogError(info+ " can not auto fix");
                    }
                }
            }
            else
            {
                Debug.LogError(info+ " error" + _check.GetType().Name);
            }

            index++;
        }
        List<ICheck> list = mDictionaryCheck[filter.CheckTagString];
        if (checkTypd.CanFix)
        {
            index = 1;
            count = fixList.Count;
            foreach (var assetPath in fixList)
            {
                string info = String.Format("{0}/{1} {2}",index,count,assetPath);
                EditorUtility.DisplayProgressBar(checkTypd.GetType().Name + " Fix", info, (float)(index / count));
                if (checkTypd.Fix(assetPath))
                {
                    Debug.Log(info+ " Fixed");  
                }
                else
                {
                    Debug.LogError(info+ "auto fix fail");
                }
                index++;
            }
        }
        EditorUtility.ClearProgressBar();
    }
    static void saveToExcel(string path,AssetsCheckSettings _settings)
    {
        File.Delete(path);
        FileInfo file = new FileInfo(path);
        using (ExcelPackage myExcelPackage = new ExcelPackage(file))
        {
            int filterIndex = 1;
            foreach (var filter in _settings.filters)
            {
                if (!filter.valid)
                {
                    filterIndex++;
                    continue;
                }

                ExcelWorksheet worksheet = null;
                worksheet = myExcelPackage.Workbook.Worksheets.Add(filter.CheckTagString+filterIndex);
                if (mDictionaryCheck.ContainsKey(filter.CheckTagString))
                {
                    Type type = Type.GetType(filter.CheckTagString);
                    FieldInfo[] fieldInfos = type.GetFields();
                    int index = 0;
                    foreach (var info in fieldInfos)
                    {
                        object[] attrs = info.GetCustomAttributes(typeof(SaveTag),true);
                        if (attrs.Length > 0)
                        {
                            foreach (Attribute attr in attrs)
                            {
                                if (attr is SaveTag)
                                {
                                    SaveTag a = (SaveTag)attr;
                                    Debug.Log(a.Title);
                                    worksheet.Cells[1, 1+index].Value = a.Title;
                                    int index1 = 1;
                                    
                                    foreach (var check in mDictionaryCheck[filter.CheckTagString])
                                    {
                                        worksheet.Cells[1 + index1, 1 + index].Value = info.GetValue(check);
                                        index1++;
                                    }
                                    index++;
                                }
                            }
                        }
                    }
                    
                   
                }

                filterIndex++;
            }
            // //创建ExcelWorkSheet对象，这个对象就是面对表的，是工作簿中单个表
            //
            // //坐标1，1赋值A1就是相当于在Excel中的A1位置赋值了一个A1字符串。
            // worksheet.Cells[1, 1].Value = "A1";
            // worksheet.Cells[1, 2].Value = "B1";
            // worksheet.Cells[1, 3].Value = "C1";
            // //save方法就保存我们这个对象，他就会去执行我们刚刚赋值的那些东西
            myExcelPackage.Save();
        }
    }
}
