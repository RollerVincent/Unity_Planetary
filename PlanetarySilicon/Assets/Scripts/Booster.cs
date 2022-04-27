using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public float throttle;
    public float power;
    public float exhaustControl;
    

    public float maxBoost;
    public float boost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDrawGizmosSelected()
    {

        Gizmos.color = new Color(throttle, 1, 0.0f, 1f);
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawLine(transform.position, transform.position+transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        
        GetComponentInParent<Rigidbody>().AddForceAtPosition(-transform.forward*throttle*power, transform.position);


        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>()){
            ps.startLifetime = throttle*exhaustControl;
        }

        
       
    }
}
