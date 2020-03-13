using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class LetterPuzzle : SerializedMonoBehaviour
{

    [SerializeField]
    private RuntimeSetLetterSlab runtimeSetLetters;

    [SerializeField]
    private Dictionary<string, UnityEvent> solutions = new Dictionary<string, UnityEvent>();

    private void OnEnable()
    {
        runtimeSetLetters.OnEnabledChange += () => Debug.Log(runtimeSetLetters.EnabledToString());
        runtimeSetLetters.OnEnabledChange += TriggerEventIfWordEnabled;
    }

    private void OnDisable()
    {

        runtimeSetLetters.OnEnabledChange = null;
    }

    private void Start()
    {
        /*
        foreach (string word in solutions.Keys)
        {
            if (!IsWordPossible(word))
            {
                Debug.Log(word + " is not possible to write!");
            }
        } */
    }

    private void TriggerEventIfWordEnabled()
    {
        foreach (string word in solutions.Keys)
        {
            if (IsWordEnabled(word))
            {
                Debug.Log(word + " EVENT TRIGGERED!");
            }
        }
    }

    private bool IsWordEnabled(string word)
    {
        return IsWordPossible(word, runtimeSetLetters.EnabledLetters) &&
            runtimeSetLetters.EnabledLetters.Count == word.ToCharArray().Length;
    }

    private bool IsWordPossible(string word, List<char> letterList)
    {
        Dictionary<char, int> charCounts = new Dictionary<char, int>();

        //Initialize letters counts
        foreach(char letter in word.ToCharArray())
        {
            if (!charCounts.ContainsKey(letter))
            {
                charCounts.Add(letter, 1);
            }
            else
            {
                charCounts[letter]++;
            }
        }

        //Search for occurences
        foreach(char letter in letterList)
        {
            if (charCounts.ContainsKey(letter))
            {
                charCounts[letter]--;
            }
        }

        //Check if there's a missing occurences
        foreach (char letter in charCounts.Keys)
        {
            if (charCounts[letter] > 0)
            {
                return false;
            }
        }

        return true;
    }
}
