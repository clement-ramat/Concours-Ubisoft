using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoystickInteractionHUD : MonoBehaviour
{

    public GameObject turnAnim;
    public GameObject allDirAnim;
    public GameObject leftRightAnim;
    public GameObject textAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateShakeAllDirections()
    {
        turnAnim.SetActive(false);
        leftRightAnim.SetActive(false);
        allDirAnim.SetActive(true);
    }

    public void ActivateShakeLeftRight()
    {
        turnAnim.SetActive(false);
        allDirAnim.SetActive(false);
        leftRightAnim.SetActive(true);
    }

    public void ActivateTurn()
    {
        allDirAnim.SetActive(false);
        leftRightAnim.SetActive(false);
        turnAnim.SetActive(true);
    }

    public void ChangeActionText(string newText)
    {
        textAction.GetComponent<TextMeshProUGUI>().text = newText;
    }
}
