using Moq;
using Xunit;
using Org.Ktu.Isk.P175B602.Autonuoma.Controllers;
using Org.Ktu.Isk.P175B602.Autonuoma.Repositories;
using Org.Ktu.Isk.P175B602.Autonuoma.Models;
using Org.Ktu.Isk.P175B602.Autonuoma.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace Solea.Tests
{
    public class AnswerControllerTests
    {
        private readonly Mock<ILikedRepo> _mockLikedRepo;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly Mock<IAnswerRepo> _mockAnswerRepo; // Assuming IAnswerRepo exists
        private AnswerController _controller;


        public AnswerControllerTests()
        {
            _mockUserRepo = new Mock<IUserRepo>();
            _mockAnswerRepo = new Mock<IAnswerRepo>();
            _mockLikedRepo = new Mock<ILikedRepo>();
            _controller = new AnswerController(_mockUserRepo.Object, _mockAnswerRepo.Object, _mockLikedRepo.Object); // Pass both mocked repositories
        }
    //Test Delete
    [Fact]
        public void Delete_RedirectsToCorrectRoute_AfterDeletion()
        {
            // Arrange
            int answerId = 1; // Example answer ID
            int questionId = 2; // Example question ID or related ID
            _mockAnswerRepo.Setup(repo => repo.Delete(answerId)); // Mock the Delete method

            // Mock other dependencies used in Delete method
            // For example: _mockAnswerRepo.Setup(repo => repo.SomeMethod()).Returns(someValue);

            // Setup HttpContext if used in the Delete method
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Mock TempData if used in the Delete method
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = _controller.Delete(answerId, questionId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Answers>(viewResult.Model);
        }

        //Test Create
       [Fact]
        public void Create_ReturnsAViewResult_WithAnswerEditVM()
        {
            // Arrange
            string questionId = "some_question_id";
            int userId = 123;
            var user = new User { Name = "Test User", Id = userId };
            _mockUserRepo.Setup(repo => repo.Find(It.IsAny<int>())).Returns(user);

            // Set TempData for the controller
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Mock additional setups if any object is used in AnswerController.Create method

            // Act
            var result = _controller.Create(questionId, userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AnswerEditVM>(viewResult.ViewData.Model);
            Assert.Equal(questionId, model.Answer.fk_Questions);
            Assert.Equal(userId, model.Lists.Questions_Id);
            Assert.Equal(user.Name, model.Answer.fk_User);
        }
        //Test Delete Confirm
        [Fact]
        public void DeleteConfirm_WithSuccessfulDeletion_RedirectsToContent()
        {
            // Arrange
            int id = 1; // Example entity ID
            int questionId = 2; // Example question ID
            _mockAnswerRepo.Setup(repo => repo.Delete(id)); // Mock the Delete method

            // Act
            var result = _controller.DeleteConfirm(id, questionId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Content", redirectToActionResult.ActionName);
            Assert.Equal("Question", redirectToActionResult.ControllerName);
            Assert.Equal(questionId, redirectToActionResult.RouteValues["id"]);
        }

        //Test Edit
        [Fact]
        public void Edit_WithValidModel_RedirectsToContent()
        {
            // Arrange
            int id = 1; // Example entity ID
            var answerEvm = new AnswerEditVM
            {
                Answer = new AnswerEditVM.AnswerM
                {
                    Answers = "Valid answer longer than 3 characters"
                }
            };

            var mockAnswerRepo = new Mock<IAnswerRepo>();
            mockAnswerRepo.Setup(repo => repo.Update(It.IsAny<AnswerEditVM>())); // Mock the Update method

            // Act
            var result = _controller.Edit(id, answerEvm);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Content", redirectToActionResult.ActionName);
            Assert.Equal("Question", redirectToActionResult.ControllerName);
            Assert.Equal(id, redirectToActionResult.RouteValues["id"]);
        }
        //Test Edit returns Action result
         [Fact]
        public void Edit_ReturnsViewResult_WithCorrectModel()
        {
            // Arrange
            int id = 1; // Example id for Question
            string q = "some_question_identifier"; // Example question identifier
            int id1 = 2; // Example id for Answer

            var answerEvm = new AnswerEditVM
            {
                Answer = new AnswerEditVM.AnswerM
                {
                    // Initialize necessary properties if needed
                },
                Lists = new AnswerEditVM.ListsM
                {
                    // Initialize necessary properties if needed
                },
                user = new User()
            };

            _mockAnswerRepo.Setup(repo => repo.Find(id1)).Returns(answerEvm);
            _mockUserRepo.Setup(repo => repo.Find(It.IsAny<int>())).Returns(new User());

            // Mock TempData
            var httpContext = new DefaultHttpContext();
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _controller.TempData["id"] = 123; // Example valid user ID as an integer

            // Act
            var result = _controller.Edit(id, q, id1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AnswerEditVM>(viewResult.Model);
            Assert.Equal(q, model.Answer.fk_Questions);
            Assert.Equal(id, model.Lists.Questions_Id);
            // Additional assertions as needed
        }
    
    }
}
