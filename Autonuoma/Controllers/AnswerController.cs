using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Org.Ktu.Isk.P175B602.Autonuoma.Repositories;
using Org.Ktu.Isk.P175B602.Autonuoma.Models;
using Org.Ktu.Isk.P175B602.Autonuoma.ViewModels;


namespace Org.Ktu.Isk.P175B602.Autonuoma.Controllers
{
	/// <summary>
	/// Controller for working with 'Modelis' entity.
	/// </summary>
	public class AnswerController : Controller
	{
		private readonly IUserRepo _userRepo;
        private readonly IAnswerRepo _answerRepo;
		private readonly ILikedRepo _likedRepo;
        public AnswerController(IUserRepo userRepo, IAnswerRepo answerRepo,ILikedRepo likedRepo)
		{
			_likedRepo = likedRepo;
		    _answerRepo = answerRepo;
            _userRepo = userRepo;
		}
		/// <summary>
		/// This is invoked when either 'Index' action is requested or no action is provided.
		/// </summary>
		/// <returns>Entity list view.</returns>
		public ActionResult Index()
		{
			var answers = _answerRepo.List();
			return View(answers);
		}

		/// <summary>
		/// This is invoked when creation form is first opened in browser.
		/// </summary>
		/// <returns>Creation form view.</returns>
		public ActionResult Create(string q, int id)
		{
			//var answerEvm = new AnswerEditVM();
			var answerEvm = new AnswerEditVM();
			//PopulateSelections(answerEvm);
			answerEvm.Answer.fk_Questions=q;
			answerEvm.Lists.Questions_Id=id;
			var user=_userRepo.Find(Convert.ToInt32(TempData["id"]));
			answerEvm.Answer.fk_User=user.Name;
			answerEvm.user=user;
			return View(answerEvm);
		}

		/// <summary>
		/// This is invoked when buttons are pressed in the creation form.
		/// </summary>
		/// <param name="answerEvm">Entity model filled with latest data.</param>
		/// <returns>Returns creation from view or redirects back to Index if save is successfull.</returns>
		[HttpPost]
		public ActionResult Create(AnswerEditVM answerEvm, int id)
		{
			//form field validation passed?
			/*if( ModelState.IsValid )
			{
				AnswerRepo.Insert(answerEvm);

				//save success, go back to the entity list
				return RedirectToAction("Content", "Question", new { id = id, userId = answerEvm.user.Id});
			}*/
			if(answerEvm.Answer.Answers == null || answerEvm.Answer.Answers.Length < 3)
				ModelState.AddModelError("answer", "The answer must be atleast 3 characters long");
			else{
                _answerRepo.Insert(answerEvm);
			//form field validation failed, go back to the form
			//PopulateSelections(answerEvm);
			return RedirectToAction("Content", "Question", new { id = id});
			}
			return View(answerEvm);
			//return View(answerEvm);
			
		}

		public ActionResult Like(int id, int idQ, string AnswerUserId)
		{
			var match = _likedRepo.Find(id, Convert.ToInt32(TempData["id"]), 0);
			var user = _userRepo.Find(AnswerUserId, 1);
			var Liked = _likedRepo.List();
			int LikedId = 0;
			if(Liked.Count==0)
				LikedId=1;
			else
				LikedId = _likedRepo.List().Last().Id+1;
			var answer= _answerRepo.Find(id);
			if(match.AnswerId != id){
				answer.Answer.Likes+=1;
				if(user.Id!=Convert.ToInt32(TempData["id"]))
					user.Currency+=5;
				_likedRepo.Insert(0, id, Convert.ToInt32(TempData["id"]), LikedId, 1);
			}
			else if(match.likedOrDisliked == 2 ){
				answer.Answer.Likes+=1;
				answer.Answer.Dislikes-=1;
				if(user.Id!=Convert.ToInt32(TempData["id"]))
					user.Currency+=5;
				_likedRepo.Update(0, id, Convert.ToInt32(TempData["id"]), match.Id, 1);
			}
			else{
				if(user.Id!=Convert.ToInt32(TempData["id"]))
					user.Currency-=5;
				answer.Answer.Likes-=1;
				_likedRepo.Delete(match.Id);
			}
			_userRepo.Update(user);
            _answerRepo.Update(answer);
			return RedirectToAction("Content","Question", new {id = idQ});
		}

