using System.Collections.Concurrent;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;

namespace HandGestureWinML
{
    public class HandJointController : MonoBehaviour
    {
        public ConcurrentQueue<float> AreaValueFIFO { get; private set; }
        public ConcurrentQueue<Vector3> NormalValueFIFO { get; private set; }

        public HandGestureData handGestureData { get; private set; }

        [SerializeField]
        private GameObject mrtkCamera;

        [SerializeField]
        private VisalizerForHandGesture visualizer;


        [SerializeField]
        private float visualizeScaleforArea = 100 * 100 / 1000;   //[m^2→cm^2] * offset

        [SerializeField]
        public int stockNum { get; private set; } = 30;

        public Transform rightPalmJointTransform { get; private set; }

        private GameObject rightPalmJoint, rightMiddleTip, rightThumbTip, rightPinkyTip;
        private float scale = 0.01f;
        private float areaS;    //[m^2] for bloom
        private bool isShowArea = false;
        private Plane plane;

        HandTriangleData[] handTriangleDataArray;
        HandTriangleData currentHandTriangleData;

        private TrackedHandJoint[] jointNum =
        {
            TrackedHandJoint.Palm,
            TrackedHandJoint.MiddleTip,
            TrackedHandJoint.ThumbTip,
            TrackedHandJoint.PinkyTip
        };

        #region Unity Methods
        private void Awake()
        {
            Time.fixedDeltaTime = 0.033f;

            AreaValueFIFO = new ConcurrentQueue<float>();
            NormalValueFIFO = new ConcurrentQueue<Vector3>();
            handTriangleDataArray = new HandTriangleData[stockNum];
            jointInitialize();
        }

        //30FPSで更新(Fixed Time Step = 0.033)
        private void FixedUpdate()
        {

            if (!isShowArea)
            {
                return;
            }

            GetJointOfHand();   //Hand Jointの値を取ってくる

            plane = CalculatePlane();

            //三角形の描画
            visualizer.SetLinePositions(rightThumbTip, rightMiddleTip, rightPinkyTip);

            areaS = CalculateArea();

            AreaEnqueue(areaS);

            //inputValueの軸は起動時のデバイスの傾きなので、MainCameraの回転を考慮する
            Quaternion invQuaternionOfCamera = Quaternion.Inverse(mrtkCamera.transform.localRotation);
            Vector3 normalConsideringCameraAxis = invQuaternionOfCamera * plane.normal;

            NormalEnqueue(normalConsideringCameraAxis);

            //三角形の法線の描画
            visualizer.SetLineNormals(plane.normal, normalConsideringCameraAxis, rightThumbTip, rightMiddleTip, rightPinkyTip);

            PackHandGestureData();

        }

        #endregion

        public void SetShowArea(bool isShow)
        {
            isShowArea = isShow;
        }

        private void jointInitialize()
        {
            rightPalmJoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightPalmJoint.transform.localScale = new Vector3(1, 1, 1) * scale;

            rightMiddleTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightMiddleTip.transform.localScale = new Vector3(1, 1, 1) * scale;

            rightThumbTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightThumbTip.transform.localScale = new Vector3(1, 1, 1) * scale;

            rightPinkyTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightPinkyTip.transform.localScale = new Vector3(1, 1, 1) * scale;

        }


        private void AreaEnqueue(float inputValue)
        {
            if (AreaValueFIFO.Count() < stockNum)
            {
                AreaValueFIFO.Enqueue(inputValue);
            }
            else
            {
                AreaValueFIFO.Enqueue(inputValue);
                float lastValue = 0.0f;
                AreaValueFIFO.TryDequeue(out lastValue);
            }
        }

        private void NormalEnqueue(Vector3 inputValue)
        {

            if (NormalValueFIFO.Count() < stockNum)
            {
                NormalValueFIFO.Enqueue(inputValue);
            }
            else
            {
                NormalValueFIFO.Enqueue(inputValue);
                Vector3 lastValue = Vector3.zero;
                NormalValueFIFO.TryDequeue(out lastValue);
            }
        }

        private void GetJointOfHand()
        {
            foreach (var num in jointNum)
            {
                if (HandJointUtils.TryGetJointPose(num, Handedness.Right, out MixedRealityPose pose))
                {
                    switch (num)
                    {
                        case TrackedHandJoint.Palm:
                            ModifyFromMixedRealityPose(pose, rightPalmJoint.transform);
                            rightPalmJointTransform = rightPalmJoint.transform;
                            break;
                        case TrackedHandJoint.MiddleTip:
                            ModifyFromMixedRealityPose(pose, rightMiddleTip.transform); break;
                        case TrackedHandJoint.ThumbTip:
                            ModifyFromMixedRealityPose(pose, rightThumbTip.transform); break;
                        case TrackedHandJoint.PinkyTip:
                            ModifyFromMixedRealityPose(pose, rightPinkyTip.transform); break;
                    }
                }
            }
        }

        private void ModifyFromMixedRealityPose(MixedRealityPose pose, Transform transform)
        {
            transform.position = pose.Position;
            transform.rotation = pose.Rotation;
        }

        private float CalculateArea()
        {
            Vector3 D = Vector3.Cross(rightMiddleTip.transform.position - rightThumbTip.transform.position,
                rightPinkyTip.transform.position - rightThumbTip.transform.position);

            return D.magnitude / 2;
        }

        private Plane CalculatePlane()
        {
            Plane ret = new Plane(rightThumbTip.transform.position, rightMiddleTip.transform.position, rightPinkyTip.transform.position);
            return ret;
        }

        private void PackHandGestureData()
        {
            if (AreaValueFIFO.Count < stockNum || NormalValueFIFO.Count < stockNum)
            {
                return;
            }

            float[] areaArray = AreaValueFIFO.ToArray();
            Vector3[] normalArray = NormalValueFIFO.ToArray();

            for (int i = 0; i < stockNum; ++i)
            {
                currentHandTriangleData = new HandTriangleData
                    (
                        i,
                        areaArray[i],
                        normalArray[i]
                    );
                handTriangleDataArray[i] = currentHandTriangleData;
            }

            if (handGestureData == null)
            {
                handGestureData = new HandGestureData(handTriangleDataArray, stockNum);
            }
            else
            {
                handGestureData.SetHandGestureData(handTriangleDataArray);
            }

        }

    }

}

