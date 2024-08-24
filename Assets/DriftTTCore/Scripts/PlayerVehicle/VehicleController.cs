﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DriftTTCore.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using EDriveType = DriftTTCore.Scripts.Data.VehicleData.EDriveType;

public class VehicleController : MonoBehaviour
{
    [SerializeField] private VehicleData vehicleData;

    private GameManager manager;
    inputManager IM;

    public int gearNum = 1;
    public bool playPauseSmoke = false, HasFinished;
    public float KPH;
    public float engineRPM;
    public bool reverse = false;
    public float nitrusValue;
    public bool nitrusFlag = false;


    private GameObject wheelMeshes, wheelColliders;
    private WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelMesh = new GameObject[4];
    private GameObject centerOfMass;
    private Rigidbody rigidbody;

    //car Shop Values

    private WheelFrictionCurve forwardFriction, sidewaysFriction;

    private float radius = 6,
        brakPower = 0,
        DownForceValue = 10f,
        wheelsRPM,
        driftFactor,
        lastValue,
        horizontal,
        vertical,
        totalPower;

    private bool flag = false;


    public VehicleData VehicleData => vehicleData;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MenuScene") return;
        getObjects();
        StartCoroutine(timedLoop());
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MenuScene") return;

        horizontal = IM.horizontal;
        vertical = IM.vertical;


        lastValue = engineRPM;


        addDownForce();
        animateWheels();
        steerVehicle();
        calculateEnginePower();
        if (gameObject.CompareTag("AI")) return;
        adjustTraction();
        activateNitrus();
    }

    private void calculateEnginePower()
    {
        wheelRPM();

        if (vertical != 0)
        {
            rigidbody.drag = 0.005f;
        }

        if (vertical == 0)
        {
            rigidbody.drag = 0.1f;
        }

        totalPower = 3.6f * VehicleData.EnginePower.Evaluate(engineRPM) * (vertical);


        float velocity = 0.0f;
        if (engineRPM >= VehicleData.MaxRPM || flag)
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, VehicleData.MaxRPM - 3500, ref velocity, 0.05f);

            flag = (engineRPM > VehicleData.MaxRPM - 3000) ? true : false;
            VehicleData.test = (lastValue > engineRPM) ? true : false;
        }
        else
        {
            engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 6f * (gearNum + 1)), ref velocity,
                VehicleData.SmoothTime);
            VehicleData.test = false;
        }

        if (engineRPM >= VehicleData.MaxRPM + 1000) engineRPM = VehicleData.MaxRPM + 1000; // clamp at max
        moveVehicle();
        shifter();
    }

    private void wheelRPM()
    {
        float sum = 0;
        int R = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }

        wheelsRPM = (R != 0) ? sum / R : 0;

        if (wheelsRPM < 0 && !reverse)
        {
            reverse = true;
            if (gameObject.tag != "AI") manager.changeGear();
        }
        else if (wheelsRPM > 0 && reverse)
        {
            reverse = false;
            if (gameObject.tag != "AI") manager.changeGear();
        }
    }

    private bool checkGears()
    {
        return (int)KPH >= VehicleData.GearsDictionary[gearNum + 1];
    }

    private void shifter()
    {
        if (!isGrounded()) return;
        //automatic
        if (engineRPM >= VehicleData.MaxRPM && gearNum < VehicleData.GearsDictionary.Count - 1 && !reverse &&
            checkGears())
        {
            gearNum++;
            if (gameObject.tag != "AI") manager.changeGear();
            return;
        }

        if (engineRPM < VehicleData.MinRPM && gearNum > 0)
        {
            gearNum--;
            if (gameObject.tag != "AI") manager.changeGear();
        }
    }

    private bool isGrounded()
    {
        if (wheels[0].isGrounded && wheels[1].isGrounded && wheels[2].isGrounded && wheels[3].isGrounded)
            return true;
        else
            return false;
    }

    private void moveVehicle()
    {
        brakeVehicle();

        if (VehicleData.DriveType == EDriveType.FullWheel)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = totalPower / 4;
                wheels[i].brakeTorque = brakPower;
            }
        }
        else if (VehicleData.DriveType == EDriveType.RearWheel)
        {
            wheels[2].motorTorque = totalPower / 2;
            wheels[3].motorTorque = totalPower / 2;

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = brakPower;
            }
        }
        else
        {
            wheels[0].motorTorque = totalPower / 2;
            wheels[1].motorTorque = totalPower / 2;

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = brakPower;
            }
        }

        KPH = rigidbody.velocity.magnitude * 3.6f;
    }

    private void brakeVehicle()
    {
        if (vertical < 0)
        {
            brakPower = (KPH >= 10) ? 500 : 0;
        }
        else if (vertical == 0 && (KPH <= 10 || KPH >= -10))
        {
            brakPower = 10;
        }
        else
        {
            brakPower = 0;
        }
    }

    private void steerVehicle()
    {
        //acerman steering formula
        //steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;

        if (horizontal > 0)
        {
            //rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontal;
        }
        else if (horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontal;
            //transform.Rotate(Vector3.up * steerHelping);
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }
    }

    private void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }

    private void getObjects()
    {
        IM = GetComponent<inputManager>();
        //else aIcontroller = GetComponent<AIcontroller>();
        manager = GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>();
        VehicleData.CarEffects = GetComponent<carEffects>();
        rigidbody = GetComponent<Rigidbody>();
        wheelColliders = gameObject.transform.Find("wheelColliders").gameObject;
        wheelMeshes = gameObject.transform.Find("wheelMeshes").gameObject;
        wheels[0] = wheelColliders.transform.Find("0").gameObject.GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.Find("1").gameObject.GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.Find("2").gameObject.GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.Find("3").gameObject.GetComponent<WheelCollider>();

        wheelMesh[0] = wheelMeshes.transform.Find("0").gameObject;
        wheelMesh[1] = wheelMeshes.transform.Find("1").gameObject;
        wheelMesh[2] = wheelMeshes.transform.Find("2").gameObject;
        wheelMesh[3] = wheelMeshes.transform.Find("3").gameObject;


        centerOfMass = gameObject.transform.Find("mass").gameObject;
        rigidbody.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void addDownForce()
    {
        rigidbody.AddForce(-transform.up * DownForceValue * rigidbody.velocity.magnitude);
    }

    private void adjustTraction()
    {
        //tine it takes to go from normal drive to drift 
        float driftSmothFactor = .7f * Time.deltaTime;

        if (IM.handbrake)
        {
            sidewaysFriction = wheels[0].sidewaysFriction;
            forwardFriction = wheels[0].forwardFriction;

            float velocity = 0;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue =
                forwardFriction.asymptoteValue =
                    Mathf.SmoothDamp(forwardFriction.asymptoteValue,
                        driftFactor * VehicleData.HandBrakeFrictionMultiplier,
                        ref velocity, driftSmothFactor);

            for (int i = 0; i < 4; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
            //extra grip for the front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels[i].sidewaysFriction = sidewaysFriction;
                wheels[i].forwardFriction = forwardFriction;
            }

            rigidbody.AddForce(transform.forward * (KPH / 400) * 10000);
        }
        //executed when handbrake is being held
        else
        {
            forwardFriction = wheels[0].forwardFriction;
            sidewaysFriction = wheels[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue =
                sidewaysFriction.asymptoteValue =
                    ((KPH * VehicleData.HandBrakeFrictionMultiplier) / 300) + 1;

            for (int i = 0; i < 4; i++)
            {
                wheels[i].forwardFriction = forwardFriction;
                wheels[i].sidewaysFriction = sidewaysFriction;
            }
        }

        //checks the amount of slip to control the drift
        for (int i = 2; i < 4; i++)
        {
            WheelHit wheelHit;

            wheels[i].GetGroundHit(out wheelHit);
            //smoke
            if (wheelHit.sidewaysSlip >= 0.3f || wheelHit.sidewaysSlip <= -0.3f || wheelHit.forwardSlip >= .3f ||
                wheelHit.forwardSlip <= -0.3f)
                playPauseSmoke = true;
            else
                playPauseSmoke = false;


            if (wheelHit.sidewaysSlip < 0) driftFactor = (1 + -IM.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);

            if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + IM.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);
        }
    }

    private IEnumerator timedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + KPH / 20;
        }
    }

    public void activateNitrus()
    {
        if (!IM.boosting && nitrusValue <= 10)
        {
            nitrusValue += Time.deltaTime / 2;
        }
        else
        {
            nitrusValue -= (nitrusValue <= 0) ? 0 : Time.deltaTime;
        }

        if (IM.boosting)
        {
            if (nitrusValue > 0)
            {
                VehicleData.CarEffects.startNitrusEmitter();
                rigidbody.AddForce(transform.forward * 5000);
            }
            else VehicleData.CarEffects.stopNitrusEmitter();
        }
        else VehicleData.CarEffects.stopNitrusEmitter();
    }
}