using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace _4080capstone.Services
{
    public class PgpKeyInfo
    {
        public string KeyType { get; set; } = ""; // Public or private
        public string Identity { get; set; } = ""; // Name/Identity of key owner
        public string Validity { get; set; } = ""; // Valid, revoked, or expired
        public DateTime CreationDate { get; set; } // Creation date
        public DateTime ExpirationDate { get; set; } // Expiration Date
        public string KeyId { get; set; } = ""; // Key ID
        public string Path { get; set; } = ""; // Path to key
    }
}
