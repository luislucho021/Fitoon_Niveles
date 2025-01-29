using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBots : MonoBehaviour
{

    public GameObject botPrefab;
    public Transform botsFolder;
   // public Transform goal;
    public List<SpawnpointData> spawnpointData;

    private string spawnedBotName = "";
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public Countdown countdownTimer;

    public struct SpawnpointData
    {
        public Vector3 _spPosition;
        public bool _isOccupied;
        public string _playerId;

        public SpawnpointData(Vector3 spPosition, bool isOccupied, string playerId)
        {
            _spPosition = spPosition;
            _isOccupied = isOccupied;
            _playerId = playerId;
        }

    }

    void Awake()
    {
        spawnpointData = new List<SpawnpointData>();
        foreach (Transform child in botsFolder)
        {
            Destroy(child.gameObject);
        }
        GenerateSpawnpointsList();
    }

    private void Start()
    {
        
    }

    public void InitializeBots()
    {
        
        FillSpawnpointsWithBots();
        countdownTimer.StartCountdown();
    }

    public void GenerateSpawnpointsList()
    {
        spawnpointData.Clear();

            foreach (var gO in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                if (gO.tag == "SpawnPoint")
                {
                    spawnpointData.Add(new SpawnpointData(gO.transform.position, false, ""));
                }
            }
    }

    public void FillSpawnpointsWithBots()
    {
        //SI es la primera carrera, llenar de bots
        if(RaceManager.Instance.numberOfRace == 1)
        {
            for (int i = 0; i < spawnpointData.Count; i++)
            {
                if (!spawnpointData[i]._isOccupied)
                {
                    //Generate unique random bot name
                    GenerateUniqueRandomName(i + 1);

                    //Updates the spawnpoint NetworkList with this spawnpoint as isOccupied = true and the playerId on it
                    UpdateSpawnpointsList(i, spawnpointData[i]._spPosition, true, spawnedBotName);

                    SpawnBot(spawnpointData[i]._spPosition + new Vector3(0, 1, 0));
                }
            }
        }

        //SI NO, leer los que han pasado
        else
        {
            for (int i = 0; i < spawnpointData.Count; i++)
            {
                if (i >= RaceManager.Instance.raceBots.Count)
                {
                    break;
                }

                else if (RaceManager.Instance.raceBots[i].isPlayer)
                {
                    continue;
                }

                else if (!spawnpointData[i]._isOccupied)
                {
                    UpdateSpawnpointsList(i, spawnpointData[i]._spPosition, true, RaceManager.Instance.raceBots[i].botID);
                    SpawnExistingBot(spawnpointData[i]._spPosition + new Vector3(0, 1, 0), RaceManager.Instance.raceBots[i]);
                }
            }

            RaceManager.Instance.raceBots.Clear();
        }
    }

    public void UpdateSpawnpointsList(int index, Vector3 spPosition, bool isOccupied, string playerId)
    {
        spawnpointData[index] = new SpawnpointData(spPosition, isOccupied, playerId);
    }

    private void GenerateUniqueRandomName(int botNum)
    {
        spawnedBotName = "Bot#" + botNum + "-"
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)];
    }

    public void SpawnBot(Vector3 spawnpointPosition)
    {
        GameObject inst = Instantiate(botPrefab, spawnpointPosition, transform.rotation);
        inst.name = spawnedBotName;
        inst.transform.parent = transform;
    }

    public void SpawnExistingBot(Vector3 spawnpointPosition, RaceManager.RaceBotsData raceBotsData)
    {
        GameObject inst = Instantiate(botPrefab, spawnpointPosition, transform.rotation);
        inst.name = raceBotsData.botID;
        inst.transform.parent = transform;

        //Configurar apariencia
        Transform botModel = inst.transform.GetChild(0).GetChild(0);

        //skin
        botModel.GetChild(raceBotsData.skinIndex).gameObject.SetActive(true);
        //pelo
        if (botModel.GetChild(raceBotsData.skinIndex).GetChild(0).childCount != 0)  botModel.GetChild(raceBotsData.skinIndex).GetChild(0).GetChild(raceBotsData.hairIndex).gameObject.SetActive(true);
        //camiseta
        if (botModel.GetChild(raceBotsData.skinIndex).GetChild(1).childCount != 0)  botModel.GetChild(raceBotsData.skinIndex).GetChild(1).GetChild(raceBotsData.topIndex).gameObject.SetActive(true);
        //pantalones
        if (botModel.GetChild(raceBotsData.skinIndex).GetChild(2).childCount != 0)  botModel.GetChild(raceBotsData.skinIndex).GetChild(2).GetChild(raceBotsData.bottomIndex).gameObject.SetActive(true);
        //zapatos
        if (botModel.GetChild(raceBotsData.skinIndex).GetChild(3).childCount != 0)  botModel.GetChild(raceBotsData.skinIndex).GetChild(3).GetChild(raceBotsData.shoeIndex).gameObject.SetActive(true);

        //actualizar diccionario bot skins
        inst.GetComponentInChildren<BotSkins>().botSkinData.Add("Skin", raceBotsData.skinIndex);
        inst.GetComponentInChildren<BotSkins>().botSkinData.Add("Hair", raceBotsData.hairIndex);
        inst.GetComponentInChildren<BotSkins>().botSkinData.Add("Shirt", raceBotsData.topIndex);
        inst.GetComponentInChildren<BotSkins>().botSkinData.Add("Pants", raceBotsData.bottomIndex);
        inst.GetComponentInChildren<BotSkins>().botSkinData.Add("Shoes", raceBotsData.shoeIndex);
    }
}
