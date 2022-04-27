using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightDirector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = (GameObject.Find("PlayerObjects").GetComponent<PlayerObjects>().activePlayerObject.transform.position - transform.position).normalized;
    }
}
