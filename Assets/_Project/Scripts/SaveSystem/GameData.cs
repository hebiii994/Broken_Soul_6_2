[System.Serializable]
public class GameData
{
    //variabili da salvare
    public int playerMaxHealth;
    public int playerMaxNostalgia;
    public string inkStoryState;
    public bool hasCompletedIntro;
    public string lastCheckpointId;
    public bool hasDefeatedFirstBoss;

    public GameData()
    {
        this.playerMaxHealth = 5; 
        this.playerMaxNostalgia = 100;
        this.inkStoryState = "";
        this.hasCompletedIntro = false;
        this.lastCheckpointId = "CorridorStartPosition";
        this.hasDefeatedFirstBoss = false;
    }
}
