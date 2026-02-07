# NipaLabel - デバッグラベルシステム

Unity用の3D空間にテキストラベルを表示するデバッグシステムです。

## LabelUI コンポーネント

テキストと背景画像を表示するUIコンポーネントです。

### 機能
- **テキスト管理**: テキスト内容、フォントサイズ、テキスト色の設定
- **背景**: 背景色とサイズの設定
- **自動サイズ調整**: テキスト内容に合わせて背景サイズを自動調整
- **位置設定**: ワールド座標に基づいてスクリーン空間の位置を更新

### シリアル化フィールド
- `Text textComponent`: Textコンポーネントへの参照
- `Image backgroundImage`: 背景Imageコンポーネントへの参照
- `RectTransform rectTransform`: RectTransformコンポーネントへの参照

### パブリックフィールド
- `int lastUpdateFrame`: 最後に更新されたフレーム番号（-1: 静的ラベル、>=0: 動的ラベル）

### パブリックメソッド
- `SetText(string text)`: テキスト内容を設定し、サイズを調整
- `SetOffset(Vector3 offset)`: 位置オフセットを設定
- `SetFontSize(int size)`: フォントサイズを設定し、サイズを調整
- `SetTextColor(Color color)`: テキスト色を設定
- `SetWorldPosition(Vector3 position)`: ワールド座標を設定
- `SetBackgroundColor(Color color)`: 背景色を設定
- `UpdatePosition()`: 位置とサイズを更新

## NLabel マネージャー

デバッグラベルを管理するシングルトンマネージャークラスです。

### アーキテクチャ
- **シングルトンパターン**: `NLabel.Instance` でアクセス
- **オブジェクトプーリング**: パフォーマンス向上のためLabelUIインスタンスを再利用
- **キャンバス管理**: 自動的にスクリーンオーバーレイキャンバスを作成
- **IDベース管理**: 文字列IDでラベルを管理

### データ構造
```csharp
Dictionary<string, LabelUI> activeLabels
```

- キー: ラベルID（文字列）
- 値: LabelUIインスタンス

### パブリックメソッド

#### InitLabelメソッド
```csharp
public void InitLabel(string id, string message, Vector3 worldPosition,
    Vector3 offset = default, bool needUpdateEveryFrame = true)
```
ラベルを初期化・作成します。
- `id`: ラベルの識別子
- `message`: 表示するメッセージ
- `worldPosition`: ワールド座標
- `offset`: 位置オフセット（オプション）
- `needUpdateEveryFrame`: 毎フレーム更新が必要かどうか（オプション）

#### UpdateLabelメソッド（オーバーロード）
```csharp
public void UpdateLabel(string id, string message, Vector3 worldPosition)
public void UpdateLabel(string id, Vector3 worldPosition)
public void UpdateLabel(string id, string message)
public void UpdateLabelOffset(string id, Vector3 offset)
```
既存のラベルを更新します。

#### 削除メソッド
```csharp
public void RemoveLabel(string id)      // 指定IDのラベルを削除
public void ClearAllLabels()            // すべてのラベルを削除
```

### 自動機能
- **位置更新**: 毎フレームで動的ラベルの位置を更新
- **フレーム管理**: 更新されていないラベルを自動的に非アクティブ化
- **プーリング**: 未使用ラベルをオブジェクトプールに戻す

## NLabelTests コンポーネント

ラベルシステムのテストとデバッグを行うMonoBehaviourコンポーネントです。

### 機能
- **ランタイムテスト**: プレイモード中にテストを実行
- **GUIインターフェース**: 画面上のコントロール（エディタ専用）
- **視覚的フィードバック**: ラベル操作の即時確認

### インスペクター設定
- `GameObject testTargetObject`: テストラベルを付けるGameObject
- `bool updateNeeded`: 毎フレーム更新が必要かどうか

### テストメソッド
- `TestShowLabel()`: 基本的なラベル作成
- `TestUpdateLabel()`: ラベルテキスト更新
- `TestMultipleLabels()`: 同じオブジェクトに複数ラベル
- `TestRemoveSpecificLabel()`: 指定IDラベルの削除
- `TestRemoveAllLabels()`: オブジェクトの全ラベル削除
- `TestPooling()`: オブジェクトプールの検証

### デバッグGUI（エディタ専用）
- **ラベルコントロール**: カスタムパラメータでラベルを作成/更新
- **削除コントロール**: 特定または全ラベルの削除
- **アクティブラベルリスト**: 全ラベル表示と個別削除
- **プールステータス**: オブジェクトプールの使用状況表示

## 使用例

### 基本的な使用
```csharp
// 毎フレーム更新されるラベル
NLabel.Instance.InitLabel("player_hp", "HP: 100", playerPos, Vector3.up, true);

// 静的ラベル（位置固定）
NLabel.Instance.InitLabel("message", "Welcome!", fixedPos, Vector3.zero, false);
```

### 更新
```csharp
// メッセージと位置を更新
NLabel.Instance.UpdateLabel("player_hp", "HP: 80", newPlayerPos);

// 位置のみ更新
NLabel.Instance.UpdateLabel("player_hp", newPlayerPos);

// メッセージのみ更新
NLabel.Instance.UpdateLabel("player_hp", "HP: 80");

// オフセットのみ更新
NLabel.Instance.UpdateLabelOffset("player_hp", Vector3.up * 3);
```

### 削除
```csharp
// 特定ラベル削除
NLabel.Instance.RemoveLabel("player_hp");

// 全ラベル削除
NLabel.Instance.ClearAllLabels();
```

