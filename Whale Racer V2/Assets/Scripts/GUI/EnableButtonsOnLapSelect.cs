using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Script to enable selecting a map only when the player has selected a number of laps.
/// </summary>
public class EnableButtonsOnLapSelect : MonoBehaviour {
    /// <summary>
    /// Enable the map buttons if the selected element of the laps dropdown is not the first element.
    /// </summary>
    public void Enable()
    {
        if (gameObject.GetComponent<Dropdown>().value != 0)
        {
            foreach(GameObject button in GameObject.FindGameObjectsWithTag("MapButton"))
            {
                button.GetComponent<Button>().enabled = true;
            }
        }
    }
}
