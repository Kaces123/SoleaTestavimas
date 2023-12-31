using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Org.Ktu.Isk.P175B602.Autonuoma.Repositories;
using Org.Ktu.Isk.P175B602.Autonuoma.Models;
using Org.Ktu.Isk.P175B602.Autonuoma.ViewModels;



namespace Org.Ktu.Isk.P175B602.Autonuoma.Controllers
{
	/// <summary>
	/// Controller for working with 'Modelis' entity.
	/// </summary>
	public class QuestionController : Controller
	{
		private readonly IUserRepo _userRepo;
        private  IAnswerRepo _answerRepo;
		private ILikedRepo _likedRepo;
		private IQuestionRepo _questionRepo;
        public QuestionController(IUserRepo userRepo, IAnswerRepo answerRepo,ILikedRepo likedRepo, IQuestionRepo questionRepo)
		{
			_likedRepo = likedRepo;
            _answerRepo = answerRepo;
            _userRepo = userRepo;
			_questionRepo = questionRepo;

        }

		/// <summary>
		/// This is invoked when either 'Index' action is requested or no action is provided.
		/// </summary>
		/// <returns>Entity list view.</returns>
		public ActionResult Index(string search, int n)
		{
			
			int idUser = Convert.ToInt32(TempData["id"]);
			var vModel = new QuestionsLog();
			if(search==null){
				var questions = _questionRepo.List(n);
				vModel.question=questions;
			}	
			else{
				var questionsSearch = _questionRepo.FindList(search);
				vModel.question=questionsSearch;
			}
			var user = _userRepo.Find(idUser);
			vModel.user=user;
			return View(vModel);
		}
		//Shows the question and it's answers. From Views gets question id. With that id
		//using _questionRepo.FindForDeletion() method finds the question and it's answers
		public ActionResult Content(int id,int n)
		{
			var answerss = _answerRepo.QuestionAnswers(id,n);
			var questions = _questionRepo.FindForDeletion(id);
			var user = _userRepo.Find(Convert.ToInt32(TempData["id"]));
			var vModel = new Answers();
			vModel.answers=answerss;
			vModel.question=questions;
			vModel.user=user;
			return View(vModel);
		}

