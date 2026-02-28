using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxelizer : MonoBehaviour
{
    public GameObject entity;
    public GameObject prefab;
    public int level = 1;
    public int numberOfCubes = 0;
    public float totalVolume = 0;

    public static bool IsInside(Vector3 position, Vector3 size, float pointSize = 0.001f) {
        // Get the eight vertices of the box
        Vector3[] boxVertices = new Vector3[8];
        Vector3 center = position;
        Vector3 halfExtents = size / 2f;

        boxVertices[0] = center + new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
        boxVertices[1] = center + new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
        boxVertices[2] = center + new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
        boxVertices[3] = center + new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        boxVertices[4] = center + new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
        boxVertices[5] = center + new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
        boxVertices[6] = center + new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
        boxVertices[7] = center + new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);

        int count = 0;

        // Check if all box vertices are inside the collider
        for (int i = 0; i < 8; i++)
        {
            if (!Physics.CheckBox(boxVertices[i], new Vector3(pointSize, pointSize, pointSize)))
            {
                count += 1;
            }
        }

        if (count < 3) {
            return true;
        }

        return false;
    }

    public List<GameObject> Subdivide(GameObject cube) {
        // Get the bounds of the parent cube
        Bounds bounds = cube.GetComponent<Renderer>().bounds;


        // Calculate the center of the parent cube
        Vector3 center = bounds.center;

        // Calculate the half-extents of the parent cube
        float halfExtents = 0.5f;

        Vector3 _halfExtents = bounds.extents / 2f;

        List<GameObject> cubes = new List<GameObject>();

        // Create eight child cubes
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    // Create a new child cube
                    GameObject childCube = Instantiate(prefab, cube.transform);
                    numberOfCubes += 1;

                    // Position the child cube relative to the parent cube
                    Vector3 position = center + new Vector3(x * _halfExtents.x, y * _halfExtents.y, z * _halfExtents.z);
                    childCube.transform.position = position;

                    // Scale the child cube to match the size of the parent cube
                    childCube.transform.localScale = new Vector3(halfExtents, halfExtents, halfExtents);
                    
                    cubes.Add(childCube);
                    childCube.transform.parent = cube.transform;
                }
            }
        }

        // Turn off renderer
        Voxelizer.TurnOffRenderer(cube);

        return cubes;
    }

    public void RecursiveSubdivide(List<GameObject> cubes, int level = 0) {
        if (level <= 0) return;

        foreach (GameObject cube in cubes)
        {
            Bounds bounds = cube.GetComponent<Renderer>().bounds;
            Vector3 size = bounds.extents;

            bool isInside = Voxelizer.IsInside(cube.transform.position, size);
            bool isIntersecting = Physics.CheckBox(cube.transform.position, size);
            
            if (!isIntersecting || isInside) {
                Destroy(cube);
                numberOfCubes -= 1;

                continue;
            }

            if (level > 1) {
                List<GameObject> _cubes = Subdivide(cube);

                RecursiveSubdivide(_cubes, level-1);
            }
        }

        return;
    }

    public GameObject PlaceInitialCube() {
        GameObject cube = Instantiate(prefab);

        // Get the renderer component of the object
        Renderer renderer = entity.GetComponent<Renderer>();

        // Get the bounding box of the object
        Bounds bounds = renderer.bounds;
        
        cube.transform.position = new Vector3(0f, 0f, 0f);

        // Set the position of the cube to the center of the bounding box
        cube.transform.position = bounds.center;

        // Set the size of the cube to match the size of the bounding box
        Vector3 vec = bounds.size;  
        float maxComponent = Mathf.Max(vec.x, Mathf.Max(vec.y, vec.z));

        cube.transform.localScale = new Vector3(maxComponent, maxComponent, maxComponent);

        return cube;
    }

    public static void TurnOffRenderer(GameObject obj) {
        try {
            Renderer renderer = obj.GetComponent<Renderer>();
            MeshCollider collider = obj.GetComponent<MeshCollider>();
            renderer.enabled = false;
            collider.enabled = false;
        } catch {
            Debug.Log("Could not turn off renderer");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject cube = PlaceInitialCube();

        List<GameObject> _cubes = new List<GameObject>();
        _cubes.Add(cube); 

        RecursiveSubdivide(_cubes, level);

        float _s = 2.0f / Mathf.Pow(2.0f, level);
        totalVolume = Mathf.Pow(_s, 3) * numberOfCubes;
    }

    // Update is called once per frame
    void Update()
    {

    }
}