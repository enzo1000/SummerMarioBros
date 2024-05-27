using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EndOfLevel : MonoBehaviour
{
    public IDictionary<string, float> levelInfo;
    public IDictionary<string, float> playerTimeInfo;
    public IDictionary<string, string> causeOfDeath;

    private bool dataExported = false; // Flag to ensure data is exported only once

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dataExported)
        {
            Debug.Log("End of level reached!");
            dataExported = true; // Set the flag to true to prevent duplicate exports
            FindDicoDatas();
        }
    }

    void FindDicoDatas()
    {
        // Get the data from DataToStore
        levelInfo = DataToStore.instance.levelInfo;
        playerTimeInfo = DataToStore.instance.playerTimeInfo;
        causeOfDeath = DataToStore.instance.causeOfDeath;
        //
        string filePath = Path.Combine(Application.dataPath, "GameData.csv");
        ExportDataToCSV(filePath);
    }

    void ExportDataToCSV(string filePath)
    {
        var allKeys = new List<string>();
        allKeys.AddRange(levelInfo.Keys);
        allKeys.AddRange(playerTimeInfo.Keys);
        allKeys.AddRange(causeOfDeath.Keys);

        var uniqueKeys = allKeys.Distinct().ToList();

        var values = new Dictionary<string, string>(); //faut changer ici en float pour pas tout detruire apres

        foreach (var key in uniqueKeys)
        {
            if (levelInfo.ContainsKey(key))
            {
                values[key] = levelInfo[key].ToString(); //et donc ici aussi
            }
            else if (playerTimeInfo.ContainsKey(key))
            {
                values[key] = playerTimeInfo[key].ToString(); //et ici aussi
            }
            else if (causeOfDeath.ContainsKey(key))
            {
                values[key] = causeOfDeath[key]; //ici on a une string et ca fait chier
            }
            else
            {
                values[key] = "";
            }
        }

        bool fileExists = File.Exists(filePath);
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // Write headers if the file does not exist
            if (!fileExists)
            {
                writer.WriteLine(string.Join(",", uniqueKeys));
            }

            // Write values
            var valueList = uniqueKeys.Select(key => values[key]).ToList();
            writer.WriteLine(string.Join(",", valueList));
        }

        Debug.Log($"Data exported to {filePath}");
    }
}
