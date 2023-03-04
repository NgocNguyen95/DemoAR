using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DemoAR
{
    public class EventManager
    {
        private static EventManager _instance;

        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EventManager();

                return _instance;
            }
        }


        public UnityEvent<SketchfabModel> eventSketchfabCellClicked = new EventSketchfabCellClicked();
    }


    public class EventSketchfabCellClicked : UnityEvent<SketchfabModel> { }
}
