// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    
    [CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
    public class GameEventEditor : Editor
    {
        protected virtual void OnEnable()
        {
            EditorApplication.update += () => Repaint();
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= () => Repaint();
        }

        public override void OnInspectorGUI()
        {
            SodaEditorHelpers.DisplayInspectorSubtitle("GameEvent");
            
            var descriptionProperty = serializedObject.FindProperty("description");
            var onRaiseGloballyProperty = serializedObject.FindProperty("onRaiseGlobally");

            serializedObject.Update();
            serializedObject.DisplayAllPropertiesExcept(false, descriptionProperty, onRaiseGloballyProperty);

            EditorGUILayout.PropertyField(descriptionProperty);
            GUILayout.Space(16);
            
            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.PropertyField(onRaiseGloballyProperty);
            GUI.enabled = true;

            DisplayDebugCheckbox();
            serializedObject.ApplyModifiedProperties();

            GUI.enabled = Application.isPlaying;

            GameEvent gameEventTarget = (GameEvent)target;
            if(GUILayout.Button("Raise"))
            {
                gameEventTarget.Raise();
            }

            if(Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Responding Objects");
                if (targets.Length == 1)
                {
                    SodaEventDrawer.DisplayListeners(gameEventTarget.onRaise);
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot display when multiple GameEvents are selected.", MessageType.Warning);
                }
            }
        }

        private void DisplayDebugCheckbox()
        {
            var gameEventTarget = (GameEvent)target;

            EditorGUILayout.HelpBox("The Debug setting is reset when entering play mode.", MessageType.Info);
            GUILayout.BeginHorizontal();
            gameEventTarget.debug = EditorGUILayout.Toggle(gameEventTarget.debug, GUILayout.Width(16));
            GUILayout.Label("Debug (Raises are logged into the console)");
            GUILayout.EndHorizontal();
        }
    }
}
