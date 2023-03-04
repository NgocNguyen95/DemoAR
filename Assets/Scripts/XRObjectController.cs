using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.UI;
using System.Linq;

namespace DemoAR
{
    public class XRObjectController : MonoBehaviour
    {
        BoundsControl _boundsControl;
        ObjectManipulator _objectManipulator;

        private int _refSize = 5;


        private void Start()
        {
            AddBoundsControl();
            AddObjectManipulator();
        }


        private void AddBoundsControl()
        {
            _boundsControl = gameObject.AddComponent<BoundsControl>();

            ConfigBoundsControl();

            OptimizeObjectSize();
        }


        private void ConfigBoundsControl()
        {
            _boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;
        }


        private void AddObjectManipulator()
        {
            _objectManipulator = gameObject.AddComponent<ObjectManipulator>();

            ConfigObjectManipulator();
        }


        private void ConfigObjectManipulator()
        {
            _objectManipulator.OnManipulationStarted.AddListener((med) => _boundsControl.HighlightWires());
            _objectManipulator.OnManipulationEnded.AddListener((med) => _boundsControl.UnhighlightWires());
        }


        private void OptimizeObjectSize()
        {
            var objectSize = _boundsControl.TargetBounds.size;

            Debug.Log($"[{nameof(XRObjectController)}] {nameof(OptimizeObjectSize)} object size: {objectSize}");

            List<float> boundSizes = new List<float>() { objectSize.x, objectSize.y, objectSize.z };
            var maxSize = boundSizes.Max();

            if (maxSize < _refSize)
                return;

            var updatedScale = 1 / (maxSize / _refSize);

            gameObject.transform.GetChild(0).transform.localScale = new Vector3(updatedScale, updatedScale, updatedScale);
            _boundsControl.UpdateBounds();

            UpdatePosition();
        }


        private void UpdatePosition()
        {
            var updatedPosition = Globals.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));

            gameObject.transform.localPosition = updatedPosition;
        }
    }
}
