namespace Data
{
    public enum PrefabKey
    {
        None = 0,
        UIFinishOverlay,
        UIContentPopup,
        UICarsPopupItem,
        UISettingsPopup,
        UIBankPopup,
        UIMultiplayerRootPopup,
        CarBug = CarKey.Bug + Constants.PrefabCarsOffset,
        CarCharger = CarKey.Charger + Constants.PrefabCarsOffset,
        CarCoupe = CarKey.Coupe + Constants.PrefabCarsOffset,
        CarDeliveryVan = CarKey.DeliveryVan + Constants.PrefabCarsOffset,
        CarDuneBuggy = CarKey.DuneBuggy + Constants.PrefabCarsOffset,
        CarFormula = CarKey.Formula + Constants.PrefabCarsOffset,
    }
}