using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using EventService.Controllers;
using EventService.Data;
using EventService.Dtos;
using EventService.Models;
using EventService.Profiles;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Xunit;

namespace EventService.UnitTests
{
    public class EventsControllerTests
    {
        private readonly Mock<IEventRepo> repositoryStub = new();
        private static readonly Random _random = new();
        private static readonly EventsProfile eventsProfile = new();
        private static readonly MapperConfiguration configuration = new(cfg => cfg.AddProfile(eventsProfile));
        private readonly IMapper _mapper = new Mapper(configuration);
        private static readonly GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        private readonly ClaimsPrincipal currentUser = new(new ClaimsIdentity(new Claim[] { new Claim("Id", "OwnerId") }));
        private readonly ClaimsPrincipal anotherUser = new(new ClaimsIdentity(new Claim[] { new Claim("Id", "UnauthorizedUserId") }));
        private readonly Event _event = new()
        {
            Id = 1,
            OwnerId = "OwnerId",
            Title = "Title",
            Description = "Description",
            PermissionRequired = false,
            IsActive = true,
            Location = geometryFactory.CreatePoint(new Coordinate(15, 10)),
            CreatedAt = DateTime.Parse("2021-11-12"),
            UpdatedAt = DateTime.Parse("2021-11-12")
        };
        private readonly Event _updatedEvent = new()
        {
            Id = 1,
            OwnerId = "OwnerId",
            Title = "UpdatedTitle",
            Description = "UpdatedDescription",
            PermissionRequired = false,
            IsActive = true,
            Location = geometryFactory.CreatePoint(new Coordinate(15, 10)),
            CreatedAt = DateTime.Parse("2021-11-12"),
            UpdatedAt = DateTime.Parse("2021-11-12")
        };
        private readonly EventCreateDto _eventCreateDto = new()
        {
            OwnerId = "OwnerId",
            Title = "Title",
            Description = "Description",
            PermissionRequired = false,
            Latitude = 10,
            Longitude = 15
        };
        private readonly EventUpdateDto _eventUpdateDto = new()
        {
            Id = 1,
            OwnerId = "OwnerId",
            Title = "UpdatedTitle",
            Description = "UpdatedDescription",
            PermissionRequired = false,
            IsActive = true,
            Latitude = 10,
            Longitude = 15
        };
        private readonly EventUserDto _eventUserDto = new()
        {
            EventId = 1,
            UserId = "OwnerId"
        };
        private readonly List<EventUserReadDto> _eventUserReadDtoList = new()
        {
            new EventUserReadDto() { UserId = "User1" },
            new EventUserReadDto() { UserId = "User2" },
        };
        private readonly EventUserUpdateDto _eventUserUpdateDto = new()
        {
            EventId = 1,
            UserId = "User1",
            Approved = true
        };

        private readonly List<EventReadDto> _eventReadDtoList = new()
        {
            new EventReadDto() { Id = 1, OwnerId = "OwnerId", Title = "Title", Description = "Description", PermissionRequired = false, IsActive = true, Latitude = 10, Longitude = 15, CreatedAt = DateTime.Parse("2021-11-12"), UpdatedAt = DateTime.Parse("2021-11-12") },
            new EventReadDto() { Id = 1, OwnerId = "OwnerId", Title = "UpdatedTitle", Description = "UpdatedDescription", PermissionRequired = false, IsActive = true, Latitude = 10, Longitude = 15, CreatedAt = DateTime.Parse("2021-11-12"), UpdatedAt = DateTime.Parse("2021-11-12") },
        };

        [Fact]
        public void GetEvent_WithUnexistingEvent_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetEvent(It.IsAny<int>()))
            .Returns((Event)null);
            var controller = new EventsController(repositoryStub.Object, _mapper);

            // Act
            var result = controller.GetEvent(_random.Next(1000));

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetEvent_WithExistingEvent_ReturnsExpectedEvent()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetEvent(It.IsAny<int>()))
            .Returns(_event);
            var controller = new EventsController(repositoryStub.Object, _mapper);

            // Act
            var actionResult = controller.GetEvent(_random.Next(1000));

            // Assert
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();

            result.Value.Should().BeEquivalentTo(
                _event,
                opt => opt.Excluding(su => su.Location));

            var point = geometryFactory.CreatePoint(new Coordinate(((EventReadDto)result.Value).Longitude, ((EventReadDto)result.Value).Latitude));
            Assert.Equal(_event.Location, point);
        }

        [Fact]
        public void CreateEvent_WithAnotherUser_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = anotherUser };

            // Act
            var result = controller.CreateEvent(_eventCreateDto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void CreateEvent_WithCurrentUser_ReturnsEvent()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.CreateEvent(It.IsAny<Event>()))
            .Verifiable();
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = currentUser };

            // Act
            var actionResult = controller.CreateEvent(_eventCreateDto);

            // Assert
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(_event,
                opt => opt
                .Excluding(su => su.Id)
                .Excluding(su => su.CreatedAt)
                .Excluding(su => su.UpdatedAt)
                .Excluding(su => su.Location)
                );


            var point = geometryFactory.CreatePoint(new Coordinate(((EventReadDto)result.Value).Longitude, ((EventReadDto)result.Value).Latitude));
            Assert.Equal(_event.Location, point);
        }

        [Fact]
        public void UpdateEvent_WithAnotherUser_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = anotherUser };

            // Act
            var result = controller.UpdateEvent(_eventUpdateDto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void UpdateEvent_WithCurrentUser_ReturnsEvent()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.UpdateEvent(It.IsAny<Event>()))
            .Verifiable();
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = currentUser
            };

            // Act
            var actionResult = controller.UpdateEvent(_eventUpdateDto);

            // Assert
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(_updatedEvent, opt => opt
                .Excluding(su => su.Id)
                        .Excluding(su => su.CreatedAt)
                        .Excluding(su => su.UpdatedAt)
                        .Excluding(su => su.Location)
                        );

            var point = geometryFactory.CreatePoint(new Coordinate(((EventUpdateDto)result.Value).Longitude, ((EventUpdateDto)result.Value).Latitude));
            Assert.Equal(_event.Location, point);
        }

        [Fact]
        public void RegisterToEvent_WithCurrentUser_ReturnsEvent()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.RegisterToEvent(It.IsAny<EventUser>()))
            .Verifiable();
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = currentUser };

            // Act
            var actionResult = controller.RegisterToEvent(1);

            // Assert
            actionResult.Should().BeOfType<OkResult>();

        }

        [Fact]
        public void DeRegisterFromEvent_WithCurrentUser_ReturnsEvent()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.DeRegisterFromEvent(It.IsAny<EventUser>()))
            .Verifiable();
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = currentUser };

            // Act
            var actionResult = controller.DeRegisterFromEvent(1);

            // Assert
            actionResult.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void GetWaitingList_WithCurrentUser_ReturnsEventList()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetWaitingList(It.IsAny<int>()))
            .Returns(new List<EventUser> {
                        new EventUser() { EventId = 1, UserId = "User1" },
                        new EventUser() { EventId = 1, UserId = "User2" }
                        });
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = currentUser };

            // Act
            var actionResult = controller.GetWaitingList(1);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(_eventUserReadDtoList);
        }

        [Fact]
        public void GetApprovedList_WithCurrentUser_ReturnsEventList()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetApprovedList(It.IsAny<int>()))
            .Returns(new List<EventUser> {
                        new EventUser() { EventId = 1, UserId = "User1" },
                        new EventUser() { EventId = 1, UserId = "User2" }
                        });
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = currentUser };

            // Act
            var actionResult = controller.GetApprovedList(1);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(_eventUserReadDtoList);
        }

        [Fact]
        public void UpdateEventUser_WithCurrentUser_ReturnsEvent()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.UpdateEventUser(It.IsAny<EventUser>()))
            .Verifiable();
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = currentUser
            };

            // Act
            var result = controller.UpdateEventUser(_eventUserUpdateDto);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void GetNearbyEvents_WithExistingEvents_ReturnsExpectedList()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetNearbyEvents(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
            .Returns(new List<Event>(){
                _event,
                _updatedEvent
            });
            var controller = new EventsController(repositoryStub.Object, _mapper);

            // Act
            var actionResult = controller.GetNearbyEvents(1, 2, 3);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();

            result.Value.Should().BeEquivalentTo(_eventReadDtoList);

        }

    }
}