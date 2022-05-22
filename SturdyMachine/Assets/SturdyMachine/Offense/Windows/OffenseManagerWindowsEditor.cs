using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using SturdyMachine.Offense.Manager;

namespace SturdyMachine.OffenseWindows
{
    [ExecuteInEditMode]
    public class OffenseManagerWindowsEditor : EditorWindow
    {
        OffenseManager _offenseManager;

        Vector2 _scrollPosition;

        string _currentAssetsPath;

        [MenuItem("Window/OffenseManagerEditor")]
        static void Initialize() 
        {
            EditorWindow windows = GetWindow(typeof(OffenseManagerWindowsEditor), false, "OffenseManagerEditor") as OffenseManagerWindowsEditor;

            windows.Show();
        }

        void OnGUI()
        {
            _offenseManager = EditorGUILayout.ObjectField(_offenseManager, typeof(OffenseManager), true) as OffenseManager;

            if (_offenseManager) 
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                _offenseManager.CustomOnInspectorGUI();

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Assets/");

                EditorGUILayout.EndHorizontal();

                _currentAssetsPath = EditorGUILayout.TextField(_currentAssetsPath);

                if (_currentAssetsPath != "")
                {
                    if (GUILayout.Button("Create"))
                    {
                        _offenseManager = CreateInstance<OffenseManager>();

                        AssetDatabase.CreateAsset(_offenseManager, $"Assets/{_currentAssetsPath}.asset");
                        AssetDatabase.SaveAssets();

                        _offenseManager.CustomOnEnable();

                        EditorUtility.FocusProjectWindow();
                    }
                }
            }
        }

        void OnEnable()
        {
            if (_offenseManager)
                _offenseManager.CustomOnEnable();
        }

        void OnDisable()
        {
            if (_offenseManager)
                _offenseManager.CustomOnDisable();
        }

    }
}