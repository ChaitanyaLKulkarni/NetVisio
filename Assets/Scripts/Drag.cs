using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour {

    public GameObject DeleteObj;

    private bool Selected = false;
    private Vector2 prev;
    private Info info;

    private static Texture2D _block = null;
    private SpriteRenderer sp;
    private bool isPlay = false;

    void Start()
    {
        DeleteObj = Manager.Instance.DeleteObj;
        info = GetComponent<Info>();
        sp = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if(isPlay != ReportManager.instance.isStarted)
        {
            isPlay = ReportManager.instance.isStarted;
            SetPlay();
        }
        if (isPlay && GetComponent<Disturbance>() == null)
            return;

        if (Selected)
        {
            
            if (Input.GetMouseButtonUp(0))
            {
                DeleteObj.SetActive(false);
                Selected = false;
                
            }

            info.Moved();
            Vector2 mousepos = Input.mousePosition;
            
            if (EventSystem.current.IsPointerOverGameObject())
            {

                return;
            }
            Vector2 pos = Camera.main.ScreenToWorldPoint(mousepos);
            transform.position = new Vector2(pos.x, pos.y);
        }
    }

    void SetPlay()
    {
        if (isPlay)
        {
            DeleteObj.SetActive(false);
            Selected = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Remove"))
        {
            Selected = false;
            GenericDialog dialog = GenericDialog.Instance();
            dialog.SetTitle("Remove Component!");
            dialog.SetMessage("Do you want to remove this component?");
            dialog.SetOnAccept("Yes", () => {
                gameObject.GetComponent<Info>().Deleted();
                Destroy(gameObject);
                dialog.Hide();
            });
            dialog.SetOnDecline("No", () =>
            {
                transform.position = prev;
                dialog.Hide();
            });
            dialog.Show();
            DeleteObj.SetActive(false);
        }
    }

    public void Delete()
    {
        Selected = false;
        GenericDialog dialog = GenericDialog.Instance();
        dialog.SetTitle("Remove Component!");
        dialog.SetMessage("Do you want to remove this component?");
        dialog.SetOnAccept("Yes", () => {
            gameObject.GetComponent<Info>().Deleted();
            Destroy(gameObject);
            dialog.Hide();
        });
        dialog.SetOnDecline("No", () =>
        {
            transform.position = prev;
            dialog.Hide();
        });
        dialog.Show();
        DeleteObj.SetActive(false);
    }

    void OnMouseOver()
    {
        if (isPlay && GetComponent<Disturbance>() == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            prev = transform.position;
            DeleteObj.SetActive(true);
            Selected = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            string ClassName = GetComponent<Info>().ClassName;
            System.Type mType = System.Type.GetType(ClassName);
            BaseComp comp = (gameObject.GetComponent(mType) as BaseComp);
            ShowProperties.Instance.ShowPorp(ref comp);
        }
    }
    
}