using UnityEngine;

public class RotateAndBob : MonoBehaviour
{
    private float actionTimer = 0;
    private float actionDuration = 2f;
    private Vector3 lowerPosition = Vector3.zero;
    private Vector3 upperPosition = Vector3.zero;
    private bool rising = false;

    void Start()
    {
        Vector3 startingPosition = transform.Find("Mesh").localPosition;

        lowerPosition = new Vector3(0, startingPosition.y - 0.05f, 0);

        upperPosition = new Vector3(0, startingPosition.y + 0.05f, 0);
    }

    void Update()
    {
        actionTimer += Time.deltaTime;

        Vector3 startingPosition = rising ? lowerPosition : upperPosition;
        Vector3 targetPosition = rising ? upperPosition : lowerPosition;
        float ratio = actionTimer / actionDuration;

        transform.Find("Mesh").localPosition = Vector3.Lerp(startingPosition, targetPosition, ratio);

        transform.Find("Mesh").rotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 360, 0), ratio));

        if (actionTimer >= actionDuration)
        {
            actionTimer = 0;
            rising = !rising;
        }
    }
}