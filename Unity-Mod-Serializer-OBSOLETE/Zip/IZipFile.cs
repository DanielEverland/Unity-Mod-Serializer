using Ionic.Zip;

namespace UMS.Zip
{
    public interface IZipFile { }
    public interface IZipFile<T> : IZipFile
    {
        /// <summary>
        /// The name of the zipfile
        /// </summary>
        string FileName { get; }
        
        void Serialize(ZipFile file);
    }
}