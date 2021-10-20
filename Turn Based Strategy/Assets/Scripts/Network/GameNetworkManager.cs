using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        GameLogic.instance.AddPlayerToGameList(conn.identity);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        GameLogic.instance.RemovePlayerFromGameList(conn.identity);

    }
}
