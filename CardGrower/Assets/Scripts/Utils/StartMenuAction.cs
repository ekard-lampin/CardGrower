using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StartMenuAction : MonoBehaviour
{
    private float actionTimer = 0;
    private float actionDuration = 2f;
    private bool rising = false;
    private Vector3 lowerPosition = new Vector3(0, -0.25f, 0);
    private Vector3 upperPosition = new Vector3(0, 0.25f, 0);

    private float rotation = 0;

    private GameObject lastHitObject = null;

    [SerializeField]
    private List<Sprite> cardSprites = new List<Sprite>();

    void Start()
    {
        lowerPosition += transform.position;
        upperPosition += transform.position;

        foreach (Card card in GameManager.instance.GetToolCards()) { cardSprites.Add(Resources.Load<Sprite>("Textures/" + card.GetCardTexture().name)); }
        foreach (Card card in GameManager.instance.GetSeedCards()) { cardSprites.Add(Resources.Load<Sprite>("Textures/" + card.GetCardTexture().name)); }
        foreach (Card card in GameManager.instance.GetBoosterCards()) { cardSprites.Add(Resources.Load<Sprite>("Textures/" + card.GetCardTexture().name)); }
        foreach (Card card in GameManager.instance.GetCropCards()) { cardSprites.Add(Resources.Load<Sprite>("Textures/" + card.GetCardTexture().name)); }
    }

    void Update()
    {
        actionTimer += Time.deltaTime;

        // Bob
        float ratio = Mathf.SmoothStep(0, 1, Mathf.Clamp(actionTimer / actionDuration, 0, 1));
        Vector3 startPosition = rising ? lowerPosition : upperPosition;
        Vector3 targetPosition = rising ? upperPosition : lowerPosition;

        transform.position = Vector3.Lerp(startPosition, targetPosition, ratio);

        // Rotate
        rotation += Time.deltaTime * 50;
        transform.localRotation = Quaternion.Euler(new Vector3(0, rotation, 0));

        if (Physics.Raycast(Camera.main.transform.position, transform.position - Camera.main.transform.position, out RaycastHit hit, 100))
        {
            if (lastHitObject == null) { lastHitObject = hit.collider.gameObject; }
            if (lastHitObject != hit.collider.gameObject)
            {
                int randomIndex = Random.Range(0, cardSprites.Count);
                transform.Find("Mesh").gameObject.GetComponent<SpriteRenderer>().sprite = cardSprites[randomIndex];
            }
            lastHitObject = hit.collider.gameObject;
        }

        if (actionTimer < actionDuration) { return; }

        actionTimer = 0;
        rising = !rising;
    }
}