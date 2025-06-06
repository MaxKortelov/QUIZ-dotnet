using System;
using System.Security.Cryptography;
using System.Text;

public class CryptoService
{
    public class EncryptedPassword
    {
        public string Salt { get; set; }
        public byte[] Hash { get; set; }
        
        public void Deconstruct(out byte[] hash, out string salt)
        {
            hash = Hash;
            salt = Salt;
        }
    }

    // Encrypt password with generated salt and PBKDF2-SHA512
    public EncryptedPassword EncryptPassword(string password)
    {
        // Generate a 16-byte salt
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        string salt = Convert.ToHexString(saltBytes); // .NET 5+; else use BitConverter.ToString(saltBytes).Replace("-", "")

        // Derive a 64-byte key using PBKDF2 with SHA512, 1000 iterations
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 1000, HashAlgorithmName.SHA512);
        byte[] hash = pbkdf2.GetBytes(64);

        return new EncryptedPassword
        {
            Salt = salt,
            Hash = hash
        };
    }

    public bool ValidatePassword(string password, byte[] storedHash, string storedSalt)
    {
        byte[] saltBytes = Convert.FromHexString(storedSalt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 1000, HashAlgorithmName.SHA512);
        byte[] newHash = pbkdf2.GetBytes(64);

        return CryptographicOperations.FixedTimeEquals(storedHash, newHash);
    }

    public string UniqueId()
    {
        return Guid.NewGuid().ToString();
    }
}