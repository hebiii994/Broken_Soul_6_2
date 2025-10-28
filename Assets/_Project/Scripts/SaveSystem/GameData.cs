
[System.Serializable]
public class GameData
{
    //variabili da salvare
    public string playerName;
    public int playerMaxHealth;
    public int playerMaxNostalgia;
    public string inkStoryState;
    public bool hasCompletedIntro;
    public string lastCheckpointId;
    public bool hasDefeatedFirstBoss;
    public SerializableDictionary<string, bool> vhsCollected;
    public SerializableDictionary<string, bool> cassetteCollected;

    public GameData()
    {
        this.playerName = "No Name";
        this.playerMaxHealth = 0; 
        this.playerMaxNostalgia = 0;
        this.inkStoryState = "";
        this.hasCompletedIntro = false;
        this.lastCheckpointId = "CorridorStartPosition";
        this.hasDefeatedFirstBoss = false;
        this.vhsCollected = new SerializableDictionary<string, bool>();
        this.cassetteCollected = new SerializableDictionary<string, bool>();
    }
}
