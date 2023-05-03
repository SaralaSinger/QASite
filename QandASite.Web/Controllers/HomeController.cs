using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QandASite.Data;
using QandASite.Web.Models;
using System.Diagnostics;

namespace QandASite.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;
        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            var repo = new QuestionsRepository(_connectionString);
            var ivm = new IndexViewModel
            {
                Questions = repo.GetAllQuestions()
            };
            return View(ivm);
        }
        [Authorize]
        public IActionResult AskAQuestion()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddQuestion(Question question, List<string> tags)
        {
            question.Date = DateTime.Now;
            question.UserId = GetCurrentUserId();

            var repo = new QuestionsRepository(_connectionString);
            repo.AddQuestion(question, tags);

            return RedirectToAction("Index");
        }
        public IActionResult ViewQuestion(int id)
        {
            var repo = new QuestionsRepository(_connectionString);
            var qvm = new QuestionViewModel
            {
                Question = repo.GetQuestionById(id)
            };
            return View(qvm);
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddAnswer(Answer answer)
        {
            answer.Date = DateTime.Now;
            answer.UserId = GetCurrentUserId();
            var repo = new QuestionsRepository(_connectionString);
            repo.AddAnswer(answer);
            return Redirect($"/home/viewquestion?id={answer.QuestionId}");
        }
        private int GetCurrentUserId()
        {
            var repo = new QuestionsRepository(_connectionString);
            var user = repo.GetByEmail(User.Identity.Name);
            return user.Id;
        }
    }
}