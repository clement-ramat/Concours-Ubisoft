using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using ThirteenPixels.Soda;
using TMPro;
using Rewired;
using UnityEngine.UI;

public class HUDBehavior : MonoBehaviour
{
    private Player player;

    private bool isHuman = false;

    [Header("Views")]
    public UIView viewHuman;
    public UIView viewGhost;


    [Header("Human Buttons")]
    public GameObject ButtonAHuman;
    public GameObject ButtonBHuman;
    public GameObject ButtonYHuman;
    public GameObject ButtonXHuman;

    [Header("Ghost Buttons")]
    public GameObject ButtonAGhost;
    public GameObject ButtonBGhost;
    public GameObject ButtonYGhost;
    public GameObject ButtonXGhost;
    public GameObject JoystickGhost;

    [Header("Global Variables")]
    [SerializeField]
    private GlobalPlayerGhost ghost;

    [SerializeField]
    private GlobalPlayerHuman human;

    [SerializeField]
    private ScopedInt characterType;

    [SerializeField]
    private PlayerAbilities playerAbilities;

    public Color colorButtonB;


    private bool initGhost = false;
    private bool initHuman = false;

    private bool isGrabbing = false;
    private bool isPossessing = false;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
        player.AddInputEventDelegate(OnInputUpdate, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);
    }

    // Start is called before the first frame update
    void Start()
    {

        characterType.onChangeValue.AddResponse(ChangeCharacterType);
        playerAbilities.BasicCamera.onChange.AddResponse(ChangeAbility);
        Init();
        if (characterType.value == 1)
        {
            isHuman = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (characterType.value == 0 && ghost.componentCache != null && !initGhost)
        {
            initGhost = true;
            isHuman = false;
            ghost.componentCache.OnPossess.AddListener(HUDOnPossess);
            ghost.componentCache.OnUnpossess.AddListener(HUDOnUnpossess);
            ghost.componentCache.interactionArea.OnPossessable.AddListener(HUDCanPossess);
            ghost.componentCache.interactionArea.OnUnpossessable.AddListener(HUDCannotPossess);
            viewHuman.gameObject.SetActive(false);
            viewGhost.gameObject.SetActive(true);

        }

        if (isHuman && !initHuman && human.componentCache != null)
        {
            initHuman = true;
            human.componentCache.OnGrab.AddListener(HUDOnGrab);
            human.componentCache.OnUngrab.AddListener(HUDOnUngrab);
            human.componentCache.OnThrow.AddListener(HUDOnUngrab);
            human.componentCache.OnUse.AddListener(HUDHasUsed);
            human.componentCache.interactionArea.OnGrabbable.AddListener(HUDCanGrab);
            human.componentCache.interactionArea.OnUngrabbable.AddListener(HUDCannotGrab);
            human.componentCache.interactionArea.OnUngrabbable.AddListener(HUDCannotUse);
            human.componentCache.interactionArea.OnUsable.AddListener(HUDCanUse);
            viewHuman.gameObject.SetActive(true);
            viewGhost.gameObject.SetActive(false);
        }

    }

    void OnInputUpdate(InputActionEventData data)
    {
        switch (data.actionName)
        {
            case "Wave":
                if (isHuman)
                    ButtonYHuman.GetComponent<UIButton>().ExecuteClick();
                break;

            case "Possess":
                if (!isHuman)
                    ButtonAGhost.GetComponent<UIButton>().ExecuteClick();
                break;

            case "LaunchPossessedObject":
                if (!isHuman)
                    ButtonXGhost.GetComponent<UIButton>().ExecuteClick();
                break;

            case "RevealBlast":
                if (!isHuman && playerAbilities.BasicCamera.value)
                    ButtonBGhost.GetComponent<UIButton>().ExecuteClick();
                break;

            case "Interact":
                if (isHuman)
                    ButtonAHuman.GetComponent<UIButton>().ExecuteClick();
                break;

            case "Flash":
                if (isHuman && playerAbilities.BasicCamera.value)
                    ButtonBHuman.GetComponent<UIButton>().ExecuteClick();
                break;

            case "Throw":
                if (isHuman)
                    ButtonXHuman.GetComponent<UIButton>().ExecuteClick();
                break;

            default:
                break;
        }
    }

    private void DisableButtonColor(GameObject button)
    {
        button.GetComponent<Button>().interactable = false;
        button.GetComponent<Image>().color = Color.white;
        var tmpColor = button.GetComponent<Image>().color;
        tmpColor.a = (57f / 255f);
        button.GetComponent<Image>().color = tmpColor;
        button.GetComponent<HUDButton>().DisableColorLetter();
    }

    private void EnableButtonColor(GameObject button)
    {
        button.GetComponent<Button>().interactable = true;
        button.GetComponent<Image>().color = colorButtonB;
        button.GetComponent<HUDButton>().EnableColorLetter();
    }

    private void Init()
    {

        ButtonAGhost.GetComponent<HUDButton>().ChangeActionText("");
        ButtonXGhost.GetComponent<HUDButton>().ChangeActionText("");
        JoystickGhost.gameObject.SetActive(false);
        DisableButtonColor(ButtonYGhost);


        ButtonYHuman.GetComponent<HUDButton>().ChangeActionText("HEY !");
        ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("");
        ButtonXHuman.GetComponent<HUDButton>().ChangeActionText("");
        if (playerAbilities.BasicCamera.value)
        {
            ButtonBHuman.GetComponent<HUDButton>().ChangeActionText("FLASH");
            ButtonBGhost.GetComponent<HUDButton>().ChangeActionText("REVEAL");
        }
        else
        {
            ButtonBHuman.GetComponent<HUDButton>().ChangeActionText("");
            ButtonBGhost.GetComponent<HUDButton>().ChangeActionText("");
            DisableButtonColor(ButtonBGhost);
            DisableButtonColor(ButtonBHuman);
        }
    }

    private void OnDestroy()
    {
        characterType.onChangeValue.RemoveResponse(ChangeCharacterType);
        if (ghost.componentCache != null)
        {
            ghost.componentCache.OnPossess.RemoveListener(HUDOnPossess);
            ghost.componentCache.OnUnpossess.RemoveListener(HUDOnUnpossess);
        }
        else if (human.componentCache != null)
        {
            human.componentCache.OnGrab.RemoveListener(HUDOnGrab);
            human.componentCache.OnUngrab.RemoveListener(HUDOnUngrab);
            human.componentCache.OnThrow.RemoveListener(HUDOnUngrab);
        }
    }

    void ChangeCharacterType(int newValue)
    {
        if (newValue == 1)
        {
            isHuman = true;
        }
    }

    void ChangeAbility(bool newValue)
    {
        if (newValue)
        {
            ButtonBHuman.GetComponent<HUDButton>().ChangeActionText("FLASH");
            ButtonBGhost.GetComponent<HUDButton>().ChangeActionText("REVEAL");
            EnableButtonColor(ButtonBHuman);
            EnableButtonColor(ButtonBGhost);
        }
    }

    void HUDOnPossess(GameObject possessObject)
    {
        isPossessing = true;
        ButtonAGhost.GetComponent<HUDButton>().ChangeActionText("UNPOSSESS");
        ButtonBGhost.GetComponent<HUDButton>().ChangeActionText("");


        if (possessObject.GetComponent<HeavyPossessInteraction>() != null)
        {
            HUDOnHeavyPossess(possessObject);
        }
        else if (possessObject.GetComponent<LightPossessInteraction>() != null)
        {
            ButtonXGhost.GetComponent<HUDButton>().ChangeActionText("THROW");
        }
    }

    void HUDOnUnpossess(GameObject unPossessObject)
    {
        isPossessing = false;
        if (unPossessObject.GetComponent<HeavyPossessInteraction>() != null)
        {
            HUDOnHeavyUnpossess();
        }
        ButtonAGhost.GetComponent<HUDButton>().ChangeActionText("");
        ButtonXGhost.GetComponent<HUDButton>().ChangeActionText("");
        if (playerAbilities.BasicCamera.value)
            ButtonBGhost.GetComponent<HUDButton>().ChangeActionText("REVEAL");
    }

    void HUDCanPossess()
    {
        if (!isPossessing)
        {
            ButtonAGhost.GetComponent<HUDButton>().ChangeActionText("POSSESS");
        }
    }

    void HUDCannotPossess()
    {
        if (!isPossessing)
        {
            ButtonAGhost.GetComponent<HUDButton>().ChangeActionText("");
        }
    }

    void HUDOnHeavyPossess(GameObject possessObject)
    {
        JoystickGhost.gameObject.SetActive(true);
        if (possessObject.GetComponent<HeavyPossessInteraction>().joyStickInteraction is AnyJoystickInteraction)
        {
            JoystickGhost.GetComponent<JoystickInteractionHUD>().ActivateShakeAllDirections();
            JoystickGhost.GetComponent<JoystickInteractionHUD>().ChangeActionText("SHAKE");
        }
        else if (possessObject.GetComponent<HeavyPossessInteraction>().joyStickInteraction is CircleJoystickInteraction)
        {
            JoystickGhost.GetComponent<JoystickInteractionHUD>().ActivateTurn();
            JoystickGhost.GetComponent<JoystickInteractionHUD>().ChangeActionText("TURN");
        }
        else if (possessObject.GetComponent<HeavyPossessInteraction>().joyStickInteraction is LeftRightJoystickInteraction)
        {
            JoystickGhost.GetComponent<JoystickInteractionHUD>().ActivateShakeLeftRight();
            JoystickGhost.GetComponent<JoystickInteractionHUD>().ChangeActionText("SHAKE");
        }
    }

    void HUDOnHeavyUnpossess()
    {
        JoystickGhost.gameObject.SetActive(false);
    }


    void HUDOnGrab()
    {
        isGrabbing = true;
        ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("LET GO");
        ButtonXHuman.GetComponent<HUDButton>().ChangeActionText("THROW");
        ButtonBHuman.GetComponent<HUDButton>().ChangeActionText("");
        ButtonYHuman.GetComponent<HUDButton>().ChangeActionText("");
    }

    void HUDOnUngrab()
    {
        isGrabbing = false;
        ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("");
        ButtonXHuman.GetComponent<HUDButton>().ChangeActionText("");
        ButtonYHuman.GetComponent<HUDButton>().ChangeActionText("HEY !");
        if (playerAbilities.BasicCamera.value)
            ButtonBHuman.GetComponent<HUDButton>().ChangeActionText("FLASH");
    }

    void HUDCanGrab()
    {
        if (!isGrabbing)
            ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("GRAB");
    }

    void HUDCannotGrab()
    {
        if (!isGrabbing)
            ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("");
    }

    void HUDCanUse()
    {
        ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("USE");
    }

    void HUDCannotUse()
    {
        if (!isGrabbing)
            ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("");
        else
            ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("LET GO");
    }

    void HUDHasUsed()
    {
        isGrabbing = false;
        ButtonAHuman.GetComponent<HUDButton>().ChangeActionText("");
        ButtonXHuman.GetComponent<HUDButton>().ChangeActionText("");
        ButtonYHuman.GetComponent<HUDButton>().ChangeActionText("HEY !");
        if (playerAbilities.BasicCamera.value)
            ButtonBHuman.GetComponent<HUDButton>().ChangeActionText("FLASH");
    }
}
