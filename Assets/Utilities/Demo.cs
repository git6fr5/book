using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour {

    /* --- Data Fields --- */
    public static string path = "DemoData/";
    public static string filetype = ".demodata";

    public int level;

    [System.Serializable]
    public class DemoData : Data {

        public int level;

        public DemoData(Demo demo) {
            this.level = demo.level;
        }

        public void Load(Demo demo) {
            demo.level = this.level;
        }
    }

    public void Save(string filename) {
        DemoData data = new DemoData(this);
        IO.SaveDataFile(data, path, filename, filetype);
    }

    public void Open(string filename) {
        DemoData data = IO.OpenDataFile(path, filename, filetype) as DemoData;
        data.Load(this);
    }

}
