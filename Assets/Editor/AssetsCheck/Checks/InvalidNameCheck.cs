using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class InvalidNameCheck : ICheck
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
    [SaveTag("非法节点名")]
    public string InValidName = "";
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    public bool Check(string path)
    {
        _assetPath = path;
        Object go =  AssetDatabase.LoadAssetAtPath<Object>(path);
        if (go == null)
        {
            return true;
        }

        if (IsInValidName(go.name))
        {
            InValidName = go.name;
            return false;
        }

        if (go.GetType() ==typeof(GameObject))
        {
            Transform[] transforms = ((GameObject)go).GetComponentsInChildren<Transform>(true);
            foreach (var transform in transforms)
            {
                if (IsInValidName(transform.name))
                {
                    InValidName = transform.name;
                    return false;
                }
            }
        }
       
        return true;
    }

    public bool Fix(string path)
    {
        return true;
    }
    
    public bool IsInValidName(string fileName)
    {
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
        {
            if (fileName.IndexOf(c)>-1)
            {
                return true;
            }
        }

        if (fileName.IndexOf(' ')>-1)
        {
            return true;
        }
        return false;
    }
}
