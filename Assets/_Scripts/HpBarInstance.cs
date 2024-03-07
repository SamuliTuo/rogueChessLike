using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class HpBarInstance : MonoBehaviour
{
    public void SetPool(IObjectPool<GameObject> pool) => this.pool = pool;
    public Transform lineBounds_left, lineBounds_right;
    public float upOffset = -0.5f;
    public BattleUI_SkillSymbols skillSymbols;

    public int maxLineCount = 100;
    [SerializeField] GameObject smallLine = null;
    [SerializeField] GameObject bigLine = null;

    private Transform cam, unit;
    [SerializeField] private GameObject barObj = null;
    [SerializeField] private GameObject bgObj = null;
    [SerializeField] private Image hpBar = null;

    private float posOffset;
    private RectTransform canvas;
    private IObjectPool<GameObject> pool;
    private bool initialized = false;
    private List<GameObject> lines = new List<GameObject>();

    [SerializeField] Image slider = null;
    [SerializeField] float SliderDuration = 0;
    private Coroutine sliderRoutine;

    void Update()
    {
        if (initialized)
        {
            canvas.position = unit.position + Vector3.up * posOffset;
            canvas.LookAt(cam.position + cam.rotation * Vector3.back, cam.rotation * Vector3.up + Vector3.up * upOffset);
        }
    }

    public void Init(Transform _unit, float _posOffset, int team, float hp)
    {
        skillSymbols = GetComponent<BattleUI_SkillSymbols>();
        if (canvas == null)
            canvas = GetComponent<RectTransform>();

        barObj.SetActive(true);
        if (team == 0)
            barObj.GetComponent<Image>().color = GameManager.Instance.hpBarTeam0Color;
        else
            barObj.GetComponent<Image>().color = GameManager.Instance.hpBarTeam1Color;

        bgObj.SetActive(true);
        SetBarMaxValue(hp);
        SetBarValue(1);
        cam = Camera.main.transform;
        unit = _unit;
        posOffset = _posOffset;
        initialized = true;
    }


    public Vector3 GetBarHpPos() 
    {
        return Vector3.Lerp(lineBounds_right.position, lineBounds_left.position, hpBar.fillAmount);
    }
    public void SetBarMaxValue(float hp) 
    {
        float count = 0;
        bool underLimit = true;
        // count the lines:
        while (true)
        {
            for (int i = 0; i < 4; i++) 
            {
                count += 20;
                if (count > hp)
                    break;

                var smallClone = Instantiate(smallLine, transform);
                smallClone.SetActive(true);
                lines.Add(smallClone);
            }

            count += 20;
            if (count > hp)
                break;

            var clone = Instantiate(bigLine, transform);
            clone.SetActive(true);
            lines.Add(clone);

            if (count >= maxLineCount * 20)
                break;
        }

        // place the lines:
        for (int i = 0; i < lines.Count; i++)
        {
            float perc = (20 + 20 * i ) / hp;   
            lines[i].transform.position = Vector3.Lerp(
                new Vector3(lineBounds_right.position.x, lines[i].transform.position.y, lineBounds_right.position.z),
                new Vector3(lineBounds_left.position.x, lines[i].transform.position.y, lineBounds_left.position.z),
                perc);
        }
    }

    public void SetBarValue(float perc)
    {
        float oldValue = hpBar.fillAmount;
        hpBar.fillAmount = perc;
        if (oldValue > perc && sliderRoutine == null)
        {
            sliderRoutine = StartCoroutine(SliderCoroutine());
        }
    }

    // Slide the other bar behind HP bar with a delay:
    IEnumerator SliderCoroutine ()
    {
        float startPerc = slider.fillAmount;

        float t = 0;
        while (t < SliderDuration)
        {
            float perc = t / SliderDuration;
            perc = perc * perc * perc;
            slider.fillAmount = Mathf.Lerp(startPerc, 0, perc);

            if (slider.fillAmount <= 0 || slider.fillAmount <= hpBar.fillAmount)
            {
                break;
            }
            t += Time.deltaTime;
            yield return null;
        }
        sliderRoutine = null;
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        unit = null;
        initialized = false;

        if (pool != null)
            pool.Release(this.gameObject);
        else
            Destroy(gameObject);
    }

    //hpScript.ScaleHpBar(Vector3.Lerp(hpBarOriginalScale + hpBarOriginalScale * (damagePerc * hpBarMaxBiggenPerc), hpBarOriginalScale, perc));
    public void ScaleHpBar(Vector3 targetScale)
    {
        if (barObj != null) barObj.transform.localScale = targetScale;
        if (bgObj != null) bgObj.transform.localScale = targetScale;
        if (slider != null) slider.transform.localScale = targetScale;
    }
}
