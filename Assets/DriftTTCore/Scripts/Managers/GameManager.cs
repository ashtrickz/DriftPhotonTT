using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //public vehicle Vehicle;
    public GameObject neeedle;
    public GameObject startPosition;
    public Text kph;
    public Text currentPosition;
    public Text gearNum;
    public Slider nitrusSlider;
    private float startPosiziton = 32f, endPosition = -211f;
    private float desiredPosition;
    private GameObject[] presentGameObjectVehicles, fullArray;

    [Header("countdown Timer")]
    public float timeLeft = 4;
    public Text timeLeftText;

    [Header ("racers list")]
    public GameObject uiList;
    public GameObject uiListFolder;
    public GameObject backImage;
    private List<AIVehicle> presentVehicles;
    private List<GameObject> temporaryList;
    private GameObject[] temporaryArray;

    private int startPositionXvalue = -50-62;
    private bool arrarDisplayed = false,countdownFlag = false;

    public Dictionary<int, GameObject> vehicles => RootData.RootInstance.Vehicles;

    public VehicleController PickedVehicle => vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<VehicleController>();
    
    private void Awake () {

        Instantiate (PickedVehicle, startPosition.transform.position, startPosition.transform.rotation);

        presentGameObjectVehicles = GameObject.FindGameObjectsWithTag ("AI");

        presentVehicles = new List<AIVehicle> ();
        foreach (GameObject R in presentGameObjectVehicles)
            presentVehicles.Add (new AIVehicle (R.GetComponent<inputManager> ().currentNode, PickedVehicle.VehicleData.VehicleName,R.GetComponent<VehicleController> ().HasFinished));

        presentVehicles.Add (new AIVehicle (PickedVehicle.GetComponent<inputManager> ().currentNode, PickedVehicle.VehicleData.VehicleName , PickedVehicle.HasFinished));

        temporaryArray = new GameObject[presentVehicles.Count];

        temporaryList = new List<GameObject> ();
        foreach (GameObject R in presentGameObjectVehicles)
            temporaryList.Add (R);
        temporaryList.Add (PickedVehicle.gameObject);

        fullArray = temporaryList.ToArray ();
        //displayArray ();
        StartCoroutine (timedLoop ());
    }

    private void FixedUpdate () {
        if(PickedVehicle.HasFinished)displayArray();
        kph.text = PickedVehicle.KPH.ToString ("0");
        updateNeedle ();
        nitrusUI ();
        coundDownTimer();
    }

    public void updateNeedle () {
        desiredPosition = startPosiziton - endPosition;
        float temp = PickedVehicle.engineRPM / 10000;
        neeedle.transform.eulerAngles = new Vector3 (0, 0, (startPosiziton - temp * desiredPosition));

    }

    public void changeGear () {
        gearNum.text = (!PickedVehicle.reverse) ? (PickedVehicle.gearNum + 1).ToString () : "R";

    }

    public void nitrusUI () {
        nitrusSlider.value = PickedVehicle.nitrusValue / 45;
    }

    private void sortArray () {

        for (int i = 0; i < fullArray.Length; i++) {
            presentVehicles[i].HasFinished = fullArray[i].GetComponent<VehicleController>().HasFinished;
            presentVehicles[i].Name = fullArray[i].GetComponent<VehicleController> ().VehicleData.VehicleName;
            presentVehicles[i].Node = fullArray[i].GetComponent<inputManager> ().currentNode;
        }

        if(!PickedVehicle.HasFinished)
        for (int i = 0; i < presentVehicles.Count; i++) {
            for (int j = i + 1; j < presentVehicles.Count; j++) {
                if (presentVehicles[j].Node < presentVehicles[i].Node) {
                    AIVehicle QQ = presentVehicles[i];
                    presentVehicles[i] = presentVehicles[j];
                    presentVehicles[j] = QQ;
                }
            }                
        }

        
        if(arrarDisplayed)
        for (int i = 0; i < temporaryArray.Length; i++) {
            temporaryArray[i].transform.Find ("vehicle node").gameObject.GetComponent<Text> ().text = (presentVehicles[i].HasFinished)?"finished":"";
            temporaryArray[i].transform.Find ("vehicle name").gameObject.GetComponent<Text> ().text = presentVehicles[i].Name.ToString ();
        }
        presentVehicles.Reverse();
        for (int i = 0; i < temporaryArray.Length; i++) {
            if(PickedVehicle.VehicleData.VehicleName == presentVehicles[i].Name)
                currentPosition.text = ((i+1) + "/" + presentVehicles.Count).ToString();
        }



    }

    private void displayArray () {
        if(arrarDisplayed)return;
        uiList.SetActive(true);
        for (int i = 0; i < presentGameObjectVehicles.Length + 1; i++) {
            generateList (i, presentVehicles[i].HasFinished, presentVehicles[i].Name);
        }

        startPositionXvalue = -50;
        arrarDisplayed = true;
        backImage.SetActive(true);
    }

    private void generateList (int index, bool num, string nameValue) {

        temporaryArray[index] = Instantiate (uiList);
        temporaryArray[index].transform.parent = uiListFolder.transform;
        //temporaryArray[index].gameObject.GetComponent<RectTransform>().localScale = new Vector3(2,2,2);
        temporaryArray[index].gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, startPositionXvalue);
        //temporaryArray[index].transform.position = new Vector3(0,startPositionXvalue,0);
        temporaryArray[index].transform.Find ("vehicle name").gameObject.GetComponent<Text> ().text = nameValue.ToString ();
        temporaryArray[index].transform.Find ("vehicle node").gameObject.GetComponent<Text> ().text =(num)?"finished":"";
        startPositionXvalue += 50;

    }

    private IEnumerator timedLoop () {
        while (true) {
            yield return new WaitForSeconds (.7f);
            sortArray ();
        }
    }

    private void coundDownTimer(){
        if(timeLeft <= -5) return;
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0)unfreezePlayers();
        else freezePlayers();

        if(timeLeft > 1 )timeLeftText.text = timeLeft.ToString("0");
        else if(timeLeft >= -1 && timeLeft <= 1 )timeLeftText.text = "GO!";
        else timeLeftText.text ="";

    }
    
    private void freezePlayers(){
        if(countdownFlag) return;
            foreach(GameObject D in fullArray){
                D.GetComponent<Rigidbody>().isKinematic = true;
            }
            countdownFlag = true;
        
    }

    private void unfreezePlayers(){
        if(!countdownFlag) return;
            foreach(GameObject D in fullArray){
                D.GetComponent<Rigidbody>().isKinematic = false;
            }
            countdownFlag = false;
        
    }

    public void loadAwakeScene(){
        SceneManager.LoadScene("awakeScene");
    }
}