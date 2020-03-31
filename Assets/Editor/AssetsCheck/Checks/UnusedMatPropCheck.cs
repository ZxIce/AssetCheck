using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UnusedMatPropCheck : ICheck
{
    private bool _canFix = true;
    public bool CanFix
    {
        get { return _canFix; }
        set { _canFix = value; }
    }

    private string _searchTag = "t:Material";
    public string SearchTag
    {
        get { return _searchTag; }
        set { _searchTag = value; }
    }
    [SaveTag("路径")]
    public string _assetPath = "";
    [SaveTag("去除的无用属性名称")]
    public string propName = "";
    public bool Check(string path)
    {
        _assetPath = path;
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (null == mat) return true;
        SerializedObject matInfo = new SerializedObject(mat);
        SerializedProperty propArr = matInfo.FindProperty("m_SavedProperties");
        SerializedProperty prop = null;
        propArr.Next(true);
        do
        {
            if (!propArr.isArray) continue;

            for (int i = propArr.arraySize - 1; i >= 0; --i)
            {
                prop = propArr.GetArrayElementAtIndex(i);
                if (!mat.HasProperty(prop.displayName))
                {
                    propName += "|"+prop.displayName;
                }
            }
        } while (propArr.Next(false));
        return false;
    }

    public bool Fix(string path)
    {
        _assetPath = path;
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (null == mat) return true;
        SerializedObject matInfo = new SerializedObject(mat);
        SerializedProperty propArr = matInfo.FindProperty("m_SavedProperties");
        SerializedProperty prop = null;
        propArr.Next(true);
        do
        {
            if (!propArr.isArray) continue;

            for (int i = propArr.arraySize - 1; i >= 0; --i)
            {
                prop = propArr.GetArrayElementAtIndex(i);
                if (!mat.HasProperty(prop.displayName))
                {
                    propArr.DeleteArrayElementAtIndex(i);
                }
            }
        } while (propArr.Next(false));
        matInfo.ApplyModifiedProperties();
        return true;
    }
}
