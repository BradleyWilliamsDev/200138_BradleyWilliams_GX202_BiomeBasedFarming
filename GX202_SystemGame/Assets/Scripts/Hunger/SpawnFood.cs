using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public GameObject foodPrefab;

    public Vector3 centre;
    public Vector3 size;

    private void Start() {
        for (int i = 0; i < 100; i++)
        {
            SpawnTheFood();
        }
    }

    public void SpawnTheFood()
    {
        Vector3 pos = centre + new Vector3(Random.Range(-size.x / 2, size.x / 2),Random.Range(-size.y / 2, size.y / 2),Random.Range(-size.z / 2, size.z / 2));

        Instantiate(foodPrefab, pos, Quaternion.identity);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.localPosition + centre, size);
    }
}
