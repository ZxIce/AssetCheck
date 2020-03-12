using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AudioCheck : ICheck
{
    private string _searchTag = "t:AudioClip";
    public string SearchTag
    {
        get { return _searchTag;}
        set { _searchTag = value; }
    }
    private string _assetPath = "";
    public string AssetPath
    {
        get { return _assetPath;}
        set { _assetPath = value; }
    }
    private bool _canFix = true;
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    public bool Check(string path)
    {
        _assetPath = path;
        AudioImporter audioImporter =  AssetImporter.GetAtPath(path) as AudioImporter;
        if (audioImporter == null)
        {
            return true;
        }
        
        bool right = true;
        
        if (!audioImporter.forceToMono)
        {
            right = false;
        }

        return right;
    }

    public bool Fix(string path)
    {
        AudioImporter audioImporter =  AssetImporter.GetAtPath(path) as AudioImporter;
        if (audioImporter == null)
        {
            return true;
        }

        audioImporter.forceToMono = true;
        audioImporter.SaveAndReimport();
        return true;
    }
}
