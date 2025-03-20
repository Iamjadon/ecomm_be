namespace ecomWebApi.DTOs
{
    public class ProductDTO
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int? Stock { get; set; }
        public bool? Published { get; set; }
        public string? ImgUrl { get; set; }

        public IFormFile? ImageFile { get; set; }


    }
}
