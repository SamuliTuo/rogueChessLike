using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraMover : MonoBehaviour
{
    public float camXPos;
    public float camYPos;
    public float lookaheadAmount;
    public float scrollSpeed;

    [SerializeField] private float returningToMapHeight = 0;

    private Transform player;
    private bool coffeeLerping = false;

    void Update()
    {
        if (player == null) 
        {
            if (GameManager.Instance.MapController != null)
            {
                player = GameManager.Instance.MapController.transform.Find("playerParty_mapUnit(Clone)");
            }
            return;
        }
        if (coffeeLerping)
        {
            return;
        }
        transform.position += (new Vector3(camXPos, camYPos, player.position.z) + Vector3.forward * lookaheadAmount - transform.position) * Time.deltaTime * scrollSpeed;
    }

    public void StartCoffeeLerp()
    {
        coffeeLerping = true;
        StartCoroutine(CoffeeCupLerp(0.1f));
    }

    [SerializeField] private float secondTiltStartPerc = 0.5f;
    IEnumerator CoffeeCupLerp(float lerpSpeed)
    {
        Vector3 targetAngle = new Vector3(-22f, 0f, 0f);
        Vector3 currentAngle = transform.eulerAngles;
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + Vector3.forward * 10;
        float t = 0;
        bool secondTiltStarted = false;
        while (t < 1)
        {
            float perc = t * t * t * (t * (6f * t - 15f) + 10f);
            if (secondTiltStarted == false)
            {
                currentAngle = new Vector3(
                    Mathf.LerpAngle(currentAngle.x, targetAngle.x, perc * 0.25f),
                    Mathf.LerpAngle(currentAngle.y, targetAngle.y, perc * 0.25f),
                    Mathf.LerpAngle(currentAngle.z, targetAngle.z, perc * 0.25f));
                transform.eulerAngles = currentAngle;
            }
            if (secondTiltStarted == false && t > secondTiltStartPerc)
            {
                secondTiltStarted = true;
                StartCoroutine(CoffeeCameraTilt2());
            }
            transform.position = Vector3.Lerp(startPos, targetPos, perc);

            t += Time.deltaTime * lerpSpeed;
            yield return null;
        }
        //coffeeLerping = false;
    }

    IEnumerator CoffeeCameraTilt2()
    {
        Vector3 targetAngle = new Vector3(-10, 0f, 0f);
        Vector3 currentAngle = transform.eulerAngles;
        float t = 0;
        while (t < 1)
        {
            float perc = t * t * t * (t * (6f * t - 15f) + 10f);
            currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, targetAngle.x, perc * 0.25f),
                Mathf.LerpAngle(currentAngle.y, targetAngle.y, perc * 0.25f),
                Mathf.LerpAngle(currentAngle.z, targetAngle.z, perc * 0.25f));
            transform.eulerAngles = currentAngle;
            t += Time.deltaTime * 0.1f;
            yield return null;
        }
    }


    public void SetCameraHeight()
    {
        
    }
    private void Start()
    {
        if (GameManager.Instance.pathTaken.Count > 0)
        {
            transform.position = GameManager.Instance.mapCameraLastPos + Vector3.up * returningToMapHeight;
        }
    }
}