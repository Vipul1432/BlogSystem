using BlogSystem.Data.Context;
using BlogSystem.Domain.DTO;
using BlogSystem.Domain.Models;
using BlogSystem.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Blogs
        [ResponseCache(Duration = 300,Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Index()
        {
            List<Blog> blogs = await _context.Blogs.ToListAsync();
            return View(blogs);

        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            Blog? blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            if (blog != null)
            {
                foreach (var comment in blog.Comments)
                {
                    Console.WriteLine(comment.Content);
                }
            }

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            var viewModel = new BlogWithCommentsViewModel
            {
                Blog = new BlogDto(),
                Comments = new List<CommentDto> { new CommentDto(), new CommentDto() }
            };
            return View(viewModel);
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogWithCommentsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var newBlog = new Blog
                        {
                            Title = viewModel.Blog.Title,
                            Content = viewModel.Blog.Content
                        };

                        _context.Blogs.Add(newBlog);
                        await _context.SaveChangesAsync();

                        foreach (var commentDto in viewModel.Comments)
                        {
                            var newComment = new Comment
                            {
                                Content = commentDto.Content,
                                BlogId = newBlog.Id
                            };

                            _context.Comments.Add(newComment);
                            await _context.SaveChangesAsync();
                        }

                        transaction.Commit();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "An error occurred while creating the blog and comments.");
                    }
                }
            }

            return View(viewModel);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var existingBlog = await _context.Blogs.Include(b => b.Comments).FirstOrDefaultAsync(b => b.Id == id);
            if (existingBlog == null)
            {
                return NotFound();
            }

            var viewModel = new BlogWithCommentsViewModel
            {
                Blog = new BlogDto
                {
                    Id = existingBlog.Id,
                    Title = existingBlog.Title,
                    Content = existingBlog.Content
                },
                Comments = existingBlog.Comments.Select(c => new CommentDto { Id = c.Id, Content = c.Content }).ToList()
            };
            return View(viewModel);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogWithCommentsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingBlog = await _context.Blogs.Include(b => b.Comments).FirstOrDefaultAsync(b => b.Id == id);
                    if (existingBlog == null)
                    {
                        return NotFound();
                    }

                    existingBlog.Title = viewModel.Blog.Title;
                    existingBlog.Content = viewModel.Blog.Content;

                    foreach (var commentDto in viewModel.Comments)
                    {
                        var existingComment = existingBlog.Comments.FirstOrDefault(c => c.Id == commentDto.Id);
                        if (existingComment != null)
                        {
                            existingComment.Content = commentDto.Content;
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("ConcurrencyToken", "The blog has been updated by another user.");
                }
            }

            return View(viewModel);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'BlogSystemContext.Blog'  is null.");
            }
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return (_context.Blogs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
