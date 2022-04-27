using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ship : MonoBehaviour
{
    
    //public Vector3 gravity_center;
    public float gravity_force;

    public List<GameObject> gravity_objects;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float m = 10000000000f;

        for (int i = 0; i < gravity_objects.Count; ++i){
            GameObject gravob = gravity_objects[i];
            Vector3 diff = transform.position-gravob.transform.position;
            float dist = diff.magnitude;//-(Mathf.Pow(gravob.GetComponentInChildren<Planet>().radius, 1));
            //dist *= dist;
            dist += 1;

            GetComponent<Rigidbody>().AddForce(-diff.normalized * gravob.GetComponentInChildren<Planet>().gravity * gravity_force / dist);

            if(dist<m){
                m = dist;
                GetComponent<RCS>().center = gravob.transform.position;
            }

        }

        


       
    }

   
}
