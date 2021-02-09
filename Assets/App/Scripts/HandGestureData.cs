using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandGestureWinML
{
    [System.Serializable]
    public class HandGestureData
    {
        public HandTriangleData[] gestureData;

        public HandGestureData(HandTriangleData[] triangleData, int count)
        {
            gestureData = new HandTriangleData[count];
            gestureData = triangleData;
        }

        public void SetHandGestureData(HandTriangleData[] triangleData)
        {
            gestureData = triangleData;
        }
    }

}

