using UnityEngine;

public class PlacementPlant : Placement
{
    [SerializeField]
    private Card plantedSeed;
    public Card GetPlantedSeed() { return plantedSeed; }
    public void SetPlantedSeed(Card plantedSeed) { this.plantedSeed = plantedSeed; }
}