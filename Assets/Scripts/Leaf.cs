using System.Collections;
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
    public Vector3 offset;
    public float endleafSetGrowthAngle;
    private bool firstTime =true;
    TubeRenderer tubeRenderer;
    Vector3[] stem;
    float[] girths;
    // Use this for initialization
    void Start()
    {
        endleafSetGrowthAngle = 30f;
        growthRate = 0.001f;
        girths = new float[2] { 0.05f, 0.025f };
        stem = new Vector3[2]; 
        currentScale = 0;
        offset = Vector3.zero;
        subleaf1 = gameObject.transform.GetChild(0).gameObject;
        subleaf2 = gameObject.transform.GetChild(1).gameObject;
        if (isLeafTriplet) {
            subleaf3 = gameObject.transform.GetChild(2).gameObject;
        }
        reset();
        tubeRenderer = gameObject.GetComponent<TubeRenderer>();
        tubeRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSimpleGrowth)
        {
            simpleLeafGrowth();
        }
        else
        {
            complexLeafGrowth();

        }
    }

    void simpleLeafGrowth()
    { if (currentBeadNumber > 1)
        {

            if (currentBeadNumber < destinationBeadNumber)
            {
                Debug.Log(gameObject.name + " : " + currentBeadNumber + "\n");
                attachedBeed = plant[currentBeadNumber - 2];
                currentGrowthDirection = cn[currentBeadNumber - 2];
                gameObject.transform.position = attachedBeed.transform.position;

                if (firstTime)
                {
                    gameObject.transform.forward = attachedBeed.transform.forward;
                    startBead = attachedBeed;
                    firstTime = false;
                }
                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, attachedBeed.transform.forward, 0.001f, 0.5f);
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
                Debug.DrawRay(transform.position, attachedBeed.transform.forward, Color.red);
                Debug.DrawRay(transform.position, currentGrowthDirection, Color.green);
                Debug.DrawRay(transform.position, transform.forward, Color.blue);
                isLeafTriplet = true;
            }
            if (currentScale < 7)
            {
                gameObject.transform.localScale = Mathf.SmoothStep(0f, 3.0f, currentScale) * new Vector3(1, 1, 1);
                currentScale = currentScale + growthRate;
            }
            if (Vector3.Angle(gameObject.transform.forward, currentGrowthDirection) > endleafSetGrowthAngle)
            {
                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, currentGrowthDirection, 0.0005f, 0.5f);
                
            }
            if (Vector3.Angle(gameObject.transform.forward, Vector3.up) > 30) {

                gameObject.transform.forward = Vector3.RotateTowards(gameObject.transform.forward, Vector3.up, 0.0005f, 0.5f);
            }
            offset = Vector3.Slerp(offset, 2 * currentGrowthDirection.normalized, 0.001f);
            gameObject.transform.position = attachedBeed.transform.position + offset;
            //gameObject.transform.position = Vector3.Slerp(gameObject.transform.position, attachedBeed.transform.position+currentGrowthDirection.normalized,0.01f);
            stem[1] = transform.InverseTransformPoint(gameObject.transform.position);
            stem[0] = transform.InverseTransformPoint(startBead.transform.position);
            Debug.Log("stem: " + stem[0] + " & " + stem[1] + " \n");
            renderStem();
        }
    }

    void complexLeafGrowth()
    {
        simpleLeafGrowth();
        subleaf1.transform.localRotation = Quaternion.Lerp(subleaf1.transform.localRotation, Quaternion.Euler(0, -1*endAngle, 0), 0.0005f);
        subleaf2.transform.localRotation = Quaternion.Lerp(subleaf2.transform.localRotation, Quaternion.Euler(0, endAngle, 0), 0.0005f);
        if (isLeafTriplet) {
            subleaf3.transform.localRotation = Quaternion.Lerp(subleaf3.transform.localRotation, Quaternion.Euler(endAngle, 0, 0), 0.0005f);
        }
        //subleaf1.transform.localPosition = Vector3.Lerp(subleaf1.transform.localPosition, new Vector3(-1, 0, 0), 0.005f);
        //subleaf2.transform.localPosition = Vector3.Lerp(subleaf2.transform.localPosition, new Vector3(1, 0, 0), 0.005f);

    }

    private void reset()
    {
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
}
