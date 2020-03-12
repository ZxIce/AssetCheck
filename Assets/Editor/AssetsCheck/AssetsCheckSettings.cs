using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsCheckSettings : ScriptableObject
{
    public List<AssetFilter> filters = new List<AssetFilter>();
}

[System.Serializable]
public class AssetFilter
{
    public bool valid = true;
    public string path = string.Empty;
    public int CheckTag;
    public string CheckTagString;
}