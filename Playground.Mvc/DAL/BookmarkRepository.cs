using System;
using System.Collections.Generic;
using System.Linq;

namespace Playground.Mvc.DAL
{
    public interface IBookmarkRepository : IDisposable
    {
        IEnumerable<BOOKMARK_CATEGORY> GetAllBookmarkCategories();

        IEnumerable<BOOKMARK> GetAllBookmarks();

        IEnumerable<BOOKMARK> GetBookmarksByCategoryName(string categoryName);

        IEnumerable<BOOKMARK> GetBookmarksByCategoryId(int categoryId);

        IEnumerable<BOOKMARK> GetBookmarksByName(string bookmarkName);

        BOOKMARK GetBookmarkByBookmarkId(int bookmarkId);

        bool AddNewBookmark(BOOKMARK newBookmarkModel);

        bool UpdateBookmark(BOOKMARK updatedBookmarkModel);

        bool DeleteBookmark(int bookmarkId);
    }

    public class BookmarkRepository : IBookmarkRepository
    {
        private bool _disposed;
        private readonly SeraphEntities _context;

        public BookmarkRepository(SeraphEntities context)
        {
            _context = context;
        }

        public IEnumerable<BOOKMARK_CATEGORY> GetAllBookmarkCategories()
        {
            var query = from bookmarkCategories in _context.BOOKMARK_CATEGORY
                         select bookmarkCategories;
            return query.ToList();
        }

        public IEnumerable<BOOKMARK> GetAllBookmarks()
        {
            var query = from b in _context.BOOKMARKs
                         select b;

            return query.ToList();
        }

        public IEnumerable<BOOKMARK> GetBookmarksByCategoryName(string categoryName)
        {
            var query = (from bookmarkCategory in _context.BOOKMARK_CATEGORY
                         join bookmark in _context.BOOKMARKs
                         on bookmarkCategory.CATEGORY_ID equals bookmark.BOOKMARK_CATEGORY_ID
                         where (bookmarkCategory.CATEGORY_NAME.ToLower().StartsWith(categoryName.ToLower()))
                         select bookmark).ToList();

            if (query.Count > 0)
            {
                return query.ToList();
            }

            return GetAllBookmarks();
        }

        public IEnumerable<BOOKMARK> GetBookmarksByCategoryId(int categoryId)
        {
            var query = from b in _context.BOOKMARKs
                         where b.BOOKMARK_CATEGORY_ID == categoryId
                         select b;

            return query.ToList();
        }

        public IEnumerable<BOOKMARK> GetBookmarksByName(string bookmarkName)
        {
            var query = from b in GetAllBookmarks()
                         where b.BOOKMARK_NAME.ToLower().StartsWith(bookmarkName.ToLower())
                         select b;

            return query.ToList();
        }

        public BOOKMARK GetBookmarkByBookmarkId(int bookmarkId)
        {
            var query = (from b in _context.BOOKMARKs
                          where b.BOOKMARK_ID == bookmarkId
                          select b).FirstOrDefault();

            return query;
        }

        public bool AddNewBookmark(BOOKMARK newBookmarkModel)
        {
            try
            {
                _context.BOOKMARKs.Add(newBookmarkModel);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        public bool UpdateBookmark(BOOKMARK updatedBookmarkModel)
        {
            bool retVal = false;

            var modelToBeUpdated = _context.BOOKMARKs.Find(updatedBookmarkModel.BOOKMARK_ID);

            if (modelToBeUpdated != null)
            {
                _context.Entry(modelToBeUpdated).CurrentValues.SetValues(updatedBookmarkModel);
                _context.SaveChanges();
                retVal = true;
            }

            return retVal;
        }

        public bool DeleteBookmark(int bookmarkId)
        {
            bool retVal = false;
            var modelToBeDeleted = _context.BOOKMARKs.Find(bookmarkId);

            if (modelToBeDeleted != null)
            {
                _context.BOOKMARKs.Remove(modelToBeDeleted);
                _context.SaveChanges();
                retVal = true;
            }

            return retVal;
        }

        #region Disposing

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Disposing
    }
}