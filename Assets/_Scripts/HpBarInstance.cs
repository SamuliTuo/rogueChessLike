using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class HpBarInstance : MonoBehaviour
{
    public void SetPool(IObjectPool<GameObject> pool) => this.pool = pool;

    private Transform cam, unit;
    private GameObject barObj, bgObj;
    private float posOffset;
    private RectTransform canvas;
    private Image bar;
    private IObjectPool<GameObject> pool;
    private bool initialized = false;


    void Update()
    {
        if (initialized)
        {
            canvas.position = unit.position + Vector3.up * posOffset;
            canvas.LookAt(cam.position + cam.rotation * Vector3.back, cam.rotation * Vector3.up + Vector3.up * 0.33f);
        }
    }



    public void Init(Transform _unit, float _posOffset)
    {
        if (canvas == null)
            canvas = GetComponent<RectTransform>();
        if (bgObj == null)
            bgObj = transform.GetChild(0).gameObject;
        if (barObj == null)
            barObj = transform.GetChild(1).gameObject;
        if (bar == null)
            bar = barObj.GetComponent<Image>();

        barObj.SetActive(false);
        bgObj.SetActive(false);
        cam = Camera.main.transform;
        unit = _unit;
        posOffset = _posOffset;
        initialized = true;
    }

    public void SetBarValue(float perc)
    {
        if (barObj.activeSelf == false && perc < 1)
        {
            barObj.SetActive(true);
            bgObj.SetActive(true);
        }
        bar.fillAmount = perc;
    }

    public void Deactivate()
    {
        unit = null;
        initialized = false;

        if (pool != null)
            pool.Release(this.gameObject);
        else
            Destroy(gameObject);
    }
}
