using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

[System.Serializable]
public struct MaterialsCharacterType {

    public Material[] materials;
    public CharacterType characterType;
}

public class SetMaterialCharacterType : MonoBehaviour
{

    [SerializeField]
    private ScopedInt characterTypeGlobal;

    [SerializeField]
    private MaterialsCharacterType[] materialsCharacterTypes;


    [SerializeField]
    private Renderer[] renderersOnChildren;

    // Start is called before the first frame update
    void Start()
    {
        characterTypeGlobal.onChangeValue.AddResponse(ManageCharacterTypeChanged);
        ManageCharacterTypeChanged(characterTypeGlobal.value);
    }

    private void OnDestroy()
    {
        characterTypeGlobal.onChangeValue.RemoveResponse(ManageCharacterTypeChanged);
    }

    private void ManageCharacterTypeChanged(int characterType)
    {
        CharacterType character = (CharacterType)characterType;
        for(int i = 0; i < materialsCharacterTypes.Length; i++)
        {
            if(materialsCharacterTypes[i].characterType == character)
            {

                foreach (Renderer rend in renderersOnChildren)
                {
                    rend.materials = materialsCharacterTypes[i].materials;
                }
            }
        }
    }
}
