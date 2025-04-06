namespace StaticStorages;

public class RolesCollectionStorage
{
    public static string[] Roles { get; private set; } = ["Users.ChangePassword", "Transfers.Period", "Transfers.MostPopular"];
}