using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DemoAR
{
    public class XRObjectController : MonoBehaviour
    {
        private int _optimumObjectSize = 5;
        private Bounds _objectBound;


        private void Start()
        {
            OptimizeObjectSize();
        }


        private void AddBoxCollider()
        {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = _objectBound.size;
        }


        private void OptimizeObjectSize()
        {
            _objectBound = BoundingBoxCalculator.CalculateBoundingBox(gameObject);

            Debug.Log($"[{nameof(XRObjectController)}] {nameof(OptimizeObjectSize)} object size: {_objectBound.size}");

            List<float> boundSizes = new List<float>() { _objectBound.size.x, _objectBound.size.y, _objectBound.size.z };
            var maxSize = boundSizes.Max();

            if (maxSize > _optimumObjectSize)
            {
                var updatedScale = 1 / (maxSize / _optimumObjectSize);

                gameObject.transform.GetChild(0).transform.localScale = new Vector3(updatedScale, updatedScale, updatedScale);

                _objectBound = BoundingBoxCalculator.CalculateBoundingBox(gameObject);
            }

            AddBoxCollider();
            UpdatePosition();
        }


        private void UpdatePosition()
        {
            var updatedPosition = Globals.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));

            gameObject.transform.localPosition = updatedPosition;
        }
    }
}
