using UnityEngine;

namespace HandGestureWinML
{
    public class VisalizerForHandGesture : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRendererThumbToMiddle;

        [SerializeField]
        private LineRenderer lineRendererMiddleToPinky;

        [SerializeField]
        private LineRenderer lineRendererPinkyToThumb;

        [SerializeField]
        private LineRenderer lineRendererOriginalNormal;

        [SerializeField]
        private LineRenderer lineRendererTransedNormal;

        [SerializeField]
        private float visualizeScaleforArea = 100 * 100 / 1000;   //[m^2→cm^2] * offset

        private void Awake()
        {
            lineInitialize(lineRendererThumbToMiddle);
            lineInitialize(lineRendererMiddleToPinky);
            lineInitialize(lineRendererPinkyToThumb);
            lineInitialize(lineRendererOriginalNormal);
            lineInitialize(lineRendererTransedNormal);
        }

        private void lineInitialize(LineRenderer lr)
        {
            lr.startWidth = 0.001f;
            lr.endWidth = 0.001f;
            lr.startColor = Color.green;
            lr.endColor = Color.green;
            lr.positionCount = 2;
        }

        public void SetLinePositions(GameObject rightThumbTip, GameObject rightMiddleTip, GameObject rightPinkyTip)
        {
            lineRendererThumbToMiddle.enabled = true;
            lineRendererMiddleToPinky.enabled = true;
            lineRendererPinkyToThumb.enabled = true;

            lineRendererThumbToMiddle.SetPosition(0, rightThumbTip.transform.position);
            lineRendererThumbToMiddle.SetPosition(1, rightMiddleTip.transform.position);

            lineRendererMiddleToPinky.SetPosition(0, rightMiddleTip.transform.position);
            lineRendererMiddleToPinky.SetPosition(1, rightPinkyTip.transform.position);

            lineRendererPinkyToThumb.SetPosition(0, rightPinkyTip.transform.position);
            lineRendererPinkyToThumb.SetPosition(1, rightThumbTip.transform.position);

        }

        public void SetLineNormals(Vector3 inputValue, Vector3 transValue, GameObject rightThumbTip, GameObject rightMiddleTip, GameObject rightPinkyTip)
        {
            Vector3 centerTrianglePos = (rightThumbTip.transform.position + rightMiddleTip.transform.position + rightPinkyTip.transform.position) / 3.0f;

            Vector3 centerFrontPos = new Vector3(0.0f, 0.0f, 0.5f);

            lineRendererOriginalNormal.enabled = true;
            lineRendererTransedNormal.enabled = true;


            lineRendererOriginalNormal.SetPosition(0, centerTrianglePos);
            lineRendererOriginalNormal.SetPosition(1, centerTrianglePos + inputValue * 0.1f);

            lineRendererTransedNormal.SetPosition(0, centerFrontPos);
            lineRendererTransedNormal.SetPosition(1, centerFrontPos + transValue * 0.1f);
        }


    }

}

