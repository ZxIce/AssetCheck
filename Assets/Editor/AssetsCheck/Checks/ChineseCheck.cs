using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ChineseCheck : ICheck
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
    [SaveTag("中文节点名")]
    public string chineseName = "";
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

        if (ContainChinese(go.name))
        {
            chineseName = go.name;
            return false;
        }

        if (go.GetType() ==typeof(GameObject))
        {
            Transform[] transforms = ((GameObject)go).GetComponentsInChildren<Transform>(true);
            foreach (var transform in transforms)
            {
                if (ContainChinese(transform.name))
                {
                    chineseName = transform.name;
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
    
    private bool ContainChinese(string input)
    {
        string pattern = "[\u4e00-\u9fbb]";
        return Regex.IsMatch(input, pattern);
    }
}
