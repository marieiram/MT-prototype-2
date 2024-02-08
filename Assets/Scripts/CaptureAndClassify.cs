using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureAndClassify : MonoBehaviour
{
    [Header("Detected Emotion")]
    [Tooltip("Material and Image for detection and detected emotion")]
    public GameObject screen;
    public Material mat; 
    public Texture2D textemotion;
    public GameObject classifier;
    public int emotion;
  //  public Texture2D DatasetImageTexture;
    Renderer m_Renderer;

    //dataset
    public Texture2D[] Happy;
    public Texture2D[] Sad;
    public Texture2D[] Anger;
    public Texture2D[] Disgust;
    public Texture2D[] Surprise;

    Texture2D ImageFromDateset;
    public GameObject screen1;
    public GameObject screen2;

    public float dist;
    private float Reward;

    public GameObject agent;
    //


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            
            Debug.Log("space key was pressed");
            Capture();
        }
    }

    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        textemotion = ScreenCapture.CaptureScreenshotAsTexture();
        m_Renderer = screen.GetComponent<Renderer>();
        m_Renderer.material.SetTexture("_MainTex", textemotion);
        classifier.GetComponent<Classifier>().imageTexture = textemotion;

        //Debug.Log(textemotion.graphicsFormat);


        emotion = classifier.GetComponent<Classifier>().emotion;
       // Debug.Log("emotion" + emotion);

        Texture2D mip1Data2 = textemotion;

        if (emotion == 0)
        {
            //ImageFromDateset = Anger[Random.Range(0, 23)];
            mip1Data2 = Anger[Random.Range(0, 23)];
        }

        if (emotion == 1)
        {
            ////ImageFromDateset = Disgust[Random.Range(0, 23)];
            mip1Data2 = Disgust[Random.Range(0, 23)];
        }

        if (emotion == 2)
        {
            // ImageFromDateset = Happy[Random.Range(0, 23)];
            mip1Data2 = Happy[Random.Range(0, 23)];
           // sentReward(0.01f);
          //  Debug.Log("rewardhappy");

        }

        if (emotion == 3)
        {
            //ImageFromDateset = Sad[Random.Range(0,23)];
            mip1Data2 = Sad[Random.Range(0, 23)];
        }

        if (emotion == 4)
        {
            //ImageFromDateset = Surprise[Random.Range(0, 23)];

            mip1Data2 = Surprise[Random.Range(0, 23)];

        }

        //Debug.Log(Surprise[Random.Range(0, 23)].format);

        //base6  if base7 //out
     //   StartCoroutine(compareMip(textemotion, ImageFromDateset));

        Texture2D newTex = new Texture2D(64, 64, TextureFormat.RGB565, false);
        newTex.SetPixels(textemotion.GetPixels());
        newTex.Apply();

        Color te1 = AverageColorFromTexture(newTex);
        Color te2 = AverageColorFromTexture(mip1Data2);
        
       // Sprite sprite1 = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0f,0f));
       // Sprite sprite2 = Sprite.Create(mip1Data2, new Rect(0, 0, mip1Data2.width, mip1Data2.height), new Vector2(1f,0f));

        screen1.gameObject.GetComponent<MeshRenderer>().material.mainTexture = newTex;
        screen2.gameObject.GetComponent<MeshRenderer>().material.mainTexture = mip1Data2;



        float H1, S1, V1;
        float H2, S2, V2;

        Color.RGBToHSV(te1, out H1, out S1, out V1);
        Color.RGBToHSV(te2, out H2, out S2, out V2);

        Vector3 c = new Vector3(H1, S1, V1);
        Vector3 b = new Vector3(H2, S2, V2);

           // StartCoroutine(compareDistance( c,b ));



            //var mip1Data = newTex.GetPixelData<Color32>(0);
            //var mip1Data2 = ImageFromDateset.GetPixelData<Color32>(0);

            //Debug.Log("ichi");
            //Debug.Log(mip1Data.Length);
            //Debug.Log("ni");
            //Debug.Log(mip1Data2.Length);

            //for (int i = 0; i < mip1Data.Length; i++)
            //{
            //Color cc = mip1Data[i];
            //Vector3 c = new Vector3(cc.r, cc.g, cc.b);
            //Color bb = mip1Data2[i];
            //Vector3 b = new Vector3(bb.r, bb.g, bb.b);
            //if(cc.r == 1.0f && cc.g == 1.0f && cc.b == 1.0f && bb.r == 1.0f && bb.g == 1.0f && bb.b == 1.0f)
            //{
            //mushi
            //}
            //else
            //{
            //    StartCoroutine(compareDistance(c, b));
            //  }

            //}




            // do something with texture
            // cleanup
            //Object.Destroy(texture);
        }

    public IEnumerator compareMip(Texture2D te, Texture2D Ima)
    {
        


       // for (int i = 0; i < mip1Data.Length; i++)
        //{
          //  Color cc = mip1Data[i];
           // Vector3 c = new Vector3(cc.r, cc.g, cc.b);
           // Color bb = mip1Data2[i];
           // Vector3 b = new Vector3(bb.r, bb.g, bb.b);
            //StartCoroutine(compareDistance(c, b));
        //}

       yield return true;
    }


    public void Capture()
    {
        //capture screen
        //Set screen as texture
        //when running inference comment record frame
    // StartCoroutine(RecordFrame());

        //uncomment on inference
     emotion = classifier.GetComponent<Classifier>().emotion;

        //Debug.Log(emotion);

    }
    public IEnumerator compareDistance(Vector3 c, Vector3 b)
    {
        dist = Vector3.Distance(c, b);
        //  Debug.Log(dist);

       

        if (dist < 0.09f)
        {
            float totalreward = 1.00f;
            totalreward -= dist;
            sentReward(totalreward);
        }
       
        else
        {
            Reward = 0f;
        }
        //Debug.Log("dist" + dist);
        // Debug.Log("reward"+Reward);
       
        yield return true;
    }

    public void sentReward(float reward)
    {

        //base6 Rittai if base7 Rittai1
        Rittai1 collectAgents = agent.GetComponent<Rittai1>();

        collectAgents.AddReward(reward);
        //collectAgents.reward = 0.5f;
        Debug.Log("reward");

    }

    Color32 AverageColorFromTexture(Texture2D tex)
    {

        Color32[] texColors = tex.GetPixels32();

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < total; i++)
        {

            r += texColors[i].r;

            g += texColors[i].g;

            b += texColors[i].b;

        }

        return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);

    }


}
