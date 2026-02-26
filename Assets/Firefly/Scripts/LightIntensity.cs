using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensity : MonoBehaviour
{
    public Light myLight;



    //Intensity Variables
    public bool changeIntensity = false;
    public float intensitySpeed = 0.5f;
    public float maxIntensity = 1.0f;
    public float changecolorSpeed = 2.00f;

    public Color skyDayColor = new Color(128.0f / 128.0f, 128.0f / 128.0f, 128.0f / 128f, 128.0f / 128.0f);
    public Color skyNightColor = new Color(33.0f / 128.0f, 36.0f / 128.0f, 46.0f / 128f, 128.0f / 128.0f);
    public Color currentColor;

    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light>();
        RenderSettings.skybox.SetColor("_Tint", skyDayColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("down"))
        {
            myLight.intensity = Mathf.PingPong(Time.time * intensitySpeed, maxIntensity);
            currentColor = Color.Lerp(skyNightColor, skyDayColor, Mathf.PingPong(Time.time,changecolorSpeed));
            RenderSettings.skybox.SetColor("_Tint", currentColor);
        }

    }
}
