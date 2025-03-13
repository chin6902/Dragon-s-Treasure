using System.Collections;
using UnityEngine;

public class Temple2 : Interactable
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

        if (GameManager.Instance.BlueCrystalCount >= count)
        {
            GameManager.Instance.BlueCrystalCount -= count;
            GameManager.Instance.Temple2_clear = true;
        }
    }

}