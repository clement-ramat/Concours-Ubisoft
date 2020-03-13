using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "TextPopup")]
public class TextPopup : ScriptableObject
{
  

  public TextPopup nextText;

  [ValueDropdown("person")]
  public string personSpeaking;

  private string[] person = { "Ghost", "Human", "Narrator" };
  
  public bool isTextDifferentSelonJoueur = false;
  public bool isAnimationDifferentSelonJoueur = false;

  [TextArea]
  [ShowIf("isTextDifferentSelonJoueur")]
  public string textToShowHuman;

  [TextArea]
  [ShowIf("isTextDifferentSelonJoueur")]
  public string textToShowGhost;

  [TextArea]
  [HideIf("isTextDifferentSelonJoueur")]
  public string textToShowInPopup;
  
  [ShowIf("isAnimationDifferentSelonJoueur")]
  public RuntimeAnimatorController animationToShowHuman;
  
  [ShowIf("isAnimationDifferentSelonJoueur")]
  public RuntimeAnimatorController animationToShowGhost;

  [HideIf("isAnimationDifferentSelonJoueur")]
  public RuntimeAnimatorController animationToShowInPopup;
}
