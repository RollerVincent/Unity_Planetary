using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmossphericFriction : MonoBehaviour
{

    
    public float spinAttenuation;
    public float intensity;
    public float sampleSize;
    public int numSamples;
    public float sampleOffset;
    public bool debug;


    void OnDrawGizmosSelected()
    {
            

             /*   Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1f);
                Gizmos.DrawSphere(transform.position, transform.localScale.x/2-100f/spinAttenuation); */
            

            
           
       
            
             
        
        


       
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LateUpdate(){
        setGlow();
        ApplyForces();
    }

    void setGlow(){


        foreach(HeatMesh heatMesh in GameObject.FindObjectsOfType<HeatMesh>()){
            Rigidbody rigidBody = heatMesh.GetComponentInParent<Rigidbody>();
            Vector3 diff = (rigidBody.transform.position - transform.position);
            float dist = diff.magnitude;

            if(dist < transform.localScale.x/2f){

                // heatmesh inside atmossphere

                heatMesh.mask = 1;
                heatMesh.velocity = rigidBody.velocity;
                heatMesh.UpdateVelocity();


            }else{
                heatMesh.mask = 0;
                heatMesh.UpdateVelocity();
            }
        }



    }

    void ApplyForces(){
        foreach(HeatMesh heatMesh in GameObject.FindObjectsOfType<HeatMesh>()){
            Rigidbody rigidBody = heatMesh.GetComponentInParent<Rigidbody>();
            Vector3 diff = (rigidBody.transform.position - transform.position);
            float dist = diff.magnitude;

            if(dist < transform.localScale.x/2f){

                // heatmesh inside atmossphere

                Vector3 velocityDir = rigidBody.velocity.normalized;
                Vector3 right = Vector3.Cross(velocityDir, new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized);
                Vector3 up = Vector3.Cross(right, velocityDir);

                int hitcount = 0;

                for(int i=0;i<numSamples;i++){


                    Vector3 samplePos = rigidBody.transform.position + velocityDir*sampleOffset + right*sampleSize*Random.Range(-1.0f, 1.0f) + up*sampleSize*Random.Range(-1.0f, 1.0f);

                    Vector3 dir = -velocityDir;
                    RaycastHit hit;

                    Vector3 finalForcePos = Vector3.zero;
                    float finalForceAmount = 0;

                    if(debug){
                            Debug.DrawLine(samplePos, samplePos+dir*sampleOffset*2, new Color(1,1,0,1), 0.01f);
                    }
                            
                            

                    if (Physics.Raycast(samplePos, dir, out hit, sampleOffset*2))
                    {
                        if(hit.transform.gameObject == rigidBody.gameObject){
                            float amount = Mathf.Max(0f, Vector3.Dot(hit.normal, -dir));
                            finalForcePos = hit.point;
                            finalForceAmount = amount;
                            Vector3 finalForce = -hit.normal * intensity * rigidBody.velocity.magnitude;//-hit.normal * finalForceAmount * intensity * rigidBody.velocity.magnitude;
                            rigidBody.AddForceAtPosition(finalForce/numSamples, finalForcePos);
                            hitcount += 1;


                            if(debug || true){
                                Debug.DrawLine(hit.point, hit.point + hit.normal*finalForceAmount*1f, new Color(1,0,1,1), 0.03f);
                            }
                        }
                                

                                

                    }

                }



            }
        }
    }

    // Update is called once per frame
  /*  void LateUpdate()
    {
        Spin spin = transform.parent.GetComponent<Spin>();
        if(spin != null){

            float speed = transform.parent.parent.GetComponent<SystemManager>().speed;

            foreach(Rigidbody rigidBody in GameObject.Find("PlayerObjects").GetComponentsInChildren<Rigidbody>()){
                if(rigidBody.gameObject.activeSelf){
                    Vector3 diff = (rigidBody.transform.position - transform.position);
                    float dist = diff.magnitude;



                    if(dist < transform.localScale.x/2f){
                        Vector3 rotated = Quaternion.AngleAxis(speed*spin.spin, spin.axis) * diff;
                        Vector3 targetVelocity = (rotated-diff)*1;


                        //float d = Mathf.Min(1f, (transform.localScale.x/2 - dist)*spinAttenuation*0.01f);
                        //rigidBody.transform.position += targetVelocity*d;


                        
                        Vector3 velocityDir = rigidBody.velocity.normalized;
                        Vector3 right = Vector3.Cross(velocityDir, new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized);
                        Vector3 up = Vector3.Cross(right, velocityDir);

                        int hitcount = 0;

                        for(int i=0;i<numSamples;i++){


                            Vector3 samplePos = rigidBody.transform.position + velocityDir*sampleOffset + right*sampleSize*Random.Range(-1.0f, 1.0f) + up*sampleSize*Random.Range(-1.0f, 1.0f);

                            Vector3 dir = -velocityDir;
                            RaycastHit hit;

                            Vector3 finalForcePos = Vector3.zero;
                            float finalForceAmount = 0;

                            if(debug){
                                Debug.DrawLine(samplePos, samplePos+dir*sampleOffset*2, new Color(1,1,0,1), 0.01f);
                            }
                            
                            

                            if (Physics.Raycast(samplePos, dir, out hit, sampleOffset*2))
                            {
                                if(hit.transform.gameObject == rigidBody.gameObject){
                                    float amount = Mathf.Max(0f, Vector3.Dot(hit.normal, -dir));
                                    finalForcePos = hit.point;
                                    finalForceAmount = amount;
                                    Vector3 finalForce = -hit.normal * intensity * rigidBody.velocity.magnitude;//-hit.normal * finalForceAmount * intensity * rigidBody.velocity.magnitude;
                                    rigidBody.AddForceAtPosition(finalForce/numSamples, finalForcePos);
                                    hitcount += 1;


                                    if(debug || true){
                                        Debug.DrawLine(hit.point, hit.point + hit.normal*finalForceAmount*1f, new Color(1,0,1,1), 0.03f);
                                    }
                                }
                                

                                

                            }

                        }

                        Debug.Log(hitcount);

                       


                    }
                }
            }


        }
    }*/

    void sampleFriction(){



    }
}
