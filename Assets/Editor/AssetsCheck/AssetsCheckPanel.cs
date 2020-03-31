using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class AssetsCheckPanel : EditorWindow
{
    [MenuItem("AssetsCheck/AssetsCheckPanel")]
    static void Open()
    {
        GetWindow<AssetsCheckPanel>("AssetsCheck", true);
    }

    private AssetsCheckSettings _settings;
    private SerializedObject _serializedObject;
    private string savePath = "Assets/Editor/AssetsCheck/settings.asset";
    private string[] taglist = new[] {"a", "b"};
    void InitSettings()
    {
        LoadSettingsPath();
        _settings = AssetDatabase.LoadAssetAtPath<AssetsCheckSettings>(savePath);
        if (_settings == null)
        {
            _settings = CreateInstance<AssetsCheckSettings>();
        }

        taglist = CheckManager.initCheckList(_settings).ToArray();
        CheckTag();
        _serializedObject = new SerializedObject(_settings);
        SerializedProperty prop = _serializedObject.FindProperty("filters");
        reorderableList = new ReorderableList(_serializedObject, prop, true, true, true, true);
        reorderableList.elementHeight = 22;
        reorderableList.drawElementCallback = OnListElementGUI;
    }

    void RefreshSettings()
    {
        savePath = AssetDatabase.GetAssetPath(_settings);
        taglist = CheckManager.initCheckList(_settings).ToArray();
        CheckTag();
        _serializedObject = new SerializedObject(_settings);
        SerializedProperty prop = _serializedObject.FindProperty("filters");
        reorderableList = new ReorderableList(_serializedObject, prop, true, true, true, true);
        reorderableList.elementHeight = 22;
        reorderableList.drawElementCallback = OnListElementGUI;
    }

    private ReorderableList reorderableList;
    void OnGUI()
    {
        if (_serializedObject == null)
        {
            InitSettings();
        }
        
        _serializedObject.Update();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Check", EditorStyles.toolbarButton))
        {
            CheckManager.StartChecks(_settings);
        }
        if (GUILayout.Button("Check and Save", EditorStyles.toolbarButton))
        {
            CheckManager.StartChecksAndSaveData(_settings);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        _settings = EditorGUILayout.ObjectField( "settings",_settings, typeof(AssetsCheckSettings),true) as AssetsCheckSettings;
        if (EditorGUI.EndChangeCheck())
        {
            if (_settings !=null)
            {
                RefreshSettings();
            }
        }
        
        if (GUILayout.Button("New",GUILayout.Width(50)))
        {
            CreateNewSettings();
        }
        EditorGUILayout.EndHorizontal();
        if (_settings !=null)
        {
            reorderableList.DoLayoutList();
            _serializedObject.ApplyModifiedProperties();
        }
    }

    void LoadSettingsPath()
    {
        string path = EditorPrefs.GetString("SettingsPath");
        if (string.IsNullOrEmpty(path))
        {
            EditorPrefs.SetString("SettingsPath",savePath);
        }
        else
        {
            savePath = path;
        }
    }

    void SaveSettingsPath()
    {
        EditorPrefs.SetString("SettingsPath",savePath);
    }

    private void OnEnable()
    {
        
    }
    
    private void OnValidate()
    {
        if (_settings == null)
        {
            InitSettings();
        }
    }

    void CreateNewSettings()
    {
        if (_settings)
        {
            EditorUtility.SetDirty(_settings);
        }
        savePath = savesettings();
        if (!string.IsNullOrEmpty(savePath))
        {
            _settings = CreateInstance<AssetsCheckSettings>();
            AssetDatabase.CreateAsset(_settings, savePath);
            RefreshSettings();
        }
    }

    private void OnDestroy()
    {
        if (AssetDatabase.LoadAssetAtPath<AssetsCheckSettings>(savePath) == null)
        {
            AssetDatabase.CreateAsset(_settings, savePath);
        }
        else
        {
            EditorUtility.SetDirty(_settings);
        }

        SaveSettingsPath();
    }
    void OnListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;

        AssetFilter filter = _settings.filters[index];
        rect.y++;

        Rect r = rect;
        r.width = 16;
        r.height = 18;
        filter.valid = GUI.Toggle(r, filter.valid, GUIContent.none);

        r.xMin = r.xMax + GAP;
        r.xMax = rect.xMax - 300;
        GUI.enabled = false;
        filter.path = GUI.TextField(r, filter.path);
        GUI.enabled = true;

        r.xMin = r.xMax + GAP;
        r.width = 50;
        if (GUI.Button(r, "Select"))
        {
            var path = SelectFolder();
            if (path != null)
                filter.path = path;
        }
        
        r.xMin = r.xMax + GAP;
        r.xMax = rect.xMax;
        filter.CheckTag = EditorGUI.Popup(r,filter.CheckTag,taglist);
        filter.CheckTagString = taglist[filter.CheckTag];
    }

    void CheckTag()
    {
        foreach (var filter in _settings.filters)
        {
            int index = 0;
            foreach (var tag in taglist)
            {
                if (tag == filter.CheckTagString)
                {
                    filter.CheckTag = index;
                    break;
                }
                index++;
            }
        }
    }

    string SelectFolder()
    {
        string dataPath = Application.dataPath;
        string selectedPath = EditorUtility.OpenFolderPanel("Path", dataPath, "");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            if (selectedPath.StartsWith(dataPath))
            {
                return "Assets/" + selectedPath.Substring(dataPath.Length + 1);
            }
            else
            {
                ShowNotification(new GUIContent("不能在Assets目录之外!"));
            }
        }
        return null;
    }
    string savesettings()
    {
        string dataPath = Application.dataPath;
        string savePath = EditorUtility.SaveFilePanel("Save Settings", dataPath, "defaultsettings.asset","asset");
        if (!string.IsNullOrEmpty(savePath))
        {
            if (savePath.StartsWith(dataPath))
            {
                return "Assets/" + savePath.Substring(dataPath.Length + 1);
            }
            else
            {
                ShowNotification(new GUIContent("不能在Assets目录之外!"));
            }
        }
        return null;
    }
    
}
