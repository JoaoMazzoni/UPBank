namespace AddressAPI.Utilis
{
    public class ProjMongoDBAPIDataBaseSettings : IProjMongoDBAPIDataBaseSettings
    {
        public string AddressCollectionName { get; set; }
        public string RemovedAddressCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
