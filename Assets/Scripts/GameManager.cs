using UnityEngine;

namespace DemoAR
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] SketchfabScrollView sketchfabScrollView;

        public GameObject SelectBoxPrefab;


        private void Awake()
        {
            Globals.mainCamera = Camera.main;
            Instance = this;
        }


        public void OnclickAddButton()
        {
            sketchfabScrollView.gameObject.SetActive(true);
            sketchfabScrollView.Init();
        }
    }
}