using UnityEngine;
using UnityEngine.UI;

public class PlacementPlant : Placement
{
    [SerializeField]
    private Card plantedSeed;
    public Card GetPlantedSeed() { return plantedSeed; }
    public void SetPlantedSeed(Card plantedSeed) { this.plantedSeed = plantedSeed; }

    [SerializeField]
    private int growthStage = 0;
    public int GetGrowthStage() { return growthStage; }

    [SerializeField]
    private GameObject cropObject;

    private float tickTimer = 0;

    void Update()
    {
        if (tickTimer > GameManager.instance.GetPlantGrowthTickDuration())
        {
            CheckForPlantGrowth();
            tickTimer = 0;
        }
        else
        {
            tickTimer += Time.deltaTime;
        }
    }

    private void CheckForPlantGrowth()
    {
        if (growthStage == 4) { return; }

        float roll = Random.Range(0, 100);
        switch (plantedSeed.GetCardRarity())
        {
            case Rarity.Common:
                // Do nothing
                break;
            case Rarity.Uncommon:
                roll *= 0.9f;
                break;
            case Rarity.Rare:
                roll *= 0.75f;
                break;
            case Rarity.Legendary:
                roll *= 0.5f;
                break;
            case Rarity.Mythical:
                roll *= 0.25f;
                break;
            default:
                break;
        }
        if (roll > GameManager.instance.GetPlantBaseGrowthChangePercentage()) { return; }

        GameObject meshObject = transform.Find("Mesh").gameObject;
        SpriteRenderer spriteOne = meshObject.transform.Find("Sprite_0").gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer spriteTwo = meshObject.transform.Find("Sprite_1").gameObject.GetComponent<SpriteRenderer>();

        growthStage++;
        Sprite sprite = Resources.Load<Sprite>("Textures/growth-stage-" + growthStage);
        spriteOne.sprite = sprite;
        spriteTwo.sprite = sprite;

        if (growthStage == 4)
        { // Spawn the crop
            GameObject newCropObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/Map/CropPrefab"),
                transform.position,
                Quaternion.identity
            );
            cropObject = newCropObject;

            Card cropCard = GameManager.instance.GetCropCardBySeedCardId(plantedSeed.GetCardId());
            SpriteRenderer cropSprite = newCropObject.transform.Find("Mesh").Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
            cropSprite.sprite = Resources.Load<Sprite>("Textures/" + cropCard.GetCardTexture().name);
        }
    }

    public bool IsFinishedGrowing()
    {
        bool isFinished = true;

        if (growthStage != 4) { isFinished = false; }

        return isFinished;
    }

    void OnDestroy()
    {
        if (cropObject != null) { Destroy(cropObject); }
    }
}