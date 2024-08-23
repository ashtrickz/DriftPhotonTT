using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectorHandler : BaseMenuHandler
{
    [SerializeField] private Button backButton;
    [SerializeField] private RectTransform mapsContainer;
    [SerializeField] private MapPresenter mapPresenterPrefab;
    
    public override void ManageMenu(MenuManager menuManager)
    {
        base.ManageMenu(menuManager);
        
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            MenuManager.SwitchMenu(MenuManager.EMenuType.VehicleSelector);
        });

        foreach (RectTransform child in mapsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var map in RootData.RootInstance.Maps)
        {
            var presenter = Instantiate(mapPresenterPrefab, mapsContainer);
            presenter.Init(map);
        }
    }
}
