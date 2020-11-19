using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Variables which the client needs to send to the server
/// </summary>
public class ClientState
{
    public Vector2Int dest = new Vector2Int();

    public void CopyFrom(ClientState state)
    {
        dest = state.dest;
    }
}
