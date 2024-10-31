using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableItemSO", menuName = "ScriptableObjects/Collectable Item SO", order = 2)]
public class CollectableItemSO : ScriptableObject
{
    [SerializeField] private int value = 1;
    [SerializeField] private Transform prefab;

    public int Value {  get { return value; } }
    public Transform Prefab {  get { return prefab; } }
}
