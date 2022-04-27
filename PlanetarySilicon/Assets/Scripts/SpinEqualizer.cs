using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinEqualizer : MonoBehaviour
{
    public bool equalize = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(equalize && GetComponent<AttractedObject>().currentAtmosphere != null){
            Equalize(GetComponent<AttractedObject>().closest_gravityObject.GetComponent<Spin>());
        }
    }

    void Equalize(Spin spin){


        transform.position = spin.applyToPoint(transform.position);
    }
}
