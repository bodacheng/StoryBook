using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using System.Threading;

namespace CruFramework.Engine
{
    
    public static class CruEventUtility
    {
        
        public static Type GetArgumentType(Type type)
        {
            if(type == typeof(int))return typeof(CruEventTargetArgInt);
            if(type == typeof(Enum) || type.IsEnum)return typeof(CruEventTargetArgInt);
            if(type == typeof(float))return typeof(CruEventTargetArgFloat);
            if(type == typeof(string))return typeof(CruEventTargetArgString);
            if(type == typeof(bool))return typeof(CruEventTargetArgBool);
            if(type == typeof(long))return typeof(CruEventTargetArgLong);
            
            return null;
        }
        
        public static bool IsAvailableType(Type type)
        {
            if(CruEventUtility.IsPrimitive(type))return true;
            
            return true;
        }
        
        private static bool IsAvailableParameter(ParameterInfo[] parameters)
        {
            // 各引数をチェック
            foreach(ParameterInfo p in parameters)
            {
                // 使用可能な型か調べる
                if(IsAvailableType(p.ParameterType) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>呼び出せるメソッド取得</summary>
        public static MethodInfo[] GetMethods(Type[] argumentTypes, Type type, string current)
        {
            
            List<MethodInfo> result = new List<MethodInfo>();
            // メソッドをすべて取得
            MethodInfo[] methods = CruEventUtility.GetMethods(type);
            // 引数をチェック
            foreach(MethodInfo method in methods)
            {
                // 属性
                if(method.Name != current && method.GetCustomAttribute<CruEventTargetAttribute>() == null)continue;
                
                ParameterInfo[] parameters = method.GetParameters();
                // 使用可能な型か調べる
                if(IsAvailableParameter(parameters) == false)continue;
                // 引数チェック
                bool matchArgsType = true;
                if(argumentTypes != null)
                {
                    // 指定した引数のほうが長い
                    if(argumentTypes.Length > parameters.Length)
                    {
                        matchArgsType = false;
                    }
                    else
                    {
                        for(int i=0;i<argumentTypes.Length;i++)
                        {
                            if(parameters[i].ParameterType != argumentTypes[i])
                            {
                                matchArgsType = false;
                                break;
                            }
                        }
                    }
                }
                
                if(matchArgsType == false)continue;
                
                // 結果に追加
                result.Add(method);
            }
            
            return result.ToArray();
        }
        
        /// <summary>MethodInfo</summary>
        public static FieldInfo GetField(Type type, string fieldName)
        {
            while(true)
            {
                if(type == typeof(object) || type == null)break;
                
                FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(field != null)return field;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>MethodInfo</summary>
        public static MethodInfo GetMethod(Type type, string methodName)
        {
            while(true)
            {
                if(type == typeof(object) || type == null)break;
                
                MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(method != null)return method;
                type = type.BaseType;
            }
            return null;
        }
        
        /// <summary>すべてのメソッドを取得</summary>
        public static MethodInfo[] GetMethods(Type type)
        {
            List<MethodInfo> result = new List<MethodInfo>();
            
            while(true)
            {
                if(type == typeof(object) || type == null)break;
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                result.AddRange(methods);
                type = type.BaseType;
            }
            
            return result.ToArray();
        }
        
        public static bool IsPrimitive(Type type)
        {
            if(type.IsPrimitive)return true;
            if(type == typeof(string))return true;
            if(type.IsEnum)return true;
            return false;
        }
    }
}