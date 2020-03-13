using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuControllerNavigation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  public void SelectNextButtonController(GameObject button)
  {
    EventSystem.current.SetSelectedGameObject(button);
  }

  public void ExitMenu()
  {
    Application.Quit();
  }
}
