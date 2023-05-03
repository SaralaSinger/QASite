using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QandASite.Data
{
    public class Question
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<QuestionsTags> QuestionsTags { get; set; }
        public List<Answer> Answers { get; set; } = new();
    }
}
