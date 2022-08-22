using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInteractable : Interactable
{
    [SerializeField] private Weapon _weapon;

    public Weapon weapon { get { return _weapon; } }

    public override void Interact(EntityInteraction interacter)
    {
        base.Interact(interacter);

        Destroy(gameObject);
    }
}
