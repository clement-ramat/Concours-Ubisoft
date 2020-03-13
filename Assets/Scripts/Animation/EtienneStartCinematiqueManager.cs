using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class EtienneStartCinematiqueManager : MonoBehaviour
{

    [SerializeField]
    private GameObject Etienne;

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private Transform endPoint1;

    [SerializeField]
    private Transform endPoint2;

    [SerializeField]
    private Transform EndpointLookS;

    [SerializeField]
    private GameObject Steph;

    public UnityEvent OnMumbling;

    [Title("Dialogue Manager")]
    [SerializeField]
    private GlobalDialogueManager dialogueManager;

    [SerializeField]
    private TextPopup textPopupToShow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(Animation());
        }
    }

    IEnumerator Animation()
    {
        //Regarde devant + avance
        Etienne.transform.LookAt(endPoint1);
        Etienne.transform.DOMove(endPoint1.position, 3f).SetEase(Ease.Linear);
        //animation cigare
        Etienne.GetComponent<Animator>().SetBool("smoking", true);
        
        yield return new WaitForSeconds(11f);

        //steph le traverse
        Etienne.GetComponent<Animator>().SetBool("surprised", true);

        yield return new WaitForSeconds(2.3f);

        //regarde Steph
        Etienne.transform.DOLookAt(new Vector3(0, 0, -90),0.8f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(3f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.PrintTextInPopup(textPopupToShow);
        }

        yield return new WaitForSeconds(1f);

        //raz bool smoking
        Etienne.GetComponent<Animator>().SetBool("smoking", false);

        //salle miroir
        Etienne.transform.DOLookAt(endPoint2.position,0.5f).SetEase(Ease.InOutSine);
        Etienne.transform.DOMove(endPoint2.position, 8f).SetEase(Ease.Linear);
        

        yield return new WaitForSeconds(3f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.HidePopup();
        }
        //raz bool surprised
        Etienne.GetComponent<Animator>().SetBool("surprised", false);
        yield return new WaitForSeconds(3f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.ShowPopup();
            dialogueManager.componentCache.NextText();
        }
        
        yield return new WaitForSeconds(2f);
        
        //regarde steph qui arrive
        Etienne.transform.DOLookAt(new Vector3(-90,0,0), 1f).SetEase(Ease.InOutSine);
        Etienne.transform.DOLookAt(EndpointLookS.position, 4f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(5f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.HidePopup();
        }
        yield return new WaitForSeconds(7f);

        //pointe
        Etienne.GetComponent<Animator>().SetBool("pointing", true);

        yield return new WaitForSeconds(1f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.ShowPopup();
            dialogueManager.componentCache.NextText();
        }

        //raz pointing
        Etienne.GetComponent<Animator>().SetBool("pointing", false);

        yield return new WaitForSeconds(6f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.HidePopup();
        }

    }
}
