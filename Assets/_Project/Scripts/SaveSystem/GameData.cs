[System.Serializable]
public class GameData
{
    // Aggiungere qui tutte le variabili da salvare
    public int playerMaxHealth;
    public int playerMaxNostalgia;

    
    public GameData()
    {
        this.playerMaxHealth = 5; 
        this.playerMaxNostalgia = 100; 
    }
}
