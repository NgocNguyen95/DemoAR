using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoAR
{
    public class GLBLoader : MonoBehaviour
    {
        private void Awake()
        {
            AddListeners();
        }


        private void AddListeners()
        {
            EventManager.Instance.eventSketchfabCellClicked.AddListener(LoadSketchfabModel);
        }


        private void LoadSketchfabModel(SketchfabModel sketchfabModel)
        {
            SketchfabAPI.GetModel(sketchfabModel.Uid, (resp) =>
            {
                if (!resp.Success)
                {
                    Debug.Log($"[{nameof(SketchfabListCell)}] {nameof(LoadSketchfabModel)} error: {resp.ErrorMessage}");
                    return;
                }

                SketchfabModelImporter.Import(resp.Object, (obj) =>
                {
                    if (obj != null)
                    {
                        Vector3 updatedPosition = Globals.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));
                        obj.transform.position = updatedPosition;

                        Animation animation = obj.GetComponent<Animation>();
                        if (animation != null)
                            animation.Play();

                        obj.AddComponent<XRObjectController>();
                    }
                });
            });
        }
    }
}
