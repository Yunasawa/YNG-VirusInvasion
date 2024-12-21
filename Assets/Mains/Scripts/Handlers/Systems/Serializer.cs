using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using YNL.Extensions.Methods;

public static class Serializer
{
    public static void SaveByte<T>(this T target, string path)
    {
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, target);
            }
        }
        catch (Exception e)
        {
            MDebug.Error("Failed to save file: " + e.Message);
        }
    }

    public static T LoadByte<T>(this string path) where T : class
    {
        if (File.Exists(path))
        {
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return formatter.Deserialize(fileStream) as T;
                }
            }
            catch (Exception e)
            {
                MDebug.Error("Failed to load file: " + e.Message);
            }
        }
        else
        {
            MDebug.Error("File not found: " + path);
        }
        return null;
    }
}