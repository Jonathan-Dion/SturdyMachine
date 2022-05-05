using System.IO;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ICustomEditor.ScriptableObjectEditor
{
    public abstract class ScriptableObjectICustomEditor : ScriptableObject, ICustomEditor 
    {
        protected string _currentExtendedFolderPath, _currentFolderPath;

        protected GUIStyle _guiStyle;

        public virtual void CustomOnEnable() 
        {
            _guiStyle = new GUIStyle();

            _guiStyle.fontStyle = FontStyle.BoldAndItalic;
        }

        public virtual void CustomOnDisable() { }
        public virtual void CustomOnDestroy() { }

        public virtual void CustomOnInspectorGUI() 
        {
            Editor.CreateEditor(this).DrawDefaultInspector();
        }
        public virtual void CustomOnSceneGUI() { }

        public virtual void ShowExplorerCreator() 
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            //Search Directory
            if (GUILayout.Button("Search folder path"))
                _currentExtendedFolderPath = EditorUtility.OpenFolderPanel("Open File Explorer", "", "");

            _currentFolderPath = EditorGUILayout.TextField(_currentFolderPath);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"{_currentExtendedFolderPath}", _guiStyle);

            EditorGUILayout.EndVertical();

            //Directory creation
            if (_currentFolderPath != "")
            {
                if (!Directory.Exists($"{_currentExtendedFolderPath}/" + _currentFolderPath))
                {
                    //Folder
                    if (GUILayout.Button("Create folder"))
                    {
                        Directory.CreateDirectory($"{_currentExtendedFolderPath}/" + _currentFolderPath);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }
}