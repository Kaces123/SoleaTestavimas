using Moq;
using Org.Ktu.Isk.P175B602.Autonuoma.Controllers;
using Org.Ktu.Isk.P175B602.Autonuoma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Org.Ktu.Isk.P175B602.Autonuoma.Repositories;
using Microsoft.AspNetCore.Http;
using Org.Ktu.Isk.P175B602.Autonuoma.ViewModels;
using static Org.Ktu.Isk.P175B602.Autonuoma.ViewModels.AnswerEditVM;

namespace Solea.Tests;

public class QuestionControllerTests
{
    private readonly Mock<ILikedRepo> _mockLikedRepo;
    private readonly Mock<IUserRepo> _mockUserRepo;
    private readonly Mock<IAnswerRepo> _mockAnswerRepo;
    private readonly Mock<IQuestionRepo> _mockQuestionRepo;
    private QuestionController _controller;

    public QuestionControllerTests()
    {
        _mockUserRepo = new Mock<IUserRepo>();
        _mockAnswerRepo = new Mock<IAnswerRepo>();
        _mockLikedRepo = new Mock<ILikedRepo>();
        _mockQuestionRepo = new Mock<IQuestionRepo>();
        _controller = new QuestionController(_mockUserRepo.Object, _mockAnswerRepo.Object, _mockLikedRepo.Object, _mockQuestionRepo.Object); // Pass both mocked repositories
    }
    [Fact]
    public void Create_ReturnsViewResult_WithQuestionEditVM()
    {
        // Arrange
        int userId = 123;
        var user = new User { Name = "Test User", Id = userId };
        _mockUserRepo.Setup(repo => repo.Find(It.IsAny<int>())).Returns(user);

        var controller = new QuestionController(_mockUserRepo.Object,_mockAnswerRepo.Object,_mockLikedRepo.Object, _mockQuestionRepo.Object);
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        tempData["id"] = userId;
        controller.TempData = tempData;

        // Act
        var result = controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<QuestionEditVM>(viewResult.Model);

        Assert.NotNull(model);
        Assert.Equal(userId, model.user.Id);
        Assert.Equal(userId, model.Lists.id);
        Assert.Equal(user.Name, model.Question.fk_User);

    }
    [Fact]
    public void Delete_RedirectsToCorrectRoute_AfterDeletion()
    {
        int questionId = 2; // Example question ID or related ID
        _mockQuestionRepo.Setup(repo => repo.Delete(questionId)); // Mock the Delete method

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
        var result = _controller.Delete(questionId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Answers>(viewResult.Model);
    }
    [Fact]
    public void Edit_ReturnsViewResult_WithCorrectModel()
    {
        // Arrange
        int id = 1; // Example id for Question

        var questionEditVM = new QuestionEditVM
        {
            Lists = new QuestionEditVM.ListsM
            {
                // Initialize necessary properties if needed
            },
            Question = new QuestionEditVM.QuestionM
            {
                // Initialize necessary properties if needed
            },
            user = new User()
        };

        int userId = 123;
        var user = new User { Name = "Test User", Id = userId };
        _mockUserRepo.Setup(repo => repo.Find(It.IsAny<int>())).Returns(user);
        _mockQuestionRepo.Setup(repo => repo.Find(id)).Returns(questionEditVM);

        var httpContext = new DefaultHttpContext();
        _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _controller.TempData["id"] = 123; // Example valid user ID as an integer

        // Act
        var result = _controller.Edit(id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<QuestionEditVM>(viewResult.Model);


        Assert.Equal("Test User", model.user.Name);
    }
   

}   