## セットアップ要件

### 1. シーン設定
- NLabelコンポーネントをシーン内のGameObjectに追加
- テスト用にNLabelTestsコンポーネントを別のGameObjectに追加

### 2. LabelUIプレハブ（オプション）
- TextとImageコンポーネントを持つプレハブを作成
- NLabelの`labelUIPrefab`フィールドに割り当てるとパフォーマンス向上

### 3. キャンバス設定
- NLabelが自動的にスクリーンオーバーレイキャンバスを作成
- カメラがUIレンダリング用に設定されていることを確認

## パフォーマンスに関する注意

- オブジェクトプーリングによりインスタンス化オーバーヘッドを最小化
- 毎フレーム更新されるラベルはパフォーマンスに影響するので注意
- 自動クリーンアップでメモリリークを防止
- ラベルはスクリーン空間でレンダリングされ、視認性が一定

## NLine ライン描画システム

Unity用の3D空間にラインを描画するデバッグシステムです。

## UILineConnection コンポーネント

2点間のラインを描画するUIコンポーネントです。

### 機能
- **ライン描画**: 2点間のラインをUIとして描画
- **厚さ設定**: ラインの太さを設定可能
- **色設定**: ラインの色を設定可能
- **自動サイズ調整**: 距離に応じた適切な描画

### パブリックフィールド
- `int lastUpdateFrame`: 最後に更新されたフレーム番号（-1: 静的ライン、>=0: 動的ライン）
- `Vector2 startTarget`: 開始点のスクリーン座標
- `Vector2 endTarget`: 終了点のスクリーン座標
- `float thickness`: ラインの太さ

### パブリックメソッド
- `UpdatePosition()`: ラインの位置を更新

## NLine マネージャー

デバッグラインを管理するシングルトンマネージャークラスです。

### アーキテクチャ
- **シングルトンパターン**: `NLine.Instance` でアクセス
- **オブジェクトプーリング**: パフォーマンス向上のためUILineConnectionインスタンスを再利用
- **キャンバス管理**: 自動的にスクリーンオーバーレイキャンバスを使用
- **IDベース管理**: 文字列IDでラインを管理

### データ構造
```csharp
Dictionary<string, UILineConnection> activeLines
```

- キー: ラインID（文字列）
- 値: UILineConnectionインスタンス

### パブリックメソッド

#### InitLineメソッド
```csharp
public void InitLine(string id, Vector3 startWorldPosition, Vector3 endWorldPosition,
    Vector2 offset = default, bool needUpdateEveryFrame = true, float thickness = 5f, Color color = default)
```
ラインを初期化・作成します。
- `id`: ラインの識別子
- `startWorldPosition`: 開始点のワールド座標
- `endWorldPosition`: 終了点のワールド座標
- `offset`: スクリーン座標オフセット（オプション）
- `needUpdateEveryFrame`: 毎フレーム更新が必要かどうか（オプション）
- `thickness`: ラインの太さ（オプション）
- `color`: ラインの色（オプション）

#### UpdateLineメソッド（オーバーロード）
```csharp
public void UpdateLine(string id, Vector3 startWorldPosition, Vector3 endWorldPosition)
public void UpdateLine(string id, Vector3 startWorldPosition)
public void UpdateLineEnd(string id, Vector3 endWorldPosition)
public void UpdateLineColor(string id, Color color)
public void UpdateLineThickness(string id, float thickness)
```
既存のラインを更新します。

#### 削除メソッド
```csharp
public void RemoveLine(string id)      // 指定IDのラインを削除
public void ClearAllLines()           // すべてのラインを削除
```

### 自動機能
- **位置更新**: 毎フレームで動的ラインの位置を更新
- **フレーム管理**: 更新されていないラインを自動的に非アクティブ化
- **プーリング**: 未使用ラインをオブジェクトプールに戻す
- **座標変換**: ワールド座標からスクリーン座標への自動変換

## 使用例

### 基本的な使用
```csharp
// 毎フレーム更新されるライン
NLine.Instance.InitLine("connection", startPos, endPos, Vector2.zero, true, 3f, Color.red);

// 静的ライン（位置固定）
NLine.Instance.InitLine("static_line", fixedStart, fixedEnd, Vector2.zero, false, 5f, Color.blue);
```

### 更新
```csharp
// 両端の位置を更新
NLine.Instance.UpdateLine("connection", newStartPos, newEndPos);

// 開始位置のみ更新
NLine.Instance.UpdateLine("connection", newStartPos);

// 終了位置のみ更新
NLine.Instance.UpdateLineEnd("connection", newEndPos);

// 色を更新
NLine.Instance.UpdateLineColor("connection", Color.green);

// 太さを更新
NLine.Instance.UpdateLineThickness("connection", 8f);
```

### 削除
```csharp
// 特定ライン削除
NLine.Instance.RemoveLine("connection");

// 全ライン削除
NLine.Instance.ClearAllLines();
```

## ラベル・ラインタイプ

### 動的タイプ（`needUpdateEveryFrame = true`）
- 毎フレーム位置とフレームカウントを更新
- LateUpdateで更新がなかった場合非アクティブ化
- 移動するオブジェクト間の接続などに適する

### 静的タイプ（`needUpdateEveryFrame = false`）
- 位置更新なし、フレームカウント更新なし
- LateUpdateでの非アクティブ化なし
- 固定されたガイドラインなどに適する 