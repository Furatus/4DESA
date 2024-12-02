namespace AzureAPI.Services;

public class idService
{
    public static Guid ParseId(string id)
    {
        if (!Guid.TryParse(id, out Guid guid))
        {
            throw new ArgumentException("Invalid id format, please use GUID format.");
        }

        return guid;
    }
}