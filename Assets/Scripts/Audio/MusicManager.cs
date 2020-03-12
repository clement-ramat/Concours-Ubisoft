using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Sirenix.OdinInspector;

public enum MusicEventType
{
    Main,
    PortraitFound,
    RemovePiano,
    AddPiano,
}

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    //[Range(0,1)]
    //public float pianoLayer = 1;

    //[Range(0, 1)]
    //public float choirLayer = 0;

    [FMODUnity.ParamRef]
    public string pianoLevelParam = "Piano Layer";

    [FMODUnity.ParamRef]
    public string choirLevelParam = "Choir Layer";



    private StudioEventEmitter musicEmitter;

    private void Awake()
    {
        if (instance == null)
        {
            musicEmitter = GetComponent<StudioEventEmitter>();
            DontDestroyOnLoad(this);
            instance = this;

        } else if (instance != this)
        {
                Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        musicEmitter.Play();
    }

    private void Update()
    {
        //musicEmitter.SetParameter("Piano Layer", pianoLayer);
        //musicEmitter.SetParameter("Choir Layer", choirLayer);
        //SetPianoLayer(pianoLayer);
        //SetChoirLayer(choirLayer);
    }

    //private void OnValidate()
    //{
    //    if (musicEmitter != null)
    //    {
    //        SetPianoLayer(pianoLayer);
    //        SetChoirLayer(choirLayer);
    //    }

    //}

    public void PlayEvent(MusicEventType eventType)
    {
        switch(eventType)
        {
            case MusicEventType.Main:
                PortraitFoundEnd();
                break;
            case MusicEventType.PortraitFound:
                PortraitFound();
                break;
            case MusicEventType.RemovePiano:
                SetPianoLayer(0);
                break;
            case MusicEventType.AddPiano:
                SetPianoLayer(1);
                break;
        }
    }

    [Button]
    public void PortraitFound()
    {
        musicEmitter.SetParameter("PortraitFound", 1);
        musicEmitter.SetParameter(pianoLevelParam, 1);
    }

    [Button]
    public void PortraitFoundEnd()
    {
        musicEmitter.SetParameter("PortraitFound", 0);
    }

    public void SetPianoLayer(float value)
    {
        musicEmitter.SetParameter(pianoLevelParam, value);
        //musicEmitter.SetParameter("Choir Layer", choirLayer);
        //pianoLayer = value;
        //musicEmitter.SetParameter("Piano Layer", value);
    }

    public void SetChoirLayer(float value)
    {
        //choirLayer = value;
        musicEmitter.SetParameter(choirLevelParam, value);
        //musicEmitter.SetParameter("Choir Layer", value);
    }
}
