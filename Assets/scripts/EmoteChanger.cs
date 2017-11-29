using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteChanger : MonoBehaviour {

    public Sprite sad;
    public Sprite smile;

    public const string smileName = "Smile";
    public const string sadName = "Sad";

    public void changeEmote(string emoteName)
    {
        if(emoteName == sadName)
        {
            GetComponent<SpriteRenderer>().sprite = sad;
            Debug.Log("Sad");
        } else if (emoteName == smileName)
        {
            GetComponent<SpriteRenderer>().sprite = smile;
        }
    }
   
}
