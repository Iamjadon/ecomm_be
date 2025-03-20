namespace ecomWebApi.DTOs
{
    public class AddressDTO
    {
        public int AddressId { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Pincode { get; set; }
        public string FlatHouseNo { get; set; }
        public string AreaStreet { get; set; }
        public string Landmark { get; set; }
        public string TownCity { get; set; }
        public int UserId { get; set; }
    }
}
