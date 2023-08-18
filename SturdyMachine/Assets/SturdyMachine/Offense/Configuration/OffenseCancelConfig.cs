using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum OffenseDirection { DEFAULT, NEUTRAL, RIGHT, LEFT, STANCE}
public enum OffenseType { DEFAULT, DEFLECTION, EVASION, SWEEP, STRIKE, HEAVY, DEATHBLOW, DAMAGEHIT, REPEL};
public enum OffenseBlockType { DEFAULT, PASSIVE, AGGRESSIVE };

class OffenceCancel
{
    OffenseCancel _offenceCancel;

    public OffenseCancel GetOffenseCancel => _offenceCancel;

    /// <summary>
    /// Default constructor
    /// </summary>
    public OffenceCancel() 
    {
        _offenceCancel = new OffenseCancel();

        //StandardCancel
        _offenceCancel.standardCancelDirection = new List<OffenseDirection>();
        _offenceCancel.standardCancelType = new List<OffenseType>();

        //Special
        _offenceCancel.specialCancelDirection = new List<OffenseDirection>();
        _offenceCancel.specialCancelType = new List<OffenseType>();
    }

    public void Add() 
    {
        //Standard
        _offenceCancel.standardCancelDirection.Add(OffenseDirection.DEFAULT);
        _offenceCancel.standardCancelType.Add(OffenseType.DEFAULT);

        //Special
        _offenceCancel.specialCancelDirection.Add(OffenseDirection.DEFAULT);
        _offenceCancel.specialCancelType.Add(OffenseType.DEFAULT);
    }

    public void Remove(int pIndex) 
    {
        //Standard
        _offenceCancel.standardCancelDirection.RemoveAt(pIndex);
        _offenceCancel.standardCancelType.RemoveAt(pIndex);

        //Special
        _offenceCancel.specialCancelDirection.RemoveAt(pIndex);
        _offenceCancel.specialCancelType.RemoveAt(pIndex);
    }

    public void Clear() 
    {
        //Standard
        _offenceCancel.standardCancelDirection.Clear();
        _offenceCancel.standardCancelType.Clear();

        //Special
        _offenceCancel.specialCancelDirection.Clear();
        _offenceCancel.specialCancelType.Clear();
    }

    #region Offense setup

    public void DirectionSetup(OffenseDirection pOffenceDirection) 
    {
        if (_offenceCancel.direction != pOffenceDirection)
            _offenceCancel.direction = pOffenceDirection;
    }

    public void TypeSetup(OffenseType pOffenseType)
    {
        if (_offenceCancel.type != pOffenseType)
            _offenceCancel.type = pOffenseType;
    }

    #endregion

    #region Cancel

    #region Standard

    public void StandardCancelDirectionSetup(OffenseDirection pOffenceDirection, int pIndex) 
    {
        _offenceCancel.standardCancelDirection[pIndex] = pOffenceDirection;
    }

    public void StandardCancelTypeSetup(OffenseType pOffenceType, int pIndex) 
    {
        _offenceCancel.standardCancelType[pIndex] = pOffenceType;
    }

    #endregion

    #region Special

    public void SpecialCancelDirectionSetup(OffenseDirection pOffenceDirection, int pIndex)
    {
        _offenceCancel.specialCancelDirection[pIndex] = pOffenceDirection;
    }

    public void SpecialCancelTypeSetup(OffenseType pOffenceType, int pIndex)
    {
        _offenceCancel.specialCancelType[pIndex] = pOffenceType;
    }

    #endregion

    #endregion
}

[System.Serializable]
public struct OffenseCancel
{
    public OffenseDirection direction;
    public OffenseType type;

    public List<OffenseDirection> standardCancelDirection;
    public List<OffenseType> standardCancelType;
    
    public List<OffenseDirection> specialCancelDirection;
    public List<OffenseType> specialCancelType;
}

[CreateAssetMenu(fileName = "OffenceCancelConfig", menuName = "Offence/Configuration/OffenceCancel", order = 51)]
public class OffenseCancelConfig : ScriptableObject
{
    static OffenseCancelConfig _instance;

    [SerializeField]
    OffenseCancel[] _offenceCancel;

    [MenuItem("Assets/SaveAssets %&s")]
    static void SaveAssets() 
    {
        AssetDatabase.SaveAssets();
    }

