using System.Collections;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PotStartCinematique : MonoBehaviour
{
    [SerializeField]
    private GameObject Pot;

    [SerializeField]
    private GameObject Table;

    [SerializeField]
    private float openingDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            Debug.Log("start");
            StartCoroutine(Animation());
        }
    }

    IEnumerator Animation()
    {
        Debug.Log("here");
        //yield return new WaitForSeconds(1.5f);
        //Pot.transform.DOLocalRotate(new Vector3(-90, 45, 0), openingDuration).SetEase(Ease.OutSine);
        Table.transform.DOMove(Table.transform.position+new Vector3(-0.1f, 0, 0), 0.1f).SetEase(Ease.Linear);
        //Pot.transform.DOMove(Pot.transform.position + new Vector3(-0.1f, 0, 0), 0.1f).SetEase(Ease.Linear);
        Pot.transform.DORotate(new Vector3(0, 0, -90), 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        Table.transform.DOMove(Table.transform.position + new Vector3(0.1f, 0, 0), 0.1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.1f);
        //Pot.transform.DOLocalRotate(new Vector3(0, 180, 0), 1f).SetEase(Ease.Linear);
        Pot.transform.DOMove(Pot.transform.position+new Vector3(0,0,-0.2f),0.5f);
        /*Table.transform.DOMove(Table.transform.position - new Vector3(0.1f, 0, 0.1f), 0.3f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.5f);
        Table.transform.DOMove(Table.transform.position + new Vector3(0.1f, 0, 0.1f), 0.3f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.5f);
        Table.transform.DOMove(Table.transform.position - new Vector3(0.1f, 0, 0.1f), 0.3f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.5f);*/

    }
}
