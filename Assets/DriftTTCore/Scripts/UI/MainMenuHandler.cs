using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EMenuType = MenuManager.EMenuType;

public class MainMenuHandler : BaseMenuHandler
{
    [Title("Main Menu", TitleAlignment = TitleAlignments.Centered),
     SerializeField] private Animator mainMenuAnimator;
    [SerializeField] private Button mainMenuToVehicleSelectionButton;
    [SerializeField] private TMP_Text mainMenuCurrency;

    public override void ManageMenu(MenuManager menuManager)
    {
        base.ManageMenu(menuManager);
        
        mainMenuToVehicleSelectionButton.onClick.RemoveAllListeners();
        mainMenuToVehicleSelectionButton.onClick.AddListener(() =>
        {
            MenuManager.ChangeMenu(EMenuType.VehicleSelector);
            FadeOut();
        });
        
        mainMenuCurrency.text = $"${RootData.RootInstance.PlayerData.PlayerMoney}";
    }
}
