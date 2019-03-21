using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AimFX", menuName = "Data/FX/AimFX", order = 1)]
public class AimFX : ScriptableObject
{
    [System.Serializable]
    public class ReticalSwap
    {
        public GameObject swapReticalPrefab;
        public LayerMask swapMask;
    }

    public enum PositionType { World, ScreenCanvas }
    public enum LineType { Constant, Firefade }
    public GameObject aimRetical;
    public PositionType reticalPosType;
    public ReticalSwap[] reticalSwaps;
    public GameObject lineRenderer;
    public LineType lineType;
    public float fadeTime;
}
