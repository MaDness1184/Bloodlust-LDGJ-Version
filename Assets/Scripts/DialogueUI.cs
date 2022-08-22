using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI main;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);
    }
        
}
