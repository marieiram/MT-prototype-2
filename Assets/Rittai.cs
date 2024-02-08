using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Clayxels;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Rittai : Agent
{
    public ClayContainer oya;
    public ClayObject[] objects;
    public Transform[] initialpositions;
    public Vector3[] towardspositions;
    public Vector3[] towardsscale;
    public Vector3[] towardsrotation;

    public float[] blends;
    


    public ClayObject[] initialcolors;
    public Color[] towardscolor;
    public int[] Emotions;
    public int currentEmotion;
    public int remo;
    private Vector3 range = new Vector3(2.0f, 2.0f, 2.0f);
    public CaptureAndClassify classifier;
    public GameObject classi;


    public Color anger = new Vector4();
    public Color disgust = new Vector4();
    public Color happy = new Vector4();
    public Color sad = new Vector4();
    public Color surprise = new Vector4();
    public Color[] emocolors; 


    public Color maincolor = new Vector4();

    ///to restart
    public Transform[] saveinitialpositions;
    public ClayObject[] saveinitialcolors;

    public float multiplier = 2.0f;
    public float scale = 2.5f;

    int frames = 0;
    int maxframes = 1000;

    // Start is called before the first frame update
    void Start()
    {
        anger = new Vector4(0.9882352941176471f, 0.011764705882352941f, 0.011764705882352941f, 1.0f);
        disgust = new Vector4(0.5215686274509804f, 0.7176470588235294f, 0.13333333333333333f, 1.0f);
        happy = new Vector4(0.9137254901960784f, 0.7529411764705882f, 0.3686274509803922f, 1.0f);
        sad = new Vector4(0.4588235294117647f, 0.6509803921568628f, 0.8823529411764706f, 1.0f);
        surprise = new Vector4(0.6235294117647059f, 0.3568627450980392f, 0.8117647058823529f, 1.0f);
        
        emocolors[0] = anger;
        emocolors[1] = disgust;
        emocolors[2] = happy;
        emocolors[3] = sad;
        emocolors[4] = surprise;

        classifier = classi.GetComponent<CaptureAndClassify>();
        for (int i = 0; i < initialpositions.Length; i++)
        {

            saveinitialcolors[i] = Instantiate(initialcolors[i], new Vector3(0, 0, 0), Quaternion.identity);
            saveinitialpositions[i] = initialpositions[i];

        }
    }

    public override void Initialize()
    {
        //saveinitialpositions = initialpositions;
        //saveinitialcolors = initialcolors;
        //for (int i = 0; i < initialpositions.Length; i++)
        //{
        //    saveinitialcolors[i] = initialcolors[i];
        //    saveinitialpositions[i] = initialpositions[i];
        //}
    }

    public override void OnEpisodeBegin()
    {

        //anger = new Vector3();
        //disgust = new Vector3();
        //happy = new Vector3();
        //sad = new Vector3();
        //surprise = new Vector3();

        maincolor = new Vector4();



      //  Debug.Log("Start////////////////////////////////////////////////////////////////////////////////////");
        //initialpositions = saveinitialpositions;
        //initialcolors = saveinitialcolors;
        for (int i = 0; i < initialpositions.Length; i++)
        {
            initialcolors[i].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, saveinitialpositions[i].position, 0.0f);
            initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, saveinitialpositions[i].localScale, 0.0f);
            initialpositions[i].eulerAngles = Vector3.MoveTowards(initialpositions[i].eulerAngles, saveinitialpositions[i].eulerAngles, 0.0f);
            


        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for (int i = 0; i < initialpositions.Length; i++)
        {
            sensor.AddObservation(initialpositions[i].position); //3 * 4
            sensor.AddObservation(initialpositions[i].localScale); //3 * 4
            sensor.AddObservation(initialpositions[i].eulerAngles); //3 * 4
            sensor.AddObservation(initialcolors[i].color.r); // 3 * 4
            sensor.AddObservation(initialcolors[i].color.g);
            sensor.AddObservation(initialcolors[i].color.b);
        }

        sensor.AddObservation(currentEmotion);

        sensor.AddObservation(anger.r);
        sensor.AddObservation(anger.g);
        sensor.AddObservation(anger.b);
        sensor.AddObservation(disgust.r);
        sensor.AddObservation(disgust.g);
        sensor.AddObservation(disgust.b);
        sensor.AddObservation(happy.r);
        sensor.AddObservation(happy.g);
        sensor.AddObservation(happy.b);
        sensor.AddObservation(sad.r);
        sensor.AddObservation(sad.g);
        sensor.AddObservation(sad.b);
        sensor.AddObservation(surprise.r);
        sensor.AddObservation(surprise.g);
        sensor.AddObservation(surprise.b);

        sensor.AddObservation(maincolor.r);
        sensor.AddObservation(maincolor.g);
        sensor.AddObservation(maincolor.b);


    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var count = 0;
        //var steps = Time.deltaTime * 0f;
        var steps = 0.1f;

        for (int i = 0; i < initialpositions.Length; i++)
        {
           // Mathf.Clamp(actions.ContinuousActions[count], 0.50f, 0.60f);
            towardspositions[i] = new Vector3(Mathf.Clamp(actions.ContinuousActions[count],-1.5f,1.5f), Mathf.Clamp(actions.ContinuousActions[count + 1], -1.5f, 1.5f), Mathf.Clamp(actions.ContinuousActions[count + 2], -1.5f, 1.5f))* multiplier;
            count += 3;
            initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
            var distance = Vector3.Distance(initialpositions[i].position, range);
           // Debug.Log("Distance: ");
           // Debug.Log(distance);

            if (distance <= 1.0f)
            {
              //  Debug.Log("END///////");
                EndEpisode();
            }
        }

        

        for (int i = 0; i < initialpositions.Length; i++)
        {
            towardsscale[i] = new Vector3(Mathf.Clamp(actions.ContinuousActions[count], 0.5f, 2.0f), Mathf.Clamp(actions.ContinuousActions[count + 1], 0.5f, 2.0f), Mathf.Clamp(actions.ContinuousActions[count + 2], 0.5f, 2.0f)) * scale;
            count += 3;
            towardsrotation[i] = new Vector3(actions.ContinuousActions[count], actions.ContinuousActions[count + 1], actions.ContinuousActions[count + 2]);
            count += 3;
            initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);


            // Calculate a rotation a step closer to the target and applies rotation to this object
            //initialpositions[i].Rotate(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z, Space.Self);


            //initialpositions[i].eulerAngles = Vector3.MoveTowards(initialpositions[i], towardsrotation[i], steps);

            float bblend = Mathf.Clamp(actions.ContinuousActions[count], 0.70f, 0.99f);
            count += 1;


            initialcolors[i].blend = Mathf.MoveTowards(initialcolors[i].blend, bblend, 0.5f);
            

            


            var distance = Vector3.Distance(initialpositions[i].localScale, range);

           

            if (distance <= 1.0f)
            {
              //  Debug.Log("END///////");
                EndEpisode();
            }
        }







        //Debug.Log(actions.DiscreteActions[0]);



        classifier.Capture();
        currentEmotion = classifier.emotion;
          //  actions.DiscreteActions[0];
        Debug.Log(actions.DiscreteActions[0]);
       //  remo = classifier.emotion;
       remo = actions.DiscreteActions[0];



        //   Debug.Log("currentEmotion" + currentEmotion);



        //if (currentEmotion == 0)
        //{
        //    anger = new Vector4(0.9882352941176471f, 0.011764705882352941f, 0.011764705882352941f,1.0f);
        //}

        //if (currentEmotion == 1)
        //{
        //    disgust = new Vector4(0.5215686274509804f, 0.7176470588235294f, 0.13333333333333333f, 1.0f);
        //}

        //if (currentEmotion == 2)
        //{
        //    happy = new Vector4(0.9137254901960784f, 0.7529411764705882f, 0.3686274509803922f, 1.0f);
        //}

        //if (currentEmotion == 3)
        //{
        //    sad = new Vector4(0.4588235294117647f, 0.6509803921568628f, 0.8823529411764706f, 1.0f);
        //}

        //if (currentEmotion == 4)
        //{
        //    surprise = new Vector4(0.6235294117647059f, 0.3568627450980392f, 0.8117647058823529f, 1.0f);
        //}



        //maincolor = new Vector4(actions.ContinuousActions[count], actions.ContinuousActions[count + 1], actions.ContinuousActions[count + 2], 1.0f);
        //count += 3;

        //maincolor = emocolors[actions.DiscreteActions[0]];

        for (int i = 0; i < initialpositions.Length; i++)
        {
            towardscolor[i] = new Vector4(Mathf.Clamp(actions.ContinuousActions[count], 0.0f, 1.0f), Mathf.Clamp(actions.ContinuousActions[count + 1], 0.0f, 1.0f), Mathf.Clamp(actions.ContinuousActions[count + 2], 0.0f, 1.0f), 1.0f);
            count += 3;
            //count += 4;
            initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardscolor[i], steps);

            //if (currentEmotion == 0)
            //{
            //    //color distance
            //    float cdist = Vector3.Distance(new Vector3(anger.r,anger.g,anger.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
            //    //Debug.Log(cdist);

            //    if (cdist <= 0.1f)
            //    {
            //        float re = 1.0f - cdist;
            //        AddReward(re);
            //    }

            //}

            //if (currentEmotion == 1)
            //{
            //    //color distance
            //    float cdist = Vector3.Distance(new Vector3(disgust.r,disgust.g,disgust.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
            //    //Debug.Log(cdist);
            //    if (cdist <= 0.1f)
            //    {
            //        float re = 1.0f - cdist;
            //        AddReward(re);
            //    }

            //}

            //if (currentEmotion == 2)
            //{
            //    //color distance
            //    float cdist = Vector3.Distance(new Vector3(happy.r,happy.g,happy.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
            //    //Debug.Log(cdist);
            //    if (cdist <= 0.1f)
            //    {
            //        float re = 1.0f - cdist;
            //        AddReward(re);
            //    }

            //}

            //if (currentEmotion == 3)
            //{
            //    //color distance
            //    float cdist = Vector3.Distance(new Vector3(sad.r,sad.g,sad.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
            //    //Debug.Log(cdist);
            //    if (cdist <= 0.1f)
            //    {
            //        float re = 1.0f - cdist;
            //        AddReward(re);
            //    }

            //}

            //if (currentEmotion == 4)
            //{
            //    //color distance
            //    float cdist = Vector3.Distance(new Vector3(surprise.r,surprise.g,surprise.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
            //    //Debug.Log(cdist);
            //    if (cdist <= 0.1f)
            //    {
            //        float re = 1.0f - cdist;
            //        AddReward(re);
            //    }

            //}


        }

        frames += 1;

        if (currentEmotion == actions.DiscreteActions[0])
        {
            
            string name = "";
            if (currentEmotion == 0)
            {
                name = "anger";
            }

            if (currentEmotion == 1)
            {
                name = "disgust";
            }

            if (currentEmotion == 2)
            {
                name = "happy";
            }

            if (currentEmotion == 3)
            {
                name = "sad";
            }

            if (currentEmotion == 4)
            {
                name = "surprise";
            }
               AddReward(1.0f);

            if(frames >= maxframes)
            {
                //freeze(name);
                frames = 0;
            }
            
            EndEpisode();

        }



    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown("space"))
    //    {
    //        freeze("test");
    //    }


    //    var steps = Time.deltaTime * 1.0f;

    //    for(int i=0; i<initialpositions.Length; i++)
    //    {
    //        //create parameter
    //        towardspositions[i] = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
    //        towardsscale[i] = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
    //        towardscolor[i] = new Vector4(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));



    //        //update parameter
    //        initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
    //        initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
    //        //initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
    //        initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardsscale[i], steps);

    //    }
    //}

    public void freeze(string name)
    {
        Mesh test;
        test = oya.generateMesh(100, true, true, 100, true);
        AssetDatabase.CreateAsset(test, "Assets/Frozen/" + name + ".mesh");
        AssetDatabase.SaveAssets();

        //oya.freezeContainersHierarchyToMesh();
        //oya.defrostContainersHierarchy();


    }
}
