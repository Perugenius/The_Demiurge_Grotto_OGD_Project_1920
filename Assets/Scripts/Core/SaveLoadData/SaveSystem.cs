using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Mechanics;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core.SaveLoadData
{
    public static class SaveSystem
    {
        public static void CreatePlayerData(string firstUnlockedCharacter, List<string> charactersNames)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/player.data";
            FileStream stream = new FileStream(path,FileMode.Create);
            PlayerData data = new PlayerData(firstUnlockedCharacter, charactersNames);
            
            formatter.Serialize(stream,data);
            stream.Close();
        }
        
        public static void SavePlayerData(PlayerData playerData)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/player.data";
            FileStream stream = new FileStream(path,FileMode.Create);
            PlayerData data = playerData;
            
            formatter.Serialize(stream,data);
            stream.Close();
            
            //trigger save event
            Debug.Log("Trigger event from saveSystem");
            EventsCollector.Instance.TriggerEvent("saveData");
        }

        public static PlayerData LoadPlayerData()
        {
            string path = Application.persistentDataPath + "/player.data";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();
                return data;
            }
            /*Debug.LogError("Save file not found");*/
             return null;
        }

        public static void DeleteDataFile(string fileName) 
        {
            string filePath = Application.persistentDataPath + "/" + fileName + ".data";

            // check if file exists
            if ( !File.Exists( filePath ) )
            {
                Debug.Log("File: " + fileName+".data doesn't exist, it can't be deleted");
            }
            else
            { 
                File.Delete( filePath );
                Debug.Log("File deleted: " + fileName+".data");
                
                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
                #endif
            }
        }
    }
}