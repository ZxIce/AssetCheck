using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;

public class MissAssetCheck : ICheck
{
    private string _searchTag = "t:Object";
    public string SearchTag
    {
        get { return _searchTag;}
        set { _searchTag = value; }
    }
    [SaveTag("路径")]
    public string _assetPath = "";
    private bool _canFix = false;
    [SaveTag("Miss组件")]
    public string missName = "";
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    public bool Check(string path)
    {
        _assetPath = path;
        Object assetObject = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (assetObject == null)
        {
            return true;
        }

        bool right = true;
        if (assetObject is GameObject)
        {
            var components = ((GameObject)assetObject).GetComponentsInChildren<Component>();
            foreach (var c in components)
            {
                if (!c)
                {
                    Debug.LogError(_assetPath);
                    continue;
                }

                SerializedObject so = new SerializedObject(c);
                var sp = so.GetIterator();
                while(sp.NextVisible(true))
                {
                
                    if(sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if(sp.objectReferenceValue == null  && sp.objectReferenceInstanceIDValue != 0)
                        {
                            missName = sp.propertyPath;
                            Debug.LogError(missName);
                        }
                    }
// #if UNITY_2018_3_OR_NEWER
//                 var fileId = sp.FindPropertyRelative("m_FileID");
//                 if (fileId != null)
//                 {
//                     if (fileId.intValue != 0)
//                     {
//                         missName = sp.propertyPath;
//                         Debug.LogError(missName);
//                     }
//                 }
//                 else
//                 {
//                 }
// #endif
                }
            }
        }
        else
        {
            SerializedObject so = new SerializedObject(assetObject);
            var sp = so.GetIterator();
            while(sp.NextVisible(true))
            {
                
                if(sp.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if(sp.objectReferenceValue == null  && sp.objectReferenceInstanceIDValue != 0)
                    {
                        missName = sp.propertyPath;
                        Debug.LogError(missName);
                    }
                }
#if UNITY_2018_3_OR_NEWER
                var fileId = sp.FindPropertyRelative("m_FileID");
                if (fileId != null)
                {
                    if (fileId.intValue != 0)
                    {
                        missName = sp.propertyPath;
                        Debug.LogError(missName);
                    }
                }
                else
                {
                }
#endif
            }
        }


        

        return right;
    }

    public bool Fix(string path)
    {
        return true;
    }
    
    
}
