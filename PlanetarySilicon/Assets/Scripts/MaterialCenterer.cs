using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class MaterialCenterer : MonoBehaviour
{
    public List<Material> materials;
    public Vector3 lightOrigin;
    public bool center;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(center){

            foreach(Material material in materials){
                material.SetVector("_Center", transform.position);
                material.SetVector("_LightCenter", lightOrigin);
            }


            center = false;
        }
    }
}
