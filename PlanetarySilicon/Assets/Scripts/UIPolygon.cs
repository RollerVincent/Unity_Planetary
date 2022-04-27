/// Credit CiaccoDavide
/// Sourced from - http://ciaccodavi.de/unity/uipolygon

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
    public class UIPolygon : MaskableGraphic
    {
        [SerializeField]
        Texture m_Texture;
        public bool fill = true;
        public float thickness = 5;
        [Range(3, 360)]
        public int sides = 3;
        [Range(0, 360)]
        public float rotation = 0;
        [Range(0, 1)]
        public float[] VerticesDistances = new float[3];
        public bool setLabels;
        [Range(-1, 1)]
        public float focus;
        [Range(0, 1)]
        public float offset;
        [Range(0, 1)]
        public float symbolColoring;
        [Range(0, 1)]
        public float ring;
        [Range(-1, 1)]
        public float spike;
        public Color BottomColor;
        public Color TopColor;
        public GameObject parameterWheelSet;
        public bool setParameterWheelSet;
        public bool updateParameterWheel;



        private float size = 0;

        public override Texture mainTexture
        {
            get
            {
                return m_Texture == null ? s_WhiteTexture : m_Texture;
            }
        }
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value) return;
                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }
        public void DrawPolygon(int _sides)
        {
            sides = _sides;
            VerticesDistances = new float[_sides + 1];
            for (int i = 0; i < _sides; i++) VerticesDistances[i] = 1; ;
            rotation = 0;
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = 0;
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = _rotation;
        }
        void Update()
        {
            size = rectTransform.rect.width;
            if (rectTransform.rect.width > rectTransform.rect.height)
                size = rectTransform.rect.height;
            else
                size = rectTransform.rect.width;
            thickness = (float)Mathf.Clamp(thickness, 0, size / 2);

            UpdateGeometry();

            if(setLabels){
                SetLabels();
            }

            if(setParameterWheelSet){
                SetParameterWheelSet();
                setParameterWheelSet=false;
            }

            if(updateParameterWheel){
                UpdateParameterWheel();
            }

            Events();
        }

        void Events(){

            if (Input.GetKeyDown(KeyCode.N))
            {     
            

                rectTransform.parent.GetComponent<RectTransform>().pivot = Vector2.one/2f;
                rectTransform.parent.GetComponent<RectTransform>().anchorMax = Vector2.one/2f;
                rectTransform.parent.GetComponent<RectTransform>().anchorMin = Vector2.one/2f;
                rectTransform.parent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                
                rectTransform.parent.GetComponent<RectTransform>().localScale = Vector3.one * 10;    

                            
            }

            if (Input.GetKeyUp(KeyCode.N))
            {     
            

                rectTransform.parent.GetComponent<RectTransform>().pivot = new Vector2(1,0);
                rectTransform.parent.GetComponent<RectTransform>().anchorMax = new Vector2(1,0);
                rectTransform.parent.GetComponent<RectTransform>().anchorMin = new Vector2(1,0);
                rectTransform.parent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                rectTransform.parent.GetComponent<RectTransform>().localScale = Vector3.one * 1;    
            }

        }


        void UpdateParameterWheel(){

            Parameter[] parameters = parameterWheelSet.GetComponentsInChildren<Parameter>();
            int count = parameters.Length;
            for(int i=0;i<count;i++){
                VerticesDistances[i] = parameters[i].GetOffset() * 0.5f;
            }


        }

        void SetParameterWheelSet(){

            Parameter[] parameters = parameterWheelSet.GetComponentsInChildren<Parameter>();
            int count = parameters.Length;
            sides = count;

            Transform[] children = GetComponentsInChildren<Transform>();

            for(int i=0;i<count;i++){
                GameObject newObj = Instantiate(children[1].gameObject);
                newObj.name = "Label " + i;
                newObj.transform.parent = transform;
                Image image = newObj.transform.GetChild(0).GetComponent<Image>();
                image.sprite = parameters[i].icon;
            }

            List<GameObject> tmp = new List<GameObject>();
            for(int i=1;i<children.Length;i++){
                if(children[i].gameObject.name.Contains("Label")){
                    tmp.Add(children[i].gameObject);
                }
            }
            foreach(GameObject g in tmp){
                DestroyImmediate(g);
            }

                                
            BottomColor = parameterWheelSet.GetComponent<ParameterSet>().WheelBottomColor;
            TopColor = parameterWheelSet.GetComponent<ParameterSet>().WheelTopColor;

            



        }

        void SetLabels(){
            for(int i=0; i<sides; i++){
                RectTransform rt = transform.GetChild(i).GetComponent<RectTransform>();

                

                //rt.anchoredPosition3D = Quaternion.AngleAxis(360f/sides*i, Vector3.forward) * -Vector3.right * (offsetTransform(VerticesDistances[i], offset)*50-0*0.65f);
                rt.anchoredPosition3D = Quaternion.AngleAxis(360f/sides*i, Vector3.forward) * -Vector3.right * (ring) * 50;
                rt.transform.GetChild(0).GetComponent<Image>().color = Color.Lerp(correctBrightness(sampleColor(VerticesDistances[i])), color, symbolColoring);
            }
        }






        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }

        Color correctBrightness(Color c){
            float m = Mathf.Max(Mathf.Max(c.r,c.g),c.b);
			m = 1-m;
			return new Color(c.r+m, c.g+m, c.b+m, 1);
        }

        Color sampleColor(float dist){
            dist = (dist-0.5f)*2;
            if(dist<0){
                dist = -dist;
            }
            return Color.Lerp(BottomColor, TopColor, 1-dist);
        }

        float offsetTransform(float x, float offset){
            return x*(1-offset)+offset;
        }

        float invOffsetTransform(float x, float offset){
            return (x-offset) / (1-offset);
        }


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            
            Vector2 pos;
            Vector2 pos1;

            int finalsides = sides*2;
            
            float degrees = 360f / finalsides;
            int vertices = sides + 1;
            if (VerticesDistances.Length != vertices)
            {
                VerticesDistances = new float[vertices];
                for (int i = 0; i < vertices - 1; i++) VerticesDistances[i] = 1;
            }
            // last vertex is also the first!
            VerticesDistances[vertices - 1] = VerticesDistances[0];


            vh.AddVert(Vector2.zero, color, new Vector2(0.0f,0));

            

            for (int i = 0; i < vertices; i++)
            {


                float outer = -rectTransform.pivot.x * size * ring;
                
                float rad = Mathf.Deg2Rad * (i*2 * degrees + rotation);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);


                pos = new Vector2(outer * c, outer * s);

                

                




                float other = 0;
                if(i==vertices-1){
                    other = VerticesDistances[0];
                }else{
                    other = VerticesDistances[i+1];
                }


                float inner = -rectTransform.pivot.x * size * ring;
                rad = Mathf.Deg2Rad * ((i*2+1) * degrees + rotation);
                c = Mathf.Cos(rad);
                s = Mathf.Sin(rad);

                pos1 = new Vector2(inner * c, inner * s);

                //vh.AddVert(pos, sampleColor(VerticesDistances[i]), new Vector2(1, 0.5f));
                //vh.AddVert(pos1, sampleColor((VerticesDistances[i] + (other-VerticesDistances[i])*0.5f)), new Vector2(1, 0.5f));

                vh.AddVert(pos, correctBrightness(sampleColor(VerticesDistances[i])), new Vector2(0, 0.5f));

                Color c1 = sampleColor(VerticesDistances[i]);
                Color c2 = sampleColor(other);



                vh.AddVert(pos1, Color.Lerp(c1,c2,0.5f), new Vector2(0, 0.5f));
                
            }

            for (int i = 0; i < finalsides-1; i++)
            {
                vh.AddTriangle(0, 1+i, 2+i);
            }
            vh.AddTriangle(0, finalsides, 1);


            for (int i = 0; i < vertices; i++)
            {
                float outer = -rectTransform.pivot.x * size * offsetTransform(VerticesDistances[i], offset);
                
                
                float rad = Mathf.Deg2Rad * (i*2 * degrees + rotation);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);
                pos = new Vector2(outer * c, outer * s);

                float other = 0;
                if(i==vertices-1){
                    other = VerticesDistances[0];
                }else{
                    other = VerticesDistances[i+1];
                }


                float inner = -rectTransform.pivot.x * size * (offsetTransform((1-spike)*((VerticesDistances[i] + other)/2f), offset));

                rad = Mathf.Deg2Rad * ((i*2+1) * degrees + rotation);
                c = Mathf.Cos(rad);
                s = Mathf.Sin(rad);
                pos1 = new Vector2(inner * c, inner * s);

                vh.AddVert(pos, correctBrightness(sampleColor(VerticesDistances[i])), new Vector2(1, 1));

                Color c1 = sampleColor(VerticesDistances[i]);
                Color c2 = sampleColor(other);

                vh.AddVert(pos1, Color.Lerp(c1,c2,0.5f), new Vector2(1, 1));
                
            }

            for (int i = 0; i < finalsides-1; i++)
            {
                vh.AddTriangle(1+i, 1+finalsides+i+2, 1+finalsides+1+i+2);
                vh.AddTriangle(1+i, 2+i, 1+finalsides+1+i+2);
            }
            vh.AddTriangle(finalsides, finalsides+finalsides+2, finalsides+1+2);
            vh.AddTriangle(finalsides, 1, finalsides+1+2);


        }
    }
}