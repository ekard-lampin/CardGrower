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
    private List<CardId> activeBoosters = new List<CardId>();
    public List<CardId> GetActiveBoosters() { return activeBoosters; }
    public bool DoesPlantHaveBooster(CardId booster)
    {
        foreach (CardId activeBooster in activeBoosters)
        {
            if (activeBooster.Equals(booster)) { return true; }
        }
        return false;
    }
    public void AddBooster(CardId booster) {
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
        foreach (CardId activeBooster in activeBoosters)
        {
            if (activeBooster.Equals(cardId)) { return true; }
        }
        return false;
    }
    public void RemoveBooster(CardId cardId)
    {
        int boosterToRemove = -1;
        for (int boosterIndex = 0; boosterIndex < activeBoosters.Count; boosterIndex++)
        {
            if (activeBoosters[boosterIndex].Equals(cardId)) { boosterToRemove = boosterIndex; }
        }
        if (boosterToRemove == -1) { return; }
        activeBoosters.RemoveAt(boosterToRemove);
    }

    private float tickTimer = 0;

    [SerializeField]
    private float timeSaverFactor = 0;

    void Update()
    {
        if (!TutorialManager.instance.IsTutorialFlagSet(TutorialState.Growing)) { return; }
        if (PlayerViewState.Dialogue.Equals(GameManager.instance.GetPlayerViewState())) { return; }
        
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
        else if (TutorialState.Growing.Equals(TutorialManager.instance.GetTutorialState())) {
            // Skip grow check.
        }
        else
        {
            float roll = Random.Range(0f, GameManager.instance.GetPlantGrowthRollRange());
            float timeSave = timeSaverFactor;
            switch (plantedSeed.GetCardRarity())
            {
                case Rarity.Common:
                    // Do nothing
                    break;
                case Rarity.Uncommon:
                    roll *= 1.1f;
                    timeSave *= 0.9f;
                    break;
                case Rarity.Rare:
                    roll *= 1.25f;
                    timeSave *= 0.75f;
                    break;
                case Rarity.Legendary:
                    roll *= 1.5f;
                    timeSave *= 0.5f;
                    break;
                case Rarity.Mythical:
                    roll *= 1.75f;
                    timeSave *= 0.25f;
                    break;
                default:
                    break;
            }
            roll -= timeSave;
            if (IsBoosterActive(CardId.WaterBucket) || IsBoosterActive(CardId.GrowthHormone)) { roll *= 0.5f; }
            if (roll > GameManager.instance.GetPlantBaseGrowthChangePercentage()) { timeSaverFactor += GameManager.instance.GetPlantTimeSaverFactor(); return; }
            if (IsBoosterActive(CardId.WaterBucket)) { RemoveBooster(CardId.WaterBucket); }
        }

        growthStage++;
        timeSaverFactor = 0;

        Material plantMaterial = Resources.Load<Material>("Materials/growth-stage-" + growthStage);
        foreach (Transform childMesh in transform.Find("Mesh"))
        {
            foreach (Transform meshTransform in childMesh)
            {
                meshTransform.gameObject.GetComponent<MeshRenderer>().material = plantMaterial;
            }
        }

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