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
    private int VehicleId => PlayerPrefs.GetInt("pointer");
    
    public override void Enter()
    {
        base.Enter();
        
        MenuManager.StartToFinal = true;
        MenuManager.FinalToStart = false;

        DrawVehicleInfo();
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
    
    public void DrawNextVehicle() => ChangeSelectorIndex(1);

    public void DrawPreviousVehicle() => ChangeSelectorIndex(-1);

    private void ChangeSelectorIndex(int index)
    {
        var vehId = MenuManager.VehiclePointer + index;
        if (vehId < 0 || vehId > Vehicles.Count - 1) return;
        
        Destroy(GameObject.FindGameObjectWithTag("Player")); // TODO Delete this frkn Find!
        MenuManager.VehiclePointer = vehId;
        
        PlayerPrefs.SetInt("pointer", MenuManager.VehiclePointer);
        MenuManager.SpawnVehicle();

        DrawVehicleInfo();
    }
    
    private void TryBuyVehicle()
    {
        var playerData = RootData.RootInstance.PlayerData;
        var pickedVehicle = Vehicles[VehicleId].GetComponent<VehicleController>();
        var pickedVehicleData = pickedVehicle.VehicleData;

        if (playerData.PlayerMoney < pickedVehicleData.VehiclePrice) return;
        
        playerData.ChangeMoneyValue(-pickedVehicle.VehicleData.VehiclePrice);
        playerData.UnlockVehicle(VehicleId);
        
        DrawVehicleInfo();
    }
    
    private void DrawVehicleInfo()
    {
        var pickedVehicle = Vehicles[VehicleId].GetComponent<VehicleController>();
        var pickedVehicleData = pickedVehicle.VehicleData;
        
        if (RootData.RootInstance.PlayerData.UnlockedVehicles.Contains(VehicleId))
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
