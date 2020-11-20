using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Variables which the client needs to send to the server
/// </summary>
public class ClientState
{
    public Vector2Int Dest = new Vector2Int();
    public ProtectPrayer ProtectPrayer;

    public void CopyFrom(ClientState state)
    {
        Dest = state.Dest;
        ProtectPrayer = state.ProtectPrayer;
    }
}
