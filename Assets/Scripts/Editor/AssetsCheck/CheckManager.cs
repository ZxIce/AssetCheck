using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class CheckManager
{
    private static Dictionary<string, ICheck> mDictionaryCheck = new Dictionary<string, ICheck>();
    
    public static List<string> initCheckList(AssetsCheckSettings _settings)
    {
        mDictionaryCheck = new Dictionary<string, ICheck>();
        
        Type[] types = typeof(ICheck).Assembly.GetTypes();
        List<string> typeNameList = new List<string>();
        foreach (var item in types)
        { if (item.GetInterface("ICheck") != null)
            {
                typeNameList.Add(item.Name);
                mDictionaryCheck.Add(item.Name,(ICheck)Activator.CreateInstance(item));
            }
        }
        
        return typeNameList;
    }

    public static void StartChecks(AssetsCheckSettings _settings)
    {
        foreach (var filter in _settings.filters)
        {
            if (filter.valid)
            {
                ItemCheck(filter.path, mDictionaryCheck[filter.CheckTagString]);
            }
        }
    }

    public static void ItemCheck(string path,ICheck _check)
    {
        string[] guids = AssetDatabase.FindAssets(_check.SearchTag,new string[]{path});
        float count = guids.Length;
        float index = 1;
        List<string> fixList = new List<string>();
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string info = String.Format("{0}/{1} {2}",index,count,assetPath);
            EditorUtility.DisplayProgressBar(_check.GetType().Name + " Check", info, (float)(index / count));
           
            if (!string.IsNullOrEmpty(assetPath))
            {
                if (!_check.Check(AssetDatabase.GUIDToAssetPath(guid)))
                {
                    fixList.Add(assetPath);
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
        }
        if (_check.CanFix)
        {
            index = 1;
            count = fixList.Count;
            foreach (var assetPath in fixList)
            {
                string info = String.Format("{0}/{1} {2}",index,count,assetPath);
                EditorUtility.DisplayProgressBar(_check.GetType().Name + " Fix", info, (float)(index / count));
                if (_check.Fix(assetPath))
                {
                    Debug.Log(info+ " Fixed");  
                }
                else
                {
                    Debug.LogError(info+ "auto fix fail");
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }

}
