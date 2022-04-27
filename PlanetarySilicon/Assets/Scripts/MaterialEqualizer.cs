using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class MaterialEqualizer : MonoBehaviour
{

    public float ambient;
    public float ambientSaturation;
    public float ambientFaces;

    public float blendNormals;

    public float faceNoise;

    public Vector4 diffraction;
    public Vector4 heightDiffraction;

    public float absorbanceExponent;
    public float absorbanceOffset;

    public float depthMinDistance;
    public float depthMaxDistance;

    public bool apply;
    public bool equalizeAmbient;
    public bool equalizeNormalBlend;
    public bool equalizeFaceNoise;
    public bool equalizeDiffration;
    public bool equalizeAbsorbacne;
    public bool equalizeFog;
    
    




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(apply){
            Equalize();
            
        }
    }

    public void Equalize(){


        HashSet<Material> materials = new HashSet<Material>();

        Transform[] children = GetComponentsInChildren<Transform>();
        foreach(Transform child in children){
            MeshRenderer mr = child.gameObject.GetComponent<MeshRenderer>();
            if(mr != null){
                materials.Add(mr.sharedMaterial);
            }
        }

        Transform atmossphere = transform.Find("Atmossphere");
        if(atmossphere == null){
            atmossphere = transform.Find("PlaceHolder");
        }

        foreach(Material mat in materials){

            if(atmossphere != null){
                mat.SetFloat("_AtmossphereRadius", atmossphere.localScale.x/2f); 
            }
            

            if(equalizeAmbient){
                mat.SetFloat("_Ambient", ambient);
                mat.SetFloat("_AmbientSaturation", ambientSaturation);
                mat.SetFloat("_AmbientFaces", ambientFaces);
            }
            if(equalizeAbsorbacne){
                mat.SetFloat("_AbsorbanceExponent", absorbanceExponent);
                mat.SetFloat("_AbsorbanceOffset", absorbanceOffset);
            }
            if(equalizeDiffration){
                mat.SetVector("_Diffraction", diffraction);
                mat.SetVector("_HeightDiffraction", heightDiffraction);
            }
            if(equalizeFaceNoise){
                mat.SetFloat("_FaceNoise", faceNoise);
            }
            if(equalizeNormalBlend){
                mat.SetFloat("_BlendNormal", blendNormals);
            }


            if(equalizeFog){
                mat.SetFloat("_DepthMinDistance", depthMinDistance);
                mat.SetFloat("_DepthMaxDistance", depthMaxDistance);
            }




            
            
            
            
            
            
        }




    }
}
