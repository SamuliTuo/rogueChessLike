using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform boardCorner_botLeft, boardCorner_botRight, boardCorner_topLeft, boardCorner_topRight;
    [SerializeField] private float cameraDistanceMultiplier = 5;
    [SerializeField] private float zoomIncrement = 0.01f;

    [SerializeField] private float upOffset = 1;
    [SerializeField] private float botOffset = 1;
    [SerializeField] private float leftOffset = 1;
    [SerializeField] private float rightOffset = 1;

    private Camera cam;
    private Vector2Int boardSize;
    private float tileSize;

    
    private void Start()
    {
        cam = GetComponent<Camera>();
    }


    

    public void SetupBattleCamera(Vector2Int boardSize, float tileSize)
    {
        this.boardSize = boardSize;
        this.tileSize = tileSize;
        SetCamera(new Vector3(GameManager.Instance.currentScenario.cameraRotationX, GameManager.Instance.currentScenario.cameraRotationY, GameManager.Instance.currentScenario.cameraRotationZ));
    }
    public void RefreshCamera(Vector2Int boardSize, float tileSize)
    {
        this.boardSize = boardSize;
        this.tileSize = tileSize;
        SetCamera(transform.eulerAngles);
    }

    public void SetCamera(Vector3 eulers)
    {
        List<Transform> corners = new List<Transform> { boardCorner_botLeft, boardCorner_botRight, boardCorner_topLeft, boardCorner_topRight };
        PlaceCornerMarkers(corners);
        transform.rotation = Quaternion.Euler(eulers);
        //transform.position = boardCorner_botLeft.position + ((boardCorner_topRight.position - boardCorner_botLeft.position) * 0.5f);
        ParentCornersToCamera();
        DoDaCoordinateTransformationMagic(corners);
    }

    


    private void PlaceCornerMarkers(List<Transform> corners)
    {
        UnparentCorners(corners);
        float yOffset = Chessboard.Instance.GetYOffset();
        boardCorner_botLeft.position = Vector3.zero + Vector3.up * yOffset;
        boardCorner_botRight.position = new Vector3(boardSize.x * tileSize, yOffset, 0f);
        boardCorner_topLeft.position = new Vector3(0f, yOffset, boardSize.y * tileSize);
        boardCorner_topRight.position = new Vector3(boardSize.x * tileSize, yOffset, boardSize.y * tileSize);
    }

    private void UnparentCorners(List<Transform> corners)
    {
        foreach (Transform t in corners)
        {
            t.SetParent(transform.parent.GetChild(1));
            t.rotation = Quaternion.identity;
        }
    }

    private void ParentCornersToCamera()
    {
        boardCorner_botLeft.SetParent(transform, true);
        boardCorner_botRight.SetParent(transform, true);
        boardCorner_topLeft.SetParent(transform, true);
        boardCorner_topRight.SetParent(transform, true);
        boardCorner_botLeft.localRotation = Quaternion.identity;
        boardCorner_botRight.localRotation = Quaternion.identity;
        boardCorner_topLeft.localRotation = Quaternion.identity;
        boardCorner_topRight.localRotation = Quaternion.identity;
    }

    void DoDaCoordinateTransformationMagic(List<Transform> corners)
    {
        // Find the min and max values for each axis
        Tuple<Transform, float> minX = null;
        Tuple<Transform, float> maxX = null;
        Tuple<Transform, float> minY = null;
        Tuple<Transform, float> maxY = null;
        foreach (var item in corners)
        {
            if (minX == null || item.localPosition.x < minX.Item2)
                minX = new Tuple<Transform, float>(item, item.localPosition.x);

            if (maxX == null || item.localPosition.x > maxX.Item2)                
                maxX = new Tuple<Transform, float>(item, item.localPosition.x);

            if (minY == null || item.localPosition.y < minY.Item2)
                minY = new Tuple<Transform, float> (item, item.localPosition.y);

            if (maxY == null || item.localPosition.y > maxY.Item2)
                maxY = new Tuple<Transform, float>(item, item.localPosition.y);
        }

        // Center the camera with defined offsets
        maxX = new Tuple<Transform, float>(maxX.Item1, maxX.Item1.localPosition.x + rightOffset);
        minX = new Tuple<Transform, float>(minX.Item1, minX.Item1.localPosition.x - leftOffset);
        maxY = new Tuple<Transform, float>(maxY.Item1, maxY.Item1.localPosition.y + upOffset);
        minY = new Tuple<Transform, float>(minY.Item1, minY.Item1.localPosition.y - botOffset);
        transform.position = new Vector3(maxY.Item1.position.x, maxY.Item1.position.y + upOffset, maxY.Item1.position.z) 
            + (new Vector3(minY.Item1.position.x, minY.Item1.position.y - botOffset, minY.Item1.position.z) - new Vector3(maxY.Item1.position.x, maxY.Item1.position.y + upOffset, maxY.Item1.position.z)) * 0.5f;

        // Calculate the camera size
        float x = maxX.Item2 - minX.Item2;
        float y = maxY.Item2 - minY.Item2;
        float cameraHeight = 2f * cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;
        if (cameraWidth / cameraHeight >= x / y)
        {
            if (cameraHeight > y)
            {
                while (cameraHeight > y)
                {
                    cam.orthographicSize -= zoomIncrement;
                    cameraHeight = 2f * cam.orthographicSize;
                    if (cam.orthographicSize <= 0)
                        break;
                }
            }
            else if (cameraHeight < y)
            {
                while (cameraHeight < y)
                {
                    cam.orthographicSize += zoomIncrement;
                    cameraHeight = 2f * cam.orthographicSize;
                    if (cam.orthographicSize >= 500)
                        break;
                }
            }
        }

        else if (cameraWidth / cameraHeight <= x / y)
        {
            if (cameraWidth > x)
            {
                while (cameraWidth > x)
                {
                    cam.orthographicSize += zoomIncrement;
                    cameraHeight = 2f * cam.orthographicSize;
                    cameraWidth = cameraHeight * cam.aspect;
                    if (cam.orthographicSize >= 500)
                        break;
                }
            }
            else if (cameraWidth < x)
            {
                while (cameraWidth < x)
                {
                    cam.orthographicSize += zoomIncrement;
                    cameraHeight = 2f * cam.orthographicSize;
                    cameraWidth = cameraHeight * cam.aspect;
                    if (cam.orthographicSize <= 0)
                        break;
                }
            }
        }

        UnparentCorners(corners);
        transform.position -= transform.forward * cam.orthographicSize + transform.forward * cameraDistanceMultiplier;
    }
}
