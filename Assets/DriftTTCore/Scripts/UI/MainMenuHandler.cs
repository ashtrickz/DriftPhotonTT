using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EMenuType = MenuManager.EMenuType;

public class MainMenuHandler : BaseMenuHandler
{
    
    [SerializeField] private Button toVehicleSelectionButton;
    [SerializeField] private TMP_Text currencyText;

    public override void ManageMenu(MenuManager menuManager)
    {
        base.ManageMenu(menuManager);
        
        toVehicleSelectionButton.onClick.RemoveAllListeners();
        toVehicleSelectionButton.onClick.AddListener(() =>
        {
            MenuManager.SwitchMenu(EMenuType.VehicleSelector);
        });
        
        currencyText.text = $"${RootData.RootInstance.PlayerData.PlayerMoney}";
    }
}
