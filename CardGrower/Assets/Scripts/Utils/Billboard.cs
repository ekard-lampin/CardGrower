using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null) { return; }

        Vector3 lookTarget = GameObject.FindGameObjectWithTag("Player").transform.position;
        lookTarget.y = 0;
        transform.LookAt(lookTarget);
    }
}