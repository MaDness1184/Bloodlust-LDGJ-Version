using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private ShaderController shaderController;

    // Start is called before the first frame update
    void Start()
    {
        shaderController = GetComponent<ShaderController>();
    }

    public void LocalHighlight(bool highlight)
    {
        if(shaderController != null)
            shaderController?.LocalHighlight(highlight);
    }

    public virtual void Interact(EntityInteraction interacter)
    {

    }
}
