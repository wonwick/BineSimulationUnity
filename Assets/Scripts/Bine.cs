using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Profiling;


public class Bine : MonoBehaviour {
    int lastEntry;
    int usedMemory;
    bool end = false;
    int fps;
    int reportIterator;
    int[,] report;
    int inTwineConsecutiveTimes = 0;
    bool[] nodes;
    float nutationalDirection =1f;
    GameObject previiousBead;
    Vector3 targetLastBeadPosition;
    float growthSpeed = 1;
    float InterNodeGrowth = 0;
    public int initialPhaseBeadCount;
    public bool inInitialPhase = true;
    public float initialPhaseInterBeadDistance;
    public int leafTravelDistance; 
    public int beedGapPerLeaf;
    bool addLeaf = true;
    public int currentNumberOfBeeds = 0;
    public int currentNumberOfLeaves = 0;
    public float leafGrowthRate;
    public GameObject seed;
    public GameObject leaf;
    public GameObject secLeaf;
    private GameObject newLeaf;
    private int frameCount = 0;
    public int growthTime = 50;
    public int particleCount = 10;
    public GameObject beed;
    public float radius = 1;
    public GameObject[] plant;
    GameObject[] leaves;
    public Vector3[] cn;
    GameObject lastBeed;
    public float beedDistance = 1;
    public Vector3 maxBeedRotation = new Vector3(5, 5, 5);
    public Vector3 circumnutationSpeed = new Vector3(0, 5, 0);
    public bool circumnutationOn = true;
    private Collision beedCollision;
    private GameObject thisGameObject;
    private bool supportFound=false;
    public int startingBeed = 0;
    private Vector3 newGrowthDirection;
    private Vector3 collisionNormal;
    public float gravitrophismLimit = 45;
    public float gravitrophismCorrectionValue = 0.01f;
    public float gravitrophismAbsoluteLimit = 80;
    private bool supportLost=false;
    private Vector3 theUpward = new Vector3(0, 100, 0);
    public float supportLostGravitrophismAdjustment = 0.01f;
    public float reCircumnutateReadyAngle = 45;
    public float growthAngleBias = 30;
    public float supportedCircumnutationSpeed = 1;
    TubeRenderer tubeRenderer;
    Vector3[] beedPositions;
    float[] girths;
    public float maxGirth = 2;
    public bool hasGroped = false;
    private bool IKInitiated = false;
    public int NoOfIKIterations = 50;
    private bool groping = false;
    Collider collidingObject;
    Collider collidingBeed;
    Leaf theLeaf;
    GameObject psudoGameObject;
    int numberOfBeasTillCotyldons = 11;

    bool growWithoutSupport = false;
    bool growAroundSupport = false;
    bool getReadyToCircumnutate = false;

    bool seedRisen = false;
    bool primaryLeafRisen = false;
    bool primaryLeafGrown =false;

    public float nutatingAmountPerTurn = 5;
    float nutatingAmountPerTurnChange = 1;
    public float speed;
    public float upwardTurningSpeed;
    bool isStemOverGrown = false;

    private void Awake()
    {
        usedMemory = 0;
        lastEntry = 0;
        reportIterator = 0;
        thisGameObject = this.gameObject;
        report = new int[2000,4];
        plant = new GameObject[particleCount];
        nodes = new bool[particleCount];
        nodes[numberOfBeasTillCotyldons] = true;
        nodes[numberOfBeasTillCotyldons+1] = true;
        leaves = new GameObject[particleCount / beedGapPerLeaf];
        cn = new Vector3[particleCount];
        beedPositions = new Vector3[particleCount];
        girths = new float[particleCount];
        lastBeed = Instantiate(beed, transform.position, transform.rotation);
        lastBeed.name = thisGameObject.name + "_beed_0";
        lastBeed.transform.parent = thisGameObject.transform;
        //Vector3 initRotation = new Vector3(270, 0, 0); //sphericalbeed
        tubeRenderer = thisGameObject.GetComponent<TubeRenderer>();

        Vector3 initRotation = new Vector3(270, 0, 0); //cylindericalBeed

        lastBeed.transform.Rotate(initRotation);
        plant[currentNumberOfBeeds] = lastBeed;
        currentNumberOfBeeds++;
        //theLeaf = leaf.GetComponent<Leaf>();
        //theLeaf.FirstLeaf = true;
        //theLeaf.plant = plant;
        //theLeaf.destinationBeadNumber = 5;
        //theLeaf.currentBeadNumber = currentNumberOfBeeds;
        //leaves[0] = leaf;
        //currentNumberOfLeaves++;
    }

