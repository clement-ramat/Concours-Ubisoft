using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FMODUnity;

public class PorteeLine : MonoBehaviour
{
    //public AudioSource audioSource;
    //private Color materialBaseColor;
    public GameObject line;
    private Renderer lineMeshRenderer;

    public PorteePart porteePart;

    public StudioEventEmitter eventEmitter;

    [ReadOnly]
    public int index;

    public Note note
    {
        get
        {
            return _note;
        }

        set
        {
            _note = value;
            particleSystem = GetComponentInChildren<ParticleSystem>();
            particleSystem.startColor = note.color;
        }
    }

    private Note _note;

    public Material notePlayedMaterial;
    private Material baseMaterial;
    public ParticleSystem particleSystem;

    public GameObject noteObject;
    private Renderer noteRenderer;

    public bool playEnabled = true;

    public bool activated = false;

    private Vector3 initialPosition;

    public float offsetY = -0.1f;
    public float animationSpeed = 1f;

    private void Start()
    {
        initialPosition = transform.position;

        //audioSource = GetComponent<AudioSource>();
        //audioSource.clip = note.audioClip;

        lineMeshRenderer = line.GetComponent<Renderer>();
        baseMaterial = lineMeshRenderer.material;

        eventEmitter = GetComponent<StudioEventEmitter>();


        /*noteObject.GetComponent<Renderer>().material.color = note.color;
        noteObject.SetActive(false);*/

        noteRenderer = noteObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        Vector3 target = !activated ? initialPosition : new Vector3(initialPosition.x, initialPosition.y + offsetY, initialPosition.z);
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * animationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playEnabled)
        {
            if (other.GetComponentInParent<Human>())
            {
                porteePart.PlayNote(index);
            }
        }

    }

    public void PlayNote()
    {

        eventEmitter.Play();
        eventEmitter.SetParameter("Piano Note", _note.indexParameter);

        lineMeshRenderer.material = notePlayedMaterial;
        lineMeshRenderer.material.color = _note.color;

        noteRenderer.material = notePlayedMaterial;
        noteRenderer.material.color = _note.color;

        activated = true;

        //noteObject.SetActive(true);
    }

    public void StopPlayNote()
    {
        //audioSource.Stop();
        lineMeshRenderer.material = baseMaterial;
        noteRenderer.material = baseMaterial;

        activated = false;

        //meshRenderer.material.color = materialBaseColor;
        //noteObject.SetActive(false);
    }

    public void PlayAnimation()
    {
        particleSystem.Play();
        eventEmitter.Play();
        eventEmitter.SetParameter("Piano Note", _note.indexParameter);
    }
}
