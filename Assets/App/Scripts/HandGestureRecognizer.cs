using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

#if WINDOWS_UWP
using Windows.AI.MachineLearning;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

using TMPro;


namespace HandGestureWinML
{
    public class HandGestureRecognizer : MonoBehaviour
    {
        [SerializeField]
        HandJointController handJointController;

        [SerializeField]
        TextMeshPro ActionLabelText;

        private string dirName = "HandGestureModel";
        private string fileName = "handgesture_v001.onnx";

        private readonly int frameCount = 30;
        private readonly int columns = 4;
        private HandGestureModel handGestureModelGen;
        private HandGestureInput handGestureInput;

        private async void Start()
        {
            handJointController.SetShowArea(true);

            await LoadModelAsync();
            handGestureInput = new HandGestureInput();
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


        private async void Update()
        {
            ActionLabelText.text = await RecognizeGesture();
        }

        private async Task<string> RecognizeGesture()
        {
#if WINDOWS_UWP
            if (handGestureModelGen == null)
            {
                return "cant model loaded";
            }

            if (handJointController.handGestureData == null)
            {
                return "please hand gesture...";
            }

            float[] data = ReadToFloatArray(handJointController.handGestureData);
            handGestureInput.Input120 = TensorFloat.CreateFromArray(new long[] { 1, columns * frameCount }, data);

            //Evaluate the model
            var handGestureOutput = await handGestureModelGen.EvaluateAsync(handGestureInput);
            IList<string> stringList = handGestureOutput.label.GetAsVectorView().ToList();
            //Debug.Log($"判定された動作ラベル:{stringList[0]}"); //IL2CPP 確認用
            var label = stringList[0];
            return label;
#else
            return "None";
#endif
        }
        private float[] ReadToFloatArray(HandGestureData gestureData)
        {
            float[] ret = new float[columns * frameCount];

            for (int i = 0; i < gestureData.gestureData.Length; i++)    //i = frame
            {
                int currentIndex = i * columns;
                ret[currentIndex] = gestureData.gestureData[i].triangleArea;
                ret[currentIndex + 1] = gestureData.gestureData[i].triangleNormal.x;
                ret[currentIndex + 2] = gestureData.gestureData[i].triangleNormal.y;
                ret[currentIndex + 3] = gestureData.gestureData[i].triangleNormal.z;
            }

            return ret;
        }
    }

}

