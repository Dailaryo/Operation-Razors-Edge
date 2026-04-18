using UnityEngine;
using System.Collections.Generic;

public class UnitViewManager : MonoBehaviour
{
    public static UnitViewManager Instance;

    public GameObject unitViewPrefab;

    private Dictionary<Unit, UnitView> views = new Dictionary<Unit, UnitView>();

    void Awake()
    {
        Instance = this;
    }

    public void CreateView(Unit unit)
    {
        GameObject obj = Instantiate(unitViewPrefab);
        UnitView view = obj.GetComponent<UnitView>();

        view.Initialize(unit);

        views[unit] = view;

        //Subscribe to events
        unit.OnDeath += HandleUnitDeath;
    }

    public UnitView GetView(Unit unit)
    {
        return views.ContainsKey(unit) ? views[unit] : null;
    }

    void HandleUnitDeath(Unit unit)
    {
        if (!views.ContainsKey(unit)) return;

        Destroy(views[unit].gameObject);
        views.Remove(unit);
    }

    public void RefreshAllPositions()
    {
        foreach (var kvp in views)
        {
            kvp.Value.UpdatePosition();
        }
    }

    public void ClearAll()
    {
        foreach (var kvp in views)
        {
            Destroy(kvp.Value.gameObject);
        }

        views.Clear();
    }
}
