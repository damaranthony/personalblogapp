using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

using BlogData.DAL;
using BlogData.Data;
using Microsoft.AspNet.Identity;
using TestBlog.Models;

namespace TestBlog.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly UnitOfWork _blogUnitOfWork = new UnitOfWork();
        
        //main blog post page
        public ActionResult Index()
        {
            return View(_blogUnitOfWork.ContentRepository.GetAll());
        }

        //manage blog posts
        //to do: authorize admin only
        public ActionResult Manage()
        {
            return View(_blogUnitOfWork.ContentRepository.GetAll());
        }

        // GET: Blog/Details/5
        public ActionResult Details(int? id)
        {
            //check if url parameter has a value
            //return blog post details if there's an id
            return id == null ? View() : View(_blogUnitOfWork.ContentRepository.GetById(id));
        }

        // GET: Blog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] //to accept html codes from form
        public ActionResult Create(Content blogPost, string submit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (submit)
                    {
                        case "Save as Draft":
                            SaveBlogPost(blogPost, ContentStateType.Draft);
                            return RedirectToAction("Edit", new { id = blogPost.Id });
                        case "Publish":
                            SaveBlogPost(blogPost, ContentStateType.ReadyToPublish);
                            return RedirectToAction("Publish", new { id = blogPost.Id, title = blogPost.Title });
                        default: //cancel
                            return RedirectToAction("Manage", "Blog");
                    }
                    
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        
        // GET: Blog/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var blogPostHistory = _blogUnitOfWork.ContentHistoryRepository.GetLatestContentHistory(id);

                if (blogPostHistory == null) return View();
                //check if the content is ready for review
                if(blogPostHistory.ContentState.Title == "Ready to publish")
                    return RedirectToAction("Publish", new { id = blogPostHistory.ContentId, title = blogPostHistory.Title });

                var blogPost = _blogUnitOfWork.ContentRepository.GetById(id);
                blogPost.MainContent = HttpContext.Server.HtmlDecode(blogPost.MainContent);

                return View(blogPost);
               
            }
            catch
            {
                return View();
            }
        }
        
        //to do: authorize for admin only
        public ActionResult Publish(int? id)
        {
            if (id == null) return View();

            var blogPostHistory = _blogUnitOfWork.ContentHistoryRepository.GetLatestContentHistory(id.Value);
            
            if(blogPostHistory.ContentState.Title == "Published")
                return RedirectToAction("Details", new { id = blogPostHistory.ContentId, title = blogPostHistory.Title });
            else
                return View(_blogUnitOfWork.ContentRepository.GetById(id));
        }

        //to do: authorize for admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Publish(Content blogPost)
        {
            try
            {
                var tempPost = _blogUnitOfWork.ContentRepository.GetById(blogPost.Id);

                if (tempPost == null) return View();

                tempPost.PublishDate = blogPost?.PublishDate ?? DateTime.Now;
                tempPost.UpdatedBy = User.Identity.GetUserId();
                tempPost.UpdatedDate = DateTime.Now;

                _blogUnitOfWork.ContentRepository.Update(tempPost);

                var blogHistory = new ContentHistory
                {
                    ContentId = blogPost.Id, //check if record exist
                    CreatedBy = User.Identity.GetUserId(),
                    CreatedDate = DateTime.Now,
                    ContentStateId = 4 //published
                };

                _blogUnitOfWork.ContentHistoryRepository.Insert(blogHistory);
                _blogUnitOfWork.Save();


                return RedirectToAction("Details", new { id = tempPost.Id, title = tempPost.Title });
            }
            catch
            {
                return View();
            }
        }

        // GET: Blog/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Blog/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        private void SaveBlogPost(Content blogPost, ContentStateType cType)
        {
            var tempBlog = _blogUnitOfWork.ContentRepository.GetById(blogPost.Id);

            if (tempBlog != null)
            {
                //update record
                tempBlog.UpdatedBy = User.Identity.GetUserId();
                tempBlog.UpdatedDate = DateTime.Now;
                tempBlog.IsDeleted = false;
                tempBlog.Title = blogPost.Title;
                tempBlog.PublishDate = blogPost.PublishDate;
                tempBlog.PublishedBy = User.Identity.GetUserId();
                //encode blog post content before saving to DB for security
                tempBlog.MainContent = Server.HtmlEncode(blogPost.MainContent);
               
                _blogUnitOfWork.ContentRepository.Update(tempBlog);
            }
            else
            {
                //insert record
                blogPost.PublishedBy = User.Identity.GetUserId();
                //encode blog post content before saving to DB for security
                blogPost.MainContent = Server.HtmlEncode(blogPost.MainContent);
                blogPost.IsDeleted = false;

                _blogUnitOfWork.ContentRepository.Insert(blogPost);
            }

            //create new history record to handle latest content state
            var blogHistory = new ContentHistory
            {
                ContentId = tempBlog?.Id ?? blogPost.Id, //check if record exist
                CreatedBy = User.Identity.GetUserId(),
                MainContent = Server.HtmlEncode(blogPost.MainContent),
                Title = blogPost.Title,
                CreatedDate = DateTime.Now,
                ContentStateId = cType == ContentStateType.Draft ? 1 : 2 //check if save for draft or publish
            };

            _blogUnitOfWork.ContentHistoryRepository.Insert(blogHistory);

            _blogUnitOfWork.Save();
        }
    }
}
