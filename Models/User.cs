namespace UserRegistryAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
        public int CountryId { get; set; }
        public int DepartmentId { get; set; }
        public int MunicipalityId { get; set; }
    }
}
