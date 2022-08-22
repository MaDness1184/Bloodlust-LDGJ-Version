using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int damage = 1;
    public float range = 1.5f;
    public float attackCdr = 0.5f;
    public GameObject gameObject;
}
