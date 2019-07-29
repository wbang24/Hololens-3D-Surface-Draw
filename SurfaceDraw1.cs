using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class SurfaceDraw1 : MonoBehaviour, IInputHandler, IInputClickHandler
{
    public GameObject drawPrefab;
    private Transform drawTransform;
    private Vector3 cursorOriginalPos;
    public Color col;
    public GameObject cursor;
    //Won't cause issue
    static public Vector3 handPosition;

    void Start()
    {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
        cam = Camera.main;
        drawTransform = this.gameObject.transform;
        //cursorOriginalPos = this.transform.position + transform.forward;
    }

    private Camera cam;
    //To delegate whether or not to draw on click, we set isDraw to true of false. True for Drawing Active and false for Drawing not active
    bool isDraw = false;

    void Update()
    {
        //cursor.transform.position = handPosition;
        //If Drawing is active. We can now draw on a terra object   
        if (isDraw == true)
        {
            //cursor.transform.position = handPosition;
            Draw2D();
        }

        //If Drawing is inactive. We cannot draw on the terra object.
        if (isDraw == false)
        {
            //when the user let's go of the hold gesture, the drawPrefab, or draw position is reset to the location of the user. The cursor is also temporarily placed, floating in front of the user.
            drawPrefab.transform.position = Camera.main.transform.position;
            cursor.transform.position = Camera.main.transform.position + transform.forward;
            
        }


    }

    //Becuase of the node based approach to our rapid prototyping we set active or inactive some gameobjects. We have to Enable certain things again (same as Start()).
    void OnEnable() {
        InputManager.Instance.PushModalInputHandler(this.gameObject);
        cam = Camera.main;
        drawTransform = this.gameObject.transform;
        cursorOriginalPos = this.transform.position + transform.forward;


    }
    //We need tp disable certain gameobjects as well when this.gameobject is set to inactive.
    void OnDisable()
    {
        isDraw = false;
        cursorOriginalPos = this.transform.position + transform.forward;
    }

    //Draw2D searches for a drawable object tagged as Terra. The object must have a meshCollider and a Renderer.
    //Draw2D finds the position of a object in 3D space and translates the width and height of that 3D object's 2D png texture. 
    //It then applies coloured pixels on to that 3D object at the RaycastHit hit location
    void Draw2D()
    {

        RaycastHit hit;
        if ((Physics.Raycast(
           drawTransform.position,
           drawTransform.forward,
           out hit,
           Mathf.Infinity,
           Physics.DefaultRaycastLayers)))
        {

            Renderer rend = hit.transform.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;
            BoxCollider boxCollider = hit.transform.GetComponent<BoxCollider>();

            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                return;

            //cursor's position is defined here, as we only want it showing when Draw2D is activated.
            cursor.transform.position = hit.point;

            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width + drawTransform.position.x;
            pixelUV.y *= tex.height + drawTransform.position.y;

            Circle(tex, (int)pixelUV.x, (int)pixelUV.y, (int)2.5, col);

            tex.Apply();

        }
    }


    //Picking a colour based on tags the button has on. The raycast collision detects if that object is hit and then returns a Color
    //The default colour is black (we do not need to set this)
    public Color ColorPicked() {
        RaycastHit hit;
        if ((Physics.Raycast(
    Camera.main.transform.position,
    Camera.main.transform.forward,
    out hit,
    Mathf.Infinity,
    Physics.DefaultRaycastLayers)))
        {
            if (hit.collider.tag == "black")
            {
                col = Color.black;
                Debug.Log(col);
            }
            if (hit.collider.tag == "white")
            {
                col = Color.white;
                Debug.Log(col);

            }
            if (hit.collider.tag == "red")
            {
                col = Color.red;
                Debug.Log(col);

            }
            if (hit.collider.tag == "yellow")
            {
                col = Color.yellow;
                Debug.Log(col);

            }
            if (hit.collider.tag == "blue")
            {

                col = Color.blue;
                Debug.Log(col);

            }
            if (hit.collider.tag == "green")
            {
                col = Color.green;
                Debug.Log(col);

            }
            Debug.Log(col);

        }
        return col;
    }

    //Circle takes in a texture2D from our Draw2D dunction as well as int for x,y, and radius for the size of the circle.
    //Color col is returned by the ColourPicked function
    
    public void Circle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);

                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);
                //If you want transparency and database options. 
                // remove function for SetPixel
                //manipulate alpha channels using  etPixels32 instead
                //Save the coordinates to a JSON file

            }
        }
    }


    public void OnInputDown(InputEventData eventData)
    {

        eventData.InputSource.TryGetPointerPosition(eventData.SourceId, out handPosition);
        //drawTransform.position = handPosition;
        RaycastHit hit;
        Debug.Log("handPosition" + handPosition);
        if (handPosition != null)
        {
            if (Physics.Raycast(
           handPosition,
           this.transform.forward,
           out hit,
           Mathf.Infinity,
           Physics.DefaultRaycastLayers))
                {
                if (hit.collider.tag == "Terra") {
                    isDraw = true;
                    
                     }
                }
                else { 
                    isDraw = false;
                }
        }

        // else
        //{
        //    if (Physics.Raycast(

        //    Camera.main.transform.position,
        //    Camera.main.transform.forward,
        //    out hit,
        //    Mathf.Infinity,
        //    Physics.DefaultRaycastLayers))
        //    {
        //        if (hit.collider.tag == "Terra");
        //        isDraw = true;
        //    }
        //    else
        //    {
        //        isDraw = false;
        //    }

        //}
    }

    public void OnInputUp(InputEventData eventData)
    {
        isDraw = false;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        ColorPicked();
    }
}