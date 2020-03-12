using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolver : MonoBehaviour
{

    [SerializeField]
    private List<Renderer> renderers = new List<Renderer>();

    [SerializeField]
    private float speed;

    [SerializeField]
    private float emissionColorSpeed = 4f;

    [SerializeField]
    private bool notDissolve = false;
    public bool NotDissolve
    {
        get
        {
            return notDissolve;
        }
        set
        {
            notDissolve = value;
        }
    }

    private float currentDissolve = 1f;
    private float targetDissolve = 1f;

    private Color currentEmissionColorTarget = Color.white;

    // Update is called once per frame
    void Update()
    {
        // ---- DISSOLVE SHADER ----
        targetDissolve = notDissolve ? 0.0f : 1.0f;
        currentDissolve = Mathf.Lerp(currentDissolve, targetDissolve, Time.deltaTime * speed);

        renderers.ForEach(renderer =>
        {
            renderer.material.SetFloat("Vector1_FEFF47F1", currentDissolve);
        });
        // -------------------------

        // -------- ALPHA ----------
        Color colorTarget = notDissolve ? Color.white : Color.black;
        currentEmissionColorTarget = Color.Lerp(currentEmissionColorTarget, colorTarget, Time.deltaTime * emissionColorSpeed);

        renderers.ForEach(renderer =>
        {
            renderer.material.SetColor("_EmissionColor", Color.white * currentEmissionColorTarget);
        });
        // -------------------------
    }


}
