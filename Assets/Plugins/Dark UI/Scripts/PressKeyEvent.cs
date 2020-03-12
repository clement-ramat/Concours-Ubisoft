using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Michsky.UI.Dark
{
    public class PressKeyEvent : MonoBehaviour
    {
        [Header("KEY")]
        [SerializeField]
        public KeyCode hotkey;
        public bool pressAnyKey;

        [Header("KEY ACTION")]
        [SerializeField]
        public UnityEvent pressAction;

    [Header("NEXT SELECTABLE BUTTON")]
    [SerializeField]
    public GameObject button;

    void Update()
        {

      
      if (pressAnyKey == true)
            {
                if (Input.anyKeyDown)
                {
          EventSystem.current.SetSelectedGameObject(button);
          pressAction.Invoke();
                } 
            }

            else
            {
                if (Input.GetKeyDown(hotkey))
                {
          EventSystem.current.SetSelectedGameObject(button);
          pressAction.Invoke();
                } 
            }
        }
    }
}