using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

using CruFramework.Engine;

namespace CruFramework.Editor
{
    [CustomPropertyDrawer(typeof(CruEventTargetBase), true)]
    public class CruEventTargetEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            
            float height = EditorGUIUtility.singleLineHeight * 2.0f;
            
            // コンポーネント
            SerializedProperty targetComponment = property.FindPropertyRelative("targetComponent");
            // メソッド名
            SerializedProperty methodName = property.FindPropertyRelative("methodName");
            
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
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //property.serializedObject.Update();
            
            // コンポーネント
            SerializedProperty targetComponment = property.FindPropertyRelative("targetComponent");
            // メソッド名
            SerializedProperty methodName = property.FindPropertyRelative("methodName");
            
            // Component
            EditorGUI.PropertyField(new Rect(position.x, position.y, 140.0f, EditorGUIUtility.singleLineHeight), targetComponment, new GUIContent(string.Empty));
            
            Type[] argTypes = null;
            // 一旦無理やり取る
            // いい方法がない
            if(fieldInfo.FieldType.IsGenericType)
            {
                Type elementType = fieldInfo.FieldType.GetGenericArguments()[0];
                if(elementType.IsGenericType)
                {
                    argTypes = elementType.GetGenericArguments();
                }
            }
            
            // コンポーネントの参照がある場合
            if(targetComponment.objectReferenceValue is Component component)
            {
                // タイプ取得
                Type type = targetComponment.objectReferenceValue.GetType();
                // コンポーネント取得
                Component[] components = component.GetComponents<Component>();
                // GUIに必要なデータ生成
                int[] componentIndex = new int[components.Length];
                string[] componentMenu = new string[components.Length];
                // 選択中Index
                int selectedComponent = -1;
                for(int i=0;i<components.Length;i++)
                {
                    // 名前
                    componentMenu[i] = components[i].GetType().Name;
                    // Index
                    componentIndex[i] = i;
                    // 選択中チェック
                    if(targetComponment.objectReferenceValue == components[i])
                    {
                        selectedComponent = i;
                    }
                }

                EditorGUI.BeginChangeCheck();
                // コンポーネント
                selectedComponent = EditorGUI.IntPopup(new Rect(position.x + 142.0f, position.y, position.width - 142.0f, EditorGUIUtility.singleLineHeight), selectedComponent, componentMenu, componentIndex);
                
                if(EditorGUI.EndChangeCheck())
                {
                    targetComponment.objectReferenceValue = components[selectedComponent];
                }
                
                // メソッド
                MethodInfo[] methods = CruEventUtility.GetMethods(argTypes, type, methodName.stringValue);
                
                // GUIに必要なデータ生成
                int[] methodIndex = new int[methods.Length];
                string[] methodMenu = new string[methods.Length];
                // 選択中Index
                int selectedMethod = 0;
                for(int i=0;i<methods.Length;i++)
                {
                    // 名前
                    methodMenu[i] = methods[i].Name;
                    // Index
                    methodIndex[i] = i;
                    // 選択中チェック
                    if(methodName.stringValue == methods[i].Name)
                    {
                        selectedMethod = i;
                    }
                }
                
                // コンポーネント
                selectedMethod = EditorGUI.IntPopup(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, 140.0f, EditorGUIUtility.singleLineHeight), selectedMethod, methodMenu, methodIndex);
                
                if(methods.Length > 0)
                {
                    methodName.stringValue = methodMenu[selectedMethod];
                }
                else
                {
                    methodName.stringValue = string.Empty;
                }
                
                // 引数
                MethodInfo method = CruEventUtility.GetMethod(type, methodName.stringValue);
                if(method != null)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    
                    // 引数
                    SerializedProperty arguments = property.FindPropertyRelative("arguments");
                    arguments.arraySize = parameters.Length;

                    Rect rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
                    
                    for(int i=0;i<parameters.Length;i++)
                    {
                        // Property
                        ParameterInfo p = parameters[i];
                        // 引数
                        SerializedProperty arg = arguments.GetArrayElementAtIndex(i);
                        
                        
                        Type checkType = p.ParameterType;
                        // Enum
                        if(checkType.IsEnum)
                        {
                            checkType = typeof(Enum);
                        }
                        
                        // 型の紐付け
                        Type argType = CruEventUtility.GetArgumentType(checkType);
                        if(argType != null)
                        {
                            if(arg.managedReferenceValue == null || arg.managedReferenceValue.GetType() != argType)
                            {
                                arg.managedReferenceValue = System.Activator.CreateInstance(argType);
                            }
                        }
                        else
                        {
                            arg.managedReferenceValue = null;
                        }
                        
                        if(arg.managedReferenceValue == null)
                        {
                            EditorGUI.LabelField(rect, p.Name, "未対応の型:" + p.ParameterType.Name);
                        }
                        else
                        {
                            SerializedProperty value = arg.FindPropertyRelative("value");
                            
                            if(p.ParameterType.IsEnum)
                            {
                                // Enum取得
                                Array enumValues = System.Enum.GetValues(p.ParameterType);
                                // Index
                                int[] values = new int[enumValues.Length];
                                // Name
                                string[] names = new string[enumValues.Length];
                                
                                // 
                                for(int n=0;n<enumValues.Length;n++)
                                {
                                    // Intに変換
                                    values[n] = (int)System.Convert.ChangeType(enumValues.GetValue(n), typeof(int));
                                    // 名前
                                    names[n] = enumValues.GetValue(n).ToString();
                                }
                                // 表示
                                value.intValue = EditorGUI.IntPopup(rect, p.Name, value.intValue, names, values);
                            }
                            else
                            {
                                // GUIField
                                EditorGUI.PropertyField(rect, value, new GUIContent(p.Name));
                            }
                        }

                        rect.y += EditorGUIUtility.singleLineHeight;
                    }
                }
            }

            //property.serializedObject.ApplyModifiedProperties();
        }
    }
}