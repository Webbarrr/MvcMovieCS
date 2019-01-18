using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MvcMovie.Data
{
    public class AuthRepository : IAuthRepository
    {
        #region Constructor
        public AuthRepository(MvcMovieContext context)
        {
            _context = context;
        }
        #endregion

        #region Containers
        private readonly MvcMovieContext _context;
        #endregion

        #region Public Methods
        // registers a user
        public async Task<User> Register(User user, string password)
        {
            // call the create password hash method to generate to byte arrays - the hash & salt for the password
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            // assign them to the user object
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // add them to the list
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // checks to see if a user with that username already exists
        public async Task<bool> UserExists(string username)
        {
            return (await _context.User.AnyAsync(x => x.Username == username));
        }

        // login
        public async Task<User> Login(string username, string password)
        {
            // create a user, if they don't exist exit out
            var user = await _context.User.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null) return null;

            // compare the given password with the hash & salt stored in the db, if it doesn't return null
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;

            // success
            return user;
        }
        #endregion

        #region Helper Methods
        // generates a pass password hash & salt
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        // compares a password hash & salt
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                // compute a new hash to test the existing password
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // check if the hashes match
                for (int i = 0; i < computedHash.Length - 1; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }

            // success
            return true;
        }
        #endregion
    }
}
