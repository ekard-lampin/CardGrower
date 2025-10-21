using System;

[Serializable]
public class SaveObject
{
    public int seed;
    public int GetSeed() { return seed; }
    public void SetSeed(int seed) { this.seed = seed; }

    public SavePlayerObject savePlayerObject;
    public SavePlayerObject GetSavePlayerObject() { return savePlayerObject; }
    public void SetSavePlayerObject(SavePlayerObject savePlayerObject) { this.savePlayerObject = savePlayerObject; }

    public SaveTile[] saveTiles;
    public SaveTile[] GetSaveTiles() { return saveTiles; }
    public void SetSaveTiles(SaveTile[] saveTiles) { this.saveTiles = saveTiles; }
}