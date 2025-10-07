using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    [Header("File Configuration")]
    [SerializeField] private string _fileName = "savegame.json";

    private GameData _gameData;
    private List<ISaveable> _saveableObjects;
    private string _filePath;

    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _filePath = Path.Combine(UnityEngine.Application.persistentDataPath, _fileName);
    }

    private void Start()
    {
        this._saveableObjects = FindAllSaveableObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this._gameData = new GameData();
    }

    public void LoadGame()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                string dataAsJson = File.ReadAllText(_filePath);
                _gameData = JsonUtility.FromJson<GameData>(dataAsJson);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("Errore nel caricare il file di salvataggio: " + e);
                NewGame();
            }
        }
        else
        {
            NewGame();
        }

        foreach (ISaveable saveable in _saveableObjects)
        {
            saveable.LoadData(_gameData);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveable saveable in _saveableObjects)
        {
            saveable.SaveData(ref _gameData);
        }

        string dataAsJson = JsonUtility.ToJson(_gameData, true);
        File.WriteAllText(_filePath, dataAsJson);
        UnityEngine.Debug.Log("Gioco salvato in: " + _filePath);
    }

    private List<ISaveable> FindAllSaveableObjects()
    {
        IEnumerable<ISaveable> saveables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>();
        return new List<ISaveable>(saveables);
    }
}
