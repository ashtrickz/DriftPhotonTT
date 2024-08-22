using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DriftTTCore.Scripts.Data
{
    [CreateAssetMenu(menuName = "DriftTT/Data/VehicleData")]
    public class VehicleData : SerializedScriptableObject
    {
        public enum EDriveType
        {
            FrontWheel,
            RearWheel,
            FullWheel
        }

        [SerializeField] public EDriveType DriveType;

        public bool test;
    

        public carEffects CarEffects; 
    
        [Header("Variables")] public float HandBrakeFrictionMultiplier = 2f;
        public float MaxRPM, MinRPM;
        public Dictionary<float, int> GearsDictionary = new();
        public AnimationCurve EnginePower;

        public int VehiclePrice;
        public string VehicleName;
        public float SmoothTime = 0.09f;
    
    }
}
