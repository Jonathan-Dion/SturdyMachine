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

        string _currentAssetPath, _newAssetPath;

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

            EditorGUILayout.BeginVertical();

            GUILayout.Label("Informations", _guiStyle);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            //Add
            if (GUILayout.Button("+"))
            {
                if (_offenseBlocking == null)
                    _offenseBlocking = new List<OffenseBlocking>();

                _offenseBlocking.Add(new OffenseBlocking());
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

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.Space();

            if (!pIsSearchFolder)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                if (_offenseBlocking != null)
                {
                    for (int i = 0; i < _offenseBlocking.Count; ++i)
                    {
                        _offenseBlocking[i] = EditorGUILayout.ObjectField(_offenseBlocking[i], typeof(OffenseBlocking), true) as OffenseBlocking;

                        if (_offenseBlocking[i])
                        {
                            #region Configuration

                            EditorGUILayout.BeginVertical(GUI.skin.box);

                            GUILayout.Label("Configuration", _guiStyle);

                            EditorGUILayout.Space();

                            EditorGUILayout.EndVertical();

                            _offenseBlocking[i].CustomOnInspectorGUI($"Assets/{_currentAssetPath}.asset", _newAssetPath + $"{_currentAssetPath}.asset");

                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                            #endregion
                        }
                        else
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField($"{_currentExtendedFolderPath}/" + _currentFolderPath, _guiStyle);

                            _currentAssetPath = EditorGUILayout.TextField(_currentAssetPath);

                            if (_newAssetPath != $"{_currentExtendedFolderPath}/" + $"{_currentFolderPath}/")
                                _newAssetPath = $"{_currentExtendedFolderPath}/" + $"{_currentFolderPath}/";

                            //File creation
                            if (_currentAssetPath != "")
                            {
                                if (!System.IO.File.Exists(_newAssetPath + $"{_currentAssetPath}.asset"))
                                {
                                    if (GUILayout.Button("Create"))
                                    {
                                        _offenseBlocking[i] = CreateInstance<OffenseBlocking>();

                                        AssetDatabase.CreateAsset(_offenseBlocking[i], $"Assets/{_currentAssetPath}.asset");

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
            }
        }

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();
        }

#endif
    }
}