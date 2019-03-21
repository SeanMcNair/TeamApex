using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "Data/Items/Tools/Buildable", order = 1)]
public class ItemBuildableData : ItemAimableData
{
    public GameObject previewPrefab;
    public GameObject placedPrefab;
    public Material validMaterial;
    public Material invalidMaterial;
    public LayerMask overlapMask;
}
