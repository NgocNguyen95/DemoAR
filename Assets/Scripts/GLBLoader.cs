using System.IO;
using UnityEngine;

namespace DemoAR
{
    public class GLBLoader : MonoBehaviour
    {
        private const string m_SketchfabModelTemporaryDownloadFolderName = "SketchfabModelTemp";
        private SketchfabModelDiskTemp m_Temp;


        private void Awake()
        {
            AddListeners();
        }


        private void Start()
        {
            m_Temp = new SketchfabModelDiskTemp(Path.Combine(Application.streamingAssetsPath, m_SketchfabModelTemporaryDownloadFolderName), 10.0f);            
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
                        var parentObject = new GameObject("GLBModel");
                        obj.transform.parent = parentObject.transform;

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
