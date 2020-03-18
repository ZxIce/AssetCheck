using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class MeshCheck : ICheck
{
    private string _searchTag = "t:Mesh";
    public string SearchTag
    {
        get { return _searchTag;}
        set { _searchTag = value; }
    }
    [SaveTag("路径")]
    public string _assetPath = "";
    [SaveTag("三角形数量")]
    public int trianglesCount = 0;
    private bool _canFix = false;
   
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    public bool Check(string path)
    {
        _assetPath = path;
        Mesh mesh =  AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (mesh == null)
        {
            return true;
        }

        trianglesCount = mesh.triangles.Length/3;
       
        return false;
    }

    public bool Fix(string path)
    {
        return true;
    }
    
}