		//When like button pressed on main page, it reloads a page and adds or substracts likes count
		public ActionResult Like(int id, string QuestiondUserId)
		{
			var match = _likedRepo.Find(id, Convert.ToInt32(TempData["id"]));
			var user = _userRepo.Find(QuestiondUserId, 1);
			var Liked = _likedRepo.List();
			int LikedId = 0;
			if(Liked.Count==0)
				LikedId=1;
			else
				LikedId = _likedRepo.List().Last().Id+1;
			var question=_questionRepo.Find(id);
			if(match.QuestionId != id){
				question.Question.Likes+=1;
				if(user.Id!=Convert.ToInt32(TempData["id"]))
					user.Currency+=5;
				_likedRepo.Insert(id, 0, Convert.ToInt32(TempData["id"]), LikedId, 1);
			}
			else if(match.likedOrDisliked == 2 ){
				question.Question.Likes+=1;
				question.Question.Dislikes-=1;
				if(user.Id!=Convert.ToInt32(TempData["id"]))
					user.Currency+=5;
				_likedRepo.Update(id, 0, Convert.ToInt32(TempData["id"]), match.Id, 1);
			}
			else{
				if(user.Id!=Convert.ToInt32(TempData["id"]))
					user.Currency-=5;
				question.Question.Likes-=1;
				_likedRepo.Delete(match.Id);
			}
			_userRepo.Update(user);
			_questionRepo.Update(question);
			return RedirectToAction("Index");
		}
		//When dislike button pressed on main page, it reloads a page and adds or substracts dislikes count
		public ActionResult Dislike(int id, string QuestiondUserId)
		{
			var match = _likedRepo.Find(id, Convert.ToInt32(TempData["id"]));
			var user = _userRepo.Find(QuestiondUserId, 1);
			var Liked = _likedRepo.List();
			int LikedId = 0;
			if(Liked.Count==0)
				LikedId=1;
			else
				LikedId = _likedRepo.List().Last().Id+1;
			var question=_questionRepo.Find(id);
			if(match.QuestionId != id){
				question.Question.Dislikes+=1;
				_likedRepo.Insert(id, 0, Convert.ToInt32(TempData["id"]), LikedId, 2);
			}
			else if(match.likedOrDisliked == 1){
				question.Question.Likes-=1;
				question.Question.Dislikes+=1;
				if(user.Id!=Convert.ToInt32(TempData["id"]))
					user.Currency-=5;
				_likedRepo.Update(id, 0, Convert.ToInt32(TempData["id"]), match.Id, 2);
			}
			else{
				question.Question.Dislikes-=1;
				_likedRepo.Delete(match.Id);
			}
			_userRepo.Update(user);
			_questionRepo.Update(question);
			return RedirectToAction("Index");
		}
		//This is invoked when "mark as the best answer" button is pressed
		public ActionResult Mark(int AnswerId)
		{
			var question = _questionRepo.Find(Convert.ToInt32(TempData["Qid"]));
			if(question.Question.topAnswer==0){
				question.Question.topAnswer=1;
				_questionRepo.Update(question);
				var answer = _answerRepo.Find(AnswerId);
				answer.Answer.best=1;
                _answerRepo.Update(answer);
				var user = _userRepo.Find(answer.Answer.fk_User, 1);
				user.Currency+=80;
				_userRepo.Update(user);
				Debug.WriteLine(user.Name);
			}
			return RedirectToAction("Content", new {id=Convert.ToInt32(TempData["Qid"])});
		}
		/// <summary>
		/// This is invoked when creation form is first opened in browser.
		/// </summary>
		/// <returns>Creation form view.</returns>
		public ActionResult Create()
		{
			var questionEvm = new QuestionEditVM();
			var user=_userRepo.Find(Convert.ToInt32(TempData["id"]));
			questionEvm.user=user;
			questionEvm.Lists.id=Convert.ToInt32(TempData["id"]);
			questionEvm.Question.fk_User=user.Name;
			//PopulateSelections(questionEvm);
			return View(questionEvm);
		}

		/// <summary>
		/// This is invoked when buttons are pressed in the creation form.
		/// </summary>
		/// <param name="questionEvm">Entity model filled with latest data.</param>
		/// <returns>Returns creation from view or redirects back to Index if save is successfull.</returns>
		[HttpPost]
		public ActionResult Create(QuestionEditVM questionEvm)
		{
			bool temp=true;
			if(questionEvm.Question.Questions == null || questionEvm.Question.Questions.Length < 5){
				ModelState.AddModelError("question", "Question must be atleast 5 characters");
				temp=false;
			}
			var question = _questionRepo.Find(questionEvm.Question.Id);
			if(question.Question.Questions == questionEvm.Question.Questions){
				ModelState.AddModelError("question", "Question with the same title already exist");
				temp=false;
			}
			if(questionEvm.Question.Content == null || questionEvm.Question.Content.Length < 15){
				ModelState.AddModelError("content", "Content must be atleast 15 characters");
				temp=false;
			}
			if(temp){
				var user = _userRepo.Find(Convert.ToInt32(TempData["id"]));
				user.Currency-=100;
				_userRepo.Update(user);
				_questionRepo.Insert(questionEvm);
				return RedirectToAction("Index");
			}
			return View(questionEvm);
			//form field validation failed, go back to the form
			//PopulateSelections(questionEvm);
			//return View(questionEvm);
			
		}

		/// <summary>
		/// This is invoked when editing form is first opened in browser.
		/// </summary>
		/// <param name="id">ID of the entity to edit.</param>
		/// <returns>Editing form view.</returns>
		public ActionResult Edit(int id)
		{
			var questionEvm = _questionRepo.Find(id);
			questionEvm.user=_userRepo.Find(Convert.ToInt32(TempData["id"]));
			return View(questionEvm);
		}

