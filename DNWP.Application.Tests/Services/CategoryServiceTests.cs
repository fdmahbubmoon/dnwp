using DNWP.Application.Services;
using DNWP.Common.Exceptions;
using DNWP.Domain.Entities;
using DNWP.Domain.Models;
using DNWP.Repository.Base;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DNWP.Application")]

namespace DNWP.Application.Tests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task AddAsync_ValidCategory_ShouldReturnCategory()
    {
        // Arrange
        var categoryRepositoryMock = new Mock<IRepository<Category>>();
        var memoryCacheMock = new Mock<IMemoryCache>();
        var _sut = new CategoryService(categoryRepositoryMock.Object, memoryCacheMock.Object);

        var newCategory = new Category { Id = 1, CategoryName = "NewCategory" };

        categoryRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                      .ReturnsAsync((Category)null);
        // Act
        var result = await _sut.AddAsync(newCategory);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newCategory.Id, result.Id);
        Assert.Equal(newCategory.CategoryName, result.CategoryName);
    }

    [Fact]
    public async Task AddAsync_DuplicateCategory_ThrowsException()
    {
        // Arrange
        var mockRepository = new Mock<IRepository<Category>>();
        var mockMemoryCache = new Mock<IMemoryCache>();

        var _sut = new CategoryService(mockRepository.Object, mockMemoryCache.Object);

        var existingCategory = new Category { Id = 2, CategoryName = "ExistingCategory" };
        var duplicateCategory = new Category { Id = 3, CategoryName = "ExistingCategory" };

        mockRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                      .ReturnsAsync(existingCategory);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.AddAsync(duplicateCategory));

        mockRepository.Verify(repo => repo.InsertAsync(It.IsAny<Category>()), Times.Never);
        mockMemoryCache.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ValidId_UpdatesCategory()
    {
        // Arrange
        long categoryId = 1;
        var categoryDto = new CategoryDto { CategoryName = "UpdatedCategoryName" };
        var entityToUpdate = new Category { Id = categoryId, CategoryName = "OriginalCategoryName" };

        var repositoryMock = new Mock<IRepository<Category>>();
        repositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync((Category)null);

        repositoryMock.Setup(repo => repo.FirstOrDefaultAsync(categoryId))
            .ReturnsAsync(entityToUpdate);

        var memoryCacheMock = new Mock<IMemoryCache>();

        var _sut = new CategoryService(repositoryMock.Object, memoryCacheMock.Object);

        // Act
        var updatedCategory = await _sut.UpdateAsync(categoryId, categoryDto);

        // Assert
        Assert.Equal(categoryDto.CategoryName, updatedCategory.CategoryName);
        repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Once);
        memoryCacheMock.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ThrowsException()
    {
        // Arrange
        long categoryId = 2;
        var categoryDto = new CategoryDto { CategoryName = "CategoryName" };

        var repositoryMock = new Mock<IRepository<Category>>();
        var entityToUpdate = new Category { Id = 1, CategoryName = "CategoryName" };

        repositoryMock.Setup(repo => repo.FirstOrDefaultAsync(categoryId))
            .ReturnsAsync((Category)null);

        var memoryCacheMock = new Mock<IMemoryCache>();

        var _sut = new CategoryService(repositoryMock.Object, memoryCacheMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAsync(categoryId, categoryDto));

        repositoryMock.Verify(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Category, bool>>>()), Times.Never);
        repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Never);
        memoryCacheMock.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_CategoryAlreadyExists_ThrowsException()
    {
        // Arrange
        long categoryId = 2;
        var categoryDto = new CategoryDto { CategoryName = "CategoryName" };

        var repositoryMock = new Mock<IRepository<Category>>();
        var entityToUpdate = new Category { Id = 1, CategoryName = "CategoryName" };

        repositoryMock.Setup(
            repo => repo.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(entityToUpdate);

        repositoryMock.Setup(repo => repo.FirstOrDefaultAsync(categoryId))
            .ReturnsAsync(entityToUpdate);

        var memoryCacheMock = new Mock<IMemoryCache>();

        var _sut = new CategoryService(repositoryMock.Object, memoryCacheMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateAsync(categoryId, categoryDto));

        repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Never);
        memoryCacheMock.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
    }


}