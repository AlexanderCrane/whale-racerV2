using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableButtonsOnLapSelect : MonoBehaviour {

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
