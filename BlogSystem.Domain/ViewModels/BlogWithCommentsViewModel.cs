using BlogSystem.Domain.DTO;
using BlogSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Domain.ViewModels
{
    public class BlogWithCommentsViewModel
    {
        public BlogDto Blog { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
