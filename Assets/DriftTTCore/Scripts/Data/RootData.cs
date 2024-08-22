using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "DriftTT/Data/RootData")]
public class RootData : SerializedScriptableObject
{

    #region Singleton

    private static RootData _instance;
    
    public static RootData RootInstance
    {
        get
        {
            if (_instance == null)
            {
#if UNITY_EDITOR
                Debug.Log($"Loading... RootData instance");
#endif
                _instance = Resources.Load<RootData>("Data/RootData");
                if (_instance == null)
                {
                    Debug.LogError("RootData doesn't found");
#if UNITY_EDITOR
                    Debug.Log($"Creating... RootData instance");
#endif
                    _instance = CreateInstance<RootData>();
                }
            }

            return _instance;
        }
    }

    #endregion

    [InlineEditor()]
    public PlayerData PlayerData;
    
    public Dictionary<int, GameObject> Vehicles = new();

}
