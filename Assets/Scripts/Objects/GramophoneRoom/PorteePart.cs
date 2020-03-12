using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorteePart : MonoBehaviour
{
    public List<PorteeLine> lines;
    public Portee portee;
    public GameObject triangle;
    public Material highlightMaterial;
    private Material baseMaterial;
    public int index;
    public int notePlayed;

    private void Start()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].porteePart = this;
            lines[i].index = i;
            lines[i].note = portee.notes[i];
            Debug.Log("rotagg part " + index + " line " + i +" " + portee.notes[i].color);
        }


        notePlayed = -1;

        baseMaterial = triangle.GetComponent<Renderer>().material;
    }

    public void PlayNote(int note)
    {
        if (notePlayed != -1)
        {
            lines[notePlayed - 1].StopPlayNote();
        }

        notePlayed = note + 1;
        lines[note].PlayNote();
    }

    public void ResetPart()
    {
        if (notePlayed != -1)
        {
            lines[notePlayed - 1].StopPlayNote();
        }

        notePlayed = -1;

        triangle.GetComponent<Renderer>().material = baseMaterial;
    }

    public void PlayAnimation()
    {
        if (notePlayed != -1)
        {
            lines[notePlayed - 1].PlayAnimation();
        }

        triangle.GetComponent<Renderer>().material = highlightMaterial;
    }

    public void enablePlay(bool enablePlay)
    {
        foreach(PorteeLine l in lines)
        {
            l.playEnabled = enablePlay;
        }
    }
}
