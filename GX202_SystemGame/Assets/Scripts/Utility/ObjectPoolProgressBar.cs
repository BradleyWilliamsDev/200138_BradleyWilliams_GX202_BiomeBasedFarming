using System.Collections;
using UnityEngine;

public class ObjectPoolProgressBar : ObjectPool<UIProgressBar> {
    [SerializeField] Transform parent;

    protected override void Awake () {
        base.Awake();
        
        OnGetFromPool.AddListener (GotFromPool);
    }

    void GotFromPool (UIProgressBar uIProgressBar) {
        uIProgressBar.transform.SetParent (parent);
    }

}