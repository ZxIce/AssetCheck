using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ParticleSystemCheck : ICheck
{
    private string _searchTag = "t:Prefab";
    public string SearchTag
    {
        get { return _searchTag;}
        set { _searchTag = value; }
    }
    [SaveTag("路径")]
    public string _assetPath = "";
    
    private bool _canFix = true;
    public bool CanFix
    {
        get { return _canFix;}
        set { _canFix = value; }
    }
    private List<string> goPathList = new List<string>();
    private string goPath = "";
    
    public bool Check(string path)
    {
        _assetPath = path;
        GameObject go =  AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null)
        {
            return true;
        }
        
        bool right = true;
        ParticleSystem[] particleSystems = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var particleSystem in particleSystems)
        {

            if (!particleSystem.trails.enabled)
            { 
                if (particleSystem.GetComponent<ParticleSystemRenderer>())
                {
                    if (particleSystem.GetComponent<ParticleSystemRenderer>().trailMaterial!=null)
                    {
                        right = false;
                    }
                }
            }
        }

        return right;
    }

    public bool Fix(string path)
    {
        _assetPath = path;
        GameObject go =  AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null)
        {
            return true;
        }
        
        ParticleSystem[] particleSystems = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var particleSystem in particleSystems)
        {

            if (!particleSystem.trails.enabled)
            {
                if (particleSystem.GetComponent<ParticleSystemRenderer>())
                {
                    particleSystem.GetComponent<ParticleSystemRenderer>().trailMaterial = null;
                }
            }
        }

        PrefabUtility.SavePrefabAsset(go);
        return true;
    }

    public void allPath(Transform tran)
    {
        if (tran == null)
        {
            foreach (var name in goPathList)
            {
                goPath = string.Format("{0}/{1}",name,goPath) ;
            }
            return;
        }
        goPathList.Add(tran.name);
        allPath(tran.parent);
    }
}
