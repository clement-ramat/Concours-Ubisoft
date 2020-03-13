// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A global set of GameObjects, curated at runtime.
    /// Stores the a reference to the LetterSlab of the GameObjects added to it.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/RuntimeSet/LetterSlab", order = 200)]
    public class RuntimeSetLetterSlab : RuntimeSetBase<LetterSlab>
    {

        [SerializeField] [ReadOnly]
        private List<char> availableLetters = new List<char>();

        [SerializeField] [ReadOnly]
        private List<char> enabledLetters = new List<char>();

        public List<char> AvailableLetters { get => availableLetters;  }
        public List<char> EnabledLetters { get => enabledLetters;  }

        public Action OnEnabledChange = null;

        public string EnabledToString()
        {
            string str = "";
            foreach (char letter in enabledLetters)
            {
                str += letter;
            }

            return str;
        }

        protected override bool TryCreateElement(GameObject go, out LetterSlab element)
        {
            element = go.GetComponent<LetterSlab>();
            return element != null;
        }

        protected override void OnAddElement(LetterSlab element)
        {
            base.OnAddElement(element);

            //On ajoute à la liste de toutes les lettres connues
            availableLetters.Add(element.Letter);

            if (element.Slab.IsToggled)
            {
                AddToEnabledList(element);
            }

            //Je suis désolé
            element.RunOnce(() => ListenToToggle(element));
        }

        protected override void OnRemoveElement(LetterSlab element)
        {
            base.OnRemoveElement(element);

            availableLetters.Remove(element.Letter);

            if (element.Slab.IsToggled)
            {
                RemoveFromEnabledList(element);
            }

            //UnlistenToToggle(element);
        }

        private void ListenToToggle(LetterSlab element)
        {

            //element.OnToggle += (elm) => Debug.Log("enabled :" + elm.Letter);
            //element.OnUntoggle += ((elm) => Debug.Log("disabled :" + elm.Letter));

            element.OnToggle += AddToEnabledList;
            element.OnUntoggle += RemoveFromEnabledList;
        }

        private void AddToEnabledList(LetterSlab element)
        {
            enabledLetters.Add(element.Letter);
            OnEnabledChange?.Invoke();
        }

        private void RemoveFromEnabledList(LetterSlab element)
        {
            enabledLetters.Remove(element.Letter);
            OnEnabledChange?.Invoke();
        }
        
    }
}
