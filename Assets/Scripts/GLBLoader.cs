using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using System.IO.Compression;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

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


        private void LoadGlbWithMrtk(SketchfabModel sketchfabModel)
        {
            SketchfabAPI.GetModel(sketchfabModel.Uid, (resp) =>
            {
                if (!resp.Success)
                {
                    Debug.Log($"[{nameof(SketchfabListCell)}] {nameof(LoadGlbWithMrtk)} error: {resp.ErrorMessage}");
                    return;
                }

                SketchfabAPI.GetGLTFModelDownloadUrl(sketchfabModel.Uid, (_modelDownloadUrl) =>
                {
                    if (!_modelDownloadUrl.Success)
                    {
                        Debug.Log($"[{nameof(GLBLoader)}] {nameof(LoadGlbWithMrtk)} error: {_modelDownloadUrl.ErrorMessage}");
                        return;
                    }

                    Import(resp.Object, _modelDownloadUrl.Object, (obj) =>
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
            });
        }


        private void Import(SketchfabModel _model, string _downloadUrl, Action<GameObject> _onModelImported)
        {
            UnityWebRequest downloadRequest = UnityWebRequest.Get(_downloadUrl);

            SketchfabWebRequestManager.Instance.SendRequest(downloadRequest, (UnityWebRequest _request) =>
            {
                if (downloadRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(downloadRequest.error);

                    _onModelImported?.Invoke(null);

                    return;
                }

                // Lock the temporary folder for all following operations to
                // avoid it from flushing itself in the middle of it
                m_Temp.Lock();

                try
                {
                    string archivePath = Path.Combine(m_Temp.AbsolutePath, _model.Uid);
                    // Make sure to save again the model if downloaded twice
                    if (Directory.Exists(archivePath))
                    {
                        Directory.Delete(archivePath, true);
                    }

                    using (ZipArchive zipArchive = new ZipArchive(new MemoryStream(downloadRequest.downloadHandler.data), ZipArchiveMode.Read))
                    {
                        zipArchive.ExtractToDirectory(archivePath);
                    }


                    SaveModelMetadata(archivePath, _model);
                    LoadGlbWithMrtkAsync($"{Path.Combine(archivePath, "scene.gltf")}", (GameObject _importedModel) =>
                    {
                        _onModelImported?.Invoke(_importedModel);
                    });
                }
                finally
                {
                    // No matter what happens, realse the lock so that
                    // it doesn't get stuck
                    m_Temp.Unlock();
                }

            });
        }


        private void SaveModelMetadata(string _destination, SketchfabModel _model)
        {
            // Write the model metadata in order to avoid server queries
            File.WriteAllText(Path.Combine(_destination, string.Format("{0}_metadata.json", _model.Uid)), _model.GetJsonString());
        }


        private async void LoadGlbWithMrtkAsync(string url, Action<GameObject> _onModelImported)
        {
            GltfObject gltfObject = null;

            try
            {
                Debug.Log($"[{nameof(GLBLoader)}] {nameof(LoadGlbWithMrtkAsync)} file path: {url}");
                gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync(url);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(GLBLoader)}] {nameof(LoadGlbWithMrtkAsync)} error: {e.Message}\n{e.StackTrace}");
            }

            if (gltfObject == null)
            {
                Debug.Log($"[{nameof(GLBLoader)}] {nameof(LoadGlbWithMrtkAsync)} Import unsuccessful!");
                _onModelImported?.Invoke(null);
                return;
            }

            _onModelImported?.Invoke(gltfObject.GameObjectReference);
        }
    }
}
