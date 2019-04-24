using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   

namespace Library
{
    class Book
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public int Page { get; set; }
            
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool isBorrowed { get; set; }
        public DateTime BorrowedAt { get; set; }

        public string Author { get; set; }
        public string Image { get; set; }
        //public string Title { get; set; }
        //public string Isbn { get; set; }
        //public string Publisher { get; set; }
        //public int Page { get; set; }
    }
}
