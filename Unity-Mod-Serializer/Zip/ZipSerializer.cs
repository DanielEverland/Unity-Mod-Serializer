using Ionic.Zip;

namespace UMS.Zip
{
    public static class ZipSerializer
    {
        /// <summary>
        /// General usage function. Will serialize using an abstract method as possible
        /// </summary>
        /// <param name="obj">To serialize</param>
        /// <param name="folderPath">Folder to serialize <paramref name="obj"/> to</param>
        public static void Create(object obj, string folderPath)
        {
            if(obj is IZipFile zipFile)
            {
                Create(zipFile, folderPath);
            }
            else
            {
                string zipFilePath = string.Format("{0}/{1}.zip", folderPath, obj.ToString());
                string entryName = string.Format("{0}.txt", obj.ToString());
                string content = obj.ToJson();

                Create(content, zipFilePath, entryName);
            }
        }

        /// <summary>
        /// Used to serialize objects that need require a specific zip file implementation
        /// </summary>
        /// <param name="zipFile">The object to serialize</param>
        /// <param name="folderPath">The folder in which to serialize the object</param>
        public static void Create(IZipFile zipFile, string folderPath)
        {
            using (ZipFile zip = new ZipFile())
            {
                zipFile.Serialize(zip);

                zip.Save(string.Format("{0}/{1}", folderPath, zipFile.FileName));
            }
        }

        /// <summary>
        /// Fallback serialization method using JsonPrinter
        /// </summary>
        /// <param name="obj">Object to zip</param>
        /// <param name="zipFullPath">The fullpath of the created zipfile</param>
        /// <param name="entryName">The name to give <paramref name="content"/> inside the zip file</param>
        public static void Create(string content, string zipFullPath, string entryName)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AddEntry(entryName, content);

                zip.Save(zipFullPath);
            }
        }
    }
}
