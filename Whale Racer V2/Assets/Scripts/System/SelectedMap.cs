using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMap : MonoBehaviour {

    public static string selectedMap;
    public static int selectedLaps;

    public void setMap(int mapNum)
    {
        selectedLaps = GameObject.Find("LapsDropdown").GetComponent<Dropdown>().value;
        switch (mapNum)
        {
            case 1:
                selectedMap = "Aduloo";
                break;
            case 2:
                selectedMap = "Map2";
                break;
            case 3:
                selectedMap = "Map3";
                break;
            default:
                selectedMap = "Aduloo";
                break;
        }

    }
}
