using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beed : MonoBehaviour {

    public float acceptedCollitionAngle = 30;
    public GameObject thePlant;
    Rigidbody rb;

    // Use this for initialization
    void Start () {
        
        Debug.Log("a new beed\n");
        rb = GetComponent<Rigidbody>();
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {

  
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("!!!!!!!! Hit on Support !!!!!!!!!");

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        float currentCollitionAngle = Vector3.Angle(this.gameObject.transform.forward, collision.contacts[0].normal*-1);

        //Debug.Log("colided: " + currentCollitionAngle);

        if (currentCollitionAngle > acceptedCollitionAngle)
        {
            //Debug.Break();
            rb.isKinematic = true;
            thePlant.GetComponent<Bine>().onHitSupportStructure(collision);


        }
        else {
            //Debug.Log("not Accepted collition: " + currentCollitionAngle+"<"+acceptedCollitionAngle);
            rb.isKinematic = true;
        }
            
    }
}
