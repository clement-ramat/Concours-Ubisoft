using UnityEngine;
using UnityEngine.UI;
using Rewired;

namespace Michsky.UI.Dark
{
    public class ScrollGamepadManager : MonoBehaviour
    {
        [Header("SLIDER")]
        public Scrollbar scrollbarObject;
        public float changeValue = 0.05f;

        [Header("INPUT")]
        public string inputAxis = "Xbox Right Stick Vertical";
        public bool invertAxis = false;

        void Update()
        {
            float h = ReInput.players.GetPlayer(0).GetAxis("UIHorizontal");

      if (invertAxis == false)
            {
                if (h == 1)
                    scrollbarObject.value -= changeValue;

                else if (h == -1)
                    scrollbarObject.value += changeValue;
            }

            else
            {
                if (h == 1)
                    scrollbarObject.value += changeValue;

                else if (h == -1)
                    scrollbarObject.value -= changeValue;
            }
        }
    }
}