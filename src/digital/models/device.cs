namespace digital
{
    // TODO: Need to fixup ids in models to be Guids (were approriate)
    // Then we can change upstream apis like DescriptionExtensions
    public class DeviceModel
    {
        public string ConnectionString { get; set; }
        public string HardwareId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string SpaceId { get; set; }
    }
}