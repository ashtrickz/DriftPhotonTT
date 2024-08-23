using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapPresenter : MonoBehaviour
{

    [SerializeField] private Button mapButton;
    [SerializeField] private TMP_Text mapNameText;

    public void Init(string mapName)
    {
        mapButton.onClick.RemoveAllListeners();
        mapButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(mapName);
        });

        mapNameText.text = mapName;
    }

}
