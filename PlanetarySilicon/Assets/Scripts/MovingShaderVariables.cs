using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MovingShaderVariables : MonoBehaviour
{

    public bool updateVariables;
    public GameObject centerObject;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(updateVariables){

            Shader.SetGlobalVector("_PlanetCenter", centerObject.transform.position);
            Shader.SetGlobalVector("_LightCenter", transform.position);

            updateVariables=false;
        }
    }
}
