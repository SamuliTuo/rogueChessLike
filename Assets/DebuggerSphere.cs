using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggerSphere : MonoBehaviour
{
    public float lifeTime;
    void Start()
    {
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
