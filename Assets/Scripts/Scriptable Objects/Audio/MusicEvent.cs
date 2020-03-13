using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "MusicEvent", menuName = "ScriptableObjects/MusicEvent", order = 1)]
public class MusicEvent : ScriptableObject
{
    public MusicEventType eventType;

    public int ptdr = 0;

    public void PlayEvent()
    {
        MusicManager.instance.PlayEvent(eventType);
    }
}
