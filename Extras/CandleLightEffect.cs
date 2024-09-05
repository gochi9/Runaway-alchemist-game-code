using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleLightEffect : MonoBehaviour
{
    private Light2D light2D;
    
    [Header("Intensity Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.2f;
    public float intensityChangeSpeed = 0.2f;
    
    [Header("Radius Settings")]
    public float minInnerRadius = 0.5f;
    public float maxInnerRadius = 1.0f;
    public float minOuterRadius = 1.0f;
    public float maxOuterRadius = 2.0f;
    public float radiusChangeSpeed = 0.2f;
    
    [Header("Falloff Settings")]
    public float minFalloffStrength = 0.5f;
    public float maxFalloffStrength = 1.5f;
    public float falloffChangeSpeed = 0.2f;
    
    private float targetIntensity;
    private float targetInnerRadius;
    private float targetOuterRadius;
    private float targetFalloffStrength;

    void Start(){
        light2D = GetComponent<Light2D>();
        SetNewTargets();
    }

    void Update(){
        UpdateLightIntensity();
        UpdateLightRadius();
        UpdateFalloffStrength();
    }

    void SetNewTargets(){
        targetIntensity = Random.Range(minIntensity, maxIntensity);
        targetInnerRadius = Random.Range(minInnerRadius, maxInnerRadius);
        targetOuterRadius = Random.Range(minOuterRadius, maxOuterRadius);
        targetFalloffStrength = Random.Range(minFalloffStrength, maxFalloffStrength);
    }

    void UpdateLightIntensity(){
        light2D.intensity = Mathf.MoveTowards(light2D.intensity, targetIntensity, intensityChangeSpeed * Time.deltaTime);
        
        if (Mathf.Approximately(light2D.intensity, targetIntensity))
            targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    void UpdateLightRadius(){
        light2D.pointLightInnerRadius = Mathf.MoveTowards(light2D.pointLightInnerRadius, targetInnerRadius, radiusChangeSpeed * Time.deltaTime);
        light2D.pointLightOuterRadius = Mathf.MoveTowards(light2D.pointLightOuterRadius, targetOuterRadius, radiusChangeSpeed * Time.deltaTime);
        
        if (Mathf.Approximately(light2D.pointLightInnerRadius, targetInnerRadius) && Mathf.Approximately(light2D.pointLightOuterRadius, targetOuterRadius)){
            targetInnerRadius = Random.Range(minInnerRadius, maxInnerRadius);
            targetOuterRadius = Random.Range(minOuterRadius, maxOuterRadius);
        }
    }

    void UpdateFalloffStrength(){
        light2D.falloffIntensity = Mathf.MoveTowards(light2D.falloffIntensity, targetFalloffStrength, falloffChangeSpeed * Time.deltaTime);
        
        if (Mathf.Approximately(light2D.falloffIntensity, targetFalloffStrength))
            targetFalloffStrength = Random.Range(minFalloffStrength, maxFalloffStrength);
    }
}