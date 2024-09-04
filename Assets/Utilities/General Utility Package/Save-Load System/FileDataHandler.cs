using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace GeneralUtility
{
    namespace SaveLoadSystem
    {
        public class FileDataHandler
        {
            private string dataDirPath = "";
            private string dataFileName = "";
            private bool useEncryption = false;
            private readonly string encryptionCodeWord = "AlpacaSpit";

            public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
            {
                this.dataDirPath = dataDirPath;
                this.dataFileName = dataFileName;
                this.useEncryption = useEncryption;
            }

            public GameData Load(string profileId)
            {
                if (profileId == null)
                {
                    return null;
                }

                //use Path.Combine to account for different OS'S having different path separators
                string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
                GameData loadedData = null;
                if (File.Exists(fullPath))
                {
                    try
                    {
                        //Load the serialized data from the file
                        string dataToLoad = "";
                        using (FileStream stream = new(fullPath, FileMode.Open))
                        {
                            using (StreamReader reader = new(stream))
                            {
                                dataToLoad = reader.ReadToEnd();
                            }
                        }

                        //optionally decrypt the data
                        if (useEncryption)
                        {
                            dataToLoad = EncryptDecrypt(dataToLoad);
                        }

                        //deserialize the data from Json back into the C# object
                        loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
                    }
                }
                return loadedData;
            }

            public void Save(GameData data, string profileId)
            {
                if (profileId == null)
                {
                    return;
                }

                //use Path.Combine to account for different OS'S having different path separators
                string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
                try
                {
                    //create the directory the file will be written to if it doesn't already exist
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                    //serialize the C# game data object into Json
                    string dataToStore = JsonUtility.ToJson(data, true);

                    //optionally encrypt the data
                    if (useEncryption)
                    {
                        dataToStore = EncryptDecrypt(dataToStore);
                    }

                    //write the serialized data to the file
                    using (FileStream stream = new(fullPath, FileMode.Create))
                    {
                        using (StreamWriter writer = new(stream))
                        {
                            writer.Write(dataToStore);
                            Debug.Log(fullPath);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
                }
            }

            public void Delete(string profileId)
            {
                if (profileId == null) return;

                string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);

                try
                {
                    if (File.Exists(fullPath))
                    {
                        Directory.Delete(Path.GetDirectoryName(fullPath), true);
                    }
                    else
                    {
                        Debug.LogWarning($"Tried to delete data but none found at path: {fullPath}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to delete data at path: {fullPath}\n{e}");
                }

            }

            public Dictionary<string, GameData> LoadAllProfiles()
            {
                Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

                //loop over all directory names in the data directory path
                IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
                foreach (DirectoryInfo dirInfo in dirInfos)
                {
                    string profileId = dirInfo.Name;

                    //defensive programming - check if the data file exists
                    // if it doesn't, then this folder isn't a profile and should be skipped
                    string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
                    if (!File.Exists(fullPath))
                    {
                        Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: " + profileId);
                        continue;
                    }

                    //load the game data for this profile and put it in the dictionary
                    GameData profileData = Load(profileId);

                    //defensive programming - ensure the profile data isn't null,
                    //because if it is then something went wrong and we should let ourselves know
                    if (profileData != null)
                    {
                        profileDictionary.Add(profileId, profileData);
                    }
                    else
                    {
                        Debug.LogError("Tried to load profile but something went wrong. ProfileId: " + profileId);
                    }
                }

                return profileDictionary;
            }

            // the below is a simple implementation of XOR encryption
            private string EncryptDecrypt(string data)
            {
                string modifiedData = "";
                for (int i = 0; i < data.Length; i++)
                {
                    modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
                }
                return modifiedData;
            }

            public string GetMostRecentlyUpdatedProfileId()
            {
                string mostRecentProfileId = null;

                Dictionary<string, GameData> profiles = LoadAllProfiles();
                foreach (var pair in profiles)
                {
                    string profileId = pair.Key;
                    GameData data = pair.Value;

                    if (data == null) continue;

                    if (mostRecentProfileId == null)
                    {
                        mostRecentProfileId = profileId;
                    }
                    else
                    {
                        DateTime mostRecentDateTime = DateTime.FromBinary(profiles[mostRecentProfileId].lastUpdated);
                        DateTime newDateTime = DateTime.FromBinary(data.lastUpdated);

                        if (newDateTime > mostRecentDateTime)
                        {
                            mostRecentProfileId = profileId;
                        }
                    }
                }
                return mostRecentProfileId;
            }
        }
    }
}