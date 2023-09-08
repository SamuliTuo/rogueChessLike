using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //rot Vector3(51.3617134,359.853943,0.0316775143)
    //pos Vector3(9.93000031,11.5,-2.5)

    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float offsetZ = -10f;
    [SerializeField] private float offsetY = 5f;
    [SerializeField] private float sceneBuilderOffsetX = -2f;
    private float offsetX = 0;
    private Vector3 startPos = new Vector3(9.93f, 11.5f, -2.5f);
    private Vector3 startRot = new Vector3(51.36f, 359.85f, 0.03f);

    private Coroutine moveCoroutine = null;
    private Vector3 coroutineStartpos;

    private void Start()
    {
        transform.position = startPos;
        transform.rotation = Quaternion.Euler(startRot);
    }

    public void SetCameraForBoardsize(Vector2Int boardSize, float tileSize)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        offsetX = GameManager.Instance.state == GameState.SCENARIO_BUILDER ? sceneBuilderOffsetX : 0f;
        var posX = offsetX + boardSize.x * tileSize * 0.5f;
        var posZ = offsetZ + boardSize.y * tileSize * -0.25f;
        var posY = offsetY + Mathf.Sqrt((boardSize.x * boardSize.x) + (boardSize.y * boardSize.y));
        moveCoroutine = StartCoroutine(MoveCameraTo(new Vector3(posX, posY, posZ)));
    }

    IEnumerator MoveCameraTo(Vector3 newPos)
    {
        coroutineStartpos = transform.position;
        float t = 0;
        float perc;

        while (t < 1)
        {
            perc = t * t * t * (t * (6f * t - 15f) + 10f); //smootherstep
            transform.position = Vector3.Lerp(coroutineStartpos, newPos, perc);
            t += Time.deltaTime * moveSpeed;
            yield return null;
        }
        transform.position = newPos;
    }
}