		/// <summary>
		/// This is invoked when buttons are pressed in the editing form.
		/// </summary>
		/// <param name="id">ID of the entity being edited</param>
		/// <param name="autoEvm">Entity model filled with latest data.</param>
		/// <returns>Returns editing from view or redirects back to Index if save is successfull.</returns>
		[HttpPost]
		public ActionResult Edit(int id, QuestionEditVM questionEvm)
		{
			//form field validation passed?
			/*if( ModelState.IsValid )
			{
				_questionRepo.Update(questionEvm);

				//save success, go back to the entity list
				return RedirectToAction("Index", new { id = questionEvm.user.Id});
			}*/
			/*if(questionEvm.Question.Questions == null || questionEvm.Question.Questions.Length < 5)
				ModelState.AddModelError("question", "Question must be atleast 5 characters");
			var question = _questionRepo.Find(questionEvm.Question.Id);
			if(question.Question.Questions == questionEvm.Question.Questions)
				ModelState.AddModelError("question", "Question with the same title already exist");*/
			if(questionEvm.Question.Content == null || questionEvm.Question.Content.Length < 15)
				ModelState.AddModelError("content", "Content must be at least 15 characters");
			else {
				_questionRepo.Update(questionEvm);
				return RedirectToAction("Index");
				}
			//form field validation failed, go back to the form
			//PopulateSelections(questionEvm);
			return View(questionEvm);
		}

		/// </summary>
		/// <param name="id">ID of the entity to delete.</param>
		/// <returns>Deletion form view.</returns>
		public ActionResult Delete(int id)
		{
			var questionLvm = _questionRepo.FindForDeletion(id);
			Answers question = new Answers();
			question.question=questionLvm;
			question.user=_userRepo.Find(Convert.ToInt32(TempData["id"]));
			return View(question);
		}

		/// <summary>
		/// This is invoked when deletion is confirmed in deletion form
		/// </summary>
		/// <param name="id">ID of the entity to delete.</param>
		/// <returns>Deletion form view on error, redirects to Index on success.</returns>
		[HttpPost]
		public ActionResult DeleteConfirm(int id)
		{
			//try deleting, this will fail if foreign key constraint fails
			try
			{
				_questionRepo.Delete(id);

				//deletion success, redired to list form
				return RedirectToAction("Index");
			}
			//entity in use, deletion not permitted
			catch( MySql.Data.MySqlClient.MySqlException )
			{
				//enable explanatory message and show delete form
				ViewData["deletionNotPermitted"] = true;
				Answers questionLvm = new Answers();
				questionLvm.question = _questionRepo.FindForDeletion(id);
				questionLvm.user=_userRepo.Find(Convert.ToInt32(TempData["id"]));

				return View("Delete", questionLvm);
			}
		}

		/// <summary>
		/// Populates select lists used to render drop down controls.
		/// </summary>
		/// <param name="questionEvm">'Automobilis' view model to append to.</param>
		/*public void PopulateSelections(QuestionEditVM questionsEvm)
		{
			//load entities for the select lists
			var users = UserRepo.List();

			//build select lists
			questionsEvm.Lists.Users =
				users.Select(it => {
					return
						new SelectListItem() {
							Value = Convert.ToString(it.Name),
							Text = Convert.ToString(it.Name)
						};
				})
				.ToList();
		}*/
		public ActionResult Share(){
			return Redirect("http://www.facebook.com");
		}
		public ActionResult Lock(int id){
			var question = _questionRepo.Find(id);
			question.Question.topAnswer=1;
			_questionRepo.Update(question);
			return RedirectToAction("Index");
		}
		public ActionResult Unlock(int id){
			var question = _questionRepo.Find(id);
			question.Question.topAnswer=0;
			_questionRepo.Update(question);
			return RedirectToAction("Index");
		}
	}
}
