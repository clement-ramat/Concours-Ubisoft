using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDButton : MonoBehaviour
{
  [SerializeField]
  private GameObject textLettre;

  public GameObject textAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ChangeActionText(string newText)
  {
    textAction.GetComponent<TextMeshProUGUI>().text = newText;
  }

  public void DisableColorLetter()
  {
    textLettre.GetComponent<TextMeshProUGUI>().color = Color.gray;
  }

  public void EnableColorLetter()
  {
    textLettre.GetComponent<TextMeshProUGUI>().color = Color.white;
  }
}
