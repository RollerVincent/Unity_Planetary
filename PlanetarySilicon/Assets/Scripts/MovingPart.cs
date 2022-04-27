using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPart : Actable
{
    public float target;
    public float current;
    public float offsetStep;

    public float offsetStart;
    public float offsetStop;

    public string triggerKey;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float diff = target-current;
        if(diff<0){
            diff *= -1;
            diff = Mathf.Min(offsetStep, diff*0.1f);
            current -= diff*Time.deltaTime*100;
        }else{
            diff = Mathf.Min(offsetStep, diff*0.1f);
            current += diff*Time.deltaTime*100;
        }


        transform.localPosition = new Vector3(0,0,(offsetStart + (offsetStop - offsetStart)*current));

    }

    public override void Act(){
        if(target==1){
            target = 0;
        }else{
            target = 1;
        }
    }
}
