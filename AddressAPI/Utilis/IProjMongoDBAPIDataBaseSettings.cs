namespace AddressAPI.Utilis
{
    public interface IProjMongoDBAPIDataBaseSettings
    {
        string AddressCollectionName { get; set; }
        string RemovedAddressCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
