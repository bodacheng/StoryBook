# cru_ui
cru_uiは、Unityゲーム開発において汎用的に使用可能なUIコンポーネントの拡張機能を提供します。

## Requirements
Unity 2021.3以降

## 導入方法

### インストール

**注意:フレームワークをインストール前にコンパイルエラーを解決してください。（コンパイルエラーがあるとセットアップスクリプトが正常に起動しません）**



#### 1. Unityエディタで、Window -> Package Managerを開き、+ボタンをクリックし、Add package from git URL...を選択します。
<img width="1170" alt="Screen Shot 2023-02-22 at 14 42 45" src="https://user-images.githubusercontent.com/114982201/220534429-3199e981-8461-469f-9b6d-2a6d8d3d3e33.png">


#### 2. 以下のURLを入力し、Enterキーを押します。
```
https://github.com/gochipon/cru_ui.git#{Version}
```
> [!NOTE]
> {Version}はインストールしたいバージョンに変更してください。 (e.g. `1.1.11`).


#### 3. カスタムパッケージとして利用する
カスタムパッケージとして利用するため、下記URLの手順を実行してください。
https://versetyles.atlassian.net/wiki/spaces/RudelUnity/pages/3134619693/CruFramework



<BR><BR>

## 主な機能

- Button
    - CruButton: 長押しイベントやクリックインターバルなど、UnityEngine.UI.Buttonのインタラクション拡張を提供します。
- Event
    - CruEvent: イベントクラス
        - UnityEventのように扱える
        - メソッドに[EnumAction(typeof(T))]のAttributeを設定すると引数にEnumを指定できるように拡張。
    - CruEventArgType: イベントの引数に使用する型
    - CruEventTarget: イベントの呼び出し対象を指定するためのクラス
    - CruEventUtility: イベント関連のユーティリティクラス
- Modal
    - ModalWindow: モーダルウィンドウの基本クラス
    - MocalManager: モーダルウィンドウの管理を行うクラス
- Page
    - ページの遷移や状態管理を行うための基本クラス Page と PageManager を提供します。
    - フェードイン/アウトのアニメーション制御や、ページのスタック管理などが可能です。
- Scroll
    - スクロールビューの機能を拡張した各種コンポーネントを提供します。
        - ScrollGrid: 要素のサイズが固定のスクロールビュー
        - ScrollDynamic: 要素のサイズが動的に変化するスクロールビュー
        - NestedScrollEventThrower: ネストされたスクロールビューのイベント制御
        - ScrollGridController: ScrollGrid の操作を行うためのコントローラー
- Sheet
    - SheetManager: シートの管理を行うクラス
    - Sheet: シートの基本クラス
    - SHeetTab: シートのタブ切り替えを行うためのコンポーネント
- Text
    - CruTextUGUI: ルビ表示や文字列置換など、TextMeshProUGUIの拡張機能を提供します。


詳細は Samples フォルダ内のサンプルコードを参照してください。