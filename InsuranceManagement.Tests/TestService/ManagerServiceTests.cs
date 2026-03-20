using FluentAssertions;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Infrastructure.Data;
using InsuranceManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class ManagerServiceTests
{
    private InsuranceDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<InsuranceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new InsuranceDbContext(options);
    }

    [Fact]
    public async Task DeleteManagerAsync_ShouldDeleteManager_WhenManagerExists()
    {
        // Arrange
        var context = GetDbContext();

        var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
        var manager = new Manager
        {
            Id = Guid.NewGuid(),
            User = user,
            FirstName = "John",
            FatherName = "Doe",
            GrandFatherName = "Smith",
            NationalIdOrPassport = "AB123456",
            PhoneNumber = "0912345678",
            Region = "Addis Ababa",
            City = "Addis Ababa",
            SubCity = "Bole"
        };

        var emailServiceMock = new Mock<IEmailService>();

        context.Users.Add(user);
        context.Managers.Add(manager);
        await context.SaveChangesAsync();

        var service = new ManagerService(context, emailServiceMock.Object);

        // Act
        var result = await service.DeleteManagerAsync(manager.Id);

        // Assert
        result.Should().BeTrue();
        (await context.Managers.CountAsync()).Should().Be(0);
        (await context.Users.CountAsync()).Should().Be(0);
    }

}
