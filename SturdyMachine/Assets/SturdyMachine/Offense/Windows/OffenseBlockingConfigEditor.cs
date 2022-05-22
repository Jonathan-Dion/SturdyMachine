using System.IO;

using UnityEngine;
using UnityEditor;

using SturdyMachine.Offense.Blocking.Manager;

namespace SturdyMachine.OffenseWindows
{
    [ExecuteInEditMode]
    public class OffenseBlockingEditor : EditorWindow 
    {
        OffenseBlockingConfig _offenseBlockingConfig;

        Vector2 _scrollPosition;

        string _currentDataPath, _currentAssetPath;

        bool _isSearchFolderPath;

        [MenuItem("Window/OffenseBlockingConfigEditor")]
        static void Initialize() 
        {
            EditorWindow windows = GetWindow(typeof(OffenseBlockingEditor), false, "OffenseBlockingEditor") as OffenseBlockingEditor;

            windows.Show();
        }

        void OnGUI()
        {
            _offenseBlockingConfig = EditorGUILayout.ObjectField(_offenseBlockingConfig, typeof(OffenseBlockingConfig), true) as OffenseBlockingConfig;

            if (_offenseBlockingConfig)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                EditorGUILayout.LabelField("Search folder path: ");

                _isSearchFolderPath = EditorGUILayout.Toggle(_isSearchFolderPath);

                EditorGUILayout.EndHorizontal();

                if (_isSearchFolderPath)
                    _offenseBlockingConfig.ShowExplorerCreator();

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);

                _offenseBlockingConfig.CustomOnInspectorGUI(_isSearchFolderPath);

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(GUI.skin.box);

                //Search Directory
                if (GUILayout.Button("Search folder path"))
                    _currentDataPath = EditorUtility.OpenFolderPanel("Open File Explorer", "", "");

                EditorGUILayout.LabelField(_currentDataPath);

                _currentAssetPath = EditorGUILayout.TextField(_currentAssetPath);

                //File creation
                if (!Directory.Exists($"{_currentAssetPath}/" + _currentAssetPath))
                {
                    //Assets
                    if (GUILayout.Button("Create"))
                    {
                        _offenseBlockingConfig = CreateInstance<OffenseBlockingConfig>();

                        AssetDatabase.CreateAsset(_offenseBlockingConfig, $"Assets/Resources/Configuration/{_currentAssetPath}.asset");
                        AssetDatabase.SaveAssets();

                        _offenseBlockingConfig.CustomOnEnable();
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        void OnEnable()
        {
            if (_offenseBlockingConfig)
                _offenseBlockingConfig.CustomOnEnable();

            if (_currentDataPath != Application.dataPath)
                _currentDataPath = Application.dataPath;
        }

        void OnDisable()
        {
            if (_offenseBlockingConfig)
                _offenseBlockingConfig.CustomOnDisable();
        }
    }
}