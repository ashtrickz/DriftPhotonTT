using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Camera")] public float lerpTime;
    public GameObject CameraObject;
    public GameObject finalCameraPosition, startCameraPosition;

    [Header("Deafault Canvas")] public GameObject DeafaultCanvas;
    public Text DeafaultCanvasCurrency;

    [Header("Map Canvas")] public GameObject mapSelectorCanvas;

    [Header("Upgrades Canvas")] public Text upgradesCurrency;
    public GameObject upgradesCanvas;

    [Header("Vehicle Select Canvas")] public GameObject vehicleSelectCanvas;
    public GameObject buyButton;
    public GameObject startButton;
    public Text currency;
    public Text carInfo;

    public Dictionary<int, GameObject> vehicles => RootData.RootInstance.Vehicles;


    public GameObject toRotate;
    [HideInInspector] public float rotateSpeed = 10f;
    [HideInInspector] public int vehiclePointer = 0;
    private bool finalToStart, startToFinal;

    private void Awake()
    {
        mapSelectorCanvas.SetActive(false);
        DeafaultCanvas.SetActive(true);
        vehicleSelectCanvas.SetActive(false);

        DeafaultCanvasCurrency.text = "$" + PlayerPrefs.GetInt("currency");
        upgradesCurrency.text = "$" + PlayerPrefs.GetInt("currency");


        vehiclePointer = PlayerPrefs.GetInt("pointer");
        GameObject childObject =
            Instantiate(vehicles[vehiclePointer], Vector3.zero, toRotate.transform.rotation);
        childObject.transform.parent = toRotate.transform;
        getCarInfo();
    }

    private void FixedUpdate()
    {
        toRotate.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        cameraTranzition();
    }

    public void rightButton()
    {
        if (vehiclePointer < vehicles.Values.Count - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer++;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject childObject =
                Instantiate(vehicles[vehiclePointer], Vector3.zero,
                    toRotate.transform.rotation);
            childObject.transform.parent = toRotate.transform;
            getCarInfo();
        }
    }

    public void leftButton()
    {
        if (vehiclePointer > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer--;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject childObject =
                Instantiate(vehicles[vehiclePointer], Vector3.zero,
                    toRotate.transform.rotation);
            childObject.transform.parent = toRotate.transform;
            getCarInfo();
        }
    }

    public void startGameButton()
    {
        DeafaultCanvas.SetActive(false);
        vehicleSelectCanvas.SetActive(false);
        mapSelectorCanvas.SetActive(true);
    }

    public void BuyButton()
    {
        var pickedVehicle = vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<VehicleController>();
        var pickedVehicleData = pickedVehicle.VehicleData;

        if (PlayerPrefs.GetInt("currency") < pickedVehicleData.VehiclePrice) return;
        PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") - pickedVehicle.VehicleData.VehiclePrice);

        PlayerPrefs.SetString(pickedVehicleData.VehicleName, pickedVehicleData.VehicleName);
        getCarInfo();
    }

    public void getCarInfo()
    {
        var pickedVehicle = vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<VehicleController>();
        var pickedVehicleData = pickedVehicle.VehicleData;

        if (pickedVehicleData.VehicleName == PlayerPrefs.GetString(pickedVehicleData.VehicleName))
        {
            carInfo.text = "Owned";
            startButton.SetActive(true);
            buyButton.SetActive(false);
            currency.text = "$" + PlayerPrefs.GetInt("currency").ToString("");

            return;
        }

        currency.text = "$" + PlayerPrefs.GetInt("currency").ToString("");

        carInfo.text = pickedVehicleData.VehicleName + " $ " + pickedVehicleData.VehiclePrice;

        startButton.SetActive(false);
        buyButton.SetActive(buyButton);
    }

    public void DeafaultCanvasStartButton()
    {
        mapSelectorCanvas.SetActive(false);
        DeafaultCanvas.SetActive(false);
        vehicleSelectCanvas.SetActive(true);
        upgradesCanvas.SetActive(false);
        startToFinal = true;
        finalToStart = false;
    }

    public void vehicleSelectCanvasStartButton()
    {
        mapSelectorCanvas.SetActive(false);
        DeafaultCanvas.SetActive(true);
        vehicleSelectCanvas.SetActive(false);
        finalToStart = true;
        startToFinal = false;
    }

    public void upgradesCanvasButton()
    {
        upgradesCanvas.SetActive(true);
        vehicleSelectCanvas.SetActive(false);
    }

    public void cameraTranzition()
    {
        if (startToFinal)
        {
            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position,
                finalCameraPosition.transform.position, lerpTime * Time.deltaTime);
        }

        if (finalToStart)
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
}