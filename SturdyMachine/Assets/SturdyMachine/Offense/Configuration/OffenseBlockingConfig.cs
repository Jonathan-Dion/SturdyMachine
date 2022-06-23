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

        int _offenseBlockingIndex = -1;

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

        public void OffenseBlockingSetup(Offense pCurrentOffense, List<OffenseBlocking> pOffenseBlocking) 
        {
            for (int i = 0; i < _offenseBlocking.Count; ++i) 
            {
                for (int j = 0; j < _offenseBlocking[i].GetOffenseBlockingData.Count; ++j) 
                {
                    if (_offenseBlocking[i].GetOffenseBlockingData[j].GetIsGoodOffenseBlocking(pCurrentOffense)) 
                    {
                        if (!pOffenseBlocking.Contains(_offenseBlocking[i]))
                            pOffenseBlocking.Add(_offenseBlocking[i]);
                    }
                }
            }
        }

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
                        EditorGUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.Label("Configuration", _guiStyle);

                        _offenseBlocking[i] = EditorGUILayout.ObjectField(_offenseBlocking[i], typeof(OffenseBlocking), true) as OffenseBlocking;

                        if (_offenseBlocking[i])
                        {
                            if (!_offenseBlocking[i].GetIsInitialzed)
                                _offenseBlocking[i].CustomOnEnable();

                            EditorGUILayout.Space();

                            _offenseBlocking[i].CustomOnInspectorGUI();

                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        }
                        else
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField($"{_currentExtendedFolderPath}/" + _currentFolderPath, _guiStyle);

                            _currentAssetPath = EditorGUILayout.TextField(_currentAssetPath);

                            if (_newAssetPath != $"{_currentExtendedFolderPath}" + $"{_currentFolderPath}/")
                                _newAssetPath = $"{_currentExtendedFolderPath}" + $"{_currentFolderPath}/";

                            //File creation
                            if (_currentAssetPath != "")
                            {
                                if (!System.IO.File.Exists(_newAssetPath + $"{_currentAssetPath}.asset"))
                                {
                                    if (GUILayout.Button("Create"))
                                    {
                                        CustomAssetsPath(out string pAssetsPath);

                                        _offenseBlocking[i] = CreateInstance<OffenseBlocking>();

                                        AssetDatabase.CreateAsset(_offenseBlocking[i], $"{pAssetsPath + _currentAssetPath}.asset");

                                        AssetDatabase.SaveAssets();

                                        _offenseBlocking[i].CustomOnEnable();

                                        EditorUtility.FocusProjectWindow();
                                    }
                                }
                            }
                        }

                        EditorGUILayout.EndVertical();

                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        public override void CustomOnEnable()
        {
            base.CustomOnEnable();

            //OffenseBlocking
            if (_offenseBlocking != null) 
            {
                for (int i = 0; i < _offenseBlocking.Count; ++i) 
                {
                    if (_offenseBlocking[i])
                        _offenseBlocking[i].CustomOnEnable();
                }
            }
        }

        void CustomAssetsPath(out string pAssetsPath) 
        {
            pAssetsPath = "";

            for (int i = 0; i < _newAssetPath.Length; ++i)
            {
                if (!pAssetsPath.Contains("Assets"))
                {
                    if (_newAssetPath[i] != '/')
                        pAssetsPath += _newAssetPath[i];
                    else
                        pAssetsPath = "";
                }
                else
                    pAssetsPath += _newAssetPath[i];
            }
        }

#endif
    }
}