using UnityEngine;

namespace HandGestureWinML
{
    [System.Serializable]
    public class HandTriangleData
    {
        public int frameCount;
        public float triangleArea;
        public Vector3 triangleNormal;

        public HandTriangleData(int _frame, float _area, Vector3 _normal)
        {
            frameCount = _frame;
            triangleArea = _area;
            triangleNormal = _normal;
        }
    }

}

