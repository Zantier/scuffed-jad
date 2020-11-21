using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    const float squareWidth = 1.0f;
    const float tickLength = 0.6f;
    const float latency = 0.1f;
    // Camera
    float eulerY = 0;
    float eulerZ = 0;
    float cameraDist = 5;
    // In square position coordinates
    Vector2Int playerPos = new Vector2Int();
    float nextTickTime = 0;
    // The next positions the player will be at, at each tick
    List<Vector2Int> playerRoute = new List<Vector2Int>();
    /// <summary>
    /// Always has the latest client state
    /// </summary>
    ClientState clientState = new ClientState();
    ClientState preServerState = new ClientState();
    ClientState serverState = new ClientState();

    public UI UI;
    public Camera Camera;
    public Text TopLeftText;
    public Transform CameraTransform;
    public Transform PlayerTransform;
    public LineRenderer LineRenderer;
    public Jad Jad;

    public void ClickProtectMagic()
    {
        ClickPrayer(ProtectPrayer.Magic);
    }
    public void ClickProtectRanged()
    {
        ClickPrayer(ProtectPrayer.Ranged);
    }
    public void ClickProtectMelee()
    {
        ClickPrayer(ProtectPrayer.Melee);
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.pixelRect = new Rect(0, 211, Screen.width-311, Screen.height-211);
        Tick();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UI.ShowPrayerPanel(false);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            UI.ShowPrayerPanel(true);
        }

        float angleChangeY = 100f * Time.deltaTime;
        float angleChangeZ = 30f * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            eulerY += angleChangeY;
        }
        if (Input.GetKey(KeyCode.D))
        {
            eulerY -= angleChangeY;
        }
        if (Input.GetKey(KeyCode.W))
        {
            eulerZ += angleChangeZ;
        }
        if (Input.GetKey(KeyCode.S))
        {
            eulerZ -= angleChangeZ;
        }
        eulerZ = Mathf.Clamp(eulerZ, 10, 50);

        cameraDist -= 0.1f * Input.mouseScrollDelta.y;
        cameraDist = Mathf.Clamp(cameraDist, 2, 10);

        RaycastHit hit;
        bool didHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
        didHit = didHit && Camera.pixelRect.Contains(Input.mousePosition);
        TopLeftText.text = "";
        if (didHit)
        {
            if (hit.collider.name == "Jad")
            {
                TopLeftText.text = "Attack TzTok-Jad (level 702)";
            }
            else
            {
                TopLeftText.text = "Walk Here";
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            if (didHit && hit.collider.name == "Ground")
            {
                int squareX = (int)Mathf.Floor(hit.point.x / squareWidth);
                int squareY = (int)Mathf.Floor(hit.point.z / squareWidth);
                Vector3[] positions = {
                    new Vector3(squareX * squareWidth, 0.2f, squareY * squareWidth), new Vector3((squareX + 1) * squareWidth, 0.2f, squareY * squareWidth),
                    new Vector3((squareX + 1) * squareWidth, 0.2f, (squareY + 1) * squareWidth), new Vector3(squareX * squareWidth, 0.2f, (squareY + 1) * squareWidth)
                };
                LineRenderer.SetPositions(positions);
                LineRenderer.enabled = true;
                clientState.Dest = new Vector2Int(squareX, squareY);
            }
        }

        // If the server can receive the data before the next tick
        if (Time.time + latency < nextTickTime)
        {
            preServerState.CopyFrom(clientState);
        }

        if (Time.time >= nextTickTime)
        {
            Tick();
        }

        float playerX = playerPos.x;
        float playerY = playerPos.y;
        if (playerRoute.Count > 0)
        {
            float fraction = (tickLength + Time.time - nextTickTime) / tickLength;
            playerX = (1 - fraction) * playerPos.x + fraction * playerRoute[0].x;
            playerY = (1 - fraction) * playerPos.y + fraction * playerRoute[0].y;
        }
        PlayerTransform.position = new Vector3((playerX + 0.5f) * squareWidth, 1, (playerY + 0.5f) * squareWidth);
        CameraTransform.localPosition = Quaternion.Euler(0, eulerY, eulerZ) * new Vector3(cameraDist, 0, 0);
        CameraTransform.LookAt(PlayerTransform);
        Jad.transform.LookAt(new Vector3(PlayerTransform.position.x, Jad.transform.position.y, PlayerTransform.position.z));
        UI.SetClientProtectPrayer(clientState.ProtectPrayer);
        Vector2 overheadPos = Camera.main.WorldToScreenPoint(PlayerTransform.position + new Vector3(0, 1.8f, 0));
        UI.SetServerProtectPrayer(serverState.ProtectPrayer, overheadPos);
    }

    void FixedUpdate()
    {
        //PlayerTransform.localPosition += new Vector3(0.01f, 0, 0);
    }

    private void Tick()
    {
        nextTickTime = Time.time + tickLength;
        serverState.CopyFrom(preServerState);

        if (playerRoute.Count > 0)
        {
            playerPos = playerRoute[0];
        }

        playerRoute.Clear();
        if (playerPos != serverState.Dest)
        {
            int posX = playerPos.x;
            int posY = playerPos.y;
            while (posX != serverState.Dest.x || posY != serverState.Dest.y)
            {
                posX += Sign(serverState.Dest.x - posX);
                posY += Sign(serverState.Dest.y - posY);
                playerRoute.Add(new Vector2Int(posX, posY));
            }
        }

        ProtectPrayer damage = Jad.Tick(playerPos);
        if (damage != ProtectPrayer.None)
        {
            int newHealth = damage == serverState.ProtectPrayer ? 1 : 0;
            UI.SetHealth(newHealth);
        }

        preServerState.CopyFrom(clientState);
    }
    private void ClickPrayer(ProtectPrayer prayer)
    {
        if (clientState.ProtectPrayer == prayer)
        {
            clientState.ProtectPrayer = ProtectPrayer.None;
        }
        else
        {
            clientState.ProtectPrayer = prayer;
        }
    }

    private int Sign(int x)
    {
        return (x > 0 ? 1 : 0) - (x < 0 ? 1 : 0);
    }
}
