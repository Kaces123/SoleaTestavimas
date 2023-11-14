using Moq;
using Org.Ktu.Isk.P175B602.Autonuoma.Controllers;
using Org.Ktu.Isk.P175B602.Autonuoma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Org.Ktu.Isk.P175B602.Autonuoma.Repositories;
using Microsoft.AspNetCore.Http;

namespace Solea.Tests;

public class UserControllerTests
{
   private readonly Mock<IUserRepo> mockUserRepo;
    private readonly UserController controller;
    private readonly TempDataDictionary tempData;
    private readonly Mock<ITempDataProvider> tempDataProvider;

    public UserControllerTests()
    {
        // Arrange
        mockUserRepo = new Mock<IUserRepo>();
        tempDataProvider = new Mock<ITempDataProvider>();
        tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);
        controller = new UserController(mockUserRepo.Object)
        {
            TempData = tempData
        };
    }

    [Fact]
    public void Login_Fails_With_Incorrect_Credentials()
    {
        // Arrange
        var user = new User { Name = "existingUser", Password = "wrongPassword" };
        mockUserRepo.Setup(repo => repo.Find(user)).Returns(new User { Name = "existingUser", Password = "correctPassword" });

        // Act
        var result = controller.Login(user) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(controller.ModelState["password"].Errors.Count > 0);
        Assert.Equal("Incorrect name or password", controller.ModelState["password"].Errors[0].ErrorMessage);
    }

    [Fact]
    public void Login_Success_Redirects_To_Question_Index()
    {
        // Arrange
        var user = new User { Name = "existingUser123zxc", Password = "correctPassword" };
        mockUserRepo.Setup(repo => repo.Find(user)).Returns(user); // Return the same user object for successful login

        // Act
        var result = controller.Login(user) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Question", result.ControllerName);
    }

    [Fact]
    public void Login_Succeeds_With_Correct_Credentials()
    {
        // Arrange
        var user = new User { Name = "existingUser", Password = "correctPassword" };
        mockUserRepo.Setup(repo => repo.Find(It.Is<User>(u => u.Name == user.Name && u.Password == user.Password))).Returns(user);

        // Act
        var result = controller.Login(user) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName); // Assuming that "Index" is the action to redirect after a successful login
        Assert.Equal("Question", result.ControllerName); // Assuming that "Question" is the controller to redirect after a successful login
    }


    [Theory]
    [InlineData("existingUser", "correctPassword", true)]
    [InlineData("existingUser", "wrongPassword", false)]
    [InlineData("nonExistentUser", "anyPassword", false)]
    public void Login_Returns_Correct_Result(string username, string password, bool shouldSucceed)
    {
        // Arrange
        var user = new User { Name = username, Password = password };
        if (shouldSucceed)
        {
            mockUserRepo.Setup(repo => repo.Find(It.IsAny<User>())).Returns(user);
        }
        else
        {
mockUserRepo.Setup(repo => repo.Find(It.Is<User>(u => u.Name == username && u.Password == password)))
            .Returns(shouldSucceed ? user : null);

        }

        // Act
        var result = controller.Login(user);

        // Assert
        if (shouldSucceed)
        {
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Question", redirectResult.ControllerName);
        }
        else
        {
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState["password"].Errors.Count > 0);
            Assert.Equal("Incorrect name or password", controller.ModelState["password"].Errors[0].ErrorMessage);
        }
    }
}   