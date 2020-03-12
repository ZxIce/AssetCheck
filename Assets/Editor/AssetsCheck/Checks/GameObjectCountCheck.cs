using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class GameObjectCountCheck : ICheck
{
    private string _searchTag = "t:Prefab";
    public string SearchTag
    {
        get { return _searchTag;}
        set { _searchTag = value; }
    }
    [SaveTag("路径")]
    public string _assetPath = "";
    private bool _canFix = false;
    [SaveTag("Gameobject数量")]
    public int gameobjectCount = 0;
    [SaveTag("粒子播放器数量")]
    public int psCount = 0;
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    public bool Check(string path)
    {
        _assetPath = path;
        GameObject go =  AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null)
        {
            return true;
        }
        Transform[] transforms = go.GetComponentsInChildren<Transform>(true);
        gameobjectCount = transforms.Length;
        ParticleSystem[] particleSystems = go.GetComponentsInChildren<ParticleSystem>(true);
        psCount = particleSystems.Length;
        return false;
    }

    public bool Fix(string path)
    {
        return true;
    }
}
