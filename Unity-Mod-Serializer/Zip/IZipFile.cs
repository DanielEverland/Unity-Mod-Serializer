using Ionic.Zip;

namespace UMS.Zip
{
    public interface IZipFile
    {
        /// <summary>
        /// The name of the zipfile
        /// </summary>
        string FileName { get; }
        
        void Serialize(ZipFile file);
    }
}