using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using System;

namespace CruFramework.Engine.UI
{
    public class CruTextUGUI : TextMeshProUGUI
    {
        /// <summary>全テキスト共通置換処理</summary>
        private static Dictionary<string, Func<Dictionary<string, string>, string>> globalReplaceFunc = new Dictionary<string, Func<Dictionary<string, string>, string>>();

        /// <summary>文字列置換の登録</summary>
        public static void SetGlobalReplace(string key, Func<Dictionary<string, string>, string> replace)
        {
            if(globalReplaceFunc.ContainsKey(key) == false)
            {
                globalReplaceFunc.Add(key, replace);
            }
            else
            {
                globalReplaceFunc[key] = replace;
            }
        }
        
        /// <summary>文字列置換の削除</summary>
        public static void RemoveGlobalReplace(string key)
        {
            globalReplaceFunc.Remove(key);
        }
        
        /// <summary>文字列置換をすべて削除</summary>
        public static void ClearGlobalReplace()
        {
            globalReplaceFunc.Clear();
        }

        private enum CheckState
        {
            Name, Value
        }
        
        private static readonly StringBuilder stringBuilder = new StringBuilder();
        
        private static readonly StringBuilder stringBuilderTag = new StringBuilder();
        private static readonly StringBuilder stringBuilderTagName = new StringBuilder();
        private static readonly StringBuilder stringBuilderTagValue = new StringBuilder();
        
        private static readonly StringBuilder stringBuilderReplace = new StringBuilder();
        private static readonly StringBuilder stringBuilderReplaceName = new StringBuilder();
        private static readonly StringBuilder stringBuilderReplaceValue = new StringBuilder();

        [SerializeField]
        private bool isDisableCruExtentions = false;
        /// <summary>拡張機能を無効にする</summary>
        public bool IsDisableCruExtentions
        {
            get
            {
                return isDisableCruExtentions;
            }
            set
            {
                isDisableCruExtentions = value;
                if(isDisableCruExtentions == false)
                {
                    base.text = uneditedText;
                    uneditedText = string.Empty;
                }
            }
        }
        
        [SerializeField][TextArea]
        private string uneditedText = string.Empty;
        /// <summary>テキスト</summary>
        public override string text
        {
            get
            {
                return isDisableCruExtentions ? base.text : uneditedText;
            }
            set
            {
                if(isDisableCruExtentions)
                {
                    base.text = value;
                }
                else
                {
                    uneditedText = value;
                    UpdateText();
                }
            }
        }

        [SerializeField]
        private bool isEnableRuby = false;
        /// <summary>ルビの有効</summary>
        public bool IsEnableRuby
        {
            get
            {
                return isEnableRuby;
            }
            set
            {
                isEnableRuby = value;
                UpdateText();
            }
        }
        
        [SerializeField]
        private bool isEnableRubyAutoUpdate = false;
        /// <summary>ルビの自動更新</summary>
        public bool IsEnableRubyAutoUpdate
        {
            get
            {
                return isEnableRubyAutoUpdate;
            }
            set
            {
                isEnableRubyAutoUpdate = value;
            }
        }
        
        [SerializeField]
        private float fixedLineHeight = 2.0f;
        /// <summary>行間の高さ</summary>
        public float FixedLineHeight
        {
            get
            {
                return fixedLineHeight;
            }
            set
            {
                fixedLineHeight = value;
                UpdateText();
            }
        }
        
        [SerializeField]
        private float rubyScale = 0.5f;
        /// <summary>ルビのサイズ</summary>
        public float RubyScale
        {
            get
            {
                return rubyScale;
            }
            set
            {
                rubyScale = value;
                UpdateText();
            }
        }
        
        [SerializeField]
        private bool isEnableReplace = false;
        /// <summary>置換の有効</summary>
        public bool IsEnableReplace
        {
            get
            {
                return isEnableReplace;
            }
            set
            {
                isEnableReplace = value;
                UpdateText();
            }
        }
        
        /// <summary>テキスト置換処理</summary>
        private Dictionary<string, Func<Dictionary<string, string>, string>> localReplaceFunc = new Dictionary<string, Func<Dictionary<string, string>, string>>();

        protected override void Start()
        {
            // 置換がある場合はテキスト更新
            if(isEnableReplace)
            {
                UpdateText();
            }
            
            base.Start();
        }
        
        /// <summary>フォーマット</summary>
        public void SetFormatString(string str, params object[] values)
        {
            uneditedText = str;
            SetFormatValues(values);
        }
        
        /// <summary>フォーマットの値</summary>
        public void SetFormatValues(params object[] values)
        {
            base.text = ReplaceStrings(string.Format(uneditedText, values));
        }

