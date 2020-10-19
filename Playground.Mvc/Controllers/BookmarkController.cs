using Playground.Mvc.DAL;
using Playground.Mvc.Helpers;
using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    [Authorize(Users = "kmubarak")]
    public class BookmarkController : BaseController
    {
        private readonly IBookmarkRepository _repository;

        public BookmarkController()
        {
            _repository = new BookmarkRepository(new SeraphEntities());
        }

        public ActionResult Index(int page = 1, string sort = "Bookmark_Name", string sortDir = "ASC", string searchTerm = null, string filterBy = null)
        {
            const int maxPages = 15;
            var totalRows = GetCount(searchTerm, filterBy);
            var dir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase);
            var bookmarks = GetBookmarks(page, maxPages, sort, dir, searchTerm, filterBy);

            var model = new BookmarkWebGridViewModel()
            {
                Pages = maxPages,
                Rows = totalRows,
                Bookmarks = bookmarks
            };

            ViewBag.Categories = GetCategories();

            return View(model);
        }

        private int GetCount(string searchTerm, string filterBy)
        {
            int retVal = 0;

            if (!string.IsNullOrEmpty(filterBy))
            {
                if (filterBy.ToUpper() == "Bookmark_Name")
                {
                    retVal = _repository.GetBookmarksByName(searchTerm).Count();
                }
                else if (filterBy == "Bookmark_Category_ID")
                {
                    retVal = _repository.GetBookmarksByCategoryId(Convert.ToInt32(searchTerm)).Count();
                }
            }
            else
            {
                retVal = _repository.GetAllBookmarks().Count();
            }

            return retVal;
        }

        private IEnumerable<BOOKMARK> GetBookmarks(int pageNumber, int pageSize, string sort, bool dir, string searchTerm, string filterBy)
        {
            var b = _repository.GetAllBookmarks();

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (filterBy == "Bookmark_Name")
                {
                    b = _repository.GetBookmarksByName(searchTerm);
                }
                else if (filterBy == "Bookmark_Category")
                {
                    b = _repository.GetBookmarksByCategoryId(Convert.ToInt32(searchTerm));
                }
            }

            if (sort == "Bookmark_Name")
            {
                return b.OrderByWithDirection(x => x.BOOKMARK_NAME, dir)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            if (sort == "Bookmark_Link")
            {
                return b.OrderByWithDirection(x => x.BOOKMARK_LINK, dir)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            return b;
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.BookmarkCategories = GetCategories();

            return View();
        }

        [HttpPost]
        public ActionResult Create(BookmarkViewModel model)
        {
            ViewBag.BookmarkCategories = GetCategories();

            if (ModelState.IsValid)
            {
                var newModel = new BOOKMARK
                {
                    BOOKMARK_NAME = model.Bookmark_Name,
                    BOOKMARK_CATEGORY_ID = Convert.ToInt32(model.Bookmark_Category_ID),
                    BOOKMARK_LINK = model.Bookmark_Link,
                    BOOKMARK_LOGIN_ID = model.Bookmark_Login_ID,
                    BOOKMARK_LOGIN_PASSWORD = model.Bookmark_Login_Password
                };


                _repository.AddNewBookmark(newModel);

                return RedirectToAction("Index");
            }

            Warning($"<b>{"WARNING"}</b> Please fill all fields!", true);

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int bookmarkId)
        {
            var model = _repository.GetBookmarkByBookmarkId(bookmarkId);
            var modelToEdit = new BookmarkViewModel
            {
                Bookmark_ID = model.BOOKMARK_ID,
                // ReSharper disable once PossibleInvalidOperationException
                Bookmark_Category_ID = (int) model.BOOKMARK_CATEGORY_ID,
                Bookmark_Name = model.BOOKMARK_NAME,
                Bookmark_Link = model.BOOKMARK_LINK,
                Bookmark_Login_ID = model.BOOKMARK_LOGIN_ID,
                Bookmark_Login_Password = model.BOOKMARK_LOGIN_PASSWORD
            };


            ViewBag.BookmarkCategories = GetPreSelectedCategory(model.BOOKMARK_CATEGORY_ID.ToString());

            return View(modelToEdit);
        }

        [HttpPost]
        public ActionResult Edit(BookmarkViewModel updatedBookmarkModel)
        {
            ViewBag.BookmarkCategories = GetPreSelectedCategory(updatedBookmarkModel.Bookmark_Category_ID.ToString());

            if (ModelState.IsValid)
            {
                var modelToUpdate = new BOOKMARK
                {
                    BOOKMARK_ID = updatedBookmarkModel.Bookmark_ID,
                    BOOKMARK_CATEGORY_ID = updatedBookmarkModel.Bookmark_Category_ID,
                    BOOKMARK_NAME = updatedBookmarkModel.Bookmark_Name,
                    BOOKMARK_LINK = updatedBookmarkModel.Bookmark_Link,
                    BOOKMARK_LOGIN_ID = updatedBookmarkModel.Bookmark_Login_ID,
                    BOOKMARK_LOGIN_PASSWORD = updatedBookmarkModel.Bookmark_Login_Password
                };


                _repository.UpdateBookmark(modelToUpdate);

                return RedirectToAction("Index");
            }

            return View(updatedBookmarkModel);
        }

        public ActionResult Delete(int bookmarkId)
        {
            _repository.DeleteBookmark(bookmarkId);
            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetCategories()
        {
            var selectedListItems = new List<SelectListItem>();

            foreach (var i in _repository.GetAllBookmarkCategories())
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = i.CATEGORY_NAME,
                    Value = i.CATEGORY_ID.ToString()
                };

                selectedListItems.Add(item);
            }

            return selectedListItems;
        }

        private IEnumerable<SelectListItem> GetPreSelectedCategory(string categoryId)
        {
            var categories = new List<SelectListItem>();
            foreach (var item in _repository.GetAllBookmarkCategories())
            {
                categories.Add(new SelectListItem
                {
                    Value = item.CATEGORY_ID.ToString(),
                    Text = item.CATEGORY_NAME,
                    Selected = item.CATEGORY_ID.ToString() == categoryId
                });
            }
            return categories;
        }
    }
}