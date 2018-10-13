namespace Bindings
{
    public enum ServerPackets
    {
        SMessage = 1,
        SFullData = 2,
        SAckRegister = 3,
        SPlayerData = 4,
		SPulse = 5,
        SGalaxy = 6,
        SItems = 7,
        SInventory = 8,
        SPlayerUpdate = 9
    }

    public enum ClientPackets
    {
        CLogin = 1,
        CRegister = 2,
        CPlayerData = 3,
        CChat = 4,
        CCombat = 5,
        CItemStack = 6,
        CItemTransaction = 7,
        CEquipItem = 8,
        CItemSale = 9,
        CLootTransaction
    }

    public enum MessageColors
    {
        Chat = 1,
        Warning = 2,
        Notification = 3,
        Minor = 4
    }
}
