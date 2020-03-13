using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class StephStartCinematiqueManager : MonoBehaviour
{

    [SerializeField]
    private GameObject Steph;

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private Transform endPoint1;

    [SerializeField]
    private Transform endPoint2;

    [SerializeField]
    private Transform endPoint3;

    [SerializeField]
    private Transform endPoint31;

    [SerializeField]
    private Transform endPoint4;

    [SerializeField]
    private Transform endPoint5;

    [SerializeField]
    private Transform EndpointLookE;


    [Title("Dialogue Manager")]
    [SerializeField]
    private GlobalDialogueManager dialogueManager;

    [SerializeField]
    private TextPopup textPopupToShow;


    public UnityEvent OnSurprised;

    public UnityEvent OnAgree;

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
        yield return new WaitForSeconds(1.5f);
        
        //look first destinqtion
        Steph.transform.LookAt(endPoint1);
        //Move first destination
        Steph.transform.DOMove(endPoint1.position, 10f).SetEase(Ease.Linear);
        //jambes marches
        Steph.GetComponent<Animator>().SetBool("moving", true);
        yield return new WaitForSeconds(2f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.PrintTextInPopup(textPopupToShow);
        }

        yield return new WaitForSeconds(4f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.NextText();
        }
        yield return new WaitForSeconds(4f);

        //traverse Etienne
        Steph.GetComponent<Animator>().SetBool("surprised", true);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.NextText();
        }

        //yield return new WaitForSeconds(0.5f);

        if (OnSurprised != null)
        {
            OnSurprised.Invoke();
        }

        //s'arrête
        Steph.GetComponent<Animator>().SetBool("moving", false);
        
        
        yield return new WaitForSeconds(3f);
        


        //repart lentement
        Steph.GetComponent<Animator>().SetBool("moving", true);
        Steph.GetComponent<Animator>().SetFloat("walkSpeed", 0.4f);
        //raz bool surprised
        Steph.GetComponent<Animator>().SetBool("surprised", false);
        //move second destination
        Steph.transform.DOMove(endPoint2.position, 4.5f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(0.5f);
        if (dialogueManager.componentCache != null && textPopupToShow != null)
        {
            dialogueManager.componentCache.HidePopup();
        }
        yield return new WaitForSeconds(4f);

        //s'arrête
        Steph.GetComponent<Animator>().SetBool("moving", false);
        //lumiere s'allume : surprise
        Steph.GetComponent<Animator>().SetBool("surprised", true);
        if (OnSurprised != null)
        {
            OnSurprised.Invoke();
        }

        yield return new WaitForSeconds(11f);
        Steph.GetComponent<Animator>().SetBool("surprised", false);
        Steph.GetComponent<Animator>().SetBool("turn", false);


        //Tp pour entree dans miroir
        Steph.transform.DOMove(endPoint4.position, 0.2f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        //marche vitessse normale
        Steph.GetComponent<Animator>().SetFloat("walkSpeed", 1f);
        Steph.GetComponent<Animator>().SetBool("moving", true);
        //haut en idle
        Steph.GetComponent<Animator>().SetBool("lastMove", true);
        //avance dans miroir
        Steph.transform.DOMove(endPoint5.position, 5f).SetEase(Ease.Linear);
        Steph.transform.DOLookAt(endPoint5.position, 5f);

        yield return new WaitForSeconds(0.3f);

        Steph.GetComponent<Animator>().SetBool("lastMove", false);


        yield return new WaitForSeconds(4.7f);

        //look etienne miroir
        Steph.transform.DOLookAt(EndpointLookE.position, 1f);
        Steph.GetComponent<Animator>().SetBool("moving", false);
        Steph.GetComponent<Animator>().SetBool("idle", true);

        yield return new WaitForSeconds(5f);

        Steph.GetComponent<Animator>().SetBool("idle", false);
        Steph.GetComponent<Animator>().SetBool("agree", true);
        if (OnAgree!= null)
        {
            OnAgree.Invoke();
        }

    }
}
