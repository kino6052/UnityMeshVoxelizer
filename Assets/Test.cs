using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<Vector3Int> GridPoints = new List<Vector3Int>();
    public float HalfSize = 1f;
    public Vector3 LocalOrigin;
    public float PointSize = 0.01f;

    public GameObject prefab;

    public Vector3 PointToPosition(Vector3Int point)
    {
        float size = HalfSize * 2f;
        Vector3 pos = new Vector3(HalfSize + point.x * size, HalfSize + point.y * size, HalfSize + point.z * size);
        return LocalOrigin + transform.TransformPoint(pos);
    }

    public static void VoxelizeMesh(MeshFilter meshFilter)
    {
        if (!meshFilter.TryGetComponent(out MeshCollider meshCollider))
        {
            meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
        }

        if (!meshFilter.TryGetComponent(out Test voxelizedMesh))
        {
            voxelizedMesh = meshFilter.gameObject.AddComponent<Test>();
        }

        Debug.Log(voxelizedMesh.HalfSize);

        Bounds bounds = meshCollider.bounds;
        Vector3 minExtents = bounds.center - bounds.extents;
        float halfSize = voxelizedMesh.HalfSize;
        Vector3 count = bounds.extents / halfSize;

        int xGridSize = Mathf.CeilToInt(count.x);
        int yGridSize = Mathf.CeilToInt(count.y);
        int zGridSize = Mathf.CeilToInt(count.z);

        voxelizedMesh.GridPoints.Clear();
        voxelizedMesh.LocalOrigin = voxelizedMesh.transform.InverseTransformPoint(minExtents);

        for (int x = 0; x < xGridSize; ++x)
        {
            for (int z = 0; z < zGridSize; ++z)
            {
                for (int y = 0; y < yGridSize; ++y)
                {
                    Vector3 pos = voxelizedMesh.PointToPosition(new Vector3Int(x, y, z));
                    if (!Test.IsInside(voxelizedMesh, pos, new Vector3(halfSize, halfSize, halfSize)) && Physics.CheckBox(pos, new Vector3(halfSize, halfSize, halfSize)))
                    {
                        voxelizedMesh.GridPoints.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }

    public static bool IsInside(Test voxelizedMesh, Vector3 position, Vector3 size) {
        // Get the eight vertices of the box
        Vector3[] boxVertices = new Vector3[8];
        Vector3 center = position;
        Vector3 halfExtents = size;

        if (voxelizedMesh.TryGetComponent(out MeshCollider otherCollider))
        {
            boxVertices[0] = center + new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
            boxVertices[1] = center + new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            boxVertices[2] = center + new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
            boxVertices[3] = center + new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
            boxVertices[4] = center + new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
            boxVertices[5] = center + new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            boxVertices[6] = center + new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
            boxVertices[7] = center + new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);

            // Check if all box vertices are inside the collider
            for (int i = 0; i < 8; i++)
            {
                if (!Physics.CheckBox(boxVertices[i], new Vector3(voxelizedMesh.PointSize, voxelizedMesh.PointSize, voxelizedMesh.PointSize)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    void Start() {
        var voxelizedMesh = gameObject.GetComponent<Test>();
        if (voxelizedMesh.TryGetComponent(out MeshFilter meshFilter))
        {
            Test.VoxelizeMesh(meshFilter);
        }

        float size = voxelizedMesh.HalfSize * 2f;

        Debug.Log(size);
        Debug.Log(voxelizedMesh.GridPoints.Count);

        foreach (Vector3Int gridPoint in voxelizedMesh.GridPoints)
        {
            Vector3 worldPos = voxelizedMesh.PointToPosition(gridPoint);
            GameObject cube = Instantiate(prefab);
            cube.transform.position = worldPos;
            cube.transform.localScale = new Vector3(size, size, size);
        }

        // Handles.color = Color.red;
        // if (voxelizedMesh.TryGetComponent(out MeshCollider meshCollider))
        // {
        //     Bounds bounds = meshCollider.bounds;
        //     Handles.DrawWireCube(bounds.center, bounds.extents * 2);
        // }
    }
}
