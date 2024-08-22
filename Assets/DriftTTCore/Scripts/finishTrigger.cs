using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finishTrigger : MonoBehaviour{

    public VehicleController vehicleController;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Finish")
            vehicleController.HasFinished = true;
            
    }

}
