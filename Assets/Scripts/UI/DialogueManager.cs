using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using DG.Tweening;
using ThirteenPixels.Soda;
using Rewired;
using UnityEngine.EventSystems;
using FMODUnity;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public List<TextPopup> dialogueList;
    private float vitesseAffichageTexte = 0.04f;

    private TextPopup currentPopup;
    public string popupName;

    private UIPopup uIPopup;

    [SerializeField]
    private ScopedInt characterType;

    [SerializeField]
    private GlobalPlayerGhost ghost;

    [SerializeField]
    private GlobalPlayerHuman human;

    //[SerializeField]
    //private GlobalHUD HUD;

    public GameObject HUD;
    [SerializeField]
    private Sprite stephImage;
    [SerializeField]
    private Sprite etienneImage;


    private bool initGhost = false;
    private bool initHuman = false;


    private bool isHuman = true;

    private StudioEventEmitter typewriterSoundEmitter;

    // Start is called before the first frame update
    void Start()
    {
        characterType.onChangeValue.AddResponse(ChangeCharacterType);
        typewriterSoundEmitter = GetComponent<StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (characterType.value == 0 && ghost.componentCache != null && !initGhost)
        {
            initGhost = true;
            isHuman = false;
        }

        if (isHuman && !initHuman)
        {
            initHuman = true;
        }
    }

    void ChangeCharacterType(int newValue)
    {
        if (newValue == 1)
        {
            isHuman = true;
        }
    }

    public void PrintTextInPopup(TextPopup popup)
    {
        if (popup.isTextDifferentSelonJoueur)
        {
            if (isHuman && popup.textToShowHuman == "")
                return;
            else if (!isHuman && popup.textToShowGhost == "")
                return;
        }


        uIPopup = UIPopupManager.GetPopup(popupName);
        if (uIPopup != null)
        {
            if(uIPopup.Data.Buttons[0] != null)
                uIPopup.Data.Buttons[0].Interactable = false;
            if(popup.personSpeaking != "Narrator")
            {
                if (popup.personSpeaking == "Ghost")
                    uIPopup.Data.Images[0].GetComponent<Image>().sprite = etienneImage;
                else if (popup.personSpeaking == "Human")
                    uIPopup.Data.Images[0].GetComponent<Image>().sprite = stephImage;
            }
            DisableCharacterMovement();
            uIPopup.Show();
            ChooseAnimation(popup);
            StartCoroutine(TypeTextSlowly(popup));

            //PrintText(popup);
            currentPopup = popup;
            uIPopup.Data.SetButtonsCallbacks(() =>
            {
                NextText();
            });
        }
    }


    void ChooseAnimation(TextPopup popup)
    {
        if (popup.isAnimationDifferentSelonJoueur)
        {
            // TODO
        }
        else
        {
            uIPopup.Data.Images[0].GetComponent<Animator>().runtimeAnimatorController = popup.animationToShowInPopup;
        }

    }

    IEnumerator TypeTextSlowly(TextPopup popup)
    {
        string tmpText = null;
        if (popup.isTextDifferentSelonJoueur)
        {
            if (isHuman)
            {
                foreach (char letter in popup.textToShowHuman.ToCharArray())
                {
                    tmpText += letter;
                    uIPopup.Data.SetLabelsTexts(tmpText);
                    PlayTickSound();
                    yield return new WaitForSeconds(vitesseAffichageTexte);
                }
            }


            else
            {
                foreach (char letter in popup.textToShowGhost.ToCharArray())
                {
                    tmpText += letter;
                    uIPopup.Data.SetLabelsTexts(tmpText);
                    PlayTickSound();
                    yield return new WaitForSeconds(vitesseAffichageTexte);
                }
            }

        }
        else
        {
            foreach (char letter in popup.textToShowInPopup.ToCharArray())
            {
                tmpText += letter;
                uIPopup.Data.SetLabelsTexts(tmpText);
                PlayTickSound();
                yield return new WaitForSeconds(vitesseAffichageTexte);
            }

        }
        if (uIPopup.Data.Buttons[0] != null)
        {
            uIPopup.Data.Buttons[0].Interactable = true;
            EventSystem.current.SetSelectedGameObject(uIPopup.Data.Buttons[0].gameObject);
        }

    }

    void PrintText(TextPopup popup)
    {
        if (popup.isTextDifferentSelonJoueur)
        {
            if (isHuman)
                uIPopup.Data.SetLabelsTexts(popup.textToShowHuman);

            else
                uIPopup.Data.SetLabelsTexts(popup.textToShowGhost);
        }
        else
        {
            uIPopup.Data.SetLabelsTexts(popup.textToShowInPopup);
        }
    }


    public void NextText()
    {
        if (currentPopup.nextText != null)
        {
            if (currentPopup.personSpeaking != currentPopup.nextText.personSpeaking)
            {
                currentPopup = currentPopup.nextText;


                uIPopup.Hide();

                Sequence mySequence = DOTween.Sequence();
                mySequence.AppendInterval(1f).AppendCallback(() =>
                {
                    if (currentPopup.animationToShowInPopup != null)
                        ChooseAnimation(currentPopup);
                    //PrintText(currentPopup);
                    if (uIPopup.Data.Buttons[0] != null)
                        uIPopup.Data.Buttons[0].Interactable = false;

                    uIPopup.Show();
                    StartCoroutine(TypeTextSlowly(currentPopup));
                });
            }
            else
            {
                currentPopup = currentPopup.nextText;
                //PrintText(currentPopup);
                if (uIPopup.Data.Buttons[0] != null)
                    uIPopup.Data.Buttons[0].Interactable = false;
                StartCoroutine(TypeTextSlowly(currentPopup));
                if (currentPopup.animationToShowInPopup != null)
                    ChooseAnimation(currentPopup);
                /*uIPopup.Data.SetButtonsCallbacks(() =>
                {
                  Debug.Log("coucou");
                  NextText();
                });*/

            }


        }
        else
        {
            HidePopup();
            EnableCharacterMovement();
        }
    }


    public void HidePopup()
    {
        uIPopup.Hide();
    }
    public void ShowPopup()
    {
        uIPopup.Show();
    }



    void DisableCharacterMovement()
    {
        /*if (HUD.componentCache != null)
        {
          HUD.componentCache.SetActive(false);
        }*/
        if (HUD != null)
        {
            HUD.SetActive(false);
        }

        ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, "Default");
        ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, "Ghost");
        ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, "Human");
    }

    void EnableCharacterMovement()
    {
        /*if (HUD.componentCache != null)
        {
          HUD.componentCache.SetActive(true);
        }*/
        if (HUD != null)
        {
            HUD.SetActive(true);
        }
        ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "Default");
        ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "Ghost");
        ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "Human");
    }

    private void PlayTickSound()
    {
        typewriterSoundEmitter.SetParameter("TypewriterNb", 0);
        typewriterSoundEmitter.Play();
    }
}
