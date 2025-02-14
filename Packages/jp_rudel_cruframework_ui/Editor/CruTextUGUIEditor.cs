using System.Collections;
using System.Collections.Generic;
using CruFramework.Engine.UI;
using Cysharp.Threading.Tasks;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.Editor.UI
{

    [CustomEditor(typeof(CruTextUGUI), true)]
    public class CruTextUGUIEditor : TMP_EditorPanelUI
    {
	    private SerializedProperty isDisableCruExtentionsProperty = null;
	    private SerializedProperty uneditedTextProperty = null;
	    private SerializedProperty isEnableRubyProperty = null;
	    private SerializedProperty isEnableRubyAutoUpdateProperty = null;
	    private SerializedProperty fixedLineHeightProperty = null;
	    private SerializedProperty rubyScaleProperty = null;
	    private SerializedProperty isEnableReplaceProperty = null;
	    
	    protected override void OnEnable()
	    {
		    base.OnEnable();
		    
		    isDisableCruExtentionsProperty = serializedObject.FindProperty("isDisableCruExtentions");
		    uneditedTextProperty = serializedObject.FindProperty("uneditedText");
		    isEnableRubyProperty = serializedObject.FindProperty("isEnableRuby");
		    isEnableRubyAutoUpdateProperty = serializedObject.FindProperty("isEnableRubyAutoUpdate");
		    fixedLineHeightProperty = serializedObject.FindProperty("fixedLineHeight");
		    rubyScaleProperty = serializedObject.FindProperty("rubyScale");
		    isEnableReplaceProperty = serializedObject.FindProperty("isEnableReplace");
	    }

	    public override void OnInspectorGUI()
        {
	        CruTextUGUI text = (CruTextUGUI)target;
	        
	        DrawText();
	        base.OnInspectorGUI();
	        
	        DrawCruSettings();
	        DrawRubySettings();
	        DrawReplaceSettings();
	        
	        if(m_TextComponent.havePropertiesChanged)
	        {
		        text.UpdateText();
		        EditorUtility.SetDirty(target);
	        }
        }
        
        private void DrawText()
        {
	        // 拡張機能Off
	        if(isDisableCruExtentionsProperty.boolValue == true)return;
	        
	        CruTextUGUI text = (CruTextUGUI)target;
	        serializedObject.Update();
	        
	        Rect rect = EditorGUILayout.GetControlRect(false, 22);
	        GUI.Label(rect, new GUIContent("<b>UneditedText</b>"), TMP_UIStyleManager.sectionHeader);
	        EditorGUI.indentLevel = 0;
	        
	        EditorGUILayout.PropertyField(uneditedTextProperty, new GUIContent(string.Empty));
	        
	        if(serializedObject.ApplyModifiedProperties())
	        {
		        m_TextComponent.havePropertiesChanged = true;
	        }
        }
        
        private void DrawCruSettings()
        {
	        CruTextUGUI text = (CruTextUGUI)target;
	        serializedObject.Update();
	        
	        Rect rect = EditorGUILayout.GetControlRect(false, 22);
	        GUI.Label(rect, new GUIContent("<b>Cru Settings</b>"), TMP_UIStyleManager.sectionHeader);
	        EditorGUI.indentLevel = 0;
	        
	        // IsEnableRuby
	        EditorGUILayout.PropertyField(isDisableCruExtentionsProperty);
	        
	        
	        if(serializedObject.ApplyModifiedProperties())
	        {
		        m_TextComponent.havePropertiesChanged = true;
	        }
        }
        
        private void DrawRubySettings()
        {
	        // 拡張機能Off
	        if(isDisableCruExtentionsProperty.boolValue == true)return;
	        
	        CruTextUGUI text = (CruTextUGUI)target;
	        serializedObject.Update();
	        
	        Rect rect = EditorGUILayout.GetControlRect(false, 22);
	        GUI.Label(rect, new GUIContent("<b>Ruby Settings</b>"), TMP_UIStyleManager.sectionHeader);
	        EditorGUI.indentLevel = 0;
	        
	        // IsEnableRuby
	        EditorGUILayout.PropertyField(isEnableRubyProperty);
	        // IsEnableRuby
	        EditorGUILayout.PropertyField(isEnableRubyAutoUpdateProperty);
	        // FixedLineHeight
	        EditorGUILayout.PropertyField(fixedLineHeightProperty);
	        // RubyScale
	        EditorGUILayout.PropertyField(rubyScaleProperty);
	        
	        if(serializedObject.ApplyModifiedProperties())
	        {
		        m_TextComponent.havePropertiesChanged = true;
	        }
        }
        
        private void DrawReplaceSettings()
        {
	        // 拡張機能Off
	        if(isDisableCruExtentionsProperty.boolValue == true)return;
	        
	        CruTextUGUI text = (CruTextUGUI)target;
	        serializedObject.Update();
	        
	        Rect rect = EditorGUILayout.GetControlRect(false, 22);
	        GUI.Label(rect, new GUIContent("<b>Replace Settings</b>"), TMP_UIStyleManager.sectionHeader);
	        EditorGUI.indentLevel = 0;
	        
	        // IsEnableRuby
	        EditorGUILayout.PropertyField(isEnableReplaceProperty);
	        
	        
	        if(serializedObject.ApplyModifiedProperties())
	        {
		        m_TextComponent.havePropertiesChanged = true;
	        }
        }
        
        protected override void DrawExtraSettings()
        {
	        base.DrawExtraSettings();
	    }
	}
}