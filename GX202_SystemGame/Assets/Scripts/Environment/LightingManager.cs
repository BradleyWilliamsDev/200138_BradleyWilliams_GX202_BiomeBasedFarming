using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    // References
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    // Variables
    [SerializeField, Range(0, 24)] private float timeOfDay;

    public GameObject[] typesOfWeather;

    private void Start() {
        typesOfWeather[0].SetActive(false);
    }

    private void Update() {
        if (preset == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * 0.2f;
            timeOfDay %= 24; //clamped between 0-24
            UpdateLighting(timeOfDay / 24f);
        }
        else
        {
            UpdateLighting(timeOfDay / 24f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        float xRotValue = (timePercent * (360f)) - 90f;
        RenderSettings.ambientLight =  preset.AmbientColour.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColour.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.DirectionalColour.Evaluate(timePercent);
            if (xRotValue < 0 && xRotValue >= -90)
            {
                xRotValue = -165f;
            }
            if (xRotValue < 0)
            {
                xRotValue = 0;
            }

            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3(xRotValue, 170, 0));
        }
    }

    private void OnValidate()
    {
        if (directionalLight != null)
        {
            return;
        }
        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}
