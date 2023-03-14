using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DemoAR
{
    public class XRObjectController : MonoBehaviour
    {
        private int _refSize = 5;


        private void Start()
        {

        }


        private void OptimizeObjectSize()
        {
            var objectSize = new Vector3();

            Debug.Log($"[{nameof(XRObjectController)}] {nameof(OptimizeObjectSize)} object size: {objectSize}");

            List<float> boundSizes = new List<float>() { objectSize.x, objectSize.y, objectSize.z };
            var maxSize = boundSizes.Max();

            if (maxSize < _refSize)
                return;

            var updatedScale = 1 / (maxSize / _refSize);

            gameObject.transform.GetChild(0).transform.localScale = new Vector3(updatedScale, updatedScale, updatedScale);

            UpdatePosition();
        }


        private void UpdatePosition()
        {
            var updatedPosition = Globals.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));

            gameObject.transform.localPosition = updatedPosition;
        }
    }
}
