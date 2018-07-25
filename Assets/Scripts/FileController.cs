using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;


public class FileController : MonoBehaviour
{

    private static string FileName = "DataCollection.txt";
    private static string path = "";

    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, FileName);
        if (!File.Exists(path))
        {
            var sr = File.CreateText(path);
            sr.WriteLine("Data Collection for Lab Safety Instruction!\n");
            sr.WriteLine("Timestamp: [trigger name], [postion], [rotation]");
            sr.Dispose();
        }

    }

    public static void momentToString(int cmd, string input)
    {
        Transform main = Camera.main.transform;
        string inputLine = "";
        switch (cmd)
        {
            case 0:
                writeFile("=================================================================");
                inputLine += System.DateTime.Now + ": Start Training";
                break;
            case 1:
                inputLine += System.DateTime.Now + ": [" + input + "], [" + main.position + "], [" + main.localEulerAngles + "]";
                break;
            case 2:
                inputLine += System.DateTime.Now + ": End Training";
                break;
        }
        writeFile(inputLine);
        
    }

    public static void writeFile(string input)
    {
        string reads = "";
        using (StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open)))
        {
            reads = sr.ReadToEnd();
            sr.Dispose();
        }

        using (StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Open)))
        {
            sw.Write(reads);
            sw.WriteLine(input);
            sw.Dispose();
        }
    }
}
