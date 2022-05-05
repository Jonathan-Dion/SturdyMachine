using System;
using System.Collections.Generic;

using UnityEngine;

using ICustomEditor.ScriptableObjectEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SturdyMachine.Offense.Blocking.Manager
{
    public class OffenseBlockingConfig : ScriptableObjectICustomEditor 
    {
        static OffenseBlockingConfig _instance;

        [SerializeField]
        List<OffenseBlocking> _offenseBlocking;

        string _currentAssetPath;

        //Instance
        public static OffenseBlockingConfig GetInstance 
        {
            get
            {
                if (!_instance)
                    _instance = Resources.Load("Configuration/OffenseBlockingConfig") as OffenseBlockingConfig;

                return _instance;
            }
        }

        public List<OffenseBlocking> GetOffenseBlocking => _offenseBlocking;

#if UNITY_EDITOR

        public virtual void CustomOnInspectorGUI(bool pIsSearchFolder) 
        {
            #region Information

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Informations", _guiStyle);

            EditorGUILayout.Space();

            //Add
            if (GUILayout.Button("+"))
            {
                if (_offenseBlocking == null)
                    _offenseBlocking = new List<OffenseBlocking>();

                _offenseBlocking.Add(null);
            }

            //Remove
            if (GUILayout.Button("-"))
            {
                if (_offenseBlocking != null)
                {
                    if (_offenseBlocking.Count > 0)
                        _offenseBlocking.RemoveAt(_offenseBlocking.Count - 1);
                }
            }

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.Space();

            if (!pIsSearchFolder)
            {
                #region Configuration

                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Label("Configuration", _guiStyle);

                EditorGUILayout.Space();

                EditorGUIUtility.labelWidth = 100f;

                ++EditorGUI.indentLevel;

                if (_offenseBlocking != null)
                {
                    for (int i = 0; i < _offenseBlocking.Count; ++i)
                    {
                        EditorGUILayout.ObjectField(_offenseBlocking[i], typeof(OffenseBlocking), true);

                        if (_offenseBlocking[i])
                        {
                            #region Configuration

                            EditorGUILayout.BeginVertical(GUI.skin.box);

                            GUILayout.Label("Configuration", _guiStyle);

                            EditorGUILayout.Space();

                            EditorGUILayout.EndVertical();

                            _offenseBlocking[i].CustomOnInspectorGUI();

                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                            #endregion
                        }
                        else
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField($"{_currentExtendedFolderPath}/" + _currentFolderPath, _guiStyle);

                            _currentAssetPath = EditorGUILayout.TextField(_currentAssetPath);

                            //File creation
                            if (_currentAssetPath != "")
                            {
                                if (!System.IO.Directory.Exists(_currentExtendedFolderPath + "/" + _currentExtendedFolderPath + "/" + _currentFolderPath + "/" + _currentAssetPath))
                                {
                                    if (GUILayout.Button("Create"))
                                    {
                                        _offenseBlocking[i] = CreateInstance<OffenseBlocking>();

                                        AssetDatabase.CreateAsset(_offenseBlocking[i], _currentExtendedFolderPath + "/" + _currentFolderPath + "/" + _currentAssetPath);
                                        AssetDatabase.SaveAssets();

                                        _offenseBlocking[i].CustomOnEnable();

                                        EditorUtility.FocusProjectWindow();
                                    }
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.EndVertical();

                #endregion
            }
        }

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();
        }

#endif
    }
}