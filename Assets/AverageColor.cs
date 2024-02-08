using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AverageColor : MonoBehaviour
{
    //averagecolor
    public Texture2D test;
    public Color calculatedcolor;

    //okoranaide ne
    public Color dancer;
    public float distance;


    // Start is called before the first frame update
    void Start()
    {
        calculatedcolor = AverageColorFromTexture(test);
    }

    // Update is called once per frame
    void Update()
    {
        float H1, S1, V1;
        float H2, S2, V2;
        Color.RGBToHSV(dancer, out H1, out S1, out V1);
        Color.RGBToHSV(calculatedcolor, out H2, out S2, out V2);
        distance = Vector3.Distance(new Vector3(H1,S1,V1), new Vector3(H2, S2, V2));
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
