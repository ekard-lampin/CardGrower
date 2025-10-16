using System.Collections.Generic;
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

    [SerializeField]
    private List<Card> activeBoosters = new List<Card>();
    public bool DoesPlantHaveBooster(Card booster)
    {
        foreach (Card activeBooster in activeBoosters)
        {
            if (activeBooster.GetCardId().Equals(booster.GetCardId())) { return true; }
        }
        return false;
    }
    public void AddBooster(Card booster) {
        activeBoosters.Add(booster);
        Destroy(
            Instantiate(
                Resources.Load<GameObject>("Prefabs/Map/BoosterSfxPrefab"),
                transform.position,
                Quaternion.identity
            ),
            1
        );
    }
    public bool IsBoosterActive(CardId cardId) {
        foreach (Card activeBooster in activeBoosters)
        {
            if (activeBooster.GetCardId().Equals(cardId)) { return true; }
        }
        return false;
    }
    public void RemoveBooster(CardId cardId)
    {
        int boosterToRemove = -1;
        for (int boosterIndex = 0; boosterIndex < activeBoosters.Count; boosterIndex++)
        {
            if (activeBoosters[boosterIndex].GetCardId().Equals(cardId)) { boosterToRemove = boosterIndex; }
        }
        if (boosterToRemove == -1) { return; }
        activeBoosters.RemoveAt(boosterToRemove);
    }

    private float tickTimer = 0;

    void Update()
    {
        if (!TutorialManager.instance.IsTutorialFlagSet(TutorialState.Growing)) { return; }
        
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

        // If booster is active, skip grow check and grow one stage.
        if (IsBoosterActive(CardId.Fertilizer))
        {
            RemoveBooster(CardId.Fertilizer);
        }
        else
        {
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
            if (IsBoosterActive(CardId.WaterBucket) || IsBoosterActive(CardId.GrowthHormone)) { roll *= 1.5f; }
            if (roll > GameManager.instance.GetPlantBaseGrowthChangePercentage()) { return; }
            if (IsBoosterActive(CardId.WaterBucket)) { RemoveBooster(CardId.WaterBucket); }
        }

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

            TutorialManager.instance.UpdateTrackedGrowingAction();
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