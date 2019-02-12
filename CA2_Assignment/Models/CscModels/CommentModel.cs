using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CA2_Assignment.Models.CscModels
{
    public class Comment
    {
        public int Id { get; set; }
        public string UploadedById { get; set; }
        public string UploadedByName { get; set; }
        public string TalentId { get; set; }
        public string Content { get; set; }
        public string TimeStamp { get; set; }
    }
}
