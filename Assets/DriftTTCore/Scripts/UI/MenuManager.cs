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


    [SerializeField] private EMenuType currentMenuState = EMenuType.MainMenu;

    private EMenuType _prevMenuState;
    
    public Dictionary<int, GameObject> Vehicles => RootData.RootInstance.Vehicles;


    public GameObject toRotate;
    [HideInInspector] public float rotateSpeed = 10f;
    [HideInInspector] public int vehiclePointer = 0;
    public bool FinalToStart;
    public bool StartToFinal;

    private void Awake()
    {

        //upgradesCurrency.text = $"Your Money: ${RootData.RootInstance.PlayerData.PlayerMoney.ToString("")}";


        vehiclePointer = PlayerPrefs.GetInt("pointer");
        GameObject childObject =
            Instantiate(Vehicles[vehiclePointer], Vector3.zero, toRotate.transform.rotation);
        childObject.transform.parent = toRotate.transform;
        //DrawVehicleInfo();
    }
    
    

    public void ChangeMenu(EMenuType menuType)
    {
        _prevMenuState = currentMenuState;
        currentMenuState = menuType;
    }

    private void FixedUpdate()
    {
        toRotate.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        cameraTranzition();
    }

    public void cameraTranzition()
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

    public void loadMarioMap()
    {
        SceneManager.LoadScene("MarioMap");
    }

    public void loadComunityMap()
    {
        SceneManager.LoadScene("CommunityMap");
    }
    
    public enum EMenuType
    {
        MainMenu,
        VehicleSelector,
        MapSelector,
        UpgradeMenu
    }
    
}