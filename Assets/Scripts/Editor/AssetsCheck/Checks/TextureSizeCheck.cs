using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TextureSizeCheck : ICheck
{
    private string _searchTag = "t:texture2D";
    public string SearchTag
    {
        get { return _searchTag;}
        set { _searchTag = value; }
    }
    private bool _canFix = false;
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    public bool Check(string path)
    {
        TextureImporter textureImporter =  AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter == null)
        {
            return true;
        }
        object[] args = new object[2] { 0, 0 };
        MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        mi.Invoke(textureImporter, args);
        int width = (int)args[0];
        int height = (int)args[1];
        int max = width > height ? width : height;
        bool right = true;
        if (textureImporter.maxTextureSize<max)
        {
            Debug.LogError(string.Format("Texture size({3}) error:{0} Platform:{1} Size:{2}",path,"Default",textureImporter.maxTextureSize,max));
            right = false;
        }
        if (textureImporter.GetPlatformTextureSettings("Android").maxTextureSize<max)
        {
            Debug.LogError(string.Format("Texture size({3}) error:{0} Platform:{1} Size:{2}",path,"Android",textureImporter.GetPlatformTextureSettings("Android").maxTextureSize,max));
            right = false;
        }
        if (textureImporter.GetPlatformTextureSettings("iPhone").maxTextureSize<max)
        {
            Debug.LogError(string.Format("Texture size({3}) error:{0} Platform:{1} Size:{2}",path,"iPhone",textureImporter.GetPlatformTextureSettings("iPhone").maxTextureSize,max));
            right = false;
        }

        return right;
    }

    public bool Fix(string path)
    {
        return true;
    }
}
