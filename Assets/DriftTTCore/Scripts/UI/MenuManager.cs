using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Title("Camera Settings", TitleAlignment = TitleAlignments.Centered),
     SerializeField] private float lerpTime;
    public GameObject CameraObject;
    public GameObject finalCameraPosition, startCameraPosition;

    [Title("Menu Handlers", TitleAlignment = TitleAlignments.Centered)] 
    [SerializeField] private        MainMenuHandler mainMenuHandler;
    [SerializeField] private VehicleSelectorHandler vehicleSelectorHandler;
    [SerializeField] private    UpgradesMenuHandler upgradesMenuHandler;
    [SerializeField] private     MapSelectorHandler mapSelectorHandler;

    [SerializeField, Space] private GameObject podium;
    [SerializeField, Range(0, 50)] private float podiumRotationSpeed = 10f;
    [SerializeField] private Transform vehicleSpawnPosition;
    
    public bool FinalToStart;
    public bool StartToFinal;

    private BaseMenuHandler _currentMenu;

    [NonSerialized] public int VehiclePointer = 0;

    public Dictionary<int, GameObject> Vehicles => RootData.RootInstance.Vehicles;

    private void Awake()
    {
        mainMenuHandler       .ManageMenu(this);
        vehicleSelectorHandler.ManageMenu(this);
        upgradesMenuHandler   .ManageMenu(this);
        mapSelectorHandler    .ManageMenu(this);

        SwitchMenu(EMenuType.MainMenu);
        
        VehiclePointer = PlayerPrefs.GetInt("pointer");
        SpawnVehicle();
    }

    public void SwitchMenu(EMenuType menuType)
    {
        _currentMenu?.Exit();
        
        _currentMenu = menuType switch
        {
            EMenuType.MainMenu => mainMenuHandler,
            EMenuType.VehicleSelector => vehicleSelectorHandler,
            EMenuType.MapSelector => mapSelectorHandler,
            EMenuType.UpgradeMenu => upgradesMenuHandler,
            _ => _currentMenu
        };
        
        _currentMenu?.Enter();
    }

    private void FixedUpdate()
    {
        podium.transform.Rotate(Vector3.up * podiumRotationSpeed * Time.deltaTime);
        MoveCamera();
    }

    public void SpawnVehicle()
    {
        GameObject childObject = Instantiate(Vehicles[VehiclePointer], vehicleSpawnPosition.position, podium.transform.rotation);
        childObject.transform.parent = podium.transform;
    }

    private void MoveCamera()
    {
        if (StartToFinal)
        {
            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position,
                finalCameraPosition.transform.position, lerpTime * Time.deltaTime);
        }

        if (FinalToStart)
        {
            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position,
                startCameraPosition.transform.position, lerpTime * Time.deltaTime);
        }
    }

    public enum EMenuType
    {
        MainMenu,
        VehicleSelector,
        MapSelector,
        UpgradeMenu
    }
    
}