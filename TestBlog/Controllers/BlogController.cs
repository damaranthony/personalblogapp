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
            //admin - display all active blog posts
            //non-admin - display active blog posts by user id
            var blogContents = _blogUnitOfWork.ContentRepository.GetAll().Where(b => !b.IsDeleted.GetValueOrDefault() && b.PublishDate <= DateTime.Now);
            
            return View(Roles.IsUserInRole("Admin") ? _blogUnitOfWork.ContentHistoryRepository.GetPublishedPosts(blogContents) : _blogUnitOfWork.ContentHistoryRepository.GetPublishedPosts(blogContents.Where(b => b.PublishedBy == User.Identity.GetUserId())));
        }

        
        public ActionResult Manage()
        {
            return (Roles.IsUserInRole("Admin")) ? View(_blogUnitOfWork.ContentRepository.GetAll().Where(b => !b.IsDeleted.GetValueOrDefault())) : View(_blogUnitOfWork.ContentRepository.GetAll().Where(b => b.PublishedBy == User.Identity.GetUserId() && !b.IsDeleted.GetValueOrDefault()));
        }
        
        public ActionResult Details(int? id)
        {
            //check if url parameter has a value
            //return blog post details if there's an id 

            if (id == null) return View();

            var cHistory = _blogUnitOfWork.ContentHistoryRepository.GetLatestContentHistory(id.Value);
            
            return View(cHistory);
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
                            TempData["Draft"] = "Your blog post has been saved as Draft!";
                            return RedirectToAction("Edit", new { id = blogPost.Id });
                        case "Publish":
                            SaveBlogPost(blogPost, ContentStateType.ReadyToPublish);
                            if (Roles.IsUserInRole("Admin"))
                                return RedirectToAction("Publish", new {id = blogPost.Id});

                            TempData["Publish"] = "Your content has been submitted for approval!";
                            return RedirectToAction("Edit", new { id = blogPost.Id });
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
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id != null)
                {
                    var blogPostHistory = _blogUnitOfWork.ContentHistoryRepository.GetLatestContentHistory(id.Value);

                    if (blogPostHistory == null) return View();
                    //check if the content is ready for review
                    if(blogPostHistory.ContentState.Id.Equals(2) && Roles.IsUserInRole("Admin"))
                        return RedirectToAction("Publish", new { id = blogPostHistory.ContentId });
                }

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
            //redirect to details page if not ready for publish state
            if (!blogPostHistory.ContentState.Id.Equals(2)) 
                return RedirectToAction("Details", new {id = blogPostHistory.ContentId, title = blogPostHistory.Title});

            var blogModel = new BlogViewModel
            {
                Title = blogPostHistory.Title.ToTitleCase(),
                Content = blogPostHistory.MainContent,
                Comment = blogPostHistory.Comment,
                UpdatedDate = blogPostHistory.Content.UpdatedDate.GetValueOrDefault(),
                PublishDate = blogPostHistory.Content.PublishDate ?? DateTime.Now,
                Id = blogPostHistory.ContentId
            };

            return View(blogModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Publish(BlogViewModel blogPost, string submit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(submit.Equals("Cancel")) return RedirectToAction("Manage");

                    var tempPost = _blogUnitOfWork.ContentRepository.GetById(blogPost.Id);

                    if (tempPost == null) return View();

                    tempPost.PublishDate = blogPost.PublishDate;
                    tempPost.UpdatedBy = User.Identity.GetUserId();
                    tempPost.UpdatedDate = DateTime.Now;

                    _blogUnitOfWork.ContentRepository.Update(tempPost);

                    var blogHistory = new ContentHistory
                    {
                        ContentId = blogPost.Id,
                        CreatedBy = User.Identity.GetUserId(),
                        Title = tempPost.Title.ToTitleCase(),
                        MainContent = tempPost.MainContent,
                        Comment = blogPost.Comment,
                        CreatedDate = DateTime.Now,
                        ContentStateId = submit.Equals("Approve") ? 4 : 3  //4 - published, 3 - rejected
                    };

                    _blogUnitOfWork.ContentHistoryRepository.Insert(blogHistory);
                    _blogUnitOfWork.Save();


                    return RedirectToAction("Details", new { id = tempPost.Id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        public ActionResult Delete(int id)
        {
            var tempPost = _blogUnitOfWork.ContentRepository.GetById(id);

            if (tempPost == null) return RedirectToAction("Manage");

            var blogHistory = new ContentHistory
            {
                ContentId = tempPost.Id,
                CreatedBy = User.Identity.GetUserId(),
                Title = tempPost.Title.ToTitleCase(),
                MainContent = tempPost.MainContent,
                CreatedDate = DateTime.Now,
                ContentStateId = 5 //archived
            };

            _blogUnitOfWork.ContentHistoryRepository.Insert(blogHistory);

            tempPost.UpdatedDate = DateTime.Now;
            tempPost.UpdatedBy = User.Identity.GetUserId();
            tempPost.IsDeleted = true;
                
            _blogUnitOfWork.Save();

            TempData["Delete"] = "Blog Post " + tempPost.Title + " has been successfully deleted!";


            return RedirectToAction("Manage");
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
                tempBlog.Title = blogPost.Title.ToTitleCase();
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
                blogPost.PublishDate = DateTime.Now;
                //encode blog post content before saving to DB for security
                blogPost.MainContent = Server.HtmlEncode(blogPost.MainContent);
                blogPost.Title = blogPost.Title.ToTitleCase();
                blogPost.IsDeleted = false;
                blogPost.Author = _blogUnitOfWork.AuthorRepository.GetAuthorName(User.Identity.GetUserId());

                _blogUnitOfWork.ContentRepository.Insert(blogPost);
            }

            _blogUnitOfWork.Save();

            //create new history record to handle latest content state
            var blogHistory = new ContentHistory
            {
                ContentId = tempBlog?.Id ?? blogPost.Id, //check if record exist
                CreatedBy = User.Identity.GetUserId(),
                MainContent = Server.HtmlEncode(blogPost.MainContent),
                Title = blogPost.Title.ToTitleCase(),
                CreatedDate = DateTime.Now,
                ContentStateId = cType == ContentStateType.Draft ? 1 : 2 //check if save for draft or publish
            };

            _blogUnitOfWork.ContentHistoryRepository.Insert(blogHistory);
            _blogUnitOfWork.Save();
        }
    }
}
