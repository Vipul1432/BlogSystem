using BlogSystem.Data.Context;
using BlogSystem.Domain.DTO;
using BlogSystem.Domain.Interfaces;
using BlogSystem.Domain.Models;
using BlogSystem.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public BlogsController(IRepository<Blog> blogRepository, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _blogRepository = blogRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        // GET: Blogs
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Index()
        {
            var blogs = await _blogRepository.GetAllAsync();
            return View(blogs);
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _blogRepository.GetByIdAsync(id.Value);
            if (blog == null)
            {
                return NotFound();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogWithCommentsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.BeginTransaction();
                try
                {
                    var newBlog = new Blog
                    {
                        Title = viewModel.Blog.Title,
                        Content = viewModel.Blog.Content
                    };

                    _blogRepository.Add(newBlog);
                    await _unitOfWork.SaveChangesAsync();

                    foreach (var commentDto in viewModel.Comments)
                    {
                        var newComment = new Comment
                        {
                            Content = commentDto.Content,
                            BlogId = newBlog.Id
                        };

                        _context.Comments.Add(newComment); 
                        await _unitOfWork.SaveChangesAsync();
                    }

                    await _unitOfWork.CommitAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    ModelState.AddModelError("", "An error occurred while creating the blog and comments.");
                }
                finally
                {
                    // It Make sure to roll back in case of any exception
                    _unitOfWork.Rollback(); 
                }
            }

            return View(viewModel);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existingBlog = await _blogRepository.GetByIdAsync(id.Value);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogWithCommentsViewModel viewModel)
        {
            if (id != viewModel.Blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBlog = await _blogRepository.GetByIdAsync(id);
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

                    await _unitOfWork.SaveChangesAsync();
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
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _blogRepository.GetByIdAsync(id.Value);
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
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            _blogRepository.Remove(blog);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _blogRepository.GetByIdAsync(id).Result != null;
        }
    }
}
