// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    /// <summary>
    /// A ScriptableObject representing a global event.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        #region Description
#pragma warning disable 0414
        [TextArea]
        [SerializeField]
        private string description = "";
#pragma warning restore 0414
        #endregion

#if UNITY_EDITOR
        [NonSerialized]
        public bool debug;
#endif
        /// <summary>
        /// This event is invoked when the GameEvent is raised.
        /// </summary>
        public readonly SodaEvent onRaise = new SodaEvent();
        [SerializeField]
        private UnityEvent onRaiseGlobally = default;
        // For preventing cyclic/recursive invocation
        private bool currentlyBeingRaised = false;

        /// <summary>
        /// Call this method to raise the event, leading to all added responses being invoked.
        /// </summary>
        public void Raise()
        {
#if UNITY_EDITOR
            if (debug)
            {
                Debug.Log("GameEvent \"" + name + "\" was raised.\n", this);
            }
#endif
            if (currentlyBeingRaised)
            {
                Debug.LogWarning("Event is already being raised, preventing re-raise.", this);
                return;
            }

            currentlyBeingRaised = true;

            try
            {
                onRaiseGlobally.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            onRaise.Invoke();

            currentlyBeingRaised = false;
        }
    }
}