		public ActionResult Dislike(int id, int idQ, string AnswerUserId)
		{
			var match = _likedRepo.Find(id, Convert.ToInt32(TempData["id"]), 0);
			var user = _userRepo.Find(AnswerUserId, 1);
			var Liked = _likedRepo.List();
			int LikedId = 0;
			if(Liked.Count==0)
				LikedId=1;
			else
				LikedId = _likedRepo.List().Last().Id+1;
			var answer= _answerRepo.Find(id);
			if(match.AnswerId != id){
				answer.Answer.Dislikes+=1;
				_likedRepo.Insert(0, id, Convert.ToInt32(TempData["id"]), LikedId, 2);
			}
			else if(match.likedOrDisliked == 1 ){
				answer.Answer.Likes-=1;
				answer.Answer.Dislikes+=1;
				if(user.Id!=Convert.ToInt32(TempData["id"]))
					user.Currency-=5;
				_likedRepo.Update(0, id, Convert.ToInt32(TempData["id"]), match.Id, 2);
			}
			else{
				answer.Answer.Dislikes-=1;
				_likedRepo.Delete(match.Id);
			}
			_userRepo.Update(user);
            _answerRepo.Update(answer);
			return RedirectToAction("Content","Question", new {id = idQ});
		}
		/// <summary>
		/// This is invoked when editing form is first opened in browser.
		/// </summary>
		/// <param name="id">ID of the entity to edit.</param>
		/// <returns>Editing form view.</returns>
		public ActionResult Edit(int id, string q, int id1)
		{
			var answerEvm = _answerRepo.Find(id1);
			answerEvm.Answer.fk_Questions=q;
			answerEvm.Lists.Questions_Id=id;
			answerEvm.user=_userRepo.Find(Convert.ToInt32(TempData["id"]));
			//PopulateSelections(answerEvm);

			return View(answerEvm);
		}

		/// <summary>
		/// This is invoked when buttons are pressed in the editing form.
		/// </summary>
		/// <param name="id">ID of the entity being edited</param>
		/// <param name="autoEvm">Entity model filled with latest data.</param>
		/// <returns>Returns editing from view or redirects back to Index if save is successfull.</returns>
		[HttpPost]
		public ActionResult Edit(int id, AnswerEditVM answerEvm)
		{
			//form field validation passed?
			//if( ModelState.IsValid )
			//{
				if( answerEvm.Answer.Answers== null){
					ModelState.AddModelError("answer", "The answer cannont be empty");
					return View(answerEvm);
				}
				else if( answerEvm.Answer.Answers.Length < 3){
					ModelState.AddModelError("answer", "The answer must be at least 3 characters long");
					return View(answerEvm);
				}
            _answerRepo.Update(answerEvm);

				//save success, go back to the entity list
				return RedirectToAction("Content","Question", new { id = id});
			//}

			//form field validation failed, go back to the form
			//PopulateSelections(answerEvm);
			//return View(answerEvm);
		}

		/// </summary>
		/// <param name="id">ID of the entity to delete.</param>
		/// <returns>Deletion form view.</returns>
		public ActionResult Delete(int id, int idQ)
		{
			Answers answerLvm = new Answers();
			answerLvm.answer = _answerRepo.FindForDeletion(id);
			answerLvm.user=_userRepo.Find(Convert.ToInt32(TempData["id"]));
			//answerLvm.question.Id=23;
			return View(answerLvm);
		}

		/// <summary>
		/// This is invoked when deletion is confirmed in deletion form
		/// </summary>
		/// <param name="id">ID of the entity to delete.</param>
		/// <returns>Deletion form view on error, redirects to Index on success.</returns>
		[HttpPost]
		public ActionResult DeleteConfirm(int id, int idQ)
		{
			//try deleting, this will fail if foreign key constraint fails
			try
			{
                _answerRepo.Delete(id);

				//deletion success, redired to list form
				return RedirectToAction("Content","Question", new{id=idQ});
			}
			//entity in use, deletion not permitted
			catch( MySql.Data.MySqlClient.MySqlException )
			{
				//enable explanatory message and show delete form
				ViewData["deletionNotPermitted"] = true;
				Answers answerLvm = new Answers();
				answerLvm.answer = _answerRepo.FindForDeletion(id);
				answerLvm.user=_userRepo.Find(Convert.ToInt32(TempData["id"]));
				answerLvm.question.Id=idQ;

				return View("Delete", answerLvm);
			}
		}

		/// <summary>
		/// Populates select lists used to render drop down controls.
		/// </summary>
		/// <param name="answerEvm">'Automobilis' view model to append to.</param>
		/*public void PopulateSelections(AnswerEditVM answersEvm)
		{
			//load entities for the select lists
			var users = UserRepo.List();

			//build select lists
			answersEvm.Lists.Users =
				users.Select(it => {
					return
						new SelectListItem() {
							Value = Convert.ToString(it.Name),
							Text = Convert.ToString(it.Name)
						};
				})
				.ToList();
             //load entities for the select lists
			/*var questions = QuestionRepo.List();

			answersEvm.Lists.Questions =
				questions.Select(it => {
					return
						new SelectListItem() {
							Value = Convert.ToString(it.Questions),
							Text = Convert.ToString(it.Questions)
						};
				})
				.ToList();
			
		}*/
	}
}
