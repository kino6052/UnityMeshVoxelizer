using System.Collections.Generic;
using UnityEngine;

public class VoxelizedMesh : MonoBehaviour
{
    public List<Vector3Int> GridPoints = new List<Vector3Int>();
    public float HalfSize = 0.1f;
    public Vector3 LocalOrigin;

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

        if (!meshFilter.TryGetComponent(out VoxelizedMesh voxelizedMesh))
        {
            voxelizedMesh = meshFilter.gameObject.AddComponent<VoxelizedMesh>();
        }

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
                    if (Physics.CheckBox(pos, new Vector3(halfSize, halfSize, halfSize)))
                    {
                        voxelizedMesh.GridPoints.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }

    public void Start() {
        var voxelizedMesh = gameObject.GetComponent<VoxelizedMesh>();
        if (voxelizedMesh.TryGetComponent(out MeshFilter meshFilter))
        {
            VoxelizedMesh.VoxelizeMesh(meshFilter);
        }

        float size = voxelizedMesh.HalfSize * 2f;

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