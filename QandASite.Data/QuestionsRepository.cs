using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QandASite.Data
{
    public class QuestionsRepository
    {
        private string _connectionString;
        public QuestionsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        private Tag GetTag(string name)
        {
            using var ctx = new QandADbContext(_connectionString);
            return ctx.Tags.FirstOrDefault(t => t.Name == name);
        }
        private int AddTag(string name)
        {
            using var ctx = new QandADbContext(_connectionString);
            var tag = new Tag { Name = name };
            ctx.Tags.Add(tag);
            ctx.SaveChanges();
            return tag.Id;
        }
        public Question GetQuestionById(int id)
        {
            using var ctx = new QandADbContext(_connectionString);
            return ctx.Questions
                    .Include(q => q.QuestionsTags).ThenInclude(qt => qt.Tag)
                    .Include(q => q.Answers).ThenInclude(a => a.User)
                    .Include(q => q.User)
                    .FirstOrDefault(q => q.Id == id);
        }
        public List<Question> GetQuestionsForTag(string name)
        {
            using var ctx = new QandADbContext(_connectionString);
            return ctx.Questions
                    .Where(c => c.QuestionsTags.Any(t => t.Tag.Name == name))
                    .Include(q => q.QuestionsTags)
                    .ThenInclude(qt => qt.Tag)
                    .ToList();
        }
        public List<Question> GetAllQuestions()
        {
            using var ctx = new QandADbContext(_connectionString);
            return ctx.Questions.Include(q => q.QuestionsTags).ThenInclude(qt => qt.Tag)
                    .Include(q => q.Answers)
                    .ToList();
        }
        public void AddQuestion(Question question, List<string> tags)
        {
            using var ctx = new QandADbContext(_connectionString);
            ctx.Questions.Add(question);
            ctx.SaveChanges();
            foreach (string tag in tags)
            {
                Tag t = GetTag(tag);
                int tagId;
                if (t == null)
                {
                    tagId = AddTag(tag);
                }
                else
                {
                    tagId = t.Id;
                }
                ctx.QuestionsTags.Add(new QuestionsTags
                {
                    QuestionId = question.Id,
                    TagId = tagId
                });
            }

            ctx.SaveChanges();
        }
        public void AddAnswer(Answer answer)
        {
            using var ctx = new QandADbContext(_connectionString);
            ctx.Answers.Add(answer);
            ctx.SaveChanges();
        }
        public void AddUser(User user, string password)
        {
            var context = new QandADbContext(_connectionString);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            context.Users.Add(user);
            context.SaveChanges();
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isValid)
            {
                return null;
            }

            return user;
        }
        public User GetByEmail(string email)
        {
            var context = new QandADbContext(_connectionString);
            return context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
