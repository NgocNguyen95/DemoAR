using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace DemoAR
{
    public class SketchfabScrollView : MonoBehaviour
    {
        [SerializeField] ScrollRect scrollView;
        [SerializeField] GameObject sketchfabListCellPrefab;
        [SerializeField] GridLayoutGroup gridLayoutGroup;

        private float _cellWidth;
        private float _cellHeight;
        private SketchfabModelList _sketchfabModelList;


        public void Init()
        {
            LoadModelList();
        }


        public void OnTapClose()
        {
            Clear();
            _sketchfabModelList = null;
            scrollView.gameObject.SetActive(false);
        }


        public void OnTapNextPage()
        {
            LoadModelList();
        }


        private void Awake()
        {
            _cellWidth = gridLayoutGroup.cellSize.x;
            _cellHeight= gridLayoutGroup.cellSize.y;

            AddListeners();
        }


        private void AddListeners()
        {
            EventManager.Instance.eventSketchfabCellClicked.AddListener(CloseScrollView);
        }


        private void Clear()
        {
            var childCount = scrollView.content.childCount;

            if ( childCount > 0 )
            {
                for (int i = childCount - 1; i >= 0; i--)
                {
                    Destroy(scrollView.content.GetChild(i).gameObject);
                }
            }

            scrollView.verticalNormalizedPosition = 1;
        }


        private void CloseScrollView(SketchfabModel unsed)
        {
            Clear();
            _sketchfabModelList = null;
            scrollView.gameObject.SetActive(false);
        }


        private void LoadModelList()
        {
            if (!SketchfabAPI.Authorized)
                SketchfabAPI.AuthorizeWithAPIToken(Config.SKETCHFAB_API_TOKEN);

            Clear();

            if (_sketchfabModelList == null)
            {
                LoadFirstModelList();
            }
            else
            {
                LoadNextPageModelList();
            }
        }


        private void LoadFirstModelList()
        {
            UnityWebRequestSketchfabModelList.Parameters p = new UnityWebRequestSketchfabModelList.Parameters();
            p.downloadable = true;
            p.sortBy = "-viewCount";
            SketchfabAPI.GetModelList(p,
                (SketchfabResponse<SketchfabModelList> _answer) =>
                {
                    if (!_answer.Success)
                    {
                        Debug.LogError($"[{nameof(SketchfabScrollView)}] {nameof(LoadFirstModelList)} error: {_answer.ErrorMessage}");
                        return;
                    }

                    SketchfabResponse<SketchfabModelList> ans = _answer;
                    _sketchfabModelList = ans.Object;
                    Debug.Log($"[{nameof(SketchfabScrollView)}] {nameof(LoadFirstModelList)} details: {ans.Object}");

                    foreach (var model in ans.Object.Models)
                    {
                        SketchfabAPI.GetModel(model.Uid, (reps) =>
                        {
                            if (!reps.Success)
                                return;

                            var sketchfabListCell = Instantiate(sketchfabListCellPrefab, scrollView.content.transform);
                            sketchfabListCell.GetComponent<SketchfabListCell>().Init(model, _cellWidth, _cellHeight);
                        });
                    }
                }
            );
        }


        private void LoadNextPageModelList()
        {
            SketchfabAPI.GetNextModelListPage(_sketchfabModelList,
                (SketchfabResponse<SketchfabModelList> _answer) =>
                {
                    if (!_answer.Success)
                    {
                        Debug.LogError($"[{nameof(SketchfabScrollView)}] {nameof(LoadNextPageModelList)} error: {_answer.ErrorMessage}");
                        return;
                    }

                    SketchfabResponse<SketchfabModelList> ans = _answer;
                    _sketchfabModelList = ans.Object;
                    Debug.Log($"[{nameof(SketchfabScrollView)}] {nameof(LoadNextPageModelList)} details: {ans.Object}");

                    foreach (var model in ans.Object.Models)
                    {
                        //SketchfabAPI.GetModel(model.Uid, (reps) =>
                        //{
                        //    if (!reps.Success)
                        //        return;

                        //    var sketchfabListCell = Instantiate(sketchfabListCellPrefab, scrollView.content.transform);
                        //    sketchfabListCell.GetComponent<SketchfabListCell>().Init(model, _cellWidth, _cellHeight);
                        //});

                        var sketchfabListCell = Instantiate(sketchfabListCellPrefab, scrollView.content.transform);
                        sketchfabListCell.GetComponent<SketchfabListCell>().Init(model, _cellWidth, _cellHeight);
                    }
                }
            );
        }
    }
}
