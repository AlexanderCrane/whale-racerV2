using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedMap : MonoBehaviour {

    public static string selectedMap;

    public void setMap(int mapNum)
    {
        switch (mapNum) {
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
