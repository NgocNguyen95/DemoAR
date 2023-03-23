using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace DemoAR
{
    public class XRObjectController : MonoBehaviour
    {
        private int _optimumObjectSize = 5;
        private Bounds _objectBound;
        private GameObject _selectBox;

        private ARRotationInteractable _rotationInteractable;
        private ARSelectionInteractable _selectionInteractable;
        private ARScaleInteractable _scaleInteractable;
        private ARTranslationInteractable _translationInteractable;


        private void Start()
        {
            OptimizeObjectSize();
        }


        private void AddBoxCollider()
        {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = _objectBound.size;
            boxCollider.center = _objectBound.center;
        }


        private void AddInteractions()
        {
            _selectionInteractable = gameObject.AddComponent<ARSelectionInteractable>();
            _translationInteractable = gameObject.AddComponent<ARTranslationInteractable>();
            _rotationInteractable = gameObject.AddComponent<ARRotationInteractable>();
            _scaleInteractable = gameObject.AddComponent<ARScaleInteractable>();

            ConfigInteractions();
        }


        private void AddSelectBox()
        {
            _selectBox = Instantiate(GameManager.Instance.SelectBoxPrefab, gameObject.transform);
            _selectBox.transform.localScale = _objectBound.size;
            _selectBox.transform.localPosition = _objectBound.center;
        }


        private void ConfigInteractions()
        {
            _translationInteractable.objectGestureTranslationMode = GestureTransformationUtility.GestureTranslationMode.Any;

            _selectionInteractable.selectionVisualization = _selectBox;

            _scaleInteractable.minScale = 0.25f;
            _scaleInteractable.maxScale = 2f;
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

            AddSelectBox();
            AddBoxCollider();
            UpdatePosition();
            AddInteractions();
        }


        private void UpdatePosition()
        {
            var updatedPosition = Globals.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));

            if (gameObject.transform.parent != null)
            {
                gameObject.transform.parent.localPosition = updatedPosition;
            }
        }
    }
}
