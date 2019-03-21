using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBuildable : ItemAimable
{
    public new ItemBuildableData Data { get { return (ItemBuildableData)data; } }
    private GameObject spawnedItem;
    private Vector2 spawnSize;
    private bool overlapped;
    private bool lastCheck;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        PositionSpawnedItem();
        CheckOverlap();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SpawnItem();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (spawnedItem)
            Destroy(spawnedItem);
    }

    protected override void OnRunItem()
    {
        base.OnRunItem();
        PlaceSpawnedItem();
    }

    void PlaceSpawnedItem()
    {
        if (overlapped)
            return;
        Instantiate(Data.placedPrefab, controller.AimPos, Quaternion.identity);
    }

    void CheckOverlap()
    {
        if (!spawnedItem)
            return;

        overlapped = Physics2D.OverlapBox(controller.AimPos, spawnSize, 0, Data.overlapMask);

        if (overlapped != lastCheck)
        {
            SetSpawnMaterials(overlapped);
            lastCheck = overlapped;
        }

    }

    void PositionSpawnedItem()
    {
        if (!spawnedItem)
            return;
        spawnedItem.transform.position = controller.AimPos;
    }

    void SpawnItem()
    {
        if (spawnedItem)
            Destroy(spawnedItem);

        if (!controller)
            return;

        spawnedItem = Instantiate(Data.previewPrefab, controller.AimPos, Quaternion.identity);
        spawnSize = spawnedItem.GetComponent<Collider2D>().bounds.size;
        spawnedItem.GetComponent<Collider2D>().enabled = false;
        SetSpawnMaterials(overlapped);
    }

    void SetSpawnMaterials(bool _invalid)
    {
            var mat = Data.validMaterial;
            if (_invalid)
                mat = Data.invalidMaterial;

        Utils.SetAllChildMaterials(spawnedItem.transform, mat);
    }

}
