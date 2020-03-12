using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.TreeViewExamples;
public class AssetTreeElement :TreeElement
{
    public string path = "";

    public AssetTreeElement(string name, int depth, int id) : base(name, depth, id)
    {
        
    }
}
