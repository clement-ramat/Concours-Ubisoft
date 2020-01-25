// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An event with a paremeter.
    /// In addition event responses, SodaEvents store the object that add the response for debugging purposes.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    public class SodaEvent<T> : SodaEventBase<Action<T>>
    {
        private Func<T> callbackParameter;

        public SodaEvent(Func<T> callbackParameter = null, Action onChangeResponseCollection = null) : base(onChangeResponseCollection)
        {
            this.callbackParameter = callbackParameter;
        }

        /// <summary>
        /// Add a response to be invoked with the event. The callback is immediately invoked with the current value.
        /// </summary>
        /// <param name="response">The response to invoke.</param>
        /// <returns>True if the response could be added, false if it was already added before.</returns>
        public bool AddResponseAndInvoke(Action<T> response)
        {
            var success = AddResponse(response);
            if (success)
            {
                response(callbackParameter());
            }
            return success;
        }

        [Obsolete("You don't need to pass a listener object anymore.")]
        public bool AddResponseAndInvoke(Action<T> response, UnityEngine.Object listener)
        {
            return AddResponseAndInvoke(response);
        }

        internal void Invoke(T parameter)
        {
            if (currentlyBeingInvoked)
            {
                LogRecursionError();
                return;
            }

            currentlyBeingInvoked = true;

            for (var i = responses.Count - 1; i >= 0; i--)
            {
                try
                {
                    responses[i](parameter);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            currentlyBeingInvoked = false;
        }

        internal override int GetListeners(object[] listeners)
        {
            return WriteToArray(responses.Select(action => action.Target), listeners);
        }
    }

    /// <summary>
    /// An event without a paremeter.
    /// In addition event responses, SodaEvents store the object that add the response for debugging purposes.
    /// </summary>
    public class SodaEvent : SodaEventBase<Action>
    {
        internal void Invoke()
        {
            if (currentlyBeingInvoked)
            {
                LogRecursionError();
                return;
            }

            currentlyBeingInvoked = true;

            for (var i = responses.Count - 1; i >= 0; i--)
            {
                try
                {
                    responses[i]();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            currentlyBeingInvoked = false;
        }

        internal override int GetListeners(object[] listeners)
        {
            return WriteToArray(responses.Select(action => action.Target), listeners);
        }
    }

    public abstract class SodaEventBase<T> : SodaEventBase
    {
        protected readonly List<T> responses = new List<T>();
        protected readonly HashSet<T> responseSet = new HashSet<T>();
        /// <summary>
        /// The number of registered responses.
        /// </summary>
        public int responseCount
        {
            get { return responses.Count; }
        }

        public SodaEventBase(Action onChangeResponseCollection = null) : base(onChangeResponseCollection)
        {
        }

        /// <summary>
        /// Add a response to be invoked with the event.
        /// </summary>
        /// <param name="response">The response to invoke.</param>
        /// <returns>True if the response could be added, false if it was already added before.</returns>
        public bool AddResponse(T response)
        {
            if (!responseSet.Contains(response))
            {
                responses.Add(response);
                responseSet.Add(response);
                onChangeResponseCollection?.Invoke();
                return true;
            }
            return false;
        }

        [Obsolete("You don't need to pass a listener object anymore.")]
        public bool AddResponse(T response, UnityEngine.Object listener)
        {
            return AddResponse(response);
        }

        /// <summary>
        /// Removes a response so it's no longer invoked.
        /// </summary>
        /// <param name="response">The response to remove.</param>
        /// <returns>True if the response was removed, false if it wasn't part of the event.</returns>
        public bool RemoveResponse(T response)
        {
            return responseSet.Remove(response) && responses.Remove(response);
        }
    }

    public abstract class SodaEventBase
    {
        protected readonly Action onChangeResponseCollection;
        // For preventing cyclic/recursive invocation
        protected bool currentlyBeingInvoked = false;

        public SodaEventBase(Action onChangeResponseCollection = null)
        {
            this.onChangeResponseCollection = onChangeResponseCollection;
        }
        
        internal abstract int GetListeners(object[] listeners);

        protected static int WriteToArray<T>(IEnumerable<T> input, T[] output)
        {
            var index = 0;
            using (var values = input.GetEnumerator())
            {
                while (index < output.Length && values.MoveNext())
                {
                    output[index] = values.Current;
                    index++;
                }
            }
            return index;
        }

        protected static void LogRecursionError()
        {
            Debug.LogError("SodaEvent is already being invoked, preventing cyclic invocation.\nDo not change a SodaEvent in a response registered to it.");
        }
    }
}