using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float scale = 2f;

    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public LODInfo[] detailLevels;
    public static float maxViewDist;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPos;
    Vector2 viewerPosOld;

    static NoiseMapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInViewDist;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleUpdate = new List<TerrainChunk>();

    private void Start()
    {
        mapGenerator = FindObjectOfType<NoiseMapGenerator>();

        maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
        chunkSize = NoiseMapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
        
        // Doing once at the start incase the if is not true to prevent errors in the code
        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z) / scale;
        if ((viewerPosOld - viewerPos).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPosOld = viewerPos;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleUpdate.Count; i++)
        {
            terrainChunksVisibleUpdate[i].SetVisible(false);
        }

        terrainChunksVisibleUpdate.Clear();


        int currentChunkCoordX = Mathf.RoundToInt(viewerPos.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPos.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    // removing this section of code as it causes a bug where pieces of terrain remain in the world outside the players view distance
                    /*if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }*/
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MapData mapData;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        LODMesh collisionLODMesh;
        bool mapDataReceived;

        int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;

            meshObject.layer = LayerMask.NameToLayer("Ground");

            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];

            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                if (detailLevels[i].useForCollider)
                {
                    collisionLODMesh = lodMeshes[i];
                }
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);

        }

        void OnMapDataReceived(MapData mapData)
        {
            // Reason you dont just fetch the mesh data is so that when you calculate for LOD you do so only when needed
            // mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
            // Debug.Log("Map Data Received");
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGen.TextureFromColourMap(mapData.colourMap, NoiseMapGenerator.mapChunkSize, NoiseMapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }

        /*void OnMeshDataReceived(MeshData meshData)
        {
            Debug.Log("Received mesh filter");
            meshFilter.mesh = meshData.CreateMesh();
        }*/

        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDistFromEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
                bool visible = viewerDistFromEdge <= maxViewDist;

                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistFromEdge > detailLevels[i].visibleDistThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            // receieve the mesh
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasBeenRequested)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                    if (lodIndex == 0)
                    {
                        if (collisionLODMesh.hasMesh)
                        {
                            meshCollider.sharedMesh = collisionLODMesh.mesh;
                        }
                        else if (!collisionLODMesh.hasBeenRequested)
                        {
                            collisionLODMesh.RequestMesh(mapData);
                        }
                    }

                    terrainChunksVisibleUpdate.Add(this);
                }
                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    // This class is responsible for fetching it's own mesh from the map generator
    class LODMesh
    {
        public Mesh mesh;
        public bool hasBeenRequested;
        public bool hasMesh;
        int lod;

        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasBeenRequested = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistThreshold;
        public bool useForCollider;
    }
}
