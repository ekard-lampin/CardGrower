using System;

[Serializable]
public class SaveTile
{
    public int xCoord;
    public int GetXCoord() { return xCoord; }
    public void SetXCoord(int xCoord) { this.xCoord = xCoord; }

    public int zCoord;
    public int GetZCoord() { return zCoord; }
    public void SetZCoord(int zCoord) { this.zCoord = zCoord; }

    public TileState tileState;
    public TileState GetTileState() { return tileState; }
    public void SetTileState(TileState tileState) { this.tileState = tileState; }

    public CardId plantedSeed;
    public CardId GetPlantedSeed() { return plantedSeed; }
    public void SetPlantedSeed(CardId plantedSeed) { this.plantedSeed = plantedSeed; }

    public int plantGrowthStage;
    public int GetPlantGrowthStage() { return plantGrowthStage; }
    public void SetPlantGrowthStage(int plantGrowthStage) { this.plantGrowthStage = plantGrowthStage; }
}