        /// <summary>テキストの更新処理</summary>
        public void UpdateText()
        {
            if(isDisableCruExtentions)
            {
                uneditedText = string.Empty;
                return;
            }
            base.text = ReplaceStrings(uneditedText);
            // ForceMeshUpdate();
        }
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            
            if(isEnableRuby && isEnableRubyAutoUpdate && enableAutoSizing)
            {
                UpdateText();
            }
            
        }

        /// <summary>文字列置換の登録</summary>
        public void SetLocalReplace(string key, Func<Dictionary<string, string>, string> replace)
        {
            if(localReplaceFunc.ContainsKey(key) == false)
            {
                localReplaceFunc.Add(key, replace);
            }
            else
            {
                localReplaceFunc[key] = replace;
            }
        }
        
        /// <summary>文字列置換の削除</summary>
        public void RemoveLocalReplace(string key)
        {
            localReplaceFunc.Remove(key);
        }
        
        /// <summary>文字列置換をすべて削除</summary>
        public void ClearLocalReplace()
        {
            localReplaceFunc.Clear();
        }
        
        private string GetReplaceString(string key, Dictionary<string, string> args)
        {
            if(globalReplaceFunc.TryGetValue(key, out Func<Dictionary<string, string>, string> globalReplace))
            {
                return globalReplace(args);
            }
            
            if(localReplaceFunc.TryGetValue(key, out Func<Dictionary<string, string>, string> localReplace))
            {
                return localReplace(args);
            }
            
            return string.Empty;
        }
        
        private bool HasReplaceKey(string key)
        {
            return globalReplaceFunc.ContainsKey(key) || localReplaceFunc.ContainsKey(key);
        }
        
