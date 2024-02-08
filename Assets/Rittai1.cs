using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Clayxels;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Rittai1 : Agent
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

    public float adist1;
    public float adist2;
    public float adist3;
    public float ddist1;
    public float ddist2;
    public float ddist3;
    public float hdist1;
    public float hdist2;
    public float hdist3;
    public float sadist1;
    public float sadist2;
    public float sadist3;
    public float sudist1;
    public float sudist2;
    public float sudist3;

    public float nudist1;
    public float nudist2; 
    public float nudist3;

    private Vector3 range = new Vector3(9.0f, 9.0f, 9.0f);
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


    ///marie is a good girl
    ///

    public Transform[] happypositions;
    public ClayObject[] happycolors;
    public Transform[] sadpositions;
    public ClayObject[] sadcolors;
    public Transform[] angerpositions;
    public ClayObject[] angercolors;
    public Transform[] surprisepositions;
    public ClayObject[] surprisecolors;
    public Transform[] disgustpositions;
    public ClayObject[] disgustcolors;


    public float multiplier = 2.0f;
    public float scale = 2.5f;

    int frames = 0;
    int maxframes = 1000;

    public float totalreward = 0;



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
            saveinitialpositions[i] = new GameObject().transform;

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
        totalreward = 0;



        Debug.Log("Start////////////////////////////////////////////////////////////////////////////////////");
        //initialpositions = saveinitialpositions;
        //initialcolors = saveinitialcolors;
        for (int i = 0; i < initialpositions.Length; i++)
        {
            //initialcolors[i].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            initialpositions[i].position = new Vector3(0f, 0f, 0f);
            initialpositions[i].localScale = new Vector3(1f, 1f, 1f);
            initialpositions[i].rotation = saveinitialpositions[i].rotation;


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
        var countnut = 0;
        var countnuty = 0;
        var countnutz = 0;

        //var steps = Time.deltaTime * 0f;
        var steps = 0.1f;

        for (int i = 0; i < initialpositions.Length; i++)
        {
            // Mathf.Clamp(actions.ContinuousActions[count], 0.50f, 0.60f);
            //multiplier = Mathf.Clamp(Mathf.Abs(actions.ContinuousActions[count] * 5),0.5f,5.0f);
            multiplier = 1.0f;

            //count += 1;
            //scale = Mathf.Clamp(Mathf.Abs(actions.ContinuousActions[count] * 5),0.5f,5.0f);
            scale = 1.0f;


            towardspositions[i] = new Vector3(Mathf.Clamp(actions.ContinuousActions[count], -5f, 5f), Mathf.Clamp(actions.ContinuousActions[count + 1], -5f, 5f), Mathf.Clamp(actions.ContinuousActions[count + 2], -5f, 5f)) * multiplier;
            count += 3;

            //initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
            var distance = Vector3.Distance(initialpositions[i].position, range);
            // Debug.Log("Distance: ");
            // Debug.Log(distance);

            /* if (distance <= 1.0f)
             {
               //  Debug.Log("END///////");
                 EndEpisode();
             }*/
        }



        for (int i = 0; i < initialpositions.Length; i++)
        {
            towardsscale[i] = new Vector3(actions.ContinuousActions[count], actions.ContinuousActions[count + 1], actions.ContinuousActions[count + 2]);
            count += 3;
            towardsrotation[i] = new Vector3(Mathf.Clamp(actions.ContinuousActions[count] * 100, -180f, 180f), Mathf.Clamp(actions.ContinuousActions[count + 1] * 100, -180f, 180f), Mathf.Clamp(actions.ContinuousActions[count + 2] * 100, -180f, 180f));
            count += 3;
            //initialpositions[i].localScale = Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);


            // Calculate a rotation a step closer to the target and applies rotation to this object
            //initialpositions[i].Rotate(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z, Space.Self);


            //initialpositions[i].eulerAngles = Vector3.MoveTowards(initialpositions[i], towardsrotation[i], steps);

            //float bblend = Mathf.Clamp(actions.ContinuousActions[count], 0.70f, 0.99f);
            //count += 1;


            //initialcolors[i].blend = Mathf.MoveTowards(initialcolors[i].blend, bblend, 0.5f);





            var distance = Vector3.Distance(initialpositions[i].localScale, range);



            /* if (distance <= 1.0f)
             {
               //  Debug.Log("END///////");
                 EndEpisode();
             }*/


        }







        //Debug.Log(actions.DiscreteActions[0]);



        classifier.Capture();

         currentEmotion = classifier.emotion;
        //  currentEmotion = actions.DiscreteActions[0];
        //  Debug.Log(actions.DiscreteActions[0]);
        remo = actions.DiscreteActions[0];
        //  remo = classifier.emotion;


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



            //anger
            if (currentEmotion == 0)
            {
                towardscolor[i] = angercolors[i].color;
                initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardscolor[i], steps);
                //towardspositions[i] = angerpositions[i].position;
                towardspositions[i] *= Time.deltaTime;
                initialpositions[i].Translate(towardspositions[i].x, towardspositions[i].y, towardspositions[i].z);

                //initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
                //towardsscale[i] = angerpositions[i].localScale;
                initialpositions[i].localScale += towardsscale[i] * Time.deltaTime;
                if (initialpositions[i].localScale.x <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(1.0f, initialpositions[i].localScale.y, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.y <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, 1.0f, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.z <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, initialpositions[i].localScale.y, 1.0f);
                }


                //Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
                //initialpositions[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, angerpositions[i].rotation,  steps);
                //Quaternion target = Quaternion.Euler(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);
                towardsrotation[i] *= Time.deltaTime;
                initialpositions[i].Rotate(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);


                // Dampen towards the target rotation

                //[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, target, steps);
                //initialpositions[i].eulerAngles = Vector3.MoveTowards(initialpositions[i].eulerAngles, towardsrotation[i], steps);




                //color distance
                float adist = Vector3.Distance(new Vector3(angercolors[i].color.r, angercolors[i].color.g, angercolors[i].color.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
                // Debug.Log("cdist"+cdist);

                if (adist <= 0.01f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    //  AddReward(re);
                    //Debug.Log("addrewardcDist");
                }


                //color distance
                adist1 = Vector3.Distance(angerpositions[i].position, initialpositions[i].position);
                //Debug.Log(cdist);

                if (adist1 < 0.8f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("anReward");
                }

                if (adist1 > 9.0f)
                {
                    Debug.Log("ENDposition/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }

                //color distance
                adist2 = Vector3.Distance(angerpositions[i].localScale, towardsscale[i]);
                //Debug.Log(adist);

                if (adist2 <= 0.1f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    //Debug.Log("anReward");

                }

                if (adist2 > 9.0f)
                {
                    Debug.Log("ENDScale/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }


                //color distance
                adist3 = Vector3.Distance(angerpositions[i].eulerAngles, initialpositions[i].eulerAngles);
                //Debug.Log(cdist);

                if (adist3 < 1.0f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("anReward");
                }





            }

            //disgust
            if (currentEmotion == 1)
            {

                towardscolor[i] = disgustcolors[i].color;
                initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardscolor[i], steps);
                //towardspositions[i] = angerpositions[i].position;
                towardspositions[i] *= Time.deltaTime;
                initialpositions[i].Translate(towardspositions[i].x, towardspositions[i].y, towardspositions[i].z);

                //initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
                //towardsscale[i] = angerpositions[i].localScale;
                initialpositions[i].localScale += towardsscale[i] * Time.deltaTime;
                if (initialpositions[i].localScale.x <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(1.0f, initialpositions[i].localScale.y, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.y <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, 1.0f, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.z <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, initialpositions[i].localScale.y, 1.0f);
                }


                //Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
                //initialpositions[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, angerpositions[i].rotation,  steps);
                //Quaternion target = Quaternion.Euler(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);
                towardsrotation[i] *= Time.deltaTime;
                initialpositions[i].Rotate(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);


                // Dampen towards the target rotation

                //[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, target, steps);
                //initialpositions[i].eulerAngles = Vector3.MoveTowards(initialpositions[i].eulerAngles, towardsrotation[i], steps);




                //color distance
                float ddist = Vector3.Distance(new Vector3(disgustcolors[i].color.r, disgustcolors[i].color.g, disgustcolors[i].color.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
                // Debug.Log("cdist"+cdist);

                if (ddist <= 0.01f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    //  AddReward(re);
                    //Debug.Log("addrewardcDist");
                }


                //color distance
                ddist1 = Vector3.Distance(disgustpositions[i].position, initialpositions[i].position);
                //Debug.Log(cdist);

                if (ddist1 < 0.8f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("disReward");
                }

                if (ddist1 > 9.0f)
                {
                    Debug.Log("ENDposition/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }

                //color distance
                ddist2 = Vector3.Distance(disgustpositions[i].localScale, towardsscale[i]);
                //Debug.Log(adist);

                if (ddist2 <= 0.1f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    //Debug.Log("anReward");

                }

                if (ddist2 > 9.0f)
                {
                    Debug.Log("ENDScale/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }


                //color distance
                ddist3 = Vector3.Distance(disgustpositions[i].eulerAngles, initialpositions[i].eulerAngles);
                //Debug.Log(cdist);

                if (ddist3 < 1.0f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("disReward");
                }

            }


            //happy
            if (currentEmotion == 2)
            {
                towardscolor[i] = happycolors[i].color;
                initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardscolor[i], steps);
                //towardspositions[i] = angerpositions[i].position;
                towardspositions[i] *= Time.deltaTime;
                initialpositions[i].Translate(towardspositions[i].x, towardspositions[i].y, towardspositions[i].z);

                //initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
                //towardsscale[i] = angerpositions[i].localScale;
                initialpositions[i].localScale += towardsscale[i] * Time.deltaTime;
                if (initialpositions[i].localScale.x <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(1.0f, initialpositions[i].localScale.y, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.y <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, 1.0f, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.z <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, initialpositions[i].localScale.y, 1.0f);
                }


                //Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
                //initialpositions[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, angerpositions[i].rotation,  steps);
                //Quaternion target = Quaternion.Euler(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);
                towardsrotation[i] *= Time.deltaTime;
                initialpositions[i].Rotate(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);


                // Dampen towards the target rotation

                //[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, target, steps);
                //initialpositions[i].eulerAngles = Vector3.MoveTowards(initialpositions[i].eulerAngles, towardsrotation[i], steps);




                //color distance
                float hdist = Vector3.Distance(new Vector3(happycolors[i].color.r, happycolors[i].color.g, happycolors[i].color.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
                // Debug.Log("cdist"+cdist);

                if (hdist <= 0.01f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    //  AddReward(re);
                    //Debug.Log("addrewardcDist");
                }


                //color distance
                hdist1 = Vector3.Distance(happypositions[i].position, initialpositions[i].position);
                //Debug.Log(cdist);

                if (hdist1 < 0.8f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("haReward");
                }

                if (hdist1 > 9.0f)
                {
                    Debug.Log("ENDposition/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }

                //color distance
                hdist2 = Vector3.Distance(happypositions[i].localScale, towardsscale[i]);
                //Debug.Log(adist);

                if (hdist2 <= 0.1f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    //Debug.Log("anReward");

                }

                if (hdist2 > 9.0f)
                {
                    Debug.Log("ENDScale/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }


                //color distance
                hdist3 = Vector3.Distance(happypositions[i].eulerAngles, initialpositions[i].eulerAngles);
                //Debug.Log(cdist);

                if (hdist3 < 1.0f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("haReward");
                }


            }


            //sad
            if (currentEmotion == 3)
            {
                towardscolor[i] = sadcolors[i].color;
                initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardscolor[i], steps);
                //towardspositions[i] = angerpositions[i].position;
                towardspositions[i] *= Time.deltaTime;
                initialpositions[i].Translate(towardspositions[i].x, towardspositions[i].y, towardspositions[i].z);

                //initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
                //towardsscale[i] = angerpositions[i].localScale;
                initialpositions[i].localScale += towardsscale[i] * Time.deltaTime;
                if (initialpositions[i].localScale.x <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(1.0f, initialpositions[i].localScale.y, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.y <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, 1.0f, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.z <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, initialpositions[i].localScale.y, 1.0f);
                }


                //Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
                //initialpositions[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, angerpositions[i].rotation,  steps);
                //Quaternion target = Quaternion.Euler(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);
                towardsrotation[i] *= Time.deltaTime;
                initialpositions[i].Rotate(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);


                // Dampen towards the target rotation

                //[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, target, steps);
                //initialpositions[i].eulerAngles = Vector3.MoveTowards(initialpositions[i].eulerAngles, towardsrotation[i], steps);




                //color distance
                float sadist = Vector3.Distance(new Vector3(sadcolors[i].color.r, sadcolors[i].color.g, sadcolors[i].color.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
                // Debug.Log("cdist"+cdist);

                if (sadist <= 0.01f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    //  AddReward(re);
                    //Debug.Log("addrewardcDist");
                }


                //color distance
                sadist1 = Vector3.Distance(sadpositions[i].position, initialpositions[i].position);
                //Debug.Log(cdist);

                if (sadist1 < 0.8f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("disReward");
                }

                if (sadist1 > 9.0f)
                {
                    Debug.Log("ENDposition/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }

                //color distance
                sadist2 = Vector3.Distance(sadpositions[i].localScale, towardsscale[i]);
                //Debug.Log(adist);

                if (sadist2 <= 0.1f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    //Debug.Log("anReward");

                }

                if (sadist2 > 9.0f)
                {
                    Debug.Log("ENDScale/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }


                //color distance
                sadist3 = Vector3.Distance(sadpositions[i].eulerAngles, initialpositions[i].eulerAngles);
                //Debug.Log(cdist);
                //   Debug.Log("sad" + surprisepositions[i].eulerAngles);

                if (sadist3 < 1.0f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("anReward");
                }

            }

            //surprise
            if (currentEmotion == 4)
            {
                towardscolor[i] = surprisecolors[i].color;
                initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardscolor[i], steps);
                //towardspositions[i] = angerpositions[i].position;
                towardspositions[i] *= Time.deltaTime;
                initialpositions[i].Translate(towardspositions[i].x, towardspositions[i].y, towardspositions[i].z);

                //initialpositions[i].position = Vector3.MoveTowards(initialpositions[i].position, towardspositions[i], steps);
                //towardsscale[i] = angerpositions[i].localScale;
                initialpositions[i].localScale += towardsscale[i] * Time.deltaTime;
                if (initialpositions[i].localScale.x <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(1.0f, initialpositions[i].localScale.y, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.y <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, 1.0f, initialpositions[i].localScale.z);
                }

                if (initialpositions[i].localScale.z <= 1.0f)
                {
                    initialpositions[i].localScale = new Vector3(initialpositions[i].localScale.x, initialpositions[i].localScale.y, 1.0f);
                }


                //Vector3.MoveTowards(initialpositions[i].localScale, towardsscale[i], steps);
                //initialpositions[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, angerpositions[i].rotation,  steps);
                //Quaternion target = Quaternion.Euler(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);
                towardsrotation[i] *= Time.deltaTime;
                initialpositions[i].Rotate(towardsrotation[i].x, towardsrotation[i].y, towardsrotation[i].z);


                // Dampen towards the target rotation

                //[i].rotation = Quaternion.Slerp(initialpositions[i].rotation, target, steps);
                //initialpositions[i].eulerAngles = Vector3.MoveTowards(initialpositions[i].eulerAngles, towardsrotation[i], steps);




                //color distance
                float sudist = Vector3.Distance(new Vector3(surprisecolors[i].color.r, surprisecolors[i].color.g, surprisecolors[i].color.b), new Vector3(towardscolor[i].r, towardscolor[i].g, towardscolor[i].b));
                //  Debug.Log("cdist"+cdist);

                if (sudist <= 0.01f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    //  AddReward(re);
                    //Debug.Log("addrewardcDist");
                }


                //color distance
                sudist1 = Vector3.Distance(surprisepositions[i].position, initialpositions[i].position);
                //Debug.Log(cdist);

                if (sudist1 < 0.8f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("suReward");
                }

                if (sudist1 > 9.0f)
                {
                    Debug.Log("ENDposition/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }

                //color distance
                sudist2 = Vector3.Distance(surprisepositions[i].localScale, towardsscale[i]);
                //Debug.Log(adist);

                if (sudist2 <= 0.1f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    //Debug.Log("anReward");

                }

                if (sudist2 > 9.0f)
                {
                    Debug.Log("ENDScale/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }


                //color distance
                sudist3 = Vector3.Distance(surprisepositions[i].eulerAngles, initialpositions[i].eulerAngles);
                //  Debug.Log("sup"+surprisepositions[i].eulerAngles);


                if (sudist3 < 1.0f)
                {
                    float re = 0.1f;
                    totalreward += 0.1f;
                    AddReward(re);
                    Debug.Log("suReward");
                }

            }

/*
            if (currentEmotion == 5)
            {

                towardscolor[i] = new Vector4(Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f),0f);
                initialcolors[i].color = Vector4.MoveTowards(initialcolors[i].color, towardscolor[i], steps);

                Vector3 posnut = towardspositions[i];

                Debug.Log("count" + countnut);
                float posx;
                float posy;
                float posz;
                posx = 1.0f * countnut;
                posy = 1.0f * countnuty;
                posz = 1.0f * countnutz;
                posnut.x = -1.5f + posx;
                posnut.y = 1.5f - posy;
                posnut.z = 1.5f - posz;

                towardspositions[i] = posnut;
                Debug.Log("pounut.x" + towardspositions[i].x);
                Debug.Log("pounut.y" + towardspositions[i].y);
                Debug.Log("pounut.z" + towardspositions[i].z);

                countnut += 1;

                if (countnut == 4)
                {
                    countnut = 0;
                    countnuty += 1;

                    if(countnuty == 4)
                    {
                        countnuty = 0;
                        countnutz += 1;
                    }
                }

                //towardspositions[i] = angerpositions[i].position;
                //  towardspositions[i] *= Time.deltaTime;
                initialpositions[i].position = towardspositions[i];
            //  initialpositions[i].Translate(towardspositions[i].x, towardspositions[i].y, towardspositions[i].z);

               nudist1 = Vector3.Distance(towardspositions[i], initialpositions[i].position);
                //Debug.Log(cdist);
                                                                 

                if (sudist1 > 3.0f)
                {
                    Debug.Log("ENDposition/////////////////////////////");
                    AddReward(-0.1f);
                    EndEpisode();
                }

            }*/


        }


        /*if(totalreward > 64.0f)
        {

            EndEpisode();
            Debug.Log("EndEpisodetotalreward");

        }*/




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
            //Debug.Log("addrewardAction");

            if (frames >= maxframes)
            {
                //freeze(name);
                frames = 0;
            }

            //EndEpisode();

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

/*    public void freeze(string name)
    {
        Mesh test;
        test = oya.generateMesh(100, true, true, 100, true);
        AssetDatabase.CreateAsset(test, "Assets/Frozen/" + name + ".mesh");
        AssetDatabase.SaveAssets();

        //oya.freezeContainersHierarchyToMesh();
        //oya.defrostContainersHierarchy();


    }*/
}