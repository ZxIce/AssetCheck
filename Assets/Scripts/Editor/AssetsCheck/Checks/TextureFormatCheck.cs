using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class TextureFormatCheck : ICheck
{
    private string _searchTag = "t:texture2D";
    public string SearchTag
    {
        get { return _searchTag;}
        set { _searchTag = value; }
    }
    private bool _canFix = true;
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
        if (textureImporter.GetPlatformTextureSettings("iPhone")==null)
        {
            return false;
        }
        if (textureImporter.GetPlatformTextureSettings("iPhone").format == TextureImporterFormat.ASTC_RGB_6x6 || textureImporter.GetPlatformTextureSettings("Android").format == TextureImporterFormat.ASTC_RGBA_6x6)
        {
            return true;
        }
        if (textureImporter.GetPlatformTextureSettings("Android")==null)
        {
            return false;
        }
        if (textureImporter.GetPlatformTextureSettings("Android").format == TextureImporterFormat.ASTC_RGB_6x6 || textureImporter.GetPlatformTextureSettings("Android").format == TextureImporterFormat.ASTC_RGBA_6x6)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool Fix(string path)
    {
        TextureImporter textureImporter =  AssetImporter.GetAtPath(path) as TextureImporter;
        TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings();
        textureImporterPlatformSettings.overridden = true;
        textureImporterPlatformSettings.name = "Android";
        if (textureImporter.DoesSourceTextureHaveAlpha())
        {
            textureImporterPlatformSettings.format = TextureImporterFormat.ASTC_RGBA_6x6;
        }
        else
        {
            textureImporterPlatformSettings.format = TextureImporterFormat.ASTC_RGB_6x6;
        }
        textureImporter.SetPlatformTextureSettings(textureImporterPlatformSettings);
        textureImporterPlatformSettings = new TextureImporterPlatformSettings();
        textureImporterPlatformSettings.overridden = true;
        textureImporterPlatformSettings.name = "iPhone";
        if (textureImporter.DoesSourceTextureHaveAlpha())
        {
            textureImporterPlatformSettings.format = TextureImporterFormat.ASTC_RGBA_6x6;
        }
        else
        {
            textureImporterPlatformSettings.format = TextureImporterFormat.ASTC_RGB_6x6;
        }
        textureImporter.SetPlatformTextureSettings(textureImporterPlatformSettings);
        textureImporter.SaveAndReimport();
        return true;
    }
}
