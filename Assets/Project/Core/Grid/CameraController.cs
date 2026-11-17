using UnityEngine;
using UnityEngine.EventSystems;

//Player-controlled combat camera: mouse wheel zooms, drag pans across the grid.
//Lives on the Main Camera. GridVisualizer calls ResetView() whenever a map is drawn.
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public enum DragButton { Left = 0, Right = 1, Middle = 2 }

    [Header("Pan")]
    //Right mouse already cancels targeting and planned actions, so Middle avoids the conflict
    [SerializeField] private DragButton dragButton = DragButton.Middle;
    //How far past the map edge (world units) the centre of the view may travel
    [SerializeField] private float edgeMargin = 2f;

    [Header("Zoom (orthographic size smaller = closer)")]
    [SerializeField] private float defaultZoom = 6f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 12f;
    [SerializeField] private float zoomStep = 0.75f;

    [Header("Reset")]
    [SerializeField] private KeyCode resetKey = KeyCode.Home;

    //How far the camera sits back from the point it looks at. Any value inside the clip planes works
    //for an orthographic camera; it only has to keep the camera above the grid
    private const float FocusDistance = 20f;

    private Camera cam;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private bool isDragging = false;
    private Vector3 dragAnchor; //ground point that stays under the cursor while dragging

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleZoom();
        HandlePan();

        if (Input.GetKeyDown(resetKey))
            ResetView();
    }

    //Centres the view on the current map at the default zoom. Called after each grid draw
    public void ResetView()
    {
        cam.orthographicSize = defaultZoom;
        SetFocus(GetGridCentre());
    }

    void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scroll, 0f)) return;
        if (IsPointerOverUI()) return; //let UI scroll views keep the wheel

        //Zoom toward the cursor: the ground point under it stays put
        bool hadPoint = TryGetGroundPoint(Input.mousePosition, out Vector3 before);

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomStep, minZoom, maxZoom);

        if (hadPoint && TryGetGroundPoint(Input.mousePosition, out Vector3 after))
            transform.position += before - after;

        ClampFocus();
    }

    void HandlePan()
    {
        int button = (int)dragButton;

        if (Input.GetMouseButtonDown(button) && !IsPointerOverUI())
            isDragging = TryGetGroundPoint(Input.mousePosition, out dragAnchor);

        if (Input.GetMouseButtonUp(button))
            isDragging = false;

        if (!isDragging) return;

        //Slide the camera so the grabbed ground point stays under the cursor
        if (TryGetGroundPoint(Input.mousePosition, out Vector3 current))
        {
            transform.position += dragAnchor - current;
            ClampFocus();
        }
    }

    //Keeps the centre of the view over the map (plus edgeMargin) so the grid can't be dragged off-screen
    void ClampFocus()
    {
        if (GridManager.Instance == null) return;
        if (!TryGetGroundPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f), out Vector3 focus)) return;

        float tileSize = GetTileSize();
        float maxX = (GridManager.Instance.width - 1) * tileSize;
        float maxZ = (GridManager.Instance.height - 1) * tileSize;

        focus.x = Mathf.Clamp(focus.x, -edgeMargin, maxX + edgeMargin);
        focus.z = Mathf.Clamp(focus.z, -edgeMargin, maxZ + edgeMargin);

        SetFocus(focus);
    }

    void SetFocus(Vector3 focus)
    {
        transform.position = focus - transform.forward * FocusDistance;
    }

    Vector3 GetGridCentre()
    {
        if (GridManager.Instance == null) return Vector3.zero;

        float tileSize = GetTileSize();
        return new Vector3(
            (GridManager.Instance.width - 1) * tileSize * 0.5f,
            0f,
            (GridManager.Instance.height - 1) * tileSize * 0.5f);
    }

    float GetTileSize()
    {
        return GridVisualizer.Instance != null ? GridVisualizer.Instance.tileSize : 1f;
    }

    bool TryGetGroundPoint(Vector3 screenPosition, out Vector3 point)
    {
        Ray ray = cam.ScreenPointToRay(screenPosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            point = ray.GetPoint(distance);
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
