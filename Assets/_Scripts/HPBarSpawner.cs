using UnityEngine;
using UnityEngine.Pool;

public class HPBarSpawner : MonoBehaviour
{
    private GameObject hpBarPrefab;
    private IObjectPool<GameObject> barPool;
    [SerializeField] private Vector3 originalScale;
    
    private void Start()
    {
        hpBarPrefab = Resources.Load<GameObject>("HpBarInstance");
        Reset();
    }

    public void Reset()
    {
        barPool = new ObjectPool<GameObject>(CreateBar, OnTakeBarFromPool, OnReturnBarToPool);
    }

    public GameObject SpawnBar()
    {
        var r = barPool.Get();
        r.transform.localScale = originalScale;
        return r;
    }


    GameObject CreateBar()
    {
        var instance = Instantiate<GameObject>(hpBarPrefab);
        instance.GetComponent<HpBarInstance>().SetPool(barPool);
        return instance;
    }
    void OnTakeBarFromPool(GameObject bar)
    {
        bar.SetActive(true);
    }
    void OnReturnBarToPool(GameObject bar)
    {
        bar.SetActive(false);
    }
}
