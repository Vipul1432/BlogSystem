﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Domain.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment content is required.")]
        public string Content { get; set; }
    }
}
