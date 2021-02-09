using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


#if WINDOWS_UWP
using Windows.AI.MachineLearning;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace HandGestureWinML
{
    /// <summary>
    /// Windows.AI.MachineLearning確認用サンプルコード
    /// </summary>
    public class HandGestureSample : MonoBehaviour
    {
        private readonly int frameCount = 30;
        private readonly int columns = 4;
        private HandGestureModel handGestureModelGen;
        private HandGestureInput handGestureInput;

        private string dirName = "HandGestureModel";
        private string fileName = "handgesture_v001.onnx";


        private async void Start()
        {
            await LoadModelAsync();

            await LoadSampleBloom();
        }



        private async Task LoadModelAsync()
        {
#if WINDOWS_UWP

            var dir = await KnownFolders.PicturesLibrary.GetFolderAsync(dirName);
            var file = await dir.GetFileAsync(fileName);
            // モデルのロード
            handGestureModelGen = await HandGestureModel.CreateFromStreamAsync(file as IRandomAccessStreamReference);

            if (handGestureModelGen == null)
            {
                Debug.LogError("モデルデータの読み込みに失敗しました. ");
            }
#endif
        }

        private async Task LoadSampleBloom()
        {
#if WINDOWS_UWP
            Debug.Log("File Load");
            var dir = await KnownFolders.PicturesLibrary.GetFolderAsync(dirName);
            var file = await dir.GetFileAsync("bloom[2020_4_13-8_45_52_183].json");
            var jsonData = await FileIO.ReadTextAsync(file);
            HandGestureData handGestureData = JsonUtility.FromJson<HandGestureData>(jsonData);
            Debug.Log("File Loaded");

            //Jsonデータから特徴ベクトルを生成する
            var feature = GenerateFloatArrayFromJsonData(handGestureData);

            HandGestureInput input = new HandGestureInput();
            input.Input120 = TensorFloat.CreateFromArray(new long[] { 1, columns * frameCount }, feature);

            //Evaluate the model
            var handGestureOutput = await handGestureModelGen.EvaluateAsync(input);
            IList<string> stringList = handGestureOutput.label.GetAsVectorView().ToList();
            Debug.Log($"判定された動作ラベル:{stringList[0]}");
#endif

        }

        /// <summary>
        /// Jsonデータからモデルの入力に使用する特徴データへ変換する関数
        /// </summary>
        /// <param name="jsonData">ロードするNewtonsoft.Jsonデータ</param>
        /// <returns> モデルの入力に使用する特徴データ</returns>
        private float[] GenerateFloatArrayFromJsonData(HandGestureData handGestureData)
        {
            var gestureData = handGestureData.gestureData;

            float[] feature = new float[columns * frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                feature[i * columns] = gestureData[i].triangleArea;
                feature[i * columns + 1] = gestureData[i].triangleNormal.x;
                feature[i * columns + 2] = gestureData[i].triangleNormal.y;
                feature[i * columns + 3] = gestureData[i].triangleNormal.z;
            }

            return feature;
        }
    }
}
