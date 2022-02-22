/* --- Modules --- */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

public class IO : MonoBehaviour {

    public static string DataPath = "Assets/Resources/Data/";

    /* --- CSV Modules --- */
    // Saves a list of 2D integer arrays to CSV format seperated by a tag.
    public static void SaveCSV(List<int[][]> channels, string path, string filename) {
        string csv = "";
        for (int i = 0; i < channels.Count; i++) {
            csv += GridToCSV(channels[i]) + "\\";
        }
        using (StreamWriter outputFile = new StreamWriter(DataPath + path + filename + ".csv")) {
            outputFile.Write(csv);
        }
    }

    // Converts a 2D integer array to CSV format.
    static string GridToCSV(int[][] grid) {
        string csv = "";
        for (int i = 0; i < grid.Length; i++) {
            for (int j = 0; j < grid[i].Length; j++) {
                csv += grid[i][j].ToString() + ",";
            }
            csv += "\n";
        }
        return csv;
    }

    // Opens a CSV file as a list of 2D integer arrays.
    public static List<int[][]> OpenCSV(string path, string filename) {
        string csv = "";
        using (StreamReader readFile = new StreamReader(DataPath + path + filename + ".csv")) {
            csv = readFile.ReadToEnd();
        }
        string[] csvs = csv.Split('\\');
        List<int[][]> grids = new List<int[][]>();
        for (int i = 0; i < csvs.Length; i++) {
            grids.Add(CSVToGrid(csvs[i]));
        }
        return grids;
    }

    // Reads a csv string to a 2D integer array.
    static int[][] CSVToGrid(string csv) {
        // put the data into the appropriate format
        string[] rows = csv.Split('\n');
        int[][] grid = new int[rows.Length - 1][];
        for (int i = 0; i < rows.Length - 1; i++) {
            string[] columns = rows[i].Split(',');
            grid[i] = new int[columns.Length - 1];
            for (int j = 0; j < columns.Length - 1; j++) {
                grid[i][j] = int.Parse(columns[j]);
            }
        }
        return grid;
    }

    // Read the list file to a dictionary.
    public static Dictionary<string, int[]> ReadListFile(string path, string filename) {
        Dictionary<string, int[]> roomIdentifiers = new Dictionary<string, int[]>();
        string txt = "";
        using (StreamReader readFile = new StreamReader(DataPath + path + filename + ".csv")) {
            txt = readFile.ReadToEnd();
        }
        string[] rows = txt.Split('\n');
        for (int i = 0; i < rows.Length; i++) {

            string[] cols = rows[i].Split(',');
            if (cols.Length > 1) {
                int[] _identifiers = new int[cols.Length - 1];
                for (int j = 1; j < cols.Length; j++) {
                    if (cols[j] != "") {
                        _identifiers[j - 1] = int.Parse(cols[j]);
                    }
                }
                roomIdentifiers.Add(cols[0], _identifiers);
            }
        }

        return roomIdentifiers;
    }

    // Find a file in the list file.
    public static int[] FindInListFile(string path, string filename) {
        // Read the list file into a dictionary.
        Dictionary<string, int[]> roomIdentifiers = ReadListFile(path, filename);
        // Check if filename is in in the dictionary.
        if (roomIdentifiers.ContainsKey(filename)) {
            return roomIdentifiers[filename];
        }
        return new int[] { 0, 0, 0 };
    }

    // Add or edit an entry to the list file.
    public static void EditListFile(int[] identifiers, string path, string filename) {
        // Read the list file into a dictionary.
        Dictionary<string, int[]> roomIdentifiers = ReadListFile(path, filename);
        // Edit the entry or add a new entry.
        if (roomIdentifiers.ContainsKey(filename)) {
            roomIdentifiers[filename] = identifiers;
        }
        else {
            roomIdentifiers.Add(filename, identifiers);
        }
        // Write the dictionary back into a string.
        string txt = "";
        foreach (KeyValuePair<string, int[]> roomIdentifier in roomIdentifiers) {
            if (roomIdentifier.Key != "") {
                txt += roomIdentifier.Key;
                for (int i = 0; i < roomIdentifier.Value.Length; i++) {
                    txt += ',' + roomIdentifier.Value[i].ToString();
                }
                txt += '\n';
            }
        }
        // Write the string back to the file.
        using (StreamWriter outputFile = new StreamWriter(DataPath + path + filename + ".csv")) {
            outputFile.Write(txt);
        }

    }

    /* --- Text Files --- */
    // Opens a text file to a string.
    public static string OpenText(string path, string filename) {
        string text = "";
        using (StreamReader readFile = new StreamReader(DataPath + path + filename + ".txt")) {
            text = readFile.ReadToEnd();
        }
        return text;
    }

    /* --- Binary Data Files --- */
    public static void SaveDataFile(Data data, string path, string filename, string filetype) {

        // Concatenate the path.
        string fullPath = DataPath + path + filename + filetype;

        // Format the data.
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(fullPath, FileMode.Create);
        formatter.Serialize(fileStream, data);

        // Close the file.
        fileStream.Close();

    }

    public static object OpenDataFile(string path, string filename, string filetype) {

        // Concatenate the path.
        string fullPath = DataPath + path + filename + filetype;

        if (File.Exists(fullPath)) {

            // Read the data.
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(fullPath, FileMode.Open);
            object data = formatter.Deserialize(fileStream);

            // Close the file.
            fileStream.Close();

            return data;
        }

        return null;
    }

}

