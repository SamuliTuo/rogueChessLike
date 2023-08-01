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
        transform.position += (new Vector3(camXPos, camYPos, player.position.z) + Vector3.forward * lookaheadAmount - transform.position) * Time.deltaTime * scrollSpeed;
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