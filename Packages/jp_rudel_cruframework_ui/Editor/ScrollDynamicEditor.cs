using System.Collections;
using System.Collections.Generic;
using CruFramework.Engine.UI;
using UnityEngine;
using UnityEditor;

namespace CruFramework.Editor.UI
{

    [CustomEditor(typeof(ScrollDynamic))]
    public class ScrollDynamicEditor : UnityEditor.UI.ScrollRectEditor
    {
        
        private bool marginFoldout = false;
        
        public override void OnInspectorGUI()
        {
            
            ScrollDynamic scroll = (ScrollDynamic)target;
            // Undo
            Undo.RecordObject(target, "ScrollDynamic");
            
            EditorGUI.BeginChangeCheck();
            
            scroll.ItemSelector = (ScrollDynamicItemSelector)EditorGUILayout.ObjectField(nameof(scroll.ItemSelector), scroll.ItemSelector, typeof(ScrollDynamicItemSelector), true);
            scroll.Spacing = EditorGUILayout.FloatField(nameof(scroll.Spacing), scroll.Spacing);
            
            // マージン
            marginFoldout = EditorGUILayout.Foldout(marginFoldout, "Margin");
            if(marginFoldout)
            {
                EditorGUI.indentLevel++;
                scroll.MarginLeft   = EditorGUILayout.FloatField(nameof(scroll.MarginLeft), scroll.MarginLeft);
                scroll.MarginRight  = EditorGUILayout.FloatField(nameof(scroll.MarginRight), scroll.MarginRight);
                scroll.MarginTop    = EditorGUILayout.FloatField(nameof(scroll.MarginTop), scroll.MarginTop);
                scroll.MarginBottom = EditorGUILayout.FloatField(nameof(scroll.MarginBottom), scroll.MarginBottom);
                EditorGUI.indentLevel--;
            }
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            
            EditorGUILayout.LabelField("Unity ScrollRect ============");
            
            base.OnInspectorGUI();
        }
    }
}