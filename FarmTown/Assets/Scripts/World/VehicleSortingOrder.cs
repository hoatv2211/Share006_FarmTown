using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 previousPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousPosition = transform.position;
        UpdateSortingOrder();
    }

    void Update()
    {
        if (transform.position == previousPosition)
            return;

        previousPosition = transform.position;
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        Vector3 temp = transform.TransformDirection(transform.position);
        spriteRenderer.sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f);
    }
}

