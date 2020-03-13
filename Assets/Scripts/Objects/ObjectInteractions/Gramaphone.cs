using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FMODUnity;


public class Gramaphone : NetworkBehaviour
{

    public ParticleSystem particleSystem;
    //public AudioSource audioSource;
    public StudioEventEmitter eventEmitter;

    
    public List<Note> notes;
    public Animation animation;

    private int noteIndex = -1;

    [ClientRpc]
    public void RpcPlay(int noteIndex)
    {
        particleSystem.startColor = notes[noteIndex].color;
        eventEmitter.SetParameter("Gramophone Note", notes[noteIndex].indexParameter);

        particleSystem.Play();
    }

    public void RpcPlayAnimation()
    {
        animation.Play();
        eventEmitter.Play();
    }

    public void RpcStopAnimation()
    {
        animation.Stop();
        eventEmitter.Stop();
    }

    private void Start()
    {
        animation = GetComponent<Animation>();
        eventEmitter = GetComponent<StudioEventEmitter>();
    }

    public void PlayNext()
    {
        if (noteIndex == notes.Count + 1)
        {
            noteIndex = 0;
        }
        else if(noteIndex < notes.Count + 1)
        {
            noteIndex++;
        }

        if (noteIndex < notes.Count)
        {
            RpcPlay(noteIndex);
        }
    }
}
