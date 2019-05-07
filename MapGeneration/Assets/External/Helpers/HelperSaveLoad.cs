using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace helper
{
    public static class HelperSaveLoad
    {
        //public static string[] GetAllFileNames(string extension)
        //{
        //    ExternalData externalData = ExternalData.Instance;

        //    CheckForSaveFolder();
        //    return Directory.GetFiles(externalData.SavePath, "*." + externalData.SaveExtention);
        //}
        //public static void CheckForSaveFolder()
        //{
        //    ExternalData externalMaster = ExternalData.Instance;
        //    if (externalMaster.SavePath == "")
        //    {
        //        externalMaster.SavePath = System.IO.Path.Combine(Application.persistentDataPath, "Levels");
        //    }

        //    if (!Directory.Exists(externalMaster.SavePath))
        //        Directory.CreateDirectory(externalMaster.SavePath);
        //}

        //public static void SaveToFile<T>(T obj, string path, string name, string extention)
        //{
        //    CheckForSaveFolder();

        //    string filePath = System.IO.Path.Combine(path, name + "." + extention);
        //    FileStream file = File.Open(filePath, FileMode.Create);

        //    BinaryFormatter binFormater = new BinaryFormatter();
        //    binFormater.Serialize(file, obj);
        //    file.Close();
        //}
        //public static T LoadFile<T>(string path, string name, string extention) where T : class
        //{
        //    CheckForSaveFolder();
        //    string[] allFiles = Directory.GetFiles(path, name + "." + extention, SearchOption.TopDirectoryOnly);
        //    if (allFiles.Length == 0)
        //        return null;

        //    FileStream file = File.Open(allFiles[0], FileMode.Open);

        //    BinaryFormatter binFormatter = new BinaryFormatter();
        //    T loadedData = (T)binFormatter.Deserialize(file);
        //    file.Close();

        //    return loadedData;
        //}

        //public static bool DeleteLevel(string levelName) //TODO: trigger event
        //{
        //    ExternalMaster externalMaster = ExternalMaster.Instance;

        //    CheckForSaveFolder();
        //    string filePath = System.IO.Path.Combine(externalMaster.SavePath, levelName + "." + externalMaster.SaveExtention);

        //    if (File.Exists(filePath))
        //    {
        //        File.Delete(filePath);

        //        EventManager.TriggerEvent(EventNames.External_Files_Changed);
        //        return true;
        //    }
        //    return false;
        //}


        public static Texture2D[] LoadTexturesFromFolder(string folder)
        {
            string path = folder;
            return Resources.LoadAll<Texture2D>(path);
        }
        public static void SaveTexture(string folder, string name, Texture2D texture)
        {
            byte[] texData = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + folder + name + ".png", texData);
        }
    }
}