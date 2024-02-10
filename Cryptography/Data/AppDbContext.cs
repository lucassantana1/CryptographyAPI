using Cryptography.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Cryptography.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Transactions = Set<Transaction>();
        }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().Property(e => e.UserDocument).HasConversion(
                v => Encrypt(v),
                v => Decrypt(v)
            );

            modelBuilder.Entity<Transaction>().Property(e => e.CreditCardToken).HasConversion(
                v => Encrypt(v),
                v => Decrypt(v)
            );

            base.OnModelCreating(modelBuilder);
        }

        private static readonly byte[] Key = new byte[] { 0x8B, 0x4D, 0x95, 0xA1, 0x62, 0xF3, 0x4A, 0x4D, 0xA1, 0xCA, 0xB3, 0xF4, 0xD4, 0xE3, 0xE1, 0xE7 };
        private static readonly byte[] IV = new byte[] { 0x8E, 0x99, 0xC6, 0xFE, 0xD3, 0xD8, 0xA0, 0xBC, 0x8E, 0x99, 0xC6, 0xFE, 0xD3, 0xD8, 0xA0, 0xBC };

        private static string Encrypt(string value)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(value);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        private static string Decrypt(string value)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(Convert.FromBase64String(value));
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
