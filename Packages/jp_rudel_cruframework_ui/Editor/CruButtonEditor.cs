using System.Collections;
using System.Collections.Generic;
using CruFramework.Engine.UI;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Editor.UI
{

    [CustomEditor(typeof(CruButton), true)]
    public class CruButtonEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            CruButton button = (CruButton)target;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            button.LongTapTriggerMode = (CruButton.LongTapTriggerType)EditorGUILayout.EnumPopup(nameof(button.LongTapTriggerMode), button.LongTapTriggerMode);
            
            if(button.LongTapTriggerMode != CruButton.LongTapTriggerType.None)
            {
                button.LongTapTriggerTime = EditorGUILayout.FloatField(nameof(button.LongTapTriggerTime), button.LongTapTriggerTime);
                
                if(button.LongTapTriggerMode == CruButton.LongTapTriggerType.Repeat)
                {
                    button.LongTapTriggerRepeatInterval = EditorGUILayout.FloatField(nameof(button.LongTapTriggerRepeatInterval), button.LongTapTriggerRepeatInterval);
                }
            }
            
            button.ClickTriggerInterval = EditorGUILayout.FloatField(nameof(button.ClickTriggerInterval), button.ClickTriggerInterval);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("==== Unity Button ====");
            base.OnInspectorGUI();
            
            SerializedProperty longTap = serializedObject.FindProperty("onLongTap");
            EditorGUILayout.PropertyField(longTap);
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        private void DrawGridInfoGUI(ScrollGrid.GridInfo grid)
        {
            grid.SpacingType = (ScrollGrid.SpacingType)EditorGUILayout.EnumPopup(nameof(grid.SpacingType), grid.SpacingType);
            if(grid.SpacingType == ScrollGrid.SpacingType.FixedSpace)
            {
                grid.Spacing = EditorGUILayout.FloatField(nameof(grid.Spacing), grid.Spacing);
            }
        }
    }
    
}