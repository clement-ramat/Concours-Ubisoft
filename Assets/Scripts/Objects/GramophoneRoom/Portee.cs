using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[System.Serializable]
public class Note
{
    public Color color;
    //Sound Emitter
    public int indexParameter;
}


//Peut-etre que le grammo et la portee devrait pas faire le mm son
public class Portee : NetworkBehaviour
{
    [Header("Scene elements")]
    public List<PorteePart> parties;
    public Gramaphone gramaphone;
    public PorteeButton button;

    [Header("Cle de sol")]
    public GameObject cle;
    private Renderer cleRenderer;
    public Material solvedMaterial;
    public Material wrongMaterial;
    private Material cleBaseMat;

    [Header("Notes")]
    public List<Note> notes;
    public int[] notesSolution;

    [Header("Checking Camera")]
    public Cinemachine.CinemachineVirtualCamera CheckingCamera;

    private RoomBehavior roomBehavior;


    private bool enigmeSolved = false;

    // Start is called before the first frame update
    void Start()
    {
        roomBehavior = GetComponentInParent<RoomBehavior>();
        foreach (int i in notesSolution)
        {
            Note n = GetNoteWithParameter(i);

            if (n != null)
            {
                gramaphone.notes.Add(n);
            }
        }

        for(int i = 0; i < parties.Count; i++)
        {
            parties[i].index = i; 
        }

        cleRenderer = cle.GetComponent<Renderer>();
        cleBaseMat = cleRenderer.material;
    }

    public Note GetNoteWithParameter(int parameter)
    {
        foreach(Note note in notes)
        {
            if(note.indexParameter == parameter)
            {
                return note;
            }
        }
        return null;
    }

    private void Update()
    {
        if (!isServer)
            return;
    }

    public void ResetPortee()
    {
        foreach(PorteePart part in parties)
        {
            part.ResetPart();
        }
    }

    public bool CheckVictory()
    {
        for(int partIndex = 0; partIndex < parties.Count; partIndex++)
        {
            if (parties[partIndex].notePlayed != notesSolution[partIndex])
                return false;
        }
        return true;
    }

    public IEnumerator playAnimation()
    {
        Debug.Log(roomBehavior);
        Cinemachine.CinemachineVirtualCamera previousCamera = null;
        if (CheckingCamera != null)
        {
            previousCamera = roomBehavior.GetCurrentCamera();
            roomBehavior.ActivateVirtualCamera(CheckingCamera);
        }

        foreach (PorteePart part in parties)
        {
            part.enablePlay(false);
        }

        foreach (PorteePart part in parties)
        {
            part.PlayAnimation();
            yield return new WaitForSeconds(1f);
        }

        if (CheckVictory() && !enigmeSolved)
        {
            cle.GetComponent<Renderer>().material = solvedMaterial;

            if(!enigmeSolved)
            {
                enigmeSolved = true;
                GetComponent<EnigmeReward>().DropReward();
            }
        }
        else if( !enigmeSolved)
        {
            cle.GetComponent<Renderer>().material = wrongMaterial;
        }

        foreach (PorteePart part in parties)
        {
            part.enablePlay(true);
        }

        ResetPortee();

        yield return new WaitForSeconds(3f);
        
        cle.GetComponent<Renderer>().material = cleBaseMat;
        button.EnableButton(false);


        if (CheckingCamera != null)
        {
            roomBehavior.ActivateVirtualCamera(previousCamera);
        }
    }


}
