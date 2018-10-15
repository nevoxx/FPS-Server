namespace FPS_Bindings
{
    public enum ServerPackets
    {
        SIngame = 1,
        SPlayerData,
        SPlayerMove,
        SPlayerDisconnect,
    }

    public enum ClientPackets
    {
        CNewAccount,
        CLogin,
        CMovement,
    }
}
