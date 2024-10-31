using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerCurrencyChangedEventArgs : EventArgs
{
    public uint currency;
}

public class Player_brain : MonoBehaviour
{
    public static event EventHandler<OnPlayerCurrencyChangedEventArgs> OnPlayerCurrencyChanged;

    public static void ResetStaticData()
    {
        OnPlayerCurrencyChanged = null;
    }

    [SerializeField] private float itemsCollectionRange = 2f;
    [SerializeField] private float itemsCollectionRangeMaxDistance = 0;
    [Space(10), Header("Collapsed field groups example")]
    public ex1 e1;
    public ex2 e2;

    private GameManager gameManager;
    private RigidBodyCharacterController characterController;
    private uint currency;

    private void Start()
    {
        currency = 0;
        characterController = GetComponent<RigidBodyCharacterController>();
        gameManager = GameManager.Instance;

        characterController.OnPlayerRespawned += Player_OnPlayerRespawned;
    }

    private void Player_OnPlayerRespawned(object sender, EventArgs e)
    {
        currency = 0;
        OnPlayerCurrencyChanged?.Invoke(this, new OnPlayerCurrencyChangedEventArgs() { currency = this.currency });
    }

    private void Update()
    {
        checkForCollectables();
    }

    private void checkForCollectables()
    {
        //TODO figure out why the sphere cast here no working
        //if (Physics.SphereCast(this.gameObject.transform.position, itemsCollectionRange, this.gameObject.transform.up * 1, 
        //    out RaycastHit hit, itemsCollectionRangeMaxDistance, CollectableItemsLayer, QueryTriggerInteraction.Collide))
        //{
        //    CollectableItem item = hit.collider.GetComponent<CollectableItem>();
        //    if (item != null)
        //    {
        //        Debug.Log("Item picked!");
        //        extractItemValue(item);
        //    }
        //    else
        //        Debug.Log("Item conversion failed!!!!!!!!!!!!!!!!!!!!!!");
        //}

        Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, 
            itemsCollectionRange);
        foreach (var c in colliders)
        {
            if (c.TryGetComponent<CollectableItem>(out CollectableItem item))
            {
                ExtractItemValue(item);
            }
        }
    }

    public void ExtractItemValue(CollectableItem item)
    {
        currency += item.ItemPicked();
        OnPlayerCurrencyChanged?.Invoke(this, new OnPlayerCurrencyChangedEventArgs() { currency = this.currency });
    }

    public int GetPlayerCurrency()
    {
        return (int)currency;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.gameObject.transform.position - 
            this.gameObject.transform.up * 1 * itemsCollectionRangeMaxDistance, itemsCollectionRange);
    }
}

[Serializable]
public struct ex1
{
    public int a;
    public int b;
}

[Serializable]
public struct ex2
{
    public int a;
    public int b;
}
