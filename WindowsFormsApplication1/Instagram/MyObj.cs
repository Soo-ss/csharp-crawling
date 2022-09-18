using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Total
{
    public class InstagramComment
    {
        public string writer;
        public string fullName;
        public string contents;
        public string createdAtUtc;
        public string profilePicture;
        public int likes;

        public InstagramComment(string writer, string fullName, string contents, 
            string createdAtUtc, string profilePicture, int likes)
        {
            this.writer = writer;
            this.fullName = fullName;
            this.contents = contents;
            this.createdAtUtc = createdAtUtc;
            this.profilePicture = profilePicture;
            this.likes = likes;
        }

        public void PrintChild()
        {
            Console.WriteLine("[writer]: " + writer);
            Console.WriteLine("[fullName]: " + fullName);
            Console.WriteLine("[contents]: " + contents);
            Console.WriteLine("[createdAtUtc]: " + createdAtUtc);
            Console.WriteLine("[profilePictrue]: " + profilePicture);
            Console.WriteLine("[likes]: " + likes);
        }
    }
}
