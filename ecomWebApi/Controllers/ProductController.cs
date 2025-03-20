using Microsoft.AspNetCore.Mvc;
using ecomWebApi.Models;
using ecomWebApi.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ecomWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext dbContext;

        private readonly IWebHostEnvironment _env;
        private static List<Product> _products = new List<Product>();

        //public ProductsController(IWebHostEnvironment env)
        //{
        //    _env = env;
        //}

        //public ProductsController(MyDbContext dbContext)
        //{
        //    this.dbContext = dbContext;
        //}

        public ProductsController(MyDbContext dbContext, IWebHostEnvironment env)
        {
            this.dbContext = dbContext;
            _env = env;
        }

        //  Get all products
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(dbContext.Products.ToList());
        }

        //  Get single product by ID
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = dbContext.Products.Find(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // Add a product
        [HttpPost]
        public IActionResult AddProduct([FromForm] ProductDTO productDto)
        {
            if (string.IsNullOrEmpty(productDto.ImgUrl) && productDto.ImageFile == null)
            {
                return BadRequest(new { message = "Either image file or image URL is required." });
            }

            // Process the image file if uploaded
            if (productDto.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + productDto.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                productDto.ImgUrl = "/uploads/" + uniqueFileName; // Save the image path
            }

            // Map DTO to Entity
            Product newProduct = new Product
            {
                Name = productDto.Name,
                Price = (decimal)productDto.Price,
                SalePrice = productDto.SalePrice,
                Stock = (int)productDto.Stock,
                Published = (bool)productDto.Published,
                ImgUrl = productDto.ImgUrl ?? "wwwroot"
            };

            // Save to Database
            dbContext.Products.Add(newProduct);
            dbContext.SaveChanges();

            return Ok(new { message = "Product added successfully", product = newProduct });
        }


       
        //  Update a product
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromForm] ProductDTO productDto)
        {
            var product = dbContext.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            // Ensure either an image file or an image URL is provided
            if (string.IsNullOrEmpty(productDto.ImgUrl) && productDto.ImageFile == null)
            {
                return BadRequest(new { message = "Either image file or image URL is required." });
            }

            // Process the image file if uploaded
            if (productDto.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + productDto.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                product.ImgUrl = "/uploads/" + uniqueFileName; // Update image path
            }
            else if (!string.IsNullOrEmpty(productDto.ImgUrl))
            {
                product.ImgUrl = productDto.ImgUrl; // Use provided image URL
            }

            // Update only if the values are not null
            if (!string.IsNullOrEmpty(productDto.Name))
            {
                product.Name = productDto.Name;
            }
            if (productDto.Price.HasValue)
            {
                product.Price = productDto.Price.Value;
            }
            if (productDto.SalePrice.HasValue)
            {
                product.SalePrice = productDto.SalePrice.Value;
            }
            if (productDto.Stock.HasValue)
            {
                product.Stock = productDto.Stock.Value;
            }
            if (productDto.Published.HasValue)
            {
                product.Published = productDto.Published.Value;
            }

            dbContext.SaveChanges();

            return Ok(new { message = "Product updated successfully", product });
        }


        //  Delete a product
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = dbContext.Products.Find(id);
            if (product == null) return NotFound();

            dbContext.Products.Remove(product);
            dbContext.SaveChanges();
            return NoContent();
        }
    }
}
