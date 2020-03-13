using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StephStepsSound : MonoBehaviour
{
    public UnityEvent OnStep;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Step(AnimationEvent animEvent)
    {
        if (animEvent.animatorClipInfo.weight > 0.2)
        {
            OnStep?.Invoke();
        }
    }
}
