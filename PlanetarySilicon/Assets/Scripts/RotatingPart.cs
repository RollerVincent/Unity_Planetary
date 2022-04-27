using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPart : MonoBehaviour
{

    public float target;
    public float current;
    public float angleStep;

    public float angleStart;
    public float angleStop;

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
            diff = Mathf.Min(angleStep, diff*0.1f);
            current -= diff*Time.deltaTime*100;
        }else{
            diff = Mathf.Min(angleStep, diff*0.1f);
            current += diff*Time.deltaTime*100;
        }


        transform.localEulerAngles = new Vector3(0,0,angleStart + (angleStop - angleStart)*current);

        if (Input.GetKeyDown(triggerKey) && !GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive)
        {
            if(target==1){
                target = 0;
            }else{
                target = 1;
            }
        }
    }
}
