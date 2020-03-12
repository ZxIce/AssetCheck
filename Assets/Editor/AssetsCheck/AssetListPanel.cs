using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor.TreeViewExamples;
public class AssetListPanel : EditorWindow
{
    [MenuItem("AssetsCheck/AssetsListPanel")]
    static void Open()
    {
        GetWindow<AssetListPanel>("AssetsCheck", true);
    }
}
