using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectableItem : MonoBehaviour
{
    [SerializeField]private CollectableItemSO collectableItemSO;

    public Transform Prefab { get { return collectableItemSO.Prefab; } }


    public static event EventHandler OnAnyItemPicked;

    public static void ResetStaticData()
    {
        OnAnyItemPicked = null;
    }

    private void Start()
    {
        if (collectableItemSO == null)
        {
            Debug.LogError("Item " + this.name + " is missing a scriptable-object refrence");
        }

        OnAnyItemPicked += CollectableItem_OnItemPicked;
    }

    private void CollectableItem_OnItemPicked(object sender, EventArgs e)
    {
        if (sender as CollectableItem == this)
            DestroySelf();
    }

    /// <summary>
    /// Called when The item was picked. resposible for the implementation of the logic
    /// when the item is picked
    /// </summary>
    /// <returns> the cuurency value of the item</returns>
    public uint ItemPicked()
    {
        OnAnyItemPicked?.Invoke(this, EventArgs.Empty);
        return ((uint)collectableItemSO.Value);
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
