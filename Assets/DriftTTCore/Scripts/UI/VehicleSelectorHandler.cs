using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using EMenuType = MenuManager.EMenuType;

public class VehicleSelectorHandler : BaseMenuHandler
{

    [SerializeField] private Button buyVehicleButton;
    [SerializeField] private Button pickMapButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    
    [SerializeField] private TMP_Text currency;
    [SerializeField] private TMP_Text carInfo;
    private Dictionary<int, GameObject> Vehicles => MenuManager.Vehicles;

    private void OnEnable()
    {
        MenuManager.StartToFinal = true;
        MenuManager.FinalToStart = false;
    }

    public override void ManageMenu(MenuManager menuManager)
    {
        base.ManageMenu(menuManager);
        
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(DrawNextVehicle);

        previousButton.onClick.RemoveAllListeners();
        previousButton.onClick.AddListener(DrawPreviousVehicle);
        
        buyVehicleButton.onClick.RemoveAllListeners();
        buyVehicleButton.onClick.AddListener(TryBuyVehicle);
        
        backToMenuButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.AddListener(() =>
        {
            MenuManager.SwitchMenu(EMenuType.MainMenu);
        });
        
        pickMapButton.onClick.RemoveAllListeners();
        pickMapButton.onClick.AddListener(() =>
        {
            MenuManager.SwitchMenu(EMenuType.MapSelector);
        });
        
        carInfo.text = $"Your Money: ${RootData.RootInstance.PlayerData.PlayerMoney.ToString("")}";
    }
    
    public void DrawNextVehicle()
    {
        if (MenuManager.vehiclePointer < Vehicles.Values.Count - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            MenuManager.vehiclePointer++;
            PlayerPrefs.SetInt("pointer", MenuManager.vehiclePointer);
            GameObject childObject =
                Instantiate(Vehicles[MenuManager.vehiclePointer], Vector3.zero,
                    MenuManager.toRotate.transform.rotation);
            childObject.transform.parent = MenuManager.toRotate.transform;
            DrawVehicleInfo();
        }
    }

    public void DrawPreviousVehicle()
    {
        if (MenuManager.vehiclePointer > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            MenuManager.vehiclePointer--;
            PlayerPrefs.SetInt("pointer", MenuManager.vehiclePointer);
            GameObject childObject =
                Instantiate(Vehicles[MenuManager.vehiclePointer], Vector3.zero,
                    MenuManager.toRotate.transform.rotation);
            childObject.transform.parent = MenuManager.toRotate.transform;
            DrawVehicleInfo();
        }
    }

    private void ChangeSelectorIndex(int i)
    {
        
    }
    
    private void TryBuyVehicle()
    {
        var playerData = RootData.RootInstance.PlayerData;
        var pickedVehicle = Vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<VehicleController>();
        var pickedVehicleData = pickedVehicle.VehicleData;

        if (playerData.PlayerMoney < pickedVehicleData.VehiclePrice) return;
        
        playerData.ChangeMoneyValue(-pickedVehicle.VehicleData.VehiclePrice);
        playerData.UnlockVehicle(PlayerPrefs.GetInt("pointer"));
        
        DrawVehicleInfo();
    }
    
    private void DrawVehicleInfo()
    {
        var vehicleId = PlayerPrefs.GetInt("pointer");
        var pickedVehicle = Vehicles[vehicleId].GetComponent<VehicleController>();
        var pickedVehicleData = pickedVehicle.VehicleData;
        

        if (RootData.RootInstance.PlayerData.UnlockedVehicles.Contains(vehicleId))
        {
            carInfo.text = $"{pickedVehicleData.VehicleName} \nOwned";
            pickMapButton.gameObject.SetActive(true);
            buyVehicleButton.gameObject.SetActive(false);
            currency.text = $"Your Money: ${RootData.RootInstance.PlayerData.PlayerMoney.ToString("")}";

            return;
        }

        currency.text = $"Your Money: ${RootData.RootInstance.PlayerData.PlayerMoney.ToString("")}";

        carInfo.text = $"{pickedVehicleData.VehicleName}\n<color=green>${pickedVehicleData.VehiclePrice}";

        pickMapButton.gameObject.SetActive(false);
        buyVehicleButton.gameObject.SetActive(buyVehicleButton);
    }
}
