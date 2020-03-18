using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ShaderCheck : ICheck
{
    private string _searchTag = "t:Material";
    public string SearchTag
    {
        get { return _searchTag;}
        set { _searchTag = value; }
    }
    [SaveTag("路径")]
    public string _assetPath = "";
    [SaveTag("shaderName")]
    public string shaderName = "";
    private bool _canFix = false;
    //shader白名单
    private string[] shaders = new[]
    {
        ""
    };
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    public bool Check(string path)
    {
        _assetPath = path;
        Material material =  AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            return true;
        }

        shaderName = material.shader.name;
        bool right = false;

        foreach (var shader in shaders)
        {
            if (shaderName.IndexOf(shader)>-1)
            {
                right = true;
                break;
            }
        }
       
        return right;
    }

    public bool Fix(string path)
    {
        return true;
    }
    
}
