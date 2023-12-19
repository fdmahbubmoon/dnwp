using DNWP.Application.RepositoryInterfaces;
using DNWP.Application.Services;
using DNWP.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace DNWP.Application.Tests.Services;

public class BaseServiceFactoryTests
{
    [Fact]
    public async Task AddAsync_Category_ShouldReturnCategory()
    {
        string cacheKey = nameof(Category);
        // Arrange
        var repositoryMock = new Mock<IRepository<Category>>();
        var memoryCacheMock = new Mock<IMemoryCache>();
        var baseServiceFactory = new BaseServiceFactory<Category>(repositoryMock.Object, cacheKey, memoryCacheMock.Object);

        var newCategory = new Category { Id = 1, CategoryName = "NewCategory" };

        repositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Func<Category, bool>>()))
            .ReturnsAsync((Category)null);

        memoryCacheMock.Setup(repo => repo.Remove(It.IsAny<Func<string>>()))
            .Verifiable();

        // Act
        var result = await baseServiceFactory.AddAsync(newCategory);

        // Assert
        memoryCacheMock.Verify(x=>x.Remove(cacheKey), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(newCategory.Id, result.Id);
        Assert.Equal(newCategory.CategoryName, result.CategoryName);
    }

    [Fact]
    public async Task AddAsync_Item_ShouldReturnItem()
    {
        // Arrange
        var repositoryMock = new Mock<IRepository<Item>>();
        var memoryCacheMock = new Mock<IMemoryCache>();
        var baseServiceFactory = new BaseServiceFactory<Item>(repositoryMock.Object, "Item", memoryCacheMock.Object);

        var newItem = new Item { 
            Id = 1, 
            ItemName = "New Item",
            ItemUnit = "KG",
            ItemQuantity = 2,
            CategoryId = 1
        };

        repositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Func<Item, bool>>()))
            .ReturnsAsync((Item)null);

        // Act
        var result = await baseServiceFactory.AddAsync(newItem);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newItem.Id, result.Id);
        Assert.Equal(newItem.ItemName, result.ItemName);
    }
}
