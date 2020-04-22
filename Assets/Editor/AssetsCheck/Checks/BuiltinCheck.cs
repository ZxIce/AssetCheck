using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

public class BuiltinCheck : ICheck
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
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    
    [SaveTag("资源名")]
    public string assetName = "";
    public bool Check(string path)
    {
        _assetPath = path;
        bool right = true;
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(_assetPath);
        Object[] dependencies = EditorUtility.CollectDependencies(new []{obj});
        foreach (var dependencie in dependencies)
        {
            string assetPath = AssetDatabase.GetAssetPath(dependencie);
            if (assetPath.IndexOf("Resources/unity_builtin_extra", StringComparison.Ordinal)>-1)
            {
                assetName = assetName + "\n" + dependencie.name;
                right = false;
            }
            
        }
        return right;
    }

    public bool Fix(string path)
    {
        return true;
    }
}
