
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    public static CameraController instance {get; private set; }

    /// <summary>
    /// Speeding of Draggin movement
    /// </summary>
    public float dragSpeed = 20f;

    /// <summary>
    /// Zomm in , Zomm out sensitivity
    /// </summary>
    public float scrollSpeed = 0.5f;

    /// <summary>
    /// Origin from which the dragging has began
    /// </summary>
    private Vector3 dragOrigin;

    /// <summary>
    /// To store Reference of main Camera in scence
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Texture of the hand to be used when draggin
    /// </summary>
    private static Texture2D _hand = null;

    /// <summary>
    /// true if draggin 
    /// </summary>
    private bool isDrag = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
        if(_hand == null)
            _hand = Resources.Load("hand", typeof(Texture2D)) as Texture2D;

    }

    void Update()
    {
        if (ShortInput.GetKey("Normal"))
        {
            Fit();
        }

        if (Input.mouseScrollDelta.y != 0 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            cam.orthographicSize -= scrollSpeed * Input.mouseScrollDelta.y;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isDrag = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            isDrag = true;
            Cursor.SetCursor(_hand, Vector2.zero, CursorMode.ForceSoftware);

        }

        if (isDrag)
        {
            
            Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed * -1, pos.y * dragSpeed * -1, 0);

            transform.Translate(move, Space.World);
            dragOrigin = Input.mousePosition;
        }

        

        

        #region "Limits"
        if (transform.position.x > 25)
            transform.position = new Vector3(25, transform.position.y, transform.position.z);
        if (transform.position.x < -25)
            transform.position = new Vector3(-25, transform.position.y, transform.position.z);
        if (transform.position.y > 25)
            transform.position = new Vector3(transform.position.x,25, transform.position.z);
        if (transform.position.y < -25)
            transform.position = new Vector3(transform.position.x, -25, transform.position.z);
        if (cam.orthographicSize > 20)
            cam.orthographicSize = 20;
        if (cam.orthographicSize < 8)
            cam.orthographicSize = 8;

        #endregion
    }

    /// <summary>
    /// Fit The zoom to screen
    /// </summary>
    public void Fit()
    {
        transform.position = new Vector3(0, 0, transform.position.z);
        cam.orthographicSize = 10f;
    }

    /// <summary>
    /// Zoom in or out
    /// </summary>
    /// <param name="z">Zomm value , +ve for zoom in and -ve for zoom out</param>
    public void Zoom (int z)
    {
        cam.orthographicSize -= z;
    }
}
