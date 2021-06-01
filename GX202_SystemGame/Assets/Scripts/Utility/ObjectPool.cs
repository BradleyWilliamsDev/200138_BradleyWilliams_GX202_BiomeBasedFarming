using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour {

    [SerializeField] GameObject opPrefab;
    [SerializeField] bool singleton = true;
    Queue<T> pool = new Queue<T> ();
    public static ObjectPool<T> instance;

    protected internal UnityEvent<T> OnGetFromPool = new UnityEvent<T> ();

    protected virtual void Awake () {
        if (singleton) instance = this;
    }

    public T GetFromObjectPool () {
        T opObject = default (T);

        if (pool.Count > 0) {
            opObject = pool.Dequeue ();
        } else {
            opObject = Instantiate (opPrefab, transform).GetComponent<T> ();
        }
        opObject.gameObject.SetActive (true);

        OnGetFromPool.Invoke (opObject);

        return opObject;
    }

    public void PutIntoObjectPool (T opObject) {
        pool.Enqueue (opObject);
        opObject.transform.SetParent (transform, false);
        opObject.gameObject.SetActive (false);
    }

}