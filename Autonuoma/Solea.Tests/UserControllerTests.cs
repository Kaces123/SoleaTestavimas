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

    // Set up
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
            // Stub Example
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
    [Fact]
    public void Logout_Redirects_To_Login()
    {
        var result = controller.Logout() as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Login", result.ActionName);
    }

    [Fact]
    public void Register_Success_Redirects_To_Login()
    {
        // Arrange
        var user = new User { Name = "newUser", Password = "newPassword" };
        mockUserRepo.Setup(repo => repo.Insert(user)).Verifiable(); // Ensure 'Insert' is called

        // Act
        var result = controller.Register(user) as RedirectToActionResult;

        // Assert
        mockUserRepo.Verify(); // Verify that 'Insert' was called
        Assert.NotNull(result);
        Assert.Equal("Login", result.ActionName);
    }



    [Fact]
    public void Profile_Returns_User_Details()
    {
        // Arrange
        int userId = 1; // Example user ID
        var user = new User { Id = userId, Name = "existingUser", Password = "correctPassword" };
        mockUserRepo.Setup(repo => repo.Find(userId)).Returns(user);
        controller.TempData["id"] = userId; // Set the expected TempData value

        // Act
        var result = controller.Profile() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result.Model);
    }


    [Fact]
    public void ChangePassword_Success_Redirects_To_Profile()
    {
        // Arrange
        var user = new User { Id = 1, Name = "existingUser", Password = "correctPassword" };
        mockUserRepo.Setup(repo => repo.Find(user.Id)).Returns(user);
        mockUserRepo.Setup(repo => repo.ChangePassword(user, "newPassword"));

        // Act
        var result = controller.ChangePassword(user, "newPassword") as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Profile", result.ActionName);
        Assert.Null(result.ControllerName);
    }


    [Fact]
    public void ChangePassword_Fails_With_Incorrect_CurrentPassword()
    {
        // Arrange
        var user = new User { Name = "existingUser", Password = "correctPassword" };
        mockUserRepo.Setup(repo => repo.Find(user)).Returns(user);

        // Act
        var result = controller.ChangePassword(user, "wrongCurrentPassword") as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.True(controller.ModelState["currentPassword"].Errors.Count > 0);
        Assert.Equal("Incorrect current password", controller.ModelState["currentPassword"].Errors[0].ErrorMessage);
    }

}   