using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    public List<GameObject> objects;
    public List<KeyCode> keys;

    private int k = 0;

    private void Start()
    {
        foreach (GameObject obj in objects) {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        
        foreach (GameObject obj in objects) {
            if (objects[k] == obj) {
                if (!obj.activeSelf) {
                    obj.SetActive(true);
                }
            } else {
                if (obj.activeSelf) {
                    obj.SetActive(false);
                }
            }
        }

        for (int i = 0; i < keys.Count; i++) {
            if (Input.GetKeyDown(keys[i]))
            {
                // Debug.Log(keys[i]);
                // objects[i].SetActive(true);
                k = i;
            }
        }
    }
}
