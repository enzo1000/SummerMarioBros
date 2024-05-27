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

    private bool dataExported = false; // Flag pour s'assurer que les données sont exportées une seule fois

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dataExported)
        {
            Debug.Log("End of level reached!");
            dataExported = true; // mettre le flag à true pour éviter les exports dupliqués
            FindDicoDatas();
        }
    }

    void FindDicoDatas()
    {
        // Récupérer les données de DataToStore
        levelInfo = DataToStore.instance.levelInfo; // Celles du niveau
        playerTimeInfo = DataToStore.instance.playerTimeInfo; // Celles du joueur et des Timer
        causeOfDeath = DataToStore.instance.causeOfDeath; // Celles des morts
        // Exporter les données au format CSV
        string filePath = Path.Combine(Application.dataPath, "GameData.csv"); // Chemin du fichier de sortie qui sera créé dans le dossier de l'application (Assets)
        ExportDataToCSV(filePath);
    }

    void ExportDataToCSV(string filePath)
    {
        // Récupérer toutes les clés de tous les dictionnaires
        var allKeys = new List<string>();
        allKeys.AddRange(levelInfo.Keys); 
        allKeys.AddRange(playerTimeInfo.Keys);
        allKeys.AddRange(causeOfDeath.Keys);

        // Récupérer les clés uniques car on ne veut pas de doublons
        var uniqueKeys = allKeys.Distinct().ToList();
        // Créer un dictionnaire pour stocker les valeurs
        var values = new Dictionary<string, string>(); 

        foreach (var key in uniqueKeys)
        {
            if (levelInfo.ContainsKey(key))
            {
                //remplacer , par . pour pas tout casser

                values[key] = levelInfo[key].ToString().Replace(",", ".");
            }
            else if (playerTimeInfo.ContainsKey(key))
            {
                values[key] = playerTimeInfo[key].ToString().Replace(",", "."); 
            }
            else if (causeOfDeath.ContainsKey(key))
            {
                values[key] = causeOfDeath[key]; 
            }
            else
            {
                values[key] = "";
            }
        }

        // Vérifier si le fichier existe déjà
        bool fileExists = File.Exists(filePath);
        // Écrire les données dans le fichier
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            // Ecrire les clés si le fichier n'existe pas
            if (!fileExists)
            {
                writer.WriteLine(string.Join(",", uniqueKeys));
            }

            // Ecrire les valeurs
            var valueList = uniqueKeys.Select(key => values[key]).ToList();
            writer.WriteLine(string.Join(",", valueList));
        }

        Debug.Log($"Data exported to {filePath}"); // Juste du debug on peut le retirer
    }
}
