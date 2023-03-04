using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DemoAR
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] SketchfabScrollView sketchfabScrollView;


        private void Awake()
        {
            Globals.mainCamera = Camera.main;
        }


        public void OnclickAddButton()
        {
            sketchfabScrollView.gameObject.SetActive(true);
            sketchfabScrollView.Init();
        }
    }
}