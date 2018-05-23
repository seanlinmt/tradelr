using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.comments
{
    public class CommentEmailContent
    {
        public string creator { get; set; }
        public string targetName { get; set; }
        public string comment { get; set; }
        public string commentsLink { get; set; }
    }
}
