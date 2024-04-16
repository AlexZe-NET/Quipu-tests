using Moq;
using Quipu_task.Commands;
using Quipu_task.Models;
using Quipu_task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quipu_task.Queries;
using System.Linq.Expressions;

namespace Quipu_tests
{
    public class AuthorRepositorytest
    {
        [Fact]
        public async Task DeleteAuthor_ExistingId_DeletesAuthorAndReturnsNoContent()
        {
            // Arrange
            var authorId = 42;
            var mockRepository = new Mock<IAuthorRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(new Author() { AuthorId = authorId, Name = "John Doe" }); // Simulate existing author

            var handler = new DeleteAuthorCommandHandler(mockRepository.Object);
            var deleteCommand = new DeleteAuthorCommand { AuthorId = authorId };

            // Act
            await handler.Handle(deleteCommand, CancellationToken.None);

            // Assert
            mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Author>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthor_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var authorId = 99;
            var mockRepository = new Mock<IAuthorRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync((Author)null); // Simulate non-existing author

            var handler = new DeleteAuthorCommandHandler(mockRepository.Object);
            var deleteCommand = new DeleteAuthorCommand { AuthorId = authorId };

            // Act and Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(deleteCommand, CancellationToken.None));
        }

        [Fact]
        public async Task GetAuthorDetails_ExistingId_ReturnsAuthorName()
        {
            // Arrange
            var authorId = 42; // Replace with an actual author ID
            var mockRepository = new Mock<IAuthorRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(new Author() { AuthorId = authorId, Name = "John Doe" }); // Simulate existing author

            var handler = new GetAuthorQueryHandler(mockRepository.Object);
            var query = new GetAuthorQuery { AuthorId = authorId };

            // Act
            var authorDetails = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal("John Doe", authorDetails.Name);
        }

        [Fact]
        public async Task GetAuthorDetails_NonExistingId_ReturnsNull()
        {
            // Arrange
            var authorId = 99;
            var mockRepository = new Mock<IAuthorRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync((Author)null); // Simulate non-existing author

            var handler = new GetAuthorQueryHandler(mockRepository.Object);
            var query = new GetAuthorQuery { AuthorId = authorId };

            // Act
            var authorDetails = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(authorDetails);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { AuthorId = 1, Name = "John Doe" },
                new Author { AuthorId = 2, Name = "Jane Smith" }
            };

            var mockRepository = new Mock<IAuthorRepository>();
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(authors);

            var handler = new GetAllAuthorsQueryHandler(mockRepository.Object);
            var query = new GetAllAuthorsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(authors.Count, result.Count);
            Assert.Equal(authors.Select(a => a.Name), result.Select(a => a.Name));
        }
    }
}
