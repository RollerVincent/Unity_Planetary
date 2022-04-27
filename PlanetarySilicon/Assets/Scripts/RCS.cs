using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCS : MonoBehaviour
{


    public Vector3 center;
    public float affection;
    public float hoverAffection;

    public Booster mainBooster;
    public List<Booster> boosters;

    public float hoverHeight;
    public float direction_attenuation = 1f;

    public Vector3 targetV;
    public Vector3 targetP;

    public bool mainEngineEnabled = true;
    public Vector3 rocketUp;


    public bool reversed = false;
    


    // Start is called before the first frame update
    void Start()
    {

        hoverHeight = (transform.position-center).magnitude;
        targetP = transform.position;

    }

    void OnDrawGizmosSelected()
    {

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1f);
        Gizmos.DrawSphere(targetP, 0.2f);
        Gizmos.DrawLine(transform.position, transform.position+targetV.normalized * 3);
    }

    
    public void storeHeight()
    {
        hoverHeight = (targetP - center).magnitude;
    }
    // Update is called once per frame
    void Update()
    {
        
        
        
        

        

       
       if(mainEngineEnabled){

            center = GetComponent<AttractedObject>().closest_gravityObject.transform.position;

            Vector3 diff = transform.position-center;
            diff = diff.normalized;

            Vector3 d_global = Vector3.Cross(diff, -transform.forward);
            float angle = Vector3.Angle(diff, transform.forward)/180f;
            d_global *= angle*affection;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddTorque(d_global.x, d_global.y, d_global.z);


            targetP = (center + (targetP - center).normalized*hoverHeight);
            targetV = (targetP-transform.position);


            for (int i = 0; i < boosters.Count; ++i){
                Booster booster = boosters[i];
                if(!(booster.gameObject.name == mainBooster.gameObject.name && !mainEngineEnabled)){
                    Vector3 dv = targetV-rb.velocity;
                    Vector3 bv = -booster.transform.forward;

                    float d = Vector3.Dot(rb.velocity-targetV, booster.transform.forward);

                    //float d  =  bv.x*dv.x  +  bv.y*dv.y  +  bv.z*dv.z;
                    d = Mathf.Max(0, d);
                    booster.boost = d * hoverAffection;
                }
                else{
                    booster.boost/= 2f;
                }

            }


        }else{



            Rigidbody rb = GetComponent<Rigidbody>();


            Vector3 cross = Vector3.Cross(transform.forward, rb.velocity.normalized);
            

            if(reversed){
                cross = -cross;
            }

            if (!(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D)))
            {
                
                rb.AddTorque(cross.x*direction_attenuation, cross.y*direction_attenuation, cross.z*direction_attenuation);
                
            }

            
            Vector3 cross2 = Vector3.Cross(transform.forward, rocketUp);

            rb.AddTorque(cross2.x, cross2.y, cross2.z);




            for (int i = 0; i < boosters.Count; ++i){
                Booster booster = boosters[i];
                
                booster.boost/= 1.1f;
                

            }
        }


        //transform.RotateAround(transform.position, Vector3.Cross(diff, transform.up), angle*affection);
    }
}
