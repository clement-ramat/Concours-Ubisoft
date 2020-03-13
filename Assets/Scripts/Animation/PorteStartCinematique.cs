using System.Collections;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PorteStartCinematique : MonoBehaviour
{

    [SerializeField]
    private Transform rightPivot;

    [SerializeField]
    private Transform leftPivot;

    [SerializeField]
    private GameObject AmbianceLight;

    [SerializeField]
    private GameObject SpotPorte1;

    [SerializeField]
    private GameObject SpotPorte2;

    [SerializeField]
    private GameObject SpotAmbiance;

    [SerializeField]
    private GameObject cam1;
    

    [SerializeField]
    private float openingDuration = 2;

    [SerializeField]
    private Transform endPoint1cam1;

    [SerializeField]
    private Transform endPoint1cam2;

    [SerializeField]
    private Transform lookCam;

    [SerializeField]
    private Transform finalPoint1cam;

    [SerializeField]
    protected List<GameObject> murs = new List<GameObject>();

    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public UnityEvent OnSwitchOn;

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
        //Ouvrir porte
        rightPivot.DOLocalRotate(new Vector3(0, -90, 0), openingDuration).SetEase(Ease.OutSine);
        leftPivot.DOLocalRotate(new Vector3(0, 90, 0), openingDuration).SetEase(Ease.OutSine);

        if (OnOpen != null)
        {
            OnOpen.Invoke();
        }

        yield return new WaitForSeconds(17f);

        //traveling avant
        cam1.transform.DOMove(endPoint1cam1.position, 12f).SetEase(Ease.InSine);
        cam1.transform.DOLookAt(new Vector3(90, 0, 0), 10f).SetEase(Ease.InSine);

        yield return new WaitForSeconds(2f);

        //Allumer lumiere 1
        SpotPorte1.SetActive(true);
        if (OnSwitchOn != null)
        {
            OnSwitchOn.Invoke();
        }

        yield return new WaitForSeconds(1f);

        //allumer lumières 2 et 3
        SpotAmbiance.SetActive(true);
        if (OnSwitchOn != null)
        {
            OnSwitchOn.Invoke();
        }
        SpotPorte2.SetActive(true);
        if (OnSwitchOn != null)
        {
            OnSwitchOn.Invoke();
        }
        //activer + eteindre lumiere 4
        AmbianceLight.SetActive(true);
        AmbianceLight.GetComponent<Light>().intensity = 0;


        yield return new WaitForSeconds(5f);

        //allumer progressivement lumiere 4
        AmbianceLight.GetComponent<Light>().DOIntensity(0.73f, 5);

        yield return new WaitForSeconds(1f);

        //look derniere position camera
        cam1.transform.DOLocalRotate(new Vector3(0, 0, 0), 10f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(2f);

        //traveling lateral → derniere pos
        cam1.transform.DOMove(endPoint1cam2.position, 8f).SetEase(Ease.OutSine);

        yield return new WaitForSeconds(6f);

        //fermer porte
        rightPivot.DOLocalRotate(new Vector3(0, 0, 0), openingDuration).SetEase(Ease.OutSine);
        leftPivot.DOLocalRotate(new Vector3(0, 0, 0), openingDuration).SetEase(Ease.OutSine);
        if (OnClose != null)
        {
            OnClose.Invoke();
        }
        //rendre murs invisibles
        murs.ForEach(_object =>
        {

            _object.gameObject.layer= LayerMask.NameToLayer("MirrorWall");

        });


        yield return new WaitForSeconds(6f);

        //
        cam1.transform.DOMove(finalPoint1cam.position, 10f).SetEase(Ease.InOutSine);
        cam1.transform.DOLocalRotate(new Vector3(22.271f, 0, 0), 10f).SetEase(Ease.InOutSine);
    }
}
