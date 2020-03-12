using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

[System.Serializable]
public class PaintingObtained
{
    public GlobalBool isPaintingAcquired;
    public bool cinematicDone = false;
}

[CreateAssetMenu(fileName = "New Paintings Obtained", menuName = "ScriptableObjects/Paintings Obtained")]
public class PaintingsObtained : ScriptableObject
{
    public List<PaintingObtained> paintings;

    public void Reset()
    {
        for(int i = 0; i < paintings.Count; i++)
        {
            PaintingObtained paintingObtained = paintings[i];
            paintingObtained.isPaintingAcquired.value = false;

            paintingObtained.cinematicDone = false;
        }
    }
}
