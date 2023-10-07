using System.Collections.Generic;

namespace SaucenaoSearch
{
    public class SaucenaoResponse
    {
        public float Similarity { get; set; }
        public List<string> OtherUrls { get; set; } = new List<string>();
        public string SourceUrl { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }
    }
}
