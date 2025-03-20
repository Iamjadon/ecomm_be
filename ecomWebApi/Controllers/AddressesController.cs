using Microsoft.AspNetCore.Mvc;
using ecomWebApi.Models;
using ecomWebApi.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace ecomWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly MyDbContext _dbContext;

        public AddressesController(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //  Get all addresses
        [HttpGet("GetAllAdresses")]
        public IActionResult GetAddresses()
        {
            var addresses = _dbContext.Addresses.Include(a => a.User).ToList();
            return Ok(addresses);
        }

        //  Get all address by ID
        [HttpGet("GetAddressById/{userId}")]
        public IActionResult GetAddress(int userId)
        {
            var addresses = _dbContext.Addresses
                                      .Where(a => a.UserId == userId)
                                      .Select(a => new AddressDTO
                                      {
                                          AddressId = a.AddressId,
                                          FullName = a.FullName,
                                          MobileNumber = a.MobileNumber,
                                          Pincode = a.Pincode,
                                          FlatHouseNo = a.FlatHouseNo,
                                          AreaStreet = a.AreaStreet,
                                          Landmark = a.Landmark,
                                          TownCity = a.TownCity,
                                          UserId = a.UserId
                                      })
                                      .ToList();

            if (addresses == null || addresses.Count == 0)
                return NotFound(new { message = "No addresses found for this user" });

            return Ok(addresses);
        }

        // Get single address by addressId
        [HttpGet("GetSingleAddressById/{addressId}")]
        public IActionResult GetSingleAddress(int addressId)
        {
            var address = _dbContext.Addresses
                                    .Where(a => a.AddressId == addressId)
                                    .Select(a => new AddressDTO
                                    {
                                        AddressId = a.AddressId,
                                        FullName = a.FullName,
                                        MobileNumber = a.MobileNumber,
                                        Pincode = a.Pincode,
                                        FlatHouseNo = a.FlatHouseNo,
                                        AreaStreet = a.AreaStreet,
                                        Landmark = a.Landmark,
                                        TownCity = a.TownCity,
                                        UserId = a.UserId
                                    })
                                    .FirstOrDefault();

            if (address == null)
            {
                return NotFound(new { message = "Address not found" });
            }

            return Ok(address);
        }


        //  Add a new address
        [HttpPost("AddAddress")]
        public IActionResult AddAddress([FromBody] AddressDTO addressDto)
        {
            // Check if user exists
            var userExists = _dbContext.Users.Any(u => u.UserId == addressDto.UserId);
            if (!userExists)
                return BadRequest(new { message = "Invalid UserId. User does not exist." });

            // Map DTO to Entity
            var newAddress = new Address
            {
                FullName = addressDto.FullName,
                MobileNumber = addressDto.MobileNumber,
                Pincode = addressDto.Pincode,
                FlatHouseNo = addressDto.FlatHouseNo,
                AreaStreet = addressDto.AreaStreet,
                Landmark = addressDto.Landmark,
                TownCity = addressDto.TownCity,
                UserId = addressDto.UserId
            };

            _dbContext.Addresses.Add(newAddress);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetAddress), new { userId = newAddress.AddressId }, newAddress);

        }

        //  Update an address
        [HttpPut("UpdateAddressById/{id}")]
        public IActionResult UpdateAddress(int id, [FromBody] AddressDTO addressDto)
        {
            var existingAddress = _dbContext.Addresses.FirstOrDefault(a => a.AddressId == id);
            if (existingAddress == null) return NotFound(new { message = "Address not found" });

            existingAddress.FullName = addressDto.FullName;
            existingAddress.MobileNumber = addressDto.MobileNumber;
            existingAddress.Pincode = addressDto.Pincode;
            existingAddress.FlatHouseNo = addressDto.FlatHouseNo;
            existingAddress.AreaStreet = addressDto.AreaStreet;
            existingAddress.Landmark = addressDto.Landmark;
            existingAddress.TownCity = addressDto.TownCity;

            _dbContext.SaveChanges();
            return Ok(new { message = "Address updated successfully", address = existingAddress });
        }

        //  Delete an address
        [HttpDelete("DeleteAddressById/{id}")]
        public IActionResult DeleteAddress(int id)
        {
            var address = _dbContext.Addresses.Find(id);
            if (address == null) return NotFound(new { message = "Address not found" });

            _dbContext.Addresses.Remove(address);
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
