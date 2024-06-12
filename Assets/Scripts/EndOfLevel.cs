using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad;

    private Uri phpPath;

    public static EndOfLevel instance;
    public Animator animator;
    public GameObject joueur;

    public IDictionary<string, string> levelName;
    public IDictionary<string, float> levelInfo;
    public IDictionary<string, float> playerTimeInfo;
    public IDictionary<string, float> causeOfDeath;

    private bool dataExported = false; // Flag pour s'assurer que les données sont exportées une seule fois

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EndOfLevel found!");
            return;
        }
        instance = this;

        StartCoroutine(GetRequestPhp(Application.streamingAssetsPath + "/script.php"));

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dataExported)
        {
            //Affiche les remerciements
            animator.SetTrigger("Remerciements");

            joueur.GetComponent<PlayerMovement>().StopPlayer();
            joueur.GetComponent<PlayerMovement>().enabled = false;

            DataToStore.instance.ProcessEndOfLevelData();   //On calcul nos taux de bonus / kill
            FindDicoDatas();                                //On envoie nos données à EndOfLevel.cs
            DataToStore.instance.ResetData(sceneToLoad);    //On reset nos dictionnaires pour le prochain niveau

            dataExported = true; // mettre le flag à true pour éviter les exports dupliqués
        }
    }

    public void FindDicoDatas()
    {
        // Récupérer les données de DataToStore
        levelName = DataToStore.instance.levelNameDic;          // Le nom du niveau (spécifiquement pour le moment)
        levelInfo = DataToStore.instance.levelInfo;             // Celles du niveau
        playerTimeInfo = DataToStore.instance.playerTimeInfo;   // Celles du joueur et des Timer
        causeOfDeath = DataToStore.instance.causeOfDeath;       // Celles des morts
        // Exporter les données au format CSV
        string filePath = Application.streamingAssetsPath; // Chemin du fichier de sortie qui sera créé dans le dossier de l'application (Assets)
        
        ExportDataToCSV(filePath);
    }

    private async void ExportDataToCSV(string filePath)
    {
        // Récupérer toutes les clés de tous les dictionnaires
        var allKeys = new List<string>();
        allKeys.AddRange(levelName.Keys);
        allKeys.AddRange(levelInfo.Keys);
        allKeys.AddRange(playerTimeInfo.Keys);
        allKeys.AddRange(causeOfDeath.Keys);

        // Récupérer les clés uniques car on ne veut pas de doublons
        var uniqueKeys = allKeys.Distinct().ToList();
        // Créer un dictionnaire pour stocker les valeurs
        var values = new Dictionary<string, string>();

        foreach (var key in uniqueKeys)
        {
            if (levelName.ContainsKey(key))
            {
                values[key] = levelName[key];
            }
            else

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
                values[key] = causeOfDeath[key].ToString().Replace(",", ".");
            }
            else
            {
                values[key] = "";
            }
        }

        //Envoie de notre dictionnaire à un fichier php pour l'exporter
        WebClient client = new WebClient();
        Debug.Log("Pre UploadStringAsync");
        var valuesCOL = new NameValueCollection();
        valuesCOL["mode1"] = "";
        valuesCOL["mode2"] = "AhAHAHAHA";
        
        try
        {
            var response = await client.UploadValuesTaskAsync(phpPath, valuesCOL);
            Debug.Log("UploadStringAsync");
        } 
        catch (Exception e)
        {
            Debug.Log(e);
        }

        Debug.Log($"Data exported to {filePath}"); // Juste du debug on peut le retirer

        //GoogleDriveAPI.instance.SendData(); // Envoie des données sur Google Drive
    }

    IEnumerator GetRequestPhp(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            phpPath = webRequest.uri;

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    UnityEngine.Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    UnityEngine.Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    UnityEngine.Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}
