using System.Collections;
using UnityEngine;

public class Temple1 : Interactable
{
    [Header("Item Data")]
    [SerializeField] private string itemName;
    [SerializeField] private GameObject ParentObject;
    [SerializeField] private float count;

    public override void Start()
    {
        base.Start();
        interactableName = itemName;
    }

    protected override void Interaction()
    {
        base.Interaction();

        if(GameManager.Instance.RedCrystalCount >= count )
        {
            GameManager.Instance.RedCrystalCount -= count;
            GameManager.Instance.Temple1_clear = true;
        }
    }

}