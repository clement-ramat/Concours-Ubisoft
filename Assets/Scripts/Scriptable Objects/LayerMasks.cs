using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LayerMasks", menuName = "ScriptableObjects/LayerMasks", order = 1)]
public class LayerMasks : ScriptableObject
{

    public LayerMask wallLayer;

    public LayerMask groundLayer;

    public LayerMask furnitureLayer;

    public LayerMask groundAndWallLayer;

    public LayerMask groundAndFurnitureLayer;

    public LayerMask furnitureAndWallLayer;

    public LayerMask groundFurnitureAndWallLayer;
}
