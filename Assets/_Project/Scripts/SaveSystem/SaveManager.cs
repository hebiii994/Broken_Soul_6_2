using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : SingletonGeneric<SaveManager>
{
    protected override bool ShouldBeDestroyOnLoad() => false;

    [Header("File Configuration")]
    [SerializeField] private string _fileName = "savegame.json";


    private GameData _gameData;
    private List<ISaveable> _saveableObjects;
    private string _filePath;


    protected override void Awake()
    {
        base.Awake();
        _filePath = Path.Combine(UnityEngine.Application.persistentDataPath, _fileName);
    }

    private void Start()
    {
        _saveableObjects = FindAllSaveableObjects();
        LoadGame();
        GameManager.Instance.SetGameReady();
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
                UnityEngine.Debug.LogError("Errore nel caricare il file: " + e);
                NewGame();
            }
        }
        else
        {
            NewGame();
        }

        if (_gameData.hasDefeatedFirstBoss)
        {

            _gameData.lastCheckpointId = "GameRealStartPoint";
        }
        else if (_gameData.hasCompletedIntro)
        {

            _gameData.lastCheckpointId = "CorridorStartPosition";
        }


        foreach (ISaveable saveable in _saveableObjects)
        {
            saveable.LoadData(_gameData);
        }

        MovePlayerToCheckpoint(_gameData.lastCheckpointId);
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }

    public void SaveGame()
    {
        _saveableObjects = FindAllSaveableObjects();
        UpdateCurrentCheckpoint();

        foreach (ISaveable saveable in _saveableObjects)
        {
            saveable.SaveData(ref _gameData);
        }
        string dataAsJson = JsonUtility.ToJson(_gameData, true);
        File.WriteAllText(_filePath, dataAsJson);
        UnityEngine.Debug.Log("Gioco salvato in: " + _filePath);
    }

    public void SetCurrentCheckpoint(CheckpointSO checkpointData)
    {
        if (_gameData != null && checkpointData != null)
        {
            _gameData.lastCheckpointId = checkpointData.checkpointId;
        }
    }

    public void MarkIntroAsCompleted()
    {
        if (_gameData != null)
        {
            _gameData.hasCompletedIntro = true;
        }
    }
    public void MarkFirstBossAsDefeated()
    {
        if (_gameData != null)
        {
            _gameData.hasDefeatedFirstBoss = true;
        }
    }
    private void MovePlayerToCheckpoint(string checkpointId)
    {
        if (string.IsNullOrEmpty(checkpointId)) return;
        Checkpoint[] allCheckpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        Checkpoint targetCheckpoint = allCheckpoints.FirstOrDefault(c => c.checkpointData != null && c.checkpointData.checkpointId == checkpointId);

        if (targetCheckpoint != null)
        {
            Transform player = FindAnyObjectByType<PlayerStateMachine>().transform;
            if (player != null)
            {
                player.position = targetCheckpoint.transform.position;
            }
        }
        else
        {
            Debug.LogWarning($"Checkpoint non trovato: {checkpointId}. Il giocatore non è stato spostato.");
        }
    }

    private void UpdateCurrentCheckpoint()
    {
        // Questo metodo è vuoto per ora, ma in futuro potremmo usarlo per
        // aggiornare il checkpoint quando si interagisce con una "panchina".
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveable> FindAllSaveableObjects()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>().ToList();
    }
}
