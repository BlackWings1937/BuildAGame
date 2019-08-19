using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPrefab : MonoBehaviour
{
    public GameObject PrefabTest;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("create",3);
    }
    public void create() {

        for (int i = 0;i<5;i++) {
            for (int z = 0;z<5;z++) {
                var g = GameObject.Instantiate(PrefabTest);

                g.transform.position = new Vector3(i*10,z*10,10);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
