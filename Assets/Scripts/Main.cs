using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    const float squareWidth = 1.5f;
    const float tickLength = 0.6f;
    const float latency = 0.1f;
    // Camera
    float eulerY = 0;
    float eulerZ = 0;
    // In square position coordinates
    Vector2Int playerPos = new Vector2Int();
    float nextTickTime = 0;
    // The next positions the player will be at, at each tick
    List<Vector2Int> playerRoute = new List<Vector2Int>();
    ClientState serverState = new ClientState();
    /// <summary>
    /// Always has the latest client state
    /// </summary>
    ClientState clientState = new ClientState();

    public Transform CameraTransform;
    public Transform PlayerTransform;
    public LineRenderer LineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Tick();
    }

    // Update is called once per frame
    void Update()
    {
        float angleChange = 50f * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            eulerY += angleChange;
        }
        if (Input.GetKey(KeyCode.D))
        {
            eulerY -= angleChange;
        }
        if (Input.GetKey(KeyCode.W))
        {
            eulerZ += angleChange;
        }
        if (Input.GetKey(KeyCode.S))
        {
            eulerZ -= angleChange;
        }
        eulerZ = Mathf.Clamp(eulerZ, 10, 50);

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                int squareX = (int)Mathf.Floor(hit.point.x / squareWidth);
                int squareY = (int)Mathf.Floor(hit.point.z / squareWidth);
                Vector3[] positions = {
                    new Vector3(squareX * squareWidth, 0.2f, squareY * squareWidth), new Vector3((squareX + 1) * squareWidth, 0.2f, squareY * squareWidth),
                    new Vector3((squareX + 1) * squareWidth, 0.2f, (squareY + 1) * squareWidth), new Vector3(squareX * squareWidth, 0.2f, (squareY + 1) * squareWidth)
                };
                LineRenderer.SetPositions(positions);
                LineRenderer.enabled = true;
                clientState.dest = new Vector2Int(squareX, squareY);
            }
        }

        // If the server can receive the data before the next tick
        if (Time.time + latency < nextTickTime)
        {
            serverState.CopyFrom(clientState);
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
        CameraTransform.localPosition = Quaternion.Euler(0, eulerY, eulerZ) * new Vector3(5, 0, 0);
        CameraTransform.LookAt(PlayerTransform);
    }

    void FixedUpdate()
    {
        //PlayerTransform.localPosition += new Vector3(0.01f, 0, 0);
    }

    private void Tick()
    {
        nextTickTime = Time.time + tickLength;

        if (playerRoute.Count > 0)
        {
            playerPos = playerRoute[0];
        }

        playerRoute.Clear();
        if (playerPos != serverState.dest)
        {
            int posX = playerPos.x;
            int posY = playerPos.y;
            while (posX != serverState.dest.x || posY != serverState.dest.y)
            {
                posX += Sign(serverState.dest.x - posX);
                posY += Sign(serverState.dest.y - posY);
                playerRoute.Add(new Vector2Int(posX, posY));
            }
        }

        serverState.CopyFrom(clientState);
    }

    private int Sign(int x)
    {
        if (x < 0)
        {
            return -1;
        }

        if (x > 0)
        {
            return 1;
        }

        return 0;
    }
}