    //Instance
    public static OffenseCancelConfig GetInstance 
    {
        get 
        {
            if (!_instance)
                _instance = Resources.Load("Configuration/OffenceCancelConfig") as OffenseCancelConfig;

            return _instance;
        }
    }
    public OffenseCancel[] GetOffenseCancel => _offenceCancel;
}

#if UNITY_EDITOR

[CustomEditor(typeof(OffenseCancelConfig))]
public class OffenceCancelConfigEditor : Editor 
{
    SerializedProperty _offenceCancel;

    List<OffenceCancel> _offence;

    GUIStyle _guiStyle;

    List<bool> _isOffenceFoldout, _isCancelFoldout, _isStandardFoldout, _isSpecialFoldout;

    enum CancelType { Standard, Special};

    void OnEnable()
    {
        _offenceCancel = serializedObject.FindProperty("_offenceCancel");

        if (_offence == null)
        {
            _offence = new List<OffenceCancel>();

            if (_isOffenceFoldout == null)
                _isOffenceFoldout = new List<bool>();

            if (_isCancelFoldout == null)
                _isCancelFoldout = new List<bool>();

            if (_isStandardFoldout == null)
                _isStandardFoldout = new List<bool>();

            if (_isSpecialFoldout == null)
                _isSpecialFoldout = new List<bool>();
        }

        //Offense setup
        if (_offenceCancel.arraySize != 0)
            if (_offence.Count != _offenceCancel.arraySize)
                OffenceSetup();
        
        if (_guiStyle == null) 
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }
    }

    void OffenceSetup() 
    {
        for (int i = 0; i < _offenceCancel.arraySize; ++i) 
        {
            _offence.Add(new OffenceCancel());
            _isOffenceFoldout.Add(false);

            _isStandardFoldout.Add(false);
            _isSpecialFoldout.Add(false);

            #region Offense setup

            //Direction
            _offence[i].DirectionSetup((OffenseDirection)_offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("direction").enumValueIndex);

            //Type
            _offence[i].TypeSetup((OffenseType)_offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex);

            #endregion

            #region Cancel

            //Standard
            OffenceCancelSetup(_offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("standardCancelDirection"), _offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("standardCancelType"), 
                               _offence[i].GetOffenseCancel.standardCancelDirection, _offence[i].GetOffenseCancel.standardCancelType, i);

            //Special
            OffenceCancelSetup(_offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("specialCancelDirection"), _offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("specialCancelType"),
                               _offence[i].GetOffenseCancel.specialCancelDirection, _offence[i].GetOffenseCancel.specialCancelType, i);

            #endregion
        }
    }

    void OffenceCancelSetup(SerializedProperty pPropertyRelativeDirection, SerializedProperty pPropertyRelativeType, List<OffenseDirection> pOffenseDirection, List<OffenseType> pOffenseType, int pOffenseIndex) 
    {
        for (int i = 0; i < pPropertyRelativeDirection.arraySize; ++i) 
        {
            pOffenseDirection.Add(OffenseDirection.DEFAULT);
            pOffenseType.Add(OffenseType.DEFAULT);

            //Standard
            if (pPropertyRelativeDirection.name.Contains("standard"))
                StandardCancelSetup(pOffenseIndex, i, pPropertyRelativeDirection.GetArrayElementAtIndex(i).enumValueIndex, pPropertyRelativeType.GetArrayElementAtIndex(i).enumValueIndex);

            //Special
            else if (pPropertyRelativeDirection.name.Contains("special"))
                SpecialCancelSetup(pOffenseIndex, i, pPropertyRelativeDirection.GetArrayElementAtIndex(i).enumValueIndex, pPropertyRelativeType.GetArrayElementAtIndex(i).enumValueIndex);
        }
    }

    void StandardCancelSetup(int pOffenceIndex, int pRelativePropertyIndex, int pOffenceDirectionEnumValueIndex, int pOffenceTypeEnumValueIndex) 
    {
        _offence[pOffenceIndex].StandardCancelDirectionSetup((OffenseDirection)pOffenceDirectionEnumValueIndex, pRelativePropertyIndex);
        _offence[pOffenceIndex].StandardCancelTypeSetup((OffenseType)pOffenceTypeEnumValueIndex, pRelativePropertyIndex);
    }

    void SpecialCancelSetup(int pOffenceIndex, int pRelativePropertyIndex, int pOffenceDirectionEnumValueIndex, int pOffenceTypeEnumValueIndex) 
    {
        _offence[pOffenceIndex].SpecialCancelDirectionSetup((OffenseDirection)pOffenceDirectionEnumValueIndex, pRelativePropertyIndex);
        _offence[pOffenceIndex].SpecialCancelTypeSetup((OffenseType)pOffenceTypeEnumValueIndex, pRelativePropertyIndex);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        #region Configuration

        GUILayout.Label("Offense configuration", _guiStyle);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        //Add
        if (GUILayout.Button("Add")) 
        {
            _offence.Add(new OffenceCancel());

            _isOffenceFoldout.Add(false);

            _isStandardFoldout.Add(false);
            _isSpecialFoldout.Add(false);
        }

        //Remove
        if (GUILayout.Button("Remove"))
        {
            if (_offence.Count != 0)
            {
                _offence.Remove(_offence[_offence.Count - 1]);
                _isOffenceFoldout.RemoveAt(_isOffenceFoldout.Count - 1);

                _isStandardFoldout.RemoveAt(_isStandardFoldout.Count - 1);
                _isSpecialFoldout.RemoveAt(_isSpecialFoldout.Count - 1);
            }
        }

        EditorGUILayout.EndHorizontal();

        #endregion

        EditorGUILayout.Space();

        ShowOffenseCancelInformation();

        EditorGUILayout.Space();

        //Save
        SaveSetup();

        EditorGUILayout.EndVertical();
    }

    void OffenseCancelSaveSetup(SerializedProperty pPropertyRelativeDirection, SerializedProperty pPropertyRelativeType, List<OffenseDirection> pOffenseDirection, List<OffenseType> pOffenseType) 
    {
        //Direction
        if (pPropertyRelativeDirection.arraySize != pOffenseDirection.Count)
            pPropertyRelativeDirection.arraySize = pOffenseDirection.Count;

        //Type
        if (pPropertyRelativeType.arraySize != pOffenseType.Count)
            pPropertyRelativeType.arraySize = pOffenseType.Count;

        //Cancel
        for (int i = 0; i < pOffenseDirection.Count; ++i) 
        {
            //Direction
            if (pPropertyRelativeDirection.GetArrayElementAtIndex(i).enumValueIndex != (int)pOffenseDirection[i])
                pPropertyRelativeDirection.GetArrayElementAtIndex(i).enumValueIndex = (int)pOffenseDirection[i];

            //Type
            if (pPropertyRelativeType.GetArrayElementAtIndex(i).enumValueIndex != (int)pOffenseType[i])
                pPropertyRelativeType.GetArrayElementAtIndex(i).enumValueIndex = (int)pOffenseType[i];
        }
    }

    void SaveSetup() 
    {
        if (GUILayout.Button("Save"))
        {
            if (_offence.Count != 0)
            {
                if (_offenceCancel.arraySize != _offence.Count)
                    _offenceCancel.arraySize = _offence.Count;

                #region Offense

                for (int i = 0; i < _offence.Count; ++i)
                {
                    //Direction
                    _offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("direction").enumValueIndex = (int)_offence[i].GetOffenseCancel.direction;

                    //Type
                    _offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex = (int)_offence[i].GetOffenseCancel.type;
                }

                #endregion

                #region Cancel

                for (int i = 0; i < _offence.Count; ++i)
                {
                    //Standard
                    OffenseCancelSaveSetup(_offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("standardCancelDirection"), _offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("standardCancelType"),
                                       _offence[i].GetOffenseCancel.standardCancelDirection, _offence[i].GetOffenseCancel.standardCancelType);

                    //Special
                    OffenseCancelSaveSetup(_offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("specialCancelDirection"), _offenceCancel.GetArrayElementAtIndex(i).FindPropertyRelative("specialCancelType"),
                                       _offence[i].GetOffenseCancel.specialCancelDirection, _offence[i].GetOffenseCancel.specialCancelType);
                }

                #endregion
            }

            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.HelpBox("Ne pas oublier d'appuyer sur le bouton 'Save' pour sauvegarder les modifications apportées!", MessageType.Warning);
    }

    void CancelConfiguration(ref List<bool> pIsCancelFoldout, int pOffenceIndex, OffenseCancel pOffenseCancel, CancelType pCancelType) 
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal();

        pIsCancelFoldout[pOffenceIndex] = EditorGUILayout.Foldout(pIsCancelFoldout[pOffenceIndex], $"{pCancelType}");

        #region Button

        //Add
        if (GUILayout.Button("Add"))
        {
            if (pCancelType == CancelType.Standard)
            {
                pOffenseCancel.standardCancelDirection.Add(OffenseDirection.DEFAULT);
                pOffenseCancel.standardCancelType.Add(OffenseType.DEFAULT);
            }

            else if (pCancelType == CancelType.Special)
            {
                pOffenseCancel.specialCancelDirection.Add(OffenseDirection.DEFAULT);
                pOffenseCancel.specialCancelType.Add(OffenseType.DEFAULT);
            }

        }
        
        //Remove
        if (GUILayout.Button("Remove"))
        {
            if (pCancelType == CancelType.Standard)
            {
                if (pOffenseCancel.standardCancelDirection.Count > 0)
                {
                    pOffenseCancel.standardCancelDirection.RemoveAt(pOffenseCancel.standardCancelDirection.Count - 1);
                    pOffenseCancel.standardCancelType.RemoveAt(pOffenseCancel.standardCancelType.Count - 1);
                }
            }

            else if (pCancelType == CancelType.Special)
            {
                if (pOffenseCancel.specialCancelDirection.Count > 0)
                {
                    pOffenseCancel.specialCancelDirection.RemoveAt(pOffenseCancel.specialCancelDirection.Count - 1);
                    pOffenseCancel.specialCancelType.RemoveAt(pOffenseCancel.specialCancelType.Count - 1);
                }
            }
        }

        #endregion

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (pIsCancelFoldout[pOffenceIndex])
        {
            EditorGUILayout.Space();

            if (EditorGUIUtility.labelWidth != 10f)
                EditorGUIUtility.labelWidth = 10f;

            //Standard
            if (pCancelType == CancelType.Standard)
            {
                if (pOffenseCancel.standardCancelDirection.Count != 0)
                {
                    for (int i = 0; i < pOffenseCancel.standardCancelDirection.Count; ++i) 
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label($"{i}: ");

                        StandardCancelSetup(pOffenceIndex, i, (int)(OffenseDirection)EditorGUILayout.EnumPopup("", pOffenseCancel.standardCancelDirection[i]), (int)(OffenseType)EditorGUILayout.EnumPopup("", pOffenseCancel.standardCancelType[i]));

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            //Special
            else if (pCancelType == CancelType.Special)
            {
                if (pOffenseCancel.specialCancelDirection.Count != 0)
                {
                    for (int i = 0; i < pOffenseCancel.specialCancelDirection.Count; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label($"{i}: ");

                        SpecialCancelSetup(pOffenceIndex, i, (int)(OffenseDirection)EditorGUILayout.EnumPopup("", pOffenseCancel.specialCancelDirection[i]), (int)(OffenseType)EditorGUILayout.EnumPopup("", pOffenseCancel.specialCancelType[i]));

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        EditorGUILayout.EndVertical();
    }

    void ShowOffenseCancelInformation() 
    {
        if (_offence.Count != 0)
        {
            for (int i = 0; i < _offence.Count; ++i)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                ++EditorGUI.indentLevel;

                _isOffenceFoldout[i] = EditorGUILayout.Foldout(_isOffenceFoldout[i], $"{_offence[i].GetOffenseCancel.direction}" + " " + $"{_offence[i].GetOffenseCancel.type}");

                if (_isOffenceFoldout[i])
                {
                    #region Offence

                    //Direction
                    _offence[i].DirectionSetup((OffenseDirection)EditorGUILayout.EnumPopup("Direction: ", _offence[i].GetOffenseCancel.direction));

                    //Type
                    _offence[i].TypeSetup((OffenseType)EditorGUILayout.EnumPopup("Type: ", _offence[i].GetOffenseCancel.type));

                    #endregion

                    EditorGUILayout.Space();

                    #region Cancel

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.Label("Cancel configuration", _guiStyle);

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    //Standard
                    CancelConfiguration(ref _isStandardFoldout, i, _offence[i].GetOffenseCancel, CancelType.Standard);

                    EditorGUILayout.Space();

                    //Special
                    CancelConfiguration(ref _isSpecialFoldout, i, _offence[i].GetOffenseCancel, CancelType.Special);

                    EditorGUILayout.Space();

                    GUILayout.EndVertical();

                    EditorGUILayout.Space();

                    EditorGUILayout.EndVertical();

                    #endregion
                }

                --EditorGUI.indentLevel;

                EditorGUILayout.EndVertical();
            }
        }
    }
}

#endif