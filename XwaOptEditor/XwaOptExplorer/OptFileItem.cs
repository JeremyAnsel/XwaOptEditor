
namespace XwaOptExplorer
{
    public class OptFileItem
    {
        public OptFileItem(string path)
        {
            this.FilePath = path;
        }

        public string FilePath { get; private set; }

        public string FileName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(this.FilePath); }
        }
    }
}