    void Start() {
        //plant initail stage building
        psudoGameObject = new GameObject();
        float curve = 270;
        float curveAmount = 3;
        lastBeed.transform.eulerAngles=new Vector3(250, 180,180);
        Vector3 Beanforward = lastBeed.transform.eulerAngles;
        //Debug.Log("startingbeadForward.Y: " + Beanforward+ "\n");
        //Debug.Log("begins \n");
        upwardTurningSpeed = upwardTurningSpeed* speed;
        while (curve > 90)
        {
            curveAmount += 3;
            curve -= curveAmount;
            Vector3 newPosition = lastBeed.transform.position + lastBeed.transform.forward * initialPhaseInterBeadDistance;
            // removing the colider of previouse beed
            DestroyImmediate(lastBeed.GetComponent<Collider>());
            DestroyImmediate(lastBeed.GetComponent<Beed>());
            lastBeed = Instantiate(beed, newPosition, lastBeed.transform.rotation);
            lastBeed.name = "Beed" + currentNumberOfBeeds;
            lastBeed.transform.parent = thisGameObject.transform;
            lastBeed.transform.eulerAngles = new Vector3(curve,0, 0);
            plant[currentNumberOfBeeds] = lastBeed;
            Beanforward = lastBeed.transform.rotation.eulerAngles;
            //Debug.Log("     beadForward.Y: " + Beanforward+ "\n");
            currentNumberOfBeeds++;
        }

        seed.transform.position = lastBeed.transform.position;


        for (int i = 0; i < 5; i++)
        {
            Vector3 newPosition = lastBeed.transform.position + lastBeed.transform.forward * 0.000001f;
            // removing the colider of previouse beed
            Vector3 prevForward = lastBeed.transform.forward;
            DestroyImmediate(lastBeed.GetComponent<Collider>());
            DestroyImmediate(lastBeed.GetComponent<Beed>());
            lastBeed = Instantiate(beed, newPosition, lastBeed.transform.rotation);
            lastBeed.name = "Beed" + currentNumberOfBeeds;
            lastBeed.transform.forward = prevForward;
            lastBeed.transform.parent = thisGameObject.transform;
            plant[currentNumberOfBeeds] = lastBeed;
            currentNumberOfBeeds++;
        }

        //seed.transform.eulerAngles =     

        //Debug.Log("ends \n");

    }

    private void FixedUpdate() {

        
        GenerateReport();
        //Debug.DrawRay(lastBeed.transform.position,lastBeed.transform.forward,Color.cyan,1);
        //Debug.DrawRay(seed.transform.position, seed.transform.forward, Color.yellow);
        // plant growth
        if (inInitialPhase)
        {

            if (!seedRisen)
            {
                //Debug.Log("seed Rising\n");
                seedRises();
                //Debug.Log( "angle: "+Vector3.Angle(plant[currentNumberOfBeeds-1].transform.forward, Vector3.up)+"\n");
                if (Vector3.Angle(plant[11 - 1].transform.forward, Vector3.up) <= 100)
                {
                    seedRisen = true;
                    startingBeed = currentNumberOfBeeds - 2;

                }
            }

            else if (!primaryLeafRisen)
            {
                //Debug.Log("primary leaf Rising\n");

                raiseThePrimaryLeaves();
                if (Vector3.Angle(lastBeed.transform.forward, Vector3.up) <= 1f)
                {
                    lastBeed.transform.forward = Vector3.up;
                    seedRisen = true;

                    startingBeed = currentNumberOfBeeds - 2;
                    growWithoutSupport = true;
                    primaryLeafRisen = true;

                }
            }

            else if (!primaryLeafGrown)
            {
                //Debug.Log("primary leaf Growing\n");

                primaryLeafGrown = growingPrimaryLeves();
            }

            else
            {
                Debug.Log("initial phase ending\n");

                inInitialPhase = false;
            }
        }

        else if (growWithoutSupport)
        {
            SupportLessGrowth();
            growWithoutSupport = false;
        }

        else if(isStemOverGrown)
        {
            fallingStem();
        }

        else if (groping)
        {
            grope();
        }

        else if (growAroundSupport)
        {
            growWithSupportIntact();
            growAroundSupport = false;
        }

        else if (getReadyToCircumnutate)
        {
            getReadyToCircumnutate = false;
            supportLostGrowth();
        }

        else
        {

            if (circumnutationOn)
            {
                if (!supportFound)
                {
                    circumnutation();
                }
                else
                {
                    twine();
                }
            }

        }    

        RenderTheStem();

        if (currentNumberOfBeeds > 2)
        {
            LeafGrowth();
        }
    }

    void growWithSupportIntact() {
        //Debug.Log("supportFoundGrowth\n");
        //Debug.DrawRay(beedCollision.contacts[0].point, collisionNormal, Color.green, 60*5, false);
        newGrowthDirection = collisionNormal;

        newGrowthDirection = Vector3.RotateTowards(lastBeed.transform.forward, newGrowthDirection, Mathf.Deg2Rad * growthAngleBias, 0.0f);
        //Debug.DrawRay(lastBeed.transform.position, collisionNormal*20, Color.yellow, 60 * 5, false);           
        //Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward, Color.blue, 60*5, false);
        //Debug.DrawRay(lastBeed.transform.position, newGrowthDirection * 2, Color.red, 60 * 5, false);

        //SupportedGrowth(newGrowthDirection);

        SupportedGrowth(lastBeed.transform.forward);


        //supportFound = false;
        circumnutationOn = true;
    }

    bool growingPrimaryLeves()
    {
        float growingPrimaryLevesSpeed = 1 * speed;
        float growthLimit =4;
        float nutationSpeed = growingPrimaryLevesSpeed * 5;        
        float primaryleafGrowingSpeed = growingPrimaryLevesSpeed * 5;
        float stemGrowthSpeed = growingPrimaryLevesSpeed * 0.7f;
        float leafScalingSpeed = growingPrimaryLevesSpeed * 1;
        float rotateAmount = 0.0002f * growingPrimaryLevesSpeed;
        bool readyToleaveThisState = false;
        if (leaf.transform.localScale.x <= growthLimit)
        {
            initialPhaseInterBeadDistance += 0.01f* stemGrowthSpeed;
            for (int i = numberOfBeasTillCotyldons-1; i < currentNumberOfBeeds - 1; i++)
            {
                plant[i].transform.forward = Vector3.RotateTowards(plant[i].transform.forward, nutationalDirection*Vector3.right, rotateAmount* nutationSpeed, 0);
                Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward * initialPhaseInterBeadDistance* stemGrowthSpeed;
                plant[i + 1].transform.position = newPosition;
                rotateAmount += nutatingAmountPerTurn*0.0001f * nutationSpeed;
            }
            
            lastBeed.transform.forward = Vector3.RotateTowards(lastBeed.transform.forward, nutationalDirection * Vector3.right, rotateAmount, 0);

            // chnage the nutating amount per each direction change;
            if (nutatingAmountPerTurn > 1)
            {
                nutatingAmountPerTurn -= 0.01f;
            }

            // Change nutational direction alternatively
            if (Vector3.Angle(plant[numberOfBeasTillCotyldons].transform.forward, Vector3.up) > nutatingAmountPerTurn)
            {
                nutationalDirection = nutationalDirection * -1;
            }

            leaf.transform.position = lastBeed.transform.position;

            //leaf.transform.forward = lastBeed.transform.forward;

      
            float spanningSpeed = 1.5f * growingPrimaryLevesSpeed;
            leaf.transform.GetChild(0).transform.localRotation = Quaternion.Lerp(leaf.transform.GetChild(0).transform.localRotation, Quaternion.Euler(0, -1 * 30, 0), 0.005f * spanningSpeed);
            leaf.transform.GetChild(1).transform.localRotation = Quaternion.Lerp(leaf.transform.GetChild(1).transform.localRotation, Quaternion.Euler(0, 30, 0), 0.005f * spanningSpeed);
       
            leaf.transform.localScale = new Vector3(leaf.transform.lossyScale.x + 0.01f * leafScalingSpeed, leaf.transform.localScale.y + 0.03f * leafScalingSpeed, leaf.transform.lossyScale.z + 0.01f * leafScalingSpeed);
        }
        else
        {
            // Once the leaf growth compleats re-arranging the stem after above nutation 
            for (int i = numberOfBeasTillCotyldons - 1; i < currentNumberOfBeeds - 1; i++)
            {
                plant[i].transform.forward = Vector3.RotateTowards(plant[i].transform.forward,Vector3.up, rotateAmount, 0);
                Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward * initialPhaseInterBeadDistance * stemGrowthSpeed;
                plant[i + 1].transform.position = newPosition;
                rotateAmount += 0.001f * primaryleafGrowingSpeed;
            }

            lastBeed.transform.forward = Vector3.RotateTowards(lastBeed.transform.forward,Vector3.up, rotateAmount, 0);

            leaf.transform.position = lastBeed.transform.position;
            //leaf.transform.forward = lastBeed.transform.forward;

            //Debug.Log("$$$" + leaf.transform.eulerAngles.y + "$$$");

            // After adjusted enough leaving the Primary Leaf Growth State
            if (Vector3.Angle(Vector3.up, lastBeed.transform.forward) < 1)
            {
                 Rigidbody rb = seed.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;
                readyToleaveThisState = true;
                //leaf.transform.Rotate(new Vector3(0, 0, 90));

            }
        }
        // Addressing gimbelLock like issue hack to set forward direction of primary leaf
        if (seed.transform.GetChild(0).localScale.z > 0.05) {
            seed.transform.GetChild(0).localScale = seed.transform.GetChild(0).localScale + (Vector3.back * 0.0001f);
            seed.transform.GetChild(1).localScale = seed.transform.GetChild(1).localScale + (Vector3.back * 0.0001f);

        }
        psudoGameObject.transform.position = lastBeed.transform.position + lastBeed.transform.forward*5;
        leaf.transform.rotation = Quaternion.LookRotation(lastBeed.transform.forward, leaf.transform.up);
        Debug.DrawRay(leaf.transform.position, leaf.transform.forward, Color.black, 10);
        Debug.DrawRay(leaf.transform.position, leaf.transform.forward, Color.black, 10);

        return readyToleaveThisState;
    }

    void raiseThePrimaryLeaves() {
        initialPhaseInterBeadDistance += 0.0005f;
        float primaryLeavesRaisingSpeed =1;
        float stemGrowthSpeed =primaryLeavesRaisingSpeed*0.5f;
        float rotateAmount = 0.015f* primaryLeavesRaisingSpeed;
        for (int i = 10; i < currentNumberOfBeeds - 1; i++)
        {
            //plant[i].transform.rotation = Quaternion.Lerp(plant[i].transform.rotation, Quaternion.LookRotation(Vector3.up), Time.time * 0.0001f);
            //Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward * initialPhaseInterBeadDistance;
            plant[i].transform.forward = Vector3.RotateTowards(plant[i].transform.forward, Vector3.up, rotateAmount, 0);
            Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward * initialPhaseInterBeadDistance* stemGrowthSpeed;
            plant[i + 1].transform.position = newPosition;
            //plant[i + 1].transform.forward = plant[i].transform.forward;
            rotateAmount -= 0.002f* primaryLeavesRaisingSpeed;
        }
        lastBeed.transform.forward = Vector3.RotateTowards(lastBeed.transform.forward, Vector3.up, rotateAmount, 0);

        leaf.transform.position = lastBeed.transform.position;
        leaf.transform.forward = lastBeed.transform.forward;
        float growthSpeed = 0.2f*primaryLeavesRaisingSpeed;
        if (leaf.transform.localScale.x < 7)
        {
            leaf.transform.localScale = new Vector3(leaf.transform.lossyScale.x + 0.01f* growthSpeed, leaf.transform.localScale.y + 0.03f* growthSpeed, leaf.transform.lossyScale.z + 0.01f * growthSpeed);
        }

        float spanningSpeed = 0.2f*primaryLeavesRaisingSpeed;
        leaf.transform.GetChild(0).transform.localRotation = Quaternion.Lerp(leaf.transform.GetChild(0).transform.localRotation, Quaternion.Euler(0, -1 * 30, 0), 0.005f* spanningSpeed);
        leaf.transform.GetChild(1).transform.localRotation = Quaternion.Lerp(leaf.transform.GetChild(1).transform.localRotation, Quaternion.Euler(0, 30, 0), 0.005f* spanningSpeed);

    }

    void seedRises() {
        float seedRisingSpeed = 1;
        initialPhaseInterBeadDistance += 0.0005f;
   
        //Debug.Log("currentNumberOfBeeds: " + numberOfBeasTillCotyldons + "\n");
        for (int i = 0; i < numberOfBeasTillCotyldons - 1; i++)
        {

            plant[i].transform.rotation = Quaternion.Lerp(plant[i].transform.rotation, Quaternion.LookRotation(Vector3.up), Time.time * 0.001f);
            Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward * initialPhaseInterBeadDistance;
            plant[i + 1].transform.position = newPosition;

        }
        
        plant[numberOfBeasTillCotyldons - 1].transform.rotation = Quaternion.Lerp(plant[numberOfBeasTillCotyldons - 1].transform.rotation, Quaternion.LookRotation(Vector3.back), Time.time * 0.0005f);

        seed.transform.position = plant[numberOfBeasTillCotyldons - 1].transform.position;
        seed.transform.forward = plant[numberOfBeasTillCotyldons - 1].transform.forward;
        


        seed.transform.GetChild(0).transform.localPosition = seed.transform.GetChild(0).transform.localPosition + new Vector3(+0.0004f, 0, 0);
        seed.transform.GetChild(1).transform.localPosition = seed.transform.GetChild(1).transform.localPosition + new Vector3(-0.0004f, 0, 0);

        seed.transform.GetChild(0).transform.localRotation = Quaternion.Euler(seed.transform.GetChild(0).transform.localRotation.eulerAngles + new Vector3(0,+0.005f,0));
        seed.transform.GetChild(1).transform.localRotation = Quaternion.Euler(seed.transform.GetChild(1).transform.localRotation.eulerAngles + new Vector3(0,-0.005f, 0));

        for (int i = numberOfBeasTillCotyldons-1; i < currentNumberOfBeeds-1 ; i++)
        {
            Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward * initialPhaseInterBeadDistance*0.5f;
            plant[i + 1].transform.forward = plant[i].transform.forward;
            plant[i + 1].transform.position = newPosition;
        }
        leaf.transform.position = lastBeed.transform.position;
        leaf.transform.forward = lastBeed.transform.forward;

        float LeafGrowthSpeed = 5 * seedRisingSpeed;

        leaf.transform.localScale = new Vector3(leaf.transform.lossyScale.x + 0.0001f* LeafGrowthSpeed, leaf.transform.localScale.y + 0.0001f* LeafGrowthSpeed, leaf.transform.lossyScale.z + 0.0001f* LeafGrowthSpeed);

    }

    void RenderTheStem() {
        beedPositions = new Vector3[currentNumberOfBeeds];
        for (int i = 0; i < currentNumberOfBeeds; i++)
        {
            beedPositions[i] = plant[i].transform.localPosition;
            girths[i] = tapperFunction_4(i, currentNumberOfBeeds, particleCount, maxGirth);
            if (nodes[i])
            {
                girths[i-2] = girths[i-2] * 1.2f;
            }
        }

        if (currentNumberOfBeeds > 1)
        {
            tubeRenderer.enabled = true;
            tubeRenderer.SetBinePoints(beedPositions, girths, Color.cyan);
        }
    }

    void LeafGrowth()
    {
        if ((currentNumberOfBeeds % beedGapPerLeaf == 0) && addLeaf)
        {
            nodes[currentNumberOfBeeds] = true;

            if (supportFound)
            {               
                newLeaf = Instantiate(secLeaf, secLeaf.transform.position, secLeaf.transform.rotation);
                newLeaf.transform.rotation = Quaternion.Euler(newLeaf.transform.rotation.eulerAngles + new Vector3(0, 90, 0));  
                newLeaf.name = "leaf" + currentNumberOfLeaves;
                theLeaf = newLeaf.GetComponent<Leaf>();
                newLeaf.transform.parent = thisGameObject.transform.FindChild("leaves");
                theLeaf.FirstLeaf = false;
                theLeaf.destinationBeadNumber = currentNumberOfBeeds + leafTravelDistance;
                theLeaf.currentBeadNumber = currentNumberOfBeeds;
                leaves[currentNumberOfLeaves] = newLeaf;
                currentNumberOfLeaves++;
                addLeaf = false;

 

            }

            else
            {/*
                newLeaf = Instantiate(secLeaf, secLeaf.transform.position, secLeaf.transform.rotation);
                newLeaf.transform.rotation = Quaternion.Euler(newLeaf.transform.rotation.eulerAngles + new Vector3(0, 90, 0));
                newLeaf.name = "leaf" + currentNumberOfLeaves;
                theLeaf = newLeaf.GetComponent<Leaf>();
                newLeaf.transform.parent = thisGameObject.transform.FindChild("leaves");
                theLeaf.FirstLeaf = false;
                
                theLeaf.destinationBeadNumber = currentNumberOfBeeds + 5;
                theLeaf.currentBeadNumber = currentNumberOfBeeds;
                leaves[currentNumberOfLeaves] = newLeaf;
                currentNumberOfLeaves++;
                addLeaf = false;
                */
            }
        }

        if (!(currentNumberOfBeeds % 5 == 0))
        {
            addLeaf = true;

        }

        for (int i = 0; i< currentNumberOfLeaves; i++)
        {
            theLeaf = leaves[i].GetComponent<Leaf>();
            theLeaf.currentBeadNumber = currentNumberOfBeeds;
        }
            
    }

    void grope()
    {
        /*
        Debug.Log("groping :D \n");
        Debug.DrawRay(lastBeed.transform.position,lastBeed.transform.forward,Color.cyan,5*60);
        bool Done = true;
        if (Vector3.Angle(lastBeed.transform.forward, collisionNormal) > 90)
        {
            for (int i = startingBeed; i < currentNumberOfBeeds - 1; i++)
            {
                plant[i].transform.forward = Vector3.RotateTowards(plant[i].transform.forward, collisionNormal, 0.01f, 0);
                plant[i + 1].transform.position = plant[i].transform.position + plant[i].transform.forward * beedDistance;
            }

            lastBeed.transform.forward = Vector3.RotateTowards(lastBeed.transform.forward, collisionNormal, 0.02f, 0);
            Done = false;
            initialPhaseInterBeadDistance = beedDistance;

        }
        */

        if (Vector3.Angle(lastBeed.transform.forward, Vector3.up) > 45)
        {

            for (int i = startingBeed; i < currentNumberOfBeeds - 1; i++)
            {
                plant[i].transform.forward = Vector3.RotateTowards(plant[i].transform.forward, Vector3.up, 0.01f, 0);
                plant[i + 1].transform.position = plant[i].transform.position + plant[i].transform.forward * beedDistance;
            }

            lastBeed.transform.forward = Vector3.RotateTowards(lastBeed.transform.forward, Vector3.up, 0.01f, 0);
            //Done = false;
            initialPhaseInterBeadDistance = beedDistance;
            //groping = false;
            //hasGroped = true;
            //startingBeed = currentNumberOfBeeds;
            //growAroundSupport = true;
        }
        
        //if (Done)
        else
        {
            Debug.Log("In Finishing Groaping\n");
            Rigidbody rb=lastBeed.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            hasGroped = true;
            initialPhaseInterBeadDistance = initialPhaseInterBeadDistance + 0.01f;
            plant[currentNumberOfBeeds - 2].transform.forward = Vector3.RotateTowards(plant[currentNumberOfBeeds - 2].transform.forward, collisionNormal*-1, 0.001f, 0);

            lastBeed.transform.position = plant[currentNumberOfBeeds-2].transform.position + plant[currentNumberOfBeeds-2].transform.forward * initialPhaseInterBeadDistance;     
            
        }
        //if (collidingBeed.bounds.Intersects(collidingObject.bounds))
        //{
        //    Vector3 newDirection = lastBeed.transform.forward;
        //    newDirection.y = 0.0f;
        //    Vector3 translatingDirection = Vector3.RotateTowards(Vector3.Normalize(newDirection), -1*Vector3.Normalize(collisionNormal), 1, 0);
        //    lastBeed.transform.Translate(0.001f * translatingDirection);
        //   //Vector3 newRotation = Vector3.RotateTowards(lastBeed.transform.forward, collisionNormal, supportLostGravitrophismAdjustment, 0);
        //    //lastBeed.transform.rotation = Quaternion.LookRotation(newRotation);
        //    //Debug.Log("goaping: translating");

        //}
        //else {
        //    groping = false;
        //    circumnutationOn = true;
        //    hasGroped = true;

        //}


    }   

    void initGrope()
    {
        
        //if (!IKInitiated) {
        //    circumnutationOn = false;
        //    Debug.Log("initialting IK\n");
        //    IKSolver theIKSolver =lastBeed.AddComponent<IKSolver>() as IKSolver;
        //    GameObject g;
        //    g = new GameObject();
        //    g.name = "IKpoletarget";
        //    theIKSolver.poleTarget = g.transform;
        //    theIKSolver.enable = false;
        //    theIKSolver.endPointOfLastBone= lastBeed.transform;
        //    theIKSolver.iterations = NoOfIKIterations;
        //    IKSolver.Bone bone;
        //    Debug.Log("current bones: " + currentNumberOfBeeds+"\n");
        //    IKSolver.Bone[] bones = new IKSolver.Bone[currentNumberOfBeeds-1];
        //    for (int i=1;i<currentNumberOfBeeds; i++)
        //    {
        //        bone = new IKSolver.Bone();
        //        bone.bone= plant[currentNumberOfBeeds - i-1].transform;
        //        bones[i - 1] = bone;
        //        //Debug.Log("bone added: "+ bone.bone.name+ "\n");

        //    }
        //    Debug.Log("adding bones to IK solver\n");
        //    theIKSolver.bones = bones;
        //    IKInitiated = true;
        //    Debug.Log("initialting IKSolver completes\n");            
        //    theIKSolver.enable = true;
        //    theIKSolver.Initialize();

        //    Debug.Log("DONE initialting IKSolver\n");

        //}

        
        circumnutationOn = false;




    }

    void fallingStem()
    {
        growWithoutSupport = false;
        circumnutationOn = false;
        initialPhaseInterBeadDistance += 0.0005f;
        float primaryLeavesRaisingSpeed = 3;
        float rotateAmount = 0.015f * primaryLeavesRaisingSpeed;
        int numberOfRotatingBeads = 5;
        float targetFallenAngle = 45;
        Debug.Log("Stem is falling");
        if (!false)//circumnutation())
        {

            bool firstAfterRotating = true;
            if (Vector3.Angle(plant[currentNumberOfBeeds - 1].transform.forward, Vector3.down) > 1)
            {
                for (int i = startingBeed; i < currentNumberOfBeeds-1; i++)
                {
                    //plant[i].transform.rotation = Quaternion.Lerp(plant[i].transform.rotation, Quaternion.LookRotation(Vector3.up), Time.time * 0.0001f);
                    //Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward * initialPhaseInterBeadDistance;
                    //plant[i].transform.forward = Vector3.RotateTowards(plant[i].transform.forward, Vector3.down, rotateAmount * primaryLeavesRaisingSpeed, 0);
                    if (i < startingBeed + 5)
                    {
                        plant[i].transform.Rotate(0.1f, 0, 0);
                    }
                    Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward ;
                    Debug.DrawRay(plant[i].transform.position, plant[i].transform.forward, Color.cyan, 10);

                    plant[i + 1].transform.position = newPosition;
                    if (firstAfterRotating)
                    {
                        firstAfterRotating = false;
                        plant[i + 1].transform.forward = plant[i].transform.forward;
                    }
                    rotateAmount += 0.002f * primaryLeavesRaisingSpeed;
                }
                plant[currentNumberOfBeeds - 1].transform.forward = Vector3.RotateTowards(plant[startingBeed + numberOfRotatingBeads].transform.forward, collisionNormal, rotateAmount, 0);

            }
            /*
            else if (Vector3.Angle(plant[currentNumberOfBeeds - 1].transform.forward, Vector3.down) > targetFallenAngle)
            {
                for (int i = startingBeed; i < currentNumberOfBeeds - 2; i++)
                {
                    //plant[i].transform.rotation = Quaternion.Lerp(plant[i].transform.rotation, Quaternion.LookRotation(Vector3.up), Time.time * 0.0001f);
                    //Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward * initialPhaseInterBeadDistance;
                    plant[i].transform.forward = Vector3.RotateTowards(plant[i].transform.forward, Vector3.down, rotateAmount, 0);
                    Vector3 newPosition = plant[i].transform.position + plant[i].transform.forward;
                    plant[i + 1].transform.position = newPosition;
                    //plant[i + 1].transform.forward = plant[i].transform.forward;
                    rotateAmount += 0.002f * primaryLeavesRaisingSpeed;
                }
                plant[currentNumberOfBeeds - 1].transform.forward = Vector3.RotateTowards(plant[currentNumberOfBeeds - 1].transform.forward, Vector3.down, rotateAmount, 0);

            }
            */

        }

    }

    void supportLostGrowth()
    {
        Debug.Log("getting ready to circumnutate..");
        Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward, Color.gray,5*60);
        float gravitrophismDifference = Vector3.Angle(lastBeed.transform.forward, theUpward);
        Vector3 newPosition;
        //Debug.Log("gravitrophismDifference:" + gravitrophismDifference);

        

        if (gravitrophismDifference > reCircumnutateReadyAngle)
        {
            if (InterNodeGrowth >= beedDistance)
            {
                InterNodeGrowth = 0.1f;
                circumnutationOn = false;
                Vector3 newRotation = Vector3.RotateTowards(lastBeed.transform.forward, theUpward, supportLostGravitrophismAdjustment, 0);
                lastBeed.transform.rotation = Quaternion.LookRotation(newRotation);
                newGrowthDirection = lastBeed.transform.forward;
                previiousBead = lastBeed;
                newPosition = lastBeed.transform.position + Vector3.Normalize(newGrowthDirection) * InterNodeGrowth;
                DestroyImmediate(lastBeed.GetComponent<Collider>());
                DestroyImmediate(lastBeed.GetComponent<Beed>());
                lastBeed = Instantiate(beed, newPosition, Quaternion.LookRotation(newGrowthDirection));
                DestroyImmediate(lastBeed.GetComponent<Collider>());
                DestroyImmediate(lastBeed.GetComponent<Beed>());
                targetLastBeadPosition = newPosition;
                //Instantiate(beed, newPosition, Quaternion.LookRotation(newGrowthDirection));

                lastBeed.name = "Beed" + currentNumberOfBeeds;
                lastBeed.transform.parent = thisGameObject.transform;
                //lastBeed.transform.Rotate(getRelativeTilt(currentNumberOfBeeds, maxBeedRotation));
                plant[currentNumberOfBeeds] = lastBeed;
                //Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward * 2, Color.red, 5, false);
                currentNumberOfBeeds++;
                getReadyToCircumnutate = true;
            }
            else
            {
                InterNodeGrowth = InterNodeGrowth + (growthSpeed * 0.01f);
                newPosition = previiousBead.transform.position + Vector3.Normalize(newGrowthDirection) * InterNodeGrowth;
                lastBeed.transform.position = newPosition;
                getReadyToCircumnutate = true;
            }

            

        }
        else
        {
            lastBeed.transform.rotation = Quaternion.LookRotation(theUpward);
            startingBeed = currentNumberOfBeeds-2;
            //Debug.Log("ready to Circumnutate!!");
            circumnutationOn = true;
            supportLost = false;
            getReadyToCircumnutate = false;
            growWithoutSupport = true;
            supportFound = false;
            InterNodeGrowth = 2;
        }

        //Debug.Log("########## In Support Lost Growth #####################\n");

    }

    void SupportedGrowth(Vector3 newGrowthDirection)
    {
        Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward, Color.yellow, 5 * 60);
        Vector3 newPosition = lastBeed.transform.position + Vector3.Normalize(newGrowthDirection)* beedDistance;
        DestroyImmediate(lastBeed.GetComponent<Collider>());
        DestroyImmediate(lastBeed.GetComponent<Beed>());
        lastBeed = Instantiate(beed, newPosition, Quaternion.LookRotation(newGrowthDirection));
        lastBeed.transform.eulerAngles = new Vector3(-15,lastBeed.transform.eulerAngles.y,lastBeed.transform.eulerAngles.z);
        Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward, Color.red,5*60);
        targetLastBeadPosition = newPosition;
        //Instantiate(beed, newPosition, Quaternion.LookRotation(newGrowthDirection));

        lastBeed.name = "Beed" + currentNumberOfBeeds;
        lastBeed.transform.parent = thisGameObject.transform;
        //lastBeed.transform.Rotate(getRelativeTilt(currentNumberOfBeeds, maxBeedRotation));
        plant[currentNumberOfBeeds] = lastBeed;
        //Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward * 2, Color.red, 5, false);
        currentNumberOfBeeds++;
    }

    void SupportLessGrowth() {

        if (currentNumberOfBeeds - startingBeed > 25)
        {
            circumnutationOn = false;
            growWithoutSupport = false;
            isStemOverGrown = true;

        }

        Vector3 newPosition = lastBeed.transform.position + lastBeed.transform.forward * 0.01f;
        // removing the colider of previouse beed
        DestroyImmediate(lastBeed.GetComponent<Collider>());
        DestroyImmediate(lastBeed.GetComponent<Beed>());
        lastBeed = Instantiate(beed, newPosition, lastBeed.transform.rotation);
        lastBeed.name = "Beed" + currentNumberOfBeeds;
        lastBeed.transform.parent = thisGameObject.transform;
        lastBeed.transform.Rotate(getRelativeTilt(currentNumberOfBeeds, maxBeedRotation));
        plant[currentNumberOfBeeds] = lastBeed;
        currentNumberOfBeeds++;
    }

    bool circumnutation() {
       

        if (isStemOverGrown)
        {
            if (Vector3.Angle(lastBeed.transform.forward, collisionNormal)<10)
            {
                return false;
            }
        }

        plant[startingBeed].transform.Rotate(getRelativeTilt(1, circumnutationSpeed), Space.World);
        GameObject prevBeed = plant[startingBeed];
        InterNodeGrowth = InterNodeGrowth + (growthSpeed * 0.01f);
        GameObject currentBeed;
        Vector3 newPosition;
        int j = startingBeed + 1;
        for (int i= startingBeed+1; i < currentNumberOfBeeds-1; i++)
        {
            j = i;
            currentBeed = plant[i];
            newPosition = prevBeed.transform.position + prevBeed.transform.forward * beedDistance;
            currentBeed.transform.position = newPosition;
            currentBeed.transform.Rotate(getRelativeTilt(i, circumnutationSpeed), Space.World);
            prevBeed = currentBeed;
        }
        j++;
        currentBeed = plant[j];
        newPosition = prevBeed.transform.position + prevBeed.transform.forward * InterNodeGrowth;
        currentBeed.transform.position = newPosition;
        currentBeed.transform.Rotate(getRelativeTilt(j, circumnutationSpeed), Space.World);
        prevBeed = currentBeed;

        if (InterNodeGrowth > beedDistance)
        {
            InterNodeGrowth = 0;
            growWithoutSupport = true;
        }

        else
        {
            InterNodeGrowth = InterNodeGrowth + (growthSpeed * 0.01f);
            growWithoutSupport = false;
        }

        return true;

    }

    void twine()
    {
        Debug.Log("inTwineConsecutiveTimes:" + inTwineConsecutiveTimes+"\n");
        inTwineConsecutiveTimes += 1;
       
        InterNodeGrowth = 0;
        float twinningSpeed = 1*speed;
        int supportedStartingBeed = startingBeed - 1;
        float MaxUpwardAngle = Mathf.Max((Vector3.Angle(plant[supportedStartingBeed - 1].transform.forward, Vector3.up)-5f),0f);

        if (Vector3.Angle(plant[supportedStartingBeed].transform.forward, Vector3.up)> MaxUpwardAngle) 
        {
            plant[supportedStartingBeed].transform.forward = Vector3.RotateTowards(plant[supportedStartingBeed].transform.forward, Vector3.up, 0.01f * upwardTurningSpeed, 0);
        }

        //Debug.Log("supportedCircumnutation"+supportedStartingBeed+"\n");
        Vector3 newRotation = Vector3.RotateTowards(plant[supportedStartingBeed].transform.forward, collisionNormal * -1, 0.01f, 0);
        //Debug.DrawRay(plant[startingBeed].transform.position, plant[startingBeed].transform.forward * 2, Color.blue, 5*60, false);
        plant[supportedStartingBeed].transform.rotation = Quaternion.LookRotation(newRotation);

        

        //else {
        //    plant[supportedStartingBeed].transform.Rotate(new Vector3(0.01f, 0, 0));
        //}
        Debug.DrawRay(plant[startingBeed].transform.position, plant[startingBeed].transform.forward * 2, Color.blue);

        //Debug.DrawRay(plant[startingBeed].transform.position, collisionNormal * -5, Color.black, 60 * 5, false);
        //Debug.DrawRay(plant[startingBeed].transform.position, plant[startingBeed].transform.forward * 2, Color.red, 60*5, false);

        GameObject prevBeed = plant[supportedStartingBeed];

        for (int i = supportedStartingBeed + 1; i < currentNumberOfBeeds; i++)
        {
            //Debug.Log("inForLoop\n");
            GameObject currentBeed = plant[i];
            Vector3 newPosition = prevBeed.transform.position + prevBeed.transform.forward * beedDistance;
            currentBeed.transform.position = newPosition;
            MaxUpwardAngle = Mathf.Max((Vector3.Angle(prevBeed.transform.forward, Vector3.up) - 5f), 0f);


            if (Vector3.Angle(currentBeed.transform.forward, Vector3.up) > MaxUpwardAngle)
            {
                currentBeed.transform.forward = Vector3.RotateTowards(currentBeed.transform.forward, Vector3.up, 0.01f* upwardTurningSpeed, 0);
            }
            //else
            //{
            //    currentBeed.transform.Rotate(new Vector3(0.01f, 0, 0));
            //}s
            newRotation = Vector3.RotateTowards(currentBeed.transform.forward, collisionNormal * -1, Mathf.Deg2Rad * supportedCircumnutationSpeed, 0);
                
                
            currentBeed.transform.forward = newRotation;
                
                //currentBeed.transform.rotation = Quaternion.LookRotation(newRotation);

                

            /*
            // Grvitrophism adjustment
            newRotation = plant[supportedStartingBeed].transform.forward;
            newRotation = new Vector3(newRotation.x, Mathf.Deg2Rad * gravitrophismLimit, newRotation.z);
            newRotation = Vector3.RotateTowards(currentBeed.transform.forward, newRotation, Mathf.Deg2Rad * gravitrophismCorrectionValue, 0);
            currentBeed.transform.rotation = Quaternion.LookRotation(newRotation);
            */
            //Debug.DrawRay(currentBeed.transform.position, newRotation * 0.5f, Color.cyan, 60 * 5, false);
            prevBeed = currentBeed;
            
        }

        if (inTwineConsecutiveTimes > 1000)
        {
            Debug.Log("Support Lost by twining wait limit !!!");
            supportFound = false;
            circumnutationOn = false;
            supportLost = true;
            getReadyToCircumnutate = true;
            InterNodeGrowth = 1;
        }
    }

    Vector3 getRelativeTilt(int currentBeedNumber, Vector3 angle) {
        float percentage = getPercentagee(currentNumberOfBeeds);
        return angle * percentage;
    }

    float getPercentagee(int currentBeedNumber) {
        return currentBeedNumber / currentNumberOfBeeds;
    }

    public void onHitSupportStructure(Collision collision)
    {
        //Debug.Log("!!!!!!!! Hit on Support !!!!!!!!!");
        beedCollision = collision;
        inTwineConsecutiveTimes = 0;
        collidingObject = collision.collider;
        collidingBeed = lastBeed.GetComponent<Collider>();
        collisionNormal = collision.contacts[0].normal;
        float collitionangleRelativeToUpward= Vector3.Angle(collisionNormal, theUpward);
        cn[currentNumberOfBeeds] = collisionNormal;
        
        if (collitionangleRelativeToUpward < gravitrophismAbsoluteLimit)
        {
            Debug.Log("Support Lost !!!");
            supportFound = false;
            circumnutationOn = false;
            supportLost = true;
            getReadyToCircumnutate = true;
            InterNodeGrowth = 1;
        }
        else {
            supportFound = true;
            circumnutationOn = false;
            
            if (!hasGroped)
            {
                if (/*(Vector3.Angle(lastBeed.transform.forward, collisionNormal) < 90) && */(Vector3.Angle(lastBeed.transform.forward, Vector3.up) < 75))
                { 
                    Debug.Log("bypassed groaping \n");
                    startingBeed = currentNumberOfBeeds;
                    hasGroped = true;
                    growAroundSupport = true;
                }
                else
                {
                    Debug.Log("groaping started\n");
                    groping = true;
                }
            }
            else
            {
                groping = false;
                Debug.Log("twinable Hit\n");
                startingBeed = currentNumberOfBeeds;
                growAroundSupport = true;
            }

        }
    }

    float tapperFunction_0(int i, int currentBeedCount, int maxBeedCount, float maxGirth)
    {
        return maxGirth;
    }

    float tapperFunction_1(int i,int currentBeedCount, int maxBeedCount, float maxGirth)
    {

        return (maxGirth* (currentBeedCount-i) / maxBeedCount);
    }

    float tapperFunction_2(int i, int currentBeedCount, int maxBeedCount, float maxGirth)
    {
        float girth = Mathf.Pow(2, -1*i/10);
        return girth;
        //return (maxGirth * (currentBeedCount - i) / maxBeedCount);
    }

    float tapperFunction_3(int i, int currentBeedCount, int maxBeedCount, float maxGirth)
    {
        float minGirth = 0.00f;
        return ((maxGirth-minGirth) * (currentBeedCount - i) / maxBeedCount) +minGirth;
    }

    float tapperFunction_4(int i, int currentBeedCount, int maxBeedCount, float maxGirth)
    {
        return Mathf.Log(currentBeedCount-i, 2.71829f)*0.08f;
    }

    void GenerateReport() {
        if (/*(currentNumberOfBeeds % 50 == 0) &&*/ (lastEntry != currentNumberOfBeeds))
        {
            lastEntry = currentNumberOfBeeds;
            usedMemory = (int)(Profiler.GetTotalAllocatedMemoryLong()/1000000);
            report[reportIterator, 0] = currentNumberOfBeeds;
            report[reportIterator, 1] = currentNumberOfLeaves;
            report[reportIterator, 2] = fps;
            report[reportIterator, 3] = usedMemory;
            reportIterator++;
        }
        if (currentNumberOfBeeds>2000 && !end)
        {
            Debug.Log("starting write\n");
            end = true;
            using (StreamWriter sw = File.AppendText("C:\\Users\\Oshan Wickramaratne\\Desktop\\PlantSim\\Oshan2019\\Analytics\\Fps.csv"))
            {
                sw.WriteLine("currentNumberOfBeeds, currentNumberOfLeaves, FPS");
                for (int i = 0; i < reportIterator + 1; i++)
                {
                    sw.WriteLine(""+report[i, 0]+","+report[i, 1]+","+report[i, 2]);

                }

            }
            Debug.Log("Done Writing\n");
            GUI.Label(new Rect(0, 100, 100, 100), "DOneWriting");

        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 50, 100, 100),""+(int)(1.0f / Time.smoothDeltaTime));
        GUI.Label(new Rect(0, 100, 100, 100), ""+currentNumberOfBeeds);
        GUI.Label(new Rect(0, 150, 100, 100), "" + usedMemory);
        fps = (int)(1.0f / Time.smoothDeltaTime);
    }
}
