using Avalonia.Controls;
using Avalonia.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4080capstone.ViewModels
{
    public class PgpKeyInfo
    {
        public string KeyType { get; set; } = "";       // Public or private
        public string UserIdentity { get; set; } = "";  // Name/Identity of key owner
        public string Validity { get; set; } = "";      // Valid, revoked, or expired
        public DateTime CreationDate { get; set; }      // Creation date
        public DateTime ExpirationDate { get; set; }    // Expiration Date
        public string KeyId { get; set; } = "";         // Key ID
        public string Path { get; set; } = "";          // Path to key
        public string DisplayName { get; set; } = "";   // Displayed in dropdown
    }
}
