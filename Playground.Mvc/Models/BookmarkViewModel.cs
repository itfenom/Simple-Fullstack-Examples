using Playground.Mvc.DAL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Playground.Mvc.Models
{
    public class BookmarkViewModel
    {
        [Display(Name = "Bookmark")]
        public int Bookmark_ID { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int Bookmark_Category_ID { get; set; }

        [Required]
        [Display(Name = "Bookmark Name")]
        public string Bookmark_Name { get; set; }

        [Required]
        [Display(Name = "Bookmark Link")]
        public string Bookmark_Link { get; set; }

        [Display(Name = "Bookmark Login ID")]
        public string Bookmark_Login_ID { get; set; }

        [Display(Name = "Bookmark Login Password")]
        public string Bookmark_Login_Password { get; set; }
    }

    public class BookmarkWebGridViewModel
    {
        public IEnumerable<BOOKMARK> Bookmarks { get; set; }
        public int Pages { get; set; }
        public int Rows { get; set; }
    }
}