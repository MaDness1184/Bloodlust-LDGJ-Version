using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemInteractableType
{
    Corpse,
    HidingSpot,
}

public class ItemInteractable : Interactable
{
    [SerializeField] private ItemInteractableType _type;
    [SerializeField] private int _value;

    public ItemInteractableType type { get { return _type; } }
    public int value { get { return _value; } }

    public override void Interact(EntityInteraction interacter)
    {
        base.Interact(interacter);

        switch (_type)
        {
            case ItemInteractableType.Corpse:
                Destroy(gameObject);
                break;
            case ItemInteractableType.HidingSpot:
                interacter.GetComponent<PlayerController>().SetIsHiden(true , transform.position);
                break;
        }
    }
}
