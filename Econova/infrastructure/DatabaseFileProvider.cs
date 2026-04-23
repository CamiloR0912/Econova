using System;
using System.IO;

namespace Econova.Infrastructure
{
    public static class DatabaseFileProvider
    {
        public static string GetDatabasePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string econovaFolder = Path.Combine(appDataPath, "Econova");
            string databasePath = Path.Combine(econovaFolder, "econova.db");

            if (!Directory.Exists(econovaFolder))
            {
                Directory.CreateDirectory(econovaFolder);
            }

            if (!File.Exists(databasePath))
            {
                using (File.Create(databasePath))
                {
                    // File creation intentionally left empty.
                }
            }

            return databasePath;
        }
    }
}
