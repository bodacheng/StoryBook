using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;

using CruFramework.Engine;

namespace CruFramework.Editor
{
    [CustomPropertyDrawer(typeof(CruEventBase), true)]
    public class CruEventEditor : PropertyDrawer
    {
        
        private ReorderableList list = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 初期化
            Initialize(property);
            
            return list.GetHeight();
        }

        private void Initialize(SerializedProperty property)
        {
            
            if(list == null)
            {
                SerializedProperty targetProperty = property.FindPropertyRelative("targets");
                list = new ReorderableList(property.serializedObject, targetProperty);
                
                //list.elementHeight = EditorGUIUtility.singleLineHeight * 2;
                
                list.elementHeightCallback += (index)=>
                {
                    
                    float height = EditorGUIUtility.singleLineHeight * 2;
                    
                    SerializedProperty target = targetProperty.GetArrayElementAtIndex(index);
                    
                    // コンポーネント
                    SerializedProperty targetComponment = target.FindPropertyRelative("targetComponent");
                    // メソッド名
                    SerializedProperty methodName = target.FindPropertyRelative("methodName");
                    
                    // コンポーネントの参照がある場合
                    if(targetComponment.objectReferenceValue != null)
                    {
                        // タイプ取得
                        Type type = targetComponment.objectReferenceValue.GetType();
                        // メソッド取得
                        MethodInfo method = CruEventUtility.GetMethod(type, methodName.stringValue);
                        
                        // 引数の数分確保
                        if(method != null)
                        {
                            height += EditorGUIUtility.singleLineHeight * method.GetParameters().Length;
                        }
                    }
                    
                    return height;
                };
                
                // 追加
                list.onAddCallback += (e)=> 
                {
                    int index = targetProperty.arraySize;
                    targetProperty.InsertArrayElementAtIndex(index);
                    // 参照がコピーされると困るので初期化
                    SerializedProperty p = targetProperty.GetArrayElementAtIndex(index);
                    SerializedProperty args = p.FindPropertyRelative("arguments");
                    args.arraySize = 0;
                };
                // 削除
                list.onRemoveCallback += (e)=>
                {
                    targetProperty.DeleteArrayElementAtIndex(e.index);
                };
                
                list.drawElementCallback += (rect, index, active, focused)=>
                {
                    EditorGUI.PropertyField(rect, targetProperty.GetArrayElementAtIndex(index), true);
                };
                
                list.drawHeaderCallback += (rect)=>
                {
                    if(fieldInfo.FieldType.IsGenericType)
                    {
                        Type[] types = fieldInfo.FieldType.GetGenericArguments();
                        EditorGUI.LabelField(rect, property.name + $"({string.Join(", ", types.Select(v=>v.Name))})");
                    }
                    else
                    {
                        EditorGUI.LabelField(rect, property.name);
                        
                    }
                };
            }
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //property.serializedObject.Update();
            
            // 初期化
            Initialize(property);
            
            // プロパティを描画
            // EditorGUI環境の場合はDoListを使う
            list.DoList(position);
            
            //property.serializedObject.ApplyModifiedProperties();
        }
    }
}