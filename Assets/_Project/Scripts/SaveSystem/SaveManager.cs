using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class SaveManager : SingletonGeneric<SaveManager>
{
    protected override bool ShouldBeDestroyOnLoad() => false;

    public const int NUM_SLOTS = 3;


    private GameData _gameData;
    private List<ISaveable> _saveableObjects;
    private int _currentSlotID = 1;

    private PlayerStateMachine _player;

    private string GetFilePath(int slotID)
    {
        return Path.Combine(UnityEngine.Application.persistentDataPath, $"savegame_{slotID}.json");
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public GameData PeekSaveData(int slotID)
    {
        string path = GetFilePath(slotID);
        if (File.Exists(path))
        {
            try
            {
                string dataAsJson = File.ReadAllText(path);
                return JsonUtility.FromJson<GameData>(dataAsJson);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Errore nel leggere lo slot {slotID}: {e}");
                return null;
            }
        }
        return null; 
    }

    public void LoadGame(int slotID, bool isNewGame)
    {
        _currentSlotID = slotID;
        string path = GetFilePath(slotID);

        if (isNewGame || !File.Exists(path))
        {
            NewGame();
            Debug.Log($"Inizio NUOVA PARTITA nello slot {slotID}.");
        }
        else
        {
            try
            {
                string dataAsJson = File.ReadAllText(path);
                _gameData = JsonUtility.FromJson<GameData>(dataAsJson);
                Debug.Log($"CARICAMENTO riuscito dallo slot {slotID}.");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("Errore nel caricare il file: " + e);
                NewGame();
            }
        }

        if (_gameData.hasDefeatedFirstBoss)
        {
            _gameData.lastCheckpointId = "GameRealStartPoint";
        }
        else if (_gameData.hasCompletedIntro)
        {
            _gameData.lastCheckpointId = "CorridorStartPosition";
        }

        StartCoroutine(CompleteGameSetupRoutine());
    }

    private IEnumerator CompleteGameSetupRoutine()
    {
        yield return null;

        _player = FindAnyObjectByType<PlayerStateMachine>();
        if (_player == null)
        {
            Debug.LogError("PlayerStateMachine non trovato nella nuova scena. Impossibile teletrasportare.");
        }

        _saveableObjects = FindAllSaveableObjects();
        foreach (ISaveable saveable in _saveableObjects)
        {
            saveable.LoadData(_gameData);
        }

        MovePlayerToCheckpoint(_gameData.lastCheckpointId);

        GameManager.Instance.SetGameReady();

        Debug.Log("SAVE MANAGER: Setup completato. Game Ready invocato.");
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }

    public void SaveGame()
    {
        if (_gameData == null)
        {
            Debug.LogWarning("SaveGame() chiamato ma _gameData è null. Salvataggio saltato.");
            return;
        }
        _saveableObjects = FindAllSaveableObjects();
        UpdateCurrentCheckpoint();

        foreach (ISaveable saveable in _saveableObjects)
        {
            saveable.SaveData(ref _gameData);
        }
        string dataAsJson = JsonUtility.ToJson(_gameData, true);
        File.WriteAllText(GetFilePath(_currentSlotID), dataAsJson);
        UnityEngine.Debug.Log($"Gioco salvato nello slot {_currentSlotID} in: {GetFilePath(_currentSlotID)}");
    }

    public void ResetSave(int slotID)
    {
        string path = GetFilePath(slotID);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Salvamento slot {slotID} resettato.");
        }
    }

    public void SetPlayerName(string newName)
    {
        if (_gameData != null)
        {
            _gameData.playerName = newName;
            SaveGame();
        }
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
        if (NarrativeManager.Instance != null)
        {
            NarrativeManager.Instance.NotifyBossDefeated();
        }
    }
    private void MovePlayerToCheckpoint(string checkpointId)
    {
        if (string.IsNullOrEmpty(checkpointId)) return;

        Checkpoint[] allCheckpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        Checkpoint targetCheckpoint = allCheckpoints.FirstOrDefault(c => c.checkpointData != null && c.checkpointData.checkpointId == checkpointId);

        if (targetCheckpoint != null)
        {
            PlayerStateMachine playerStateMachine = FindAnyObjectByType<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                Vector3 oldPosition = playerStateMachine.transform.position;
                Vector3 newPosition = targetCheckpoint.transform.position;

                playerStateMachine.transform.position = newPosition;

                CinemachineCamera playerCamera = playerStateMachine.PlayerCamera;
                if (playerCamera != null)
                {
                    Vector3 deltaPosition = newPosition - oldPosition;
                    playerCamera.OnTargetObjectWarped(playerStateMachine.transform, deltaPosition);
                }
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
