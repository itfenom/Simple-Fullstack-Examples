using System.Collections.Generic;
using System.Linq;

namespace Playground.Mvc.DAL
{
    public interface IBookRepository
    {
        List<string> GetAuthors();

        List<Book> GetBooksByAuthor(string author);

        List<Book> GetAllBooks();
    }

    public class Book
    {
        public string Author { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
    }

    public class BookRepository : IBookRepository
    {
        private static readonly List<Book> Books = new List<Book>();

        static BookRepository()
        {
            Books.Add(new Book { Author = "Charles Petzold", Subject = "Programming Microsoft Technologies", Title = "Some Dummy Title" });
            Books.Add(new Book { Author = "Adam Nathan", Subject = "WPF", Title = "Some Dummy Title" });
            Books.Add(new Book { Author = "Jesse Liberty", Subject = "Programming C#", Title = "Some Dummy Title" });
            Books.Add(new Book { Author = "Bill Wagner", Subject = "Effective C#", Title = "Some Dummy Title" });
            Books.Add(new Book { Author = "Charles Petzold", Subject = "LINQ Unleashed", Title = "Some Dummy Title" });
            Books.Add(new Book { Author = "Shay Friedman", Subject = "MVC", Title = "Some Dummy Title" });
            Books.Add(new Book { Author = "Shay Friedman", Subject = "C#", Title = "Some Dummy Title" });
            Books.Add(new Book { Author = "Mike Gunderloy", Subject = "Javascripts / jQuery", Title = "Some Dummy Title" });
        }

        public List<string> GetAuthors()
        {
            var retVal = (from b in Books
                          orderby b.Author ascending
                          select b.Author).Distinct();

            return retVal.ToList();
        }

        public List<Book> GetBooksByAuthor(string author)
        {
            var retVal = (from b in Books
                          orderby b.Title ascending
                          where b.Author == author
                          select b);

            return retVal.ToList();
        }

        public List<Book> GetAllBooks()
        {
            return Books.ToList();
        }
    }
}