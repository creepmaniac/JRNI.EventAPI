using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using JRNI.EventAPI.Controllers;
using JRNI.EventAPI.Interface;
using JRNI.EventAPI.Model;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JRNI.EventAPI.Tests
{
    [TestClass]
    public class EventsControllerTests
    {
        private IEventApiService? EventApiService;
        private const string Email = "test@example.com";

        [TestInitialize]
        public void TestInitialize()
        {
            EventApiService = Substitute.For<IEventApiService>();

        }
        [TestMethod]
        public async Task GetEventsAsync_ValidEmail_ReturnsOkResult()
        {
            // Arrange
            var expectedEvents = new List<Event> { new Event { Status = "Busy" } };

            EventApiService.GetFutureEventsAsync(Email).Returns(new OkObjectResult(expectedEvents));

            var controller = new EventsController(EventApiService);

            // Act
            var result = await controller.GetEventsAsync(Email);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task GetEventsAsync_InvalidEmail_ReturnsBadRequestResult()
        {
            // Arrange
            var eventApiService = Substitute.For<IEventApiService>();
            eventApiService.GetFutureEventsAsync(string.Empty).Returns(new StatusCodeResult((int)HttpStatusCode.BadRequest));
            var controller = new EventsController(eventApiService);

            // Act
            var result = await controller.GetEventsAsync(string.Empty);

            // Assert
            result.Result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task GetEventsAsync_ApiError_ReturnsTooManyRequests()
        {
            // Arrange
            var eventApiService = Substitute.For<IEventApiService>();
            eventApiService.GetFutureEventsAsync(Email).Returns(new StatusCodeResult((int)HttpStatusCode.TooManyRequests));

            var controller = new EventsController(eventApiService);

            // Act
            var result = await controller.GetEventsAsync(Email);

            // Assert
            result.Result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.TooManyRequests);
        }

        [TestMethod]
        public async Task GetEventsAsync_ApiError_ReturnsGatewayTimeout()
        {
            // Arrange

            EventApiService.GetFutureEventsAsync(Email).Returns(new StatusCodeResult((int)HttpStatusCode.GatewayTimeout));

            var controller = new EventsController(EventApiService);

            // Act
            var result = await controller.GetEventsAsync(Email);

            // Assert
            result.Result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.GatewayTimeout);
        }

        [TestMethod]
        public async Task GetEventsAsync_ApiError_ReturnsInternalServerError()
        {
            // Arrange
            var eventApiService = Substitute.For<IEventApiService>();
            eventApiService.GetFutureEventsAsync(Email).Returns(new StatusCodeResult((int)HttpStatusCode.InternalServerError));

            var controller = new EventsController(eventApiService);

            // Act
            var result = await controller.GetEventsAsync(Email);

            // Assert
            result.Result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
