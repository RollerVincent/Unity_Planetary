using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractedChildObject : MonoBehaviour
{

    public AttractedObject parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddForce(parent.currentForce);
    }
}
