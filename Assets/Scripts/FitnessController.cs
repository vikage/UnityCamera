using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessController : MonoBehaviour
{
    public GameObject square;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 worldSize = Utils.WorldSize();
        Debug.Log("World size " + worldSize);

        // square.transform.position = new Vector2(0, 0);
        // square.transform.localScale = new Vector3(worldSize.x, worldSize.y, 1);

        // Jump
        // square.transform.position = new Vector2(0, -worldSize.y / 4 - worldSize.y / 8);
        // square.transform.localScale = new Vector3(worldSize.x, worldSize.y / 4, 1);

        // Lie
        square.transform.position = new Vector2(0, worldSize.y / 8);
        square.transform.localScale = new Vector3(worldSize.x, 3 * worldSize.y / 4, 1);
    }
}
