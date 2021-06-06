using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientToSurface : MonoBehaviour {

    [SerializeField] Transform[] samplePositions;

    void OrientToTerrainHeight () {
        Terrain terrain = Terrain.activeTerrain;

        if (samplePositions.Length > 0) {
            Vector3 averagePos = Vector3.zero;
            Vector3 averageDirection = Vector3.zero;
            Vector3 previousPos = Vector3.zero;
            for (var i = 0; i < samplePositions.Length; i++) {
                Vector3 samplePos = samplePositions[i].position;
                samplePos.y = terrain.SampleHeight (samplePos);
                averagePos += samplePos;

                if (i > 0) {
                    averageDirection += samplePos - previousPos;
                }

                previousPos = samplePos;
            }

            averagePos /= samplePositions.Length;
            averageDirection /= samplePositions.Length - 1;

            transform.position = new Vector3 (transform.position.x, averagePos.y, transform.position.z);
            Debug.DrawRay (transform.position, averageDirection, Color.blue, 10);

            float angle = Vector3.SignedAngle (transform.right, averageDirection, transform.forward);
            transform.Rotate (Vector3.forward * angle);
        }
    }
    void PositionOnTerrain () {
        Terrain terrain = Terrain.activeTerrain;
        transform.position = new Vector3 (transform.position.x, terrain.SampleHeight (transform.position), transform.position.z);
    }

}