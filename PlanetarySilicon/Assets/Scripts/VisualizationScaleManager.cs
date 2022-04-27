using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VisualizationScaleManager : MonoBehaviour
{

    [Range(1,8)]
    public float visualizationScale = 1f;
    float currentVisualizationScale;

    public float visualizationDistance = 100000f;
    float currentVisualizationDistance;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentVisualizationScale != visualizationScale){
            Shader.SetGlobalFloat("_VisualizationScale", visualizationScale);
            currentVisualizationScale = visualizationScale;
        }

        if(currentVisualizationDistance != visualizationDistance){
            Shader.SetGlobalFloat("_VisualizationDistance", visualizationDistance);
            currentVisualizationDistance = visualizationDistance;
        }
        
    }
}
