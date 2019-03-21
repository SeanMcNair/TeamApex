using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkinManager", menuName = "Data/Managers/PlayerSkinManager", order = 1)]
public class PlayerSkinManager : ScriptableObject
{
    public List<PlayerSkinData> playerSkins = new List<PlayerSkinData>();
}
