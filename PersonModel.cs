using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BCrypt.Net;
using BCr = BCrypt.Net;


namespace MVCSql.Models
{
    public class PersonModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        //[StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string password { get; set; }

        private static string GetRandomSalt()
        {
            return BCr.BCrypt.GenerateSalt(12); 
        }

        public static string HashPassword(string password)
        {
            return BCr.BCrypt.HashPassword(password, GetRandomSalt());
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            return BCr.BCrypt.Verify(password, correctHash);
        }
    }

 }