        /// <summary>文字列を変換</summary>
        private string ReplaceStrings(string value)
        {
            stringBuilder.Clear();
            
            // ルビが有効の場合
            if(isEnableRuby)
            {
                float space = GetPreferredValues("_").x * 0.5f;
                stringBuilder.Append($"<line-height={fixedLineHeight}em>");
                stringBuilder.Append($"<voffset=1em><alpha=#00><size={((int)(rubyScale * 100.0f))}%>_</size><alpha=#ff></voffset><space=-{space}>");
            }
            
            bool isInTag = false;
            bool isEscape = false;
            bool isReplace = false;
            
            CheckState tagCheckState = CheckState.Name;
            CheckState replaceCheckState = CheckState.Name;

            for(int i=0;i<value.Length;i++)
            {
                char c = value[i];
                
                // エスケープ
                if(isEscape)
                {
                    isEscape = false;
                    stringBuilder.Append(c);
                    continue;
                }
                
                switch(c)
                {
                    // エスケープ
                    case '¥':
                    {
                        isEscape = true;
                        continue;
                    }
                    
                    // 置換
                    case '{':
                    {
                        // 置換が無効の場合はそのまま出力
                        if(isEnableReplace == false)
                        {
                            break;
                        }
                        
                        stringBuilderReplace.Clear();
                        stringBuilderReplaceName.Clear();
                        stringBuilderReplaceValue.Clear();
                        replaceCheckState = CheckState.Name;
                        isReplace = true;
                        continue;
                    }
                    
                    // 置換
                    case '}':
                    {
                        // 置換が無効の場合はそのまま出力
                        if(isEnableReplace == false)
                        {
                            break;
                        }
                        
                        // キー
                        string replaceKey = stringBuilderReplaceName.ToString();
                        // 置換キーがある場合
                        if(HasReplaceKey(replaceKey))
                        {
                            // 値
                            string replaceValue = stringBuilderReplaceValue.ToString();
                            // 引数
                            string[] arguments = replaceValue.Split(',');
                            // Dic
                            Dictionary<string, string> argumentsDic = new Dictionary<string, string>();
                            for(int n=0;n<arguments.Length;n++)
                            {
                                string[] keyValue = arguments[n].Split('=');
                                if(keyValue.Length == 2)
                                {
                                    argumentsDic.Add(keyValue[0], keyValue[1]);
                                }
                                else
                                {
                                    argumentsDic.Add(keyValue[0], string.Empty);
                                }
                            }
                            // 置換処理
                            string replaceString = GetReplaceString(replaceKey, argumentsDic);
                            // 文字出力
                            stringBuilder.Append(replaceString);
                        }
                        // キーがない場合はそのまま出力
                        else
                        {
                            stringBuilder.Append($"{{{stringBuilderReplace}}}");
                        }
                        // 初期化
                        stringBuilderReplace.Clear();
                        stringBuilderReplaceName.Clear();
                        stringBuilderReplaceValue.Clear();
                        
                        isReplace = false;
                        continue;
                    }
                    
                    // タグ
                    case '<':
                    {
                        // チェックステートを初期化
                        tagCheckState = CheckState.Name;
                        // StringBuilderを初期化
                        stringBuilderTag.Clear();
                        stringBuilderTagName.Clear();
                        stringBuilderTagValue.Clear();
                        // タグの処理へ
                        isInTag = true;
                        continue;
                    }
                    // タグ
                    case '>':
                    {
                        // タグの処理解除
                        isInTag = false;
                        
                        // タグの処理
                        string tagName = stringBuilderTagName.ToString();
                        string tagValue = stringBuilderTagValue.ToString();
                        // StringBuilder初期化
                        stringBuilderTagName.Clear();
                        stringBuilderTagValue.Clear();
                        
                        // タグごとに処理
                        switch(tagName)
                        {
                            case "r":
                            {
                                // ルビの有効チェック
                                if(isEnableRuby == false)
                                {
                                    stringBuilder.Append($"<{stringBuilderTag}>");
                                    break;
                                }
                                
                                // ルビの範囲を取得
                                for(int n=i+1;n<value.Length-3;n++)
                                {
                                    if(value[n] == '<' && value[n + 1] == '/' && value[n+2] == 'r' && value[n+3] == '>')
                                    {
                                        break;
                                    }
                                    stringBuilderTagValue.Append(value[n]);
                                }
                                
                                // ルビを振る対象の文字列
                                string rubyTarget = stringBuilderTagValue.ToString();
                                stringBuilderTagValue.Clear();
                                
                                // ルビテキスト
                                string rubyText = tagValue;
                                // ルビのテキストサイズ
                                Vector2 rubyTextSize = GetPreferredValues(rubyText) * rubyScale;
                                // ルビ対象のテキストサイズ
                                Vector2 rubyTargetTextSize = GetPreferredValues(rubyTarget);
                                
                                // AutoSize
                                if(enableAutoSizing)
                                {
                                    // Rectサイズ
                                    Vector2 rectSize = rectTransform.rect.size;
                                    // テキストサイズ
                                    Vector2 textSize = new Vector2(rubyTargetTextSize.x, rubyTextSize.y + rubyTargetTextSize.y);
                                    // スケール
                                    float scale = Mathf.Min(rectSize.x / textSize.x, rectSize.y / textSize.y);
                                    // 領域をはみ出てる場合はスケール補正
                                    if(scale < 1.0f)
                                    {
                                        rubyTextSize *= scale;
                                        rubyTargetTextSize *= scale;
                                    }
                                }
                                
                                // スペースの計算
                                float rubyTextSpace = (rubyTargetTextSize.x - rubyTextSize.x) * 0.5f;
                                float rubyTargetTextSpace = rubyTextSpace + rubyTextSize.x + characterSpacing * 0.5f;
                                // タグを付けて文字列を生成
                                stringBuilder.Append($"<voffset=1em><space={rubyTextSpace}><size={((int)(rubyScale * 100.0f))}%>{rubyText}</size></voffset><space=-{rubyTargetTextSpace}><nobr>");
                                break;
                            }
                            
                            case "/r":
                            {
                                // ルビの有効チェック
                                if(isEnableRuby == false)
                                {
                                    stringBuilder.Append($"<{stringBuilderTag}>");
                                    break;
                                }
                                
                                stringBuilder.Append("</nobr>");
                                break;
                            }
                            
                            // その他はそのまま出力
                            default:
                            {
                                stringBuilder.Append($"<{tagName}");
                                if(tagValue.Length > 0)
                                {
                                    stringBuilder.Append($"={tagValue}");
                                }
                                stringBuilder.Append(">");
                                break;
                            }
                        }
                        continue;
                    }
                }
                
                
                // 置換
                if(isReplace)
                {
                    stringBuilderReplace.Append(c);

                    switch(replaceCheckState)
                    {
                        case CheckState.Name:
                        {
                            if(c == ':')
                            {
                                replaceCheckState = CheckState.Value;
                            }
                            else
                            {
                                stringBuilderReplaceName.Append(c);
                            }
                            break;
                        }
                        case CheckState.Value:
                        {
                            stringBuilderReplaceValue.Append(c);
                            break;
                        }
                    }
                }
                // タグ内の処理
                else if(isInTag)
                {
                    stringBuilderTag.Append(c);

                    switch(tagCheckState)
                    {
                        case CheckState.Name:
                        {
                            if(c == '=')
                            {
                                tagCheckState = CheckState.Value;
                            }
                            else
                            {
                                stringBuilderTagName.Append(c);
                            }
                            break;
                        }
                        case CheckState.Value:
                        {
                            stringBuilderTagValue.Append(c);
                            break;
                        }
                    }
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
            
            string result = stringBuilder.ToString();
            stringBuilder.Clear();
            return result;
        }
    }
}
