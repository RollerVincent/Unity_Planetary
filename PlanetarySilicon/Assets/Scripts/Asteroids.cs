using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Asteroids : MonoBehaviour
{

    public Material material;
    public Material planetMaterial;
    public Material resourceVisMaterial;
    
    

    public float Radius;

    public Texture2D Add1;
    public float Detail1;
    public float Impact1;

    public Texture2D Mult2;
    public float Detail2;
    public float Impact2;

    public Texture2D Add3;
    public float Detail3;
    public float Impact3;

    public Texture2D Add4;
    public float Detail4;
    public float Impact4;

    public float Seed = 0;

    public SystemManager systemManager;

    public bool createEmpty = false;

    public bool createInstance = false;

    public bool createTestSet = false;

    public int numAsteroids = 30;
    public float asteroidRange1 = 300;
    public float asteroidRange2 = 1000;
    public bool generateAsteroids;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        

        


        material.SetFloat("_Radius", Radius);
        material.SetFloat("_Seed", Seed);

        material.SetFloat("_NoiseDetail1", Detail1);
        material.SetFloat("_NoiseDetail2", Detail2);
        material.SetFloat("_NoiseDetail3", Detail3);
        material.SetFloat("_NoiseDetail4", Detail4);

        material.SetFloat("_NoiseImpact1", Impact1);
        material.SetFloat("_NoiseImpact2", Impact2);
        material.SetFloat("_NoiseImpact3", Impact3);
        material.SetFloat("_NoiseImpact4", Impact4);

        material.SetTexture("_MainTex1", Add1);
        material.SetTexture("_MainTex2", Mult2);
        material.SetTexture("_MainTex3", Add3);
        material.SetTexture("_MainTex4", Add4);

        if(createEmpty){
            AdobtMesh();
            createEmpty = false;
        }

        if(createInstance){
            CreateInstance(Vector3.zero, null);
            createInstance = false;
        }

        if(createTestSet){
            for(int i=0; i<3; i++){
                for(int j=0; j<3; j++){
                    Vector3 pos = new Vector3(i*Radius*8, j*Radius*8, j*Radius*8);
                    CreateInstance(pos, null);
                }
            }
            createTestSet = false;
        }
        if(generateAsteroids){
            GenerateAsteroids();

            generateAsteroids=false;
        }
        
    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    float setSeed(Vector3 position){
        Vector3 universePos = position;
        if(systemManager != null){
            universePos += systemManager.UniversePosition();
        }

        float tmpSeed = Seed;
        Seed = universePos.x+universePos.y+universePos.z;
        Random.seed = (int)(Seed); 
        Seed /= 100000f;

        

        return tmpSeed;
    }

    public GameObject CreateInstance(Vector3 position, SystemManager parentSystem){
        GetComponent<PlaneSphere>().createSphere();


        float tmpSeed = setSeed(position);

        float tmpRadius = Radius;

        float radiusVariance = 0.2f;

        float radiusOffset = Random.Range(-radiusVariance, radiusVariance);

        Radius = Radius + Radius * radiusOffset;


        // set seed to system star index 
        GameObject newObj = AdobtMesh();




        newObj.name = "Asteroid Instance";

        GameObject copyObj = transform.GetChild(0).gameObject;
        
        float atmosOffset = 0;

        int c = copyObj.transform.childCount;
        for(int i=0; i< c; i++){
            GameObject child = copyObj.transform.GetChild(i).gameObject;
            if(child.name != "Planar Surface" && child.name != "Marching Surface" && child.name != "Resource Manager"){
                if(!child.name.Contains("Decoration")){
                    GameObject newChild = GameObject.Instantiate(child);
                    newChild.transform.parent = newObj.transform;

                    if((child.name.Contains("Atmossphere") || child.name.Contains("PlaceHolder")) && atmosOffset == 0){
                        atmosOffset = newChild.transform.localScale.x/2f - tmpRadius;
                        atmosOffset += Radius;
                        atmosOffset *= 2;
                    }

                    if((child.name.Contains("Atmossphere") || child.name.Contains("PlaceHolder"))){
                        newChild.transform.localScale = new Vector3(atmosOffset, atmosOffset, atmosOffset);
                    }

                }
            }
        }



        

        newObj.transform.position = position;

        tmpSeed = setSeed(position);

        foreach(Decorator decorator in copyObj.GetComponents<Decorator>()){
            Decorator copyDecorator = CopyComponent<Decorator>(decorator, newObj);
            float p = Random.Range(0.0f, 1.0f);
            if(p > copyDecorator.existenceProbability){
                DestroyImmediate(copyDecorator);
            }
            copyDecorator.maxHeight += (Radius - tmpRadius);
            copyDecorator.minHeight += (Radius - tmpRadius);
        }
       /* foreach(Decorator decorator in newObj.GetComponents<Decorator>()){
            decorator.Place();
        }*/


        foreach(GrassDecorator decorator in copyObj.GetComponents<GrassDecorator>()){
            GrassDecorator copyDecorator = CopyComponent<GrassDecorator>(decorator, newObj);
        }
        foreach(MaterialEqualizer equalizer in copyObj.GetComponents<MaterialEqualizer>()){
            MaterialEqualizer copy = CopyComponent<MaterialEqualizer>(equalizer, newObj);
        }
        /*foreach(GrassDecorator decorator in newObj.GetComponents<GrassDecorator>()){
            decorator.Place();
        }*/


        newObj.GetComponent<RecenterMesh>().RecenterAll();
        newObj.AddComponent<GravityObject>().mass = 5000;


        if(parentSystem != null){
            newObj.transform.parent = parentSystem.transform;
        }else{
            newObj.transform.parent = transform;
        }
        



        




        Radius = tmpRadius;
        Seed = tmpSeed;


        
       

        return newObj;
    }

    GameObject AdobtMesh(){

        GameObject newObj = new GameObject("New Planet");
        newObj.transform.position = transform.position;

        GameObject planarObj = new GameObject("Planar Surface");
        planarObj.transform.position = transform.position;
        planarObj.transform.parent = newObj.transform;

        GameObject surfObj = new GameObject("Marching Surface");
        surfObj.transform.position = transform.position;
        surfObj.transform.parent = newObj.transform;
        surfObj.AddComponent<UvMapper>();
        surfObj.AddComponent<ResourceColoring>();

        MarchingSurfaceManager msm = newObj.AddComponent<MarchingSurfaceManager>();
        msm.material = planetMaterial;
        newObj.AddComponent<Marching>();
        newObj.AddComponent<CaveCreator>();
        newObj.AddComponent<RecenterMesh>();

        GameObject resObj = new GameObject("Resource Manager");
        resObj.transform.position = transform.position;
        resObj.transform.parent = newObj.transform;
        ResourceManager rm = resObj.AddComponent<ResourceManager>();
        rm.visMaterial = resourceVisMaterial;
        resObj.AddComponent<ResourceDistribution>();

        List<Transform> t = new List<Transform>();
        foreach (Transform child in transform) {
            if(child.gameObject.tag != "Planet"){
                t.Add(child);
            }
            
        }

        foreach (Transform child in t) {
            GameObject obj = child.gameObject;
            obj.transform.parent = planarObj.transform;
            

            MeshFilter mf = obj.GetComponent<MeshFilter>();
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            Mesh mesh = mf.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;

            for(int i=0; i<vertices.Length; i++){
                
                vertices[i] = vertices[i].normalized;

				float add1 = sampleTexture3D(vertices[i], Add1, Detail1);
				float mult2 = sampleTexture3D(vertices[i], Mult2, Detail2);
				float add3 = sampleTexture3D(vertices[i], Add3, Detail3);
				float add4 = sampleTexture3D(vertices[i], Add4, Detail4);

                vertices[i] = vertices[i] * (Radius + (add1*Impact1 * Mathf.Pow(mult2,Impact2))+add3*Impact3+add4*Impact4);

            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mf.sharedMesh = mesh;
            mr.sharedMaterial = planetMaterial;

            obj.AddComponent<MeshCollider>();
        }


        UvMapper uvMapper = planarObj.AddComponent<UvMapper>();
        uvMapper.offsetRadius = Radius;
        uvMapper.Recolor();
        surfObj.GetComponent<UvMapper>().offsetRadius = Radius;
        

        newObj.transform.parent = transform;
        newObj.gameObject.tag = "Planet";

       
        return newObj;

        
    }

    float fromHp(Color c){
        return (c.r+c.g+c.b)/(3);

    }

    float sampleTexture3D(Vector3 normal, Texture2D tex, float detail) {
				
                normal = ((normal + new Vector3(1.25f+Seed,3.50f+Seed,5.75f+Seed))/detail) * Radius*0.01f;

                float dx = normal.x%1.0f;
                float ax = (normal.x - dx)%2;
                ax = (ax-0.5f)*2;
                if(ax<0){
                    ax = dx;
                 
                }else{
                    ax = 1-dx;
                }

                float dy = normal.y%1.0f;
                float ay = (normal.y - dy)%2;
                ay = (ay-0.5f)*2;
                if(ay<0){
                    ay = dy;
                 
                }else{
                    ay = 1-dy;
                }

                float dz = normal.z%1.0f;
                float az = (normal.z - dz)%2;
                az = (az-0.5f)*2;
                if(az<0){
                    az = dz;
                 
                }else{
                    az = 1-dz;
                }


                normal = new Vector3(ax,ay,az);






				Vector3 position = normal;//*Radius*0.01f;// + new Vector3(1.25f,3.50f,5.70f);

				float c1 = fromHp(tex.GetPixelBilinear(position.x, position.y));
				float c2 = fromHp(tex.GetPixelBilinear(position.x, position.z));
				float c3 = fromHp(tex.GetPixelBilinear(position.z, position.y));

				return (c1+c2+c3)/3;
	}

    public void GenerateAsteroids(){
        float tmpSeed = setSeed(transform.position);
        for(int i=0; i<numAsteroids; i++){

            Vector3 dir = new Vector3(Random.Range(-1.0f,1.0f), Random.Range(-1.0f,1.0f), Random.Range(-1.0f,1.0f)).normalized;
            Vector3 pos = transform.position + dir*Random.Range(asteroidRange1/2f,asteroidRange2/2f);
            GameObject newInstance = CreateInstance(pos, null);
            newInstance.transform.parent = transform;
        }

        Seed = tmpSeed;
    }
}
