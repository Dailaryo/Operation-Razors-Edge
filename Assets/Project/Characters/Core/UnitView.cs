using UnityEngine;

public class UnitView :MonoBehaviour
{
    public Unit boundUnit;

    public GameObject labelPrefab;
    public Transform labelAnchor;

    public void Initialize(Unit unit)
    {
        boundUnit = unit;
        UpdatePosition();
        CreateLabel();
    }

    public void UpdatePosition()
    {
        Vector3 worldPos = GridToWorld(boundUnit.position);
        transform.position = worldPos;
    }

    void CreateLabel()
    {
        if (labelPrefab == null)
        {
            Debug.LogWarning("LabelPrefab is not assigned!");
            return;
        }

        if (labelAnchor == null)
        {
            Debug.LogWarning("LabelAnchor is not assigned!");
            return;
        }

        GameObject labelObj = Instantiate(labelPrefab, labelAnchor);
        labelObj.transform.localPosition = Vector3.zero;

        UnitLabel label = labelObj.GetComponent<UnitLabel>();

        if (label != null)
        {
            label.Initialize(boundUnit);
        }
        else
        {
            Debug.LogWarning("UnitLabel component missing on prefab!");
        }
    }

    Vector3 GridToWorld(Vector2Int pos)
    {
        float size = GridVisualizer.Instance.tileSize;
        return new Vector3(pos.x * size, 0, pos.y * size);
    }
}
