using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class SolExp : MonoBehaviour {

    public List<string> recents = new List<string>();
    public static SolExp instance;
    public GameObject recfile;
    public GameObject Comps;

    [System.Serializable]
    public struct SaveStuff
    {
        public List<string> words;
    }

    public SaveStuff sf;


    public void Start()
    {
        instance = this;
        Change();
    }

    public void Change(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(UnityEngine.Application.persistentDataPath + "/Rec.txt"))
        {

            SaveStuff saveData;
            using (Stream filestream = File.Open(UnityEngine.Application.persistentDataPath + "/Rec.txt", FileMode.Open, FileAccess.Read))
            {
                saveData = (SaveStuff)binaryFormatter.Deserialize(filestream);
            }
            sf = saveData;

        }

        if (sf.words.Count >= 5)
        {
            sf.words.Remove(sf.words[0]);
        }

        sf.words.Add(path);

        using (Stream filestream = File.Open(UnityEngine.Application.persistentDataPath + "/Rec.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            binaryFormatter.Serialize(filestream, sf);
        }   
        Showobj();

    }

    

    public void Change()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(UnityEngine.Application.persistentDataPath + "/Rec.txt"))
        {

            SaveStuff saveData;
            using (Stream filestream = File.Open(UnityEngine.Application.persistentDataPath + "/Rec.txt", FileMode.Open, FileAccess.Read))
            {
                saveData = (SaveStuff)binaryFormatter.Deserialize(filestream);
            }
            sf = saveData;
        }
        Showobj();

    }

    private void Showobj()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            Destroy(this.gameObject.transform.GetChild(i).gameObject);
        }

        for (int i = sf.words.Count-1; i >=0; i--)
        {
            GameObject go = Instantiate(recfile, transform.position, Quaternion.identity, transform);
            go.GetComponent<recentFiles>().Init(sf.words[i]);
        }

        Instantiate(Comps, transform.position, Quaternion.identity, transform);

    }


    private void OnDisable()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            Destroy(this.gameObject.transform.GetChild(i).gameObject);
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            Destroy(this.gameObject.transform.GetChild(i).gameObject);
        }
        Start();
    }
}
