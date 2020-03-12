using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class EZTools : EditorWindow
{
    static int selectedLayer = 0;

    static MonoScript componentToAdd;

    static MonoScript componentCondition;

    //void OnEnable()
    //{
    //    componentToAdd = MonoScript.FromMonoBehaviour((MyScript)target);
    //}

    [MenuItem("EZTools/Layer Field Window")]
    static void Init()
    {
        EZTools window = (EZTools)GetWindow(typeof(EZTools));
        window.Show();
    }

    // Disable menu if we dont have at least 1 gameobject selected
    [MenuItem("EZTools/Layer Field Window", true)]
    static bool ValidateSelection()
    {
        //return Selection.activeGameObject != null;
        return true;
    }

    void OnGUI()
    {
        selectedLayer = EditorGUILayout.LayerField("Layer for Objects:", selectedLayer);
        if (GUILayout.Button("Set Layer"))
            SetLayer();
        if (GUILayout.Button("Set Layer in all children"))
            SetLayer(checkChildren: true);


        EditorGUILayout.ObjectField("Component to add:", componentToAdd, typeof(MonoScript), true);
        EditorGUILayout.ObjectField("Component needed to add:", componentCondition, typeof(MonoScript), true);
        if (GUILayout.Button("Add Component"))
            AddComponent(checkChildren: true);

        if (GUILayout.Button("Test debug"))
            TestDebug();

    }

    static void SetLayer(bool checkChildren = false)
    {
        List<GameObject> gameObjects = GetSelectedGameObjects();
        foreach (GameObject go in gameObjects)
        {
            go.layer = selectedLayer;
            if (checkChildren)
            {
                SetLayerRecursive(go);
            }
        }

    }

    static void SetLayerRecursive(GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = selectedLayer;
            SetLayerRecursive(child.gameObject);
        }

    }

    static void AddComponent(bool checkChildren = false)
    {
        List<GameObject> gameObjects = GetSelectedGameObjects();
        foreach (GameObject go in gameObjects)
        {
            //Debug.Log(go);
            if (go.GetComponent<Light>() != null)
            {
                Debug.Log(go);
                //go.AddComponent(componentToAdd.GetType());
                //go.AddComponent<ManageLight>();
            }
            if (checkChildren)
            {
                AddComponentRecursive(go);
            }
        }
    }

    static void AddComponentRecursive(GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            if (go.GetComponent<Light>() != null)
            {
                //go.AddComponent(componentToAdd.GetType());
                //go.AddComponent<ManageLight>();
            }
            SetLayerRecursive(child.gameObject);
        }
    }

    static void TestDebug()
    {
        Debug.Log(GetSelectedGameObjects());
        Debug.Log(GetSelectedGameObjects().Count);
        foreach (Object o in GetSelectedGameObjects())
        {
            Debug.Log(o);
        }
    }

    static void GetAssets()
    {
        string currentAssetFolderPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (currentAssetFolderPath.Length > 0)
        {
            if (Directory.Exists(currentAssetFolderPath))
            {

            }
        }

        
    }

    static List<GameObject> GetSelectedGameObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        foreach(Object o in Selection.GetFiltered<Object>(SelectionMode.DeepAssets))
        {
            if (o.GetType().Equals(typeof(GameObject)))
            {
                gameObjects.Add(o as GameObject);
            }
        }
        return gameObjects;
    }
}
