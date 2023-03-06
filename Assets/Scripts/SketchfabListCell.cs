using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace DemoAR
{
    public class SketchfabListCell : MonoBehaviour
    {
        [SerializeField] RawImage thumbnail;
        [SerializeField] TMP_Text modelName;

        private SketchfabModel _sketchfabModel;


        public void Init(SketchfabModel model, float cellWidth, float cellHeight)
        {
            _sketchfabModel = model;
            var fitThumbnail = model.Thumbnails.ClosestThumbnailToSizeWithoutGoingBelow((int)cellWidth, (int)cellHeight);
            StartCoroutine(LoadThumbnailCoroutine(fitThumbnail));
            LoadModelInfo();
        }


        public void OnClickCell()
        {
            EventManager.Instance.eventSketchfabCellClicked.Invoke(_sketchfabModel);
        }


        private IEnumerator LoadThumbnailCoroutine(SketchfabThumbnail sketchfabThumbnail)
        {
            var thumbnailURL = sketchfabThumbnail.Url;

            using(UnityWebRequest request = UnityWebRequestTexture.GetTexture(thumbnailURL))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[{nameof(SketchfabListCell)}] {nameof(LoadThumbnailCoroutine)} error: {request.error}");
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(request);
                    thumbnail.texture = texture;

                    thumbnail.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = texture.width / (float)texture.height;
                }
            }
        }


        private void LoadModelInfo()
        {
            modelName.text = _sketchfabModel.Name;
        }


        private void Clear()
        {
            if (thumbnail.texture != null)
                Destroy(thumbnail.texture);
        }


        private void OnDestroy()
        {
            Clear();
        }
    }
}
