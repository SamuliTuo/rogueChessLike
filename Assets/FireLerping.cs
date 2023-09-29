using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLerping : MonoBehaviour
{
    [SerializeField] private float minIntensity = 20;
    [SerializeField] private float maxIntensity = 140;
    [SerializeField] private float minRange = 6;
    [SerializeField] private float maxRange = 12;

    [Space(20)]
    [SerializeField] private float minLerpSpeed = 0.1f;
    [SerializeField] private float maxLerpSpeed = 3f;

    //intensity20 140    range6 12
    private Light light;
    private bool goingUp;

    private void Start()
    {
        light = GetComponent<Light>();
        goingUp = false;
        StartCoroutine(FireCoroutine());
    }

    IEnumerator FireCoroutine()
    {
        float t = 0;
        float speed = Random.Range(minLerpSpeed, maxLerpSpeed);
        goingUp = !goingUp;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            if (goingUp)
            {
                light.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
                light.range = Mathf.Lerp(minRange, maxRange, t);
            }
            else
            {
                light.intensity = Mathf.Lerp(maxIntensity, minIntensity, t);
                light.range = Mathf.Lerp(maxRange, minRange, t);
            }
            yield return null;
        }
        StartCoroutine(FireCoroutine());
    }
}
