using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightsOutEvent : MonoBehaviour
{
    [Header("Timing")]
    public float darknessDuration = 1f;
    public float teleportDelay = 0.3f; 
    public bool disableFog = true;

    [Header("Teleport")]
    public Transform teleportTarget;
    public Transform player;
    

    [Header("Room Stretch ")]
    public RoomStretchEvent roomStretchEvent;
    public AudioSource stretchSound;

    private bool hasTriggered = false;

    private List<Light> sceneLights = new List<Light>();

    private Color originalAmbientLight;
    private float originalAmbientIntensity;

    private bool originalFog;
    private Color originalFogColor;
    private float originalFogDensity;
    private FogMode originalFogMode;
    private float originalFogStartDistance;
    private float originalFogEndDistance;

    void Start()
    {
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);

foreach (Light sceneLight in allLights)
{
    if (sceneLight != null)
        sceneLights.Add(sceneLight);
}

        foreach (Light light in allLights)
        {
            if (light != null)
                sceneLights.Add(light);
        }

        originalAmbientLight = RenderSettings.ambientLight;
        originalAmbientIntensity = RenderSettings.ambientIntensity;

        originalFog = RenderSettings.fog;
        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogMode = RenderSettings.fogMode;
        originalFogStartDistance = RenderSettings.fogStartDistance;
        originalFogEndDistance = RenderSettings.fogEndDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;

        StartCoroutine(LightsOut());
    }

    IEnumerator LightsOut()
    {
        
        foreach (Light light in sceneLights)
        {
            if (light != null)
                light.enabled = false;
        }

        RenderSettings.ambientIntensity = 0f;
        RenderSettings.ambientLight = Color.black;

        if (disableFog)
            RenderSettings.fog = false;

        
        yield return new WaitForSeconds(teleportDelay);

        
        if (player != null && teleportTarget != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            player.position = teleportTarget.position;
            player.rotation = teleportTarget.rotation;

            

            if (controller != null)
                controller.enabled = true;
        }

        float remainingDarkness = darknessDuration - teleportDelay;
        if (remainingDarkness > 0f)
            yield return new WaitForSeconds(remainingDarkness);

        
        if (roomStretchEvent != null)
            roomStretchEvent.StretchRoom();

        if (stretchSound != null)
            stretchSound.Play();

        if (roomStretchEvent != null)
            yield return new WaitForSeconds(roomStretchEvent.stretchDuration);

        
        foreach (Light light in sceneLights)
        {
            if (light != null)
                light.enabled = true;
        }

        RenderSettings.ambientLight = originalAmbientLight;
        RenderSettings.ambientIntensity = originalAmbientIntensity;

        if (disableFog)
        {
            RenderSettings.fog = originalFog;
            RenderSettings.fogColor = originalFogColor;
            RenderSettings.fogDensity = originalFogDensity;
            RenderSettings.fogMode = originalFogMode;
            RenderSettings.fogStartDistance = originalFogStartDistance;
            RenderSettings.fogEndDistance = originalFogEndDistance;
        }
    }
}