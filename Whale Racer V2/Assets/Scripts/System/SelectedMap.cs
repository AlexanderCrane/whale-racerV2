using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to store selected map.
/// </summary>
public class SelectedMap : MonoBehaviour {

    public static string selectedMap;
    public static int selectedLaps;

    /// <summary>
    /// Store the map the player selected for later use once they select a mode.
    /// </summary>
    /// <param name="mapNum"></param>
    public void setMap(int mapNum)
    {
        selectedLaps = GameObject.Find("LapsDropdown").GetComponent<Dropdown>().value;
        switch (mapNum)
        {
            case 1:
                selectedMap = "Aduloo";
                break;
            case 2:
                selectedMap = "TheShipyard";
                break;
            case 3:
                selectedMap = "TheMinefield";
                break;
            default:
                selectedMap = "Aduloo";
                break;
        }

    }
}
