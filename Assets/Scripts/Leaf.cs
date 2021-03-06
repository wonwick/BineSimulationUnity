﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    public GameObject[] plant;
    public GameObject attachedBeed;
    GameObject startBead;
    public int destinationBeadNumber;
    public int currentBeadNumber;
    public float growthRate;
    public float currentScale;
    public bool FirstLeaf = true;
    public bool isSimpleGrowth = true;
    public Vector3[] cn;
    GameObject subleaf1;
    GameObject subleaf2;
    GameObject subleaf3;
    Vector3 targetEuler = new Vector3(0, 0, 0);
    public float startAngle = 85f;
    public float endAngle = 30;
    public bool isLeafTriplet;
    Vector3 currentGrowthDirection;
    public Vector3 horizntalOffset;
    public Vector3 VerticalOffset;
    public float endleafSetGrowthAngle;
    private bool firstTime =true;
    TubeRenderer tubeRenderer;
    public float StemGrowthSpeed;
    Vector3[] stem;
    float[] girths;
    float Speed;
    float SpanningSpeed;
    float leafScalingSpeed;
    float stemGrowthSpeed;
    float rotatingSpeed;
    float heliotropicMovementAngle = 0.1f;
    int heliotropicMovementDirection = 1;
    float upwardRoatingAmount = 0.1f;
    Vector3 startingForwardDirection;
    bool heliotropicLeafs = false;
    // Use this for initialization
    void Start()
    {
        Speed = 0.5f* 1;
        SpanningSpeed = 10*Speed;
        leafScalingSpeed = 50    * Speed;
        stemGrowthSpeed = 100 * Speed;
        rotatingSpeed = 100 * Speed;
        StemGrowthSpeed = 0.0005f;
        endleafSetGrowthAngle = 30f;
        growthRate = 0.0001f*leafScalingSpeed;
        girths = new float[2] { 0.05f, 0.025f };
        stem = new Vector3[2]; 
        currentScale = 0;
        horizntalOffset = Vector3.zero;
        VerticalOffset = Vector3.zero;
        subleaf1 = gameObject.transform.GetChild(0).gameObject;
        subleaf2 = gameObject.transform.GetChild(1).gameObject;
        if (isLeafTriplet) {
            subleaf3 = gameObject.transform.GetChild(2).gameObject;
        }
        reset();
        tubeRenderer = gameObject.GetComponent<TubeRenderer>();
        tubeRenderer.enabled = false;
        plant = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<Bine>().plant;
        cn = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<Bine>().cn;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward, Color.cyan);
        //Debug.DrawRay(gameObject.transform.position, currentGrowthDirection, Color.blue);

        if (isSimpleGrowth)
        {
            simpleLeafGrowth();
        }
        else
        {
            complexLeafGrowth();

        }
        if (heliotropicLeafs)
        {
            //showHeliotropicMovements();
        }
    }

    void simpleLeafGrowth()
    { if (currentBeadNumber > 1)
        {

            if (currentBeadNumber < destinationBeadNumber)
            {
                //Debug.Log(gameObject.name + " : " + currentBeadNumber + "\n");
                if (plant.Length< (currentBeadNumber - 2)) {
                    Debug.Log("plantLength:" + plant.Length + " VS " + (currentBeadNumber - 2));
                }
                
                attachedBeed = plant[currentBeadNumber - 2];
                if ((currentBeadNumber - 2) <= cn.Length) {
                    currentGrowthDirection = cn[currentBeadNumber - 2];
                    
                    if (Vector3.Equals(currentGrowthDirection, Vector3.zero))
                    {
                        Debug.Log("Leaf without Collion normal");
                        currentGrowthDirection = Vector3.Cross(attachedBeed.transform.forward, Vector3.up);
                    }
                }

               
                else
                {
                    Debug.Log("null Vector Found Handle This!!!"+"  cnLength:" + cn.Length + " VS " + (currentBeadNumber - 2));                   
                    return;
                }

       

                gameObject.transform.position = attachedBeed.transform.position;

                if (firstTime)
                {
                    gameObject.transform.forward = attachedBeed.transform.forward;
                    startBead = attachedBeed;
                    firstTime = false;
                }
                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, attachedBeed.transform.forward, 0.001f*rotatingSpeed, 0.5f);
            }


            //else {
            //    if (Vector3.Distance(gameObject.transform.position, plant[destinationBeadNumber + 1].transform.position) > 0.01)
            //    {
            //        gameObject.transform.position = Vector3.Slerp(gameObject.transform.position, plant[destinationBeadNumber + 1].transform.position, 0.01f);
            //       // gameObject.transform.forward = Vector3.Slerp(gameObject.transform.forward, plant[destinationBeadNumber + 1].transform.forward, 0.0001f);
            //    }
            //    else {
            //        destinationBeadNumber++;
            //    }   
            //}

            if (!FirstLeaf)
            {
                //Debug.DrawRay(transform.position, attachedBeed.transform.forward, Color.red);
                //Debug.DrawRay(transform.position, currentGrowthDirection, Color.green);
                //Debug.DrawRay(transform.position, transform.forward, Color.blue);
                isLeafTriplet = true;
            }
            if (currentScale < 7)
            {
                gameObject.transform.localScale = Mathf.SmoothStep(0f, 3.0f, currentScale) * new Vector3(1, 1, 1);
                currentScale = currentScale + growthRate;
            }
            if (!(currentScale < 3) && !heliotropicLeafs)
            {
                heliotropicLeafs = true;
                startingForwardDirection = gameObject.transform.forward;
            }
            if (Vector3.Angle(gameObject.transform.forward, currentGrowthDirection) > endleafSetGrowthAngle)
            {
                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, currentGrowthDirection, 0.0005f*rotatingSpeed, 0.5f);
                
            }
            if (Vector3.Angle(gameObject.transform.forward, Vector3.up) > 30) {

                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, Vector3.up, 0.0005f * rotatingSpeed, 0.5f);
            }
            horizntalOffset = Vector3.Slerp(horizntalOffset, 2 * currentGrowthDirection.normalized, 0.0001f*stemGrowthSpeed);
            VerticalOffset =  Vector3.Slerp(VerticalOffset, 2 * Vector3.up.normalized, 0.00005f * stemGrowthSpeed);

            gameObject.transform.position = attachedBeed.transform.position + horizntalOffset+ VerticalOffset;
            //gameObject.transform.position = Vector3.Slerp(gameObject.transform.position, attachedBeed.transform.position+currentGrowthDirection.normalized,0.01f);
            stem[1] = transform.InverseTransformPoint(gameObject.transform.position);

            //Debug.Log("StartBeadAvailable:"+(startBead!=null)+"\n");
            stem[0] = transform.InverseTransformPoint(attachedBeed.transform.position);           
            renderStem();
        }
    }

    void complexLeafGrowth()
    {
        simpleLeafGrowth();
        subleaf1.transform.localRotation = Quaternion.Lerp(subleaf1.transform.localRotation, Quaternion.Euler(0, -1*endAngle, 0), 0.0005f*SpanningSpeed);
        subleaf2.transform.localRotation = Quaternion.Lerp(subleaf2.transform.localRotation, Quaternion.Euler(0, endAngle, 0), 0.0005f*SpanningSpeed);
        if (isLeafTriplet) {
            subleaf3.transform.localRotation = Quaternion.Lerp(subleaf3.transform.localRotation, Quaternion.Euler(endAngle, 0, 0), 0.0005f*SpanningSpeed);
        }
        //subleaf1.transform.localPosition = Vector3.Lerp(subleaf1.transform.localPosition, new Vector3(-1, 0, 0), 0.005f);
        //subleaf2.transform.localPosition = Vector3.Lerp(subleaf2.transform.localPosition, new Vector3(1, 0, 0), 0.005f);

    }

    private void reset()
    {
        firstTime = true;
        currentScale = 0;
        subleaf1.transform.localRotation = Quaternion.Euler(0, startAngle, 0);
        subleaf2.transform.localRotation = Quaternion.Euler(0, -1*startAngle, 0);
        if (isLeafTriplet) {
            subleaf3.transform.localRotation = Quaternion.Euler(-1*startAngle, 0, 0);
        }
    }


    private void renderStem()
    {
        tubeRenderer.enabled = true;
        tubeRenderer.SetBinePoints(stem, girths, Color.cyan);
    }

    void showHeliotropicMovements()
    {
        Debug.Log("heliotropic Movements\n  ");
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.right, Color.black, 5);
        Debug.DrawRay(gameObject.transform.position, currentGrowthDirection, Color.green, 5);

        //if (Vector3.Angle(gameObject.transform.forward, startingForwardDirection) > 45)
        //{
        //   heliotropicMovementDirection = heliotropicMovementDirection * -1;
        //}
        gameObject.transform.RotateAroundLocal(currentGrowthDirection,  0.5f);
        //gameObject.transform.RotateAround(transform.position, currentGrowthDirection, heliotropicMovementDirection * 0.1f);




    }
}
