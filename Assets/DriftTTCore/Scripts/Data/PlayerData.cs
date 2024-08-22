using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "DriftTT/Data/PlayerData")]
public class PlayerData : SerializedScriptableObject
{

    [SerializeField]
    private int playerMoney;

    [SerializeField]
    private HashSet<int> unlockedCarsId = new();

    public int PlayerMoney => playerMoney;
    public HashSet<int> UnlockedVehicles => unlockedCarsId;

    public void ChangeMoneyValue(int value) => playerMoney += value;

    public void UnlockVehicle(int id) => unlockedCarsId.Add(id);
}
