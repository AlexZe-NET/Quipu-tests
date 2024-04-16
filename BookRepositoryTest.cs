using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Quipu_task.Controllers;
using Quipu_task.Models;
using Quipu_task.Commands;
using Quipu_task.Queries;
using Quipu_task;
using MediatR;
using System.Net;

namespace Quipu_tests
{
    public class BookRepositoryTest
    {
        [Fact]
        public async Task UpdateExistingBook_Success()
        {
            // Arrange
            var bookId = 21;
            var mockRepository = new Mock<IBookRepository>();
            var existingBook = new Book { BookId = bookId, Title = "Existing Title", SubTitle = "Existing Subtitle" };
            mockRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(existingBook);

            var handler = new UpdateBookCommandHandler(mockRepository.Object);
            var updateCommand = new UpdateBookCommand
            {
                BookId = bookId,
                Title = "New Title",
                SubTitle = "New Subtitle"
            };

            // Act
            await handler.Handle(updateCommand, CancellationToken.None);

            // Assert
            mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Once);
            Assert.Equal("New Title", existingBook.Title);
            Assert.Equal("New Subtitle", existingBook.SubTitle);
        }

        [Fact]
        public async Task UpdateNonExistingBook_Fail()
        {
            // Arrange
            var bookId = 42;
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync((Book)null); // Simulate non-existing book

            var handler = new UpdateBookCommandHandler(mockRepository.Object);
            var updateCommand = new UpdateBookCommand
            {
                BookId = bookId,
                Title = "New Title",
                SubTitle = "New Subtitle"
            };

            // Act and Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(updateCommand, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteBook_ExistingId_DeletesBookAndReturnsNoContent()
        {
            // Arrange
            var bookId = 42;
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(new Book() { BookId = bookId, Title = "Book title"}); // Simulate existing book

            var handler = new DeleteBookCommandHandler(mockRepository.Object);
            var deleteCommand = new DeleteBookCommand { BookId = bookId };

            // Act
            await handler.Handle(deleteCommand, CancellationToken.None);

            // Assert
            mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBook_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var bookId = 99;
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync((Book)null); // Simulate non-existing book

            var handler = new DeleteBookCommandHandler(mockRepository.Object);
            var deleteCommand = new DeleteBookCommand { BookId = bookId };

            // Act and Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(deleteCommand, CancellationToken.None));
        }

    }
}

    
