using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hpdi.Vss2Git
{
    /// <summary>
    /// Looks up email addresses based on account names. The mapping is defined in a file.
    /// </summary>
    /// We do not use AD lookup as old users may have been deleted from AD.
    class EmailMapper
    {
        private const string MappingFile = "emails.txt";
        private IDictionary<string, string> Map;

        public EmailMapper()
        {
            Map = ReadDictionaryFile(Path.Combine(EmailMapper.AssemblyDirectory, MappingFile));
        }

        /// <summary>
        /// Return email for given account or null. Account mappings come from file emails.txt.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string GetEmail(string account, string domain = null)
        {
            if (String.IsNullOrWhiteSpace(account))
                return null;
            account = account.ToLower().Replace(' ', '.');
            string email;
            if (!Map.TryGetValue(account, out email))
                email = string.Format("{0}@{1}", account, domain);
            return email;
        }

        private IDictionary<string, string> ReadDictionaryFile(string filePath)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (string line in File.ReadAllLines(filePath))
            {
                // read lines that contain a '=' sign and skip comment lines starting with a '#'
                if ((!string.IsNullOrWhiteSpace(line)) &&
                    (!line.StartsWith("#")) &&
                    (line.Contains("=")))
                {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();
                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
