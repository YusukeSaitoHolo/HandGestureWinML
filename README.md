# HandGestureWinML
HoloLens2 で手の動作認識を行うサンプルです。

（認識動作：bloom, Throw up, bubble(huwahuwa), other）

## Usage
- リポジトリをクローン、TextMeshProをインポートしてください
- HoloLens2用に各種設定を行い、ビルドしてslnプロジェクトを生成、ビルドしてください

- HoloLens2実機のPicturesフォルダ以下にHandGestureModelフォルダを作成してください
- Unityプロジェクト側StreamingAssets以下にあるhandgesture_v001.onnxを、先ほど作成したHandGestureModelフォルダ以下にコピーしてください


## Requirements

Unity 2019.4.x with UWP Platform
MRTK (submodule) -> Unityプロジェクト内にシンボリックリンクを貼ってください
