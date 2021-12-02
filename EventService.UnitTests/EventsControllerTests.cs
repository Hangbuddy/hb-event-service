using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using EventService.Controllers;
using EventService.Data;
using EventService.Dtos;
using EventService.Dtos.Enums;
using EventService.Dtos.Requests;
using EventService.Dtos.Responses;
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
        private static readonly Guid eventId = Guid.NewGuid();
        private static readonly EventsProfile eventsProfile = new();
        private static readonly MapperConfiguration configuration = new(cfg => cfg.AddProfile(eventsProfile));
        private readonly IMapper _mapper = new Mapper(configuration);
        private static readonly GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        private readonly ClaimsPrincipal currentUser = new(new ClaimsIdentity(new Claim[] { new Claim("Id", "OwnerId") }));
        private readonly Event _event = new()
        {
            Id = eventId,
            OwnerId = "OwnerId",
            Title = "Title",
            EventType = EventType.Other,
            Description = "Description",
            IsActive = true,
            Location = geometryFactory.CreatePoint(new Coordinate(15, 10)),
            StartTime = DateTime.Parse("2021-12-07"),
            EndTime = DateTime.Parse("2021-12-07"),
            CreatedAt = DateTime.Parse("2021-11-12"),
            UpdatedAt = DateTime.Parse("2021-11-12")
        };
        private readonly Event _updatedEvent = new()
        {
            Id = eventId,
            OwnerId = "OwnerId",
            Title = "UpdatedTitle",
            EventType = EventType.Other,
            Description = "UpdatedDescription",
            IsActive = true,
            Location = geometryFactory.CreatePoint(new Coordinate(15, 10)),
            StartTime = DateTime.Parse("2021-12-07"),
            EndTime = DateTime.Parse("2021-12-07"),
            CreatedAt = DateTime.Parse("2021-11-12"),
            UpdatedAt = DateTime.Parse("2021-11-12")
        };
        private readonly EventCreateDto _eventCreateDto = new()
        {
            Title = "Title",
            EventType = EventType.Other,
            Description = "Description",
            Latitude = 10,
            Longitude = 15,
            StartTime = DateTime.Parse("2021-12-07"),
            EndTime = DateTime.Parse("2021-12-07")
        };
        private readonly EventUpdateDto _eventUpdateDto = new()
        {
            Id = eventId.ToString(),
            Title = "UpdatedTitle",
            EventType = EventType.Other,
            Description = "UpdatedDescription",
            IsActive = true,
            Latitude = 10,
            Longitude = 15,
            StartTime = DateTime.Parse("2021-12-07"),
            EndTime = DateTime.Parse("2021-12-07")
        };
        private readonly List<EventUserReadDto> _eventUserReadDtoList = new()
        {
            new EventUserReadDto() { UserId = "User1" },
            new EventUserReadDto() { UserId = "User2" },
        };
        private readonly EventUserUpdateDto _eventUserUpdateDto = new()
        {
            EventId = eventId.ToString(),
            UserId = "User1",
            Approved = true
        };
        private readonly List<EventReadDto> _eventReadDtoList = new()
        {
            new EventReadDto()
            {
                Id = eventId.ToString(),
                OwnerId = "OwnerId",
                Title = "Title",
                EventType = EventType.Other,
                Description = "Description",
                IsActive = true,
                Latitude = 10,
                Longitude = 15,
                StartTime = DateTime.Parse("2021-12-07"),
                EndTime = DateTime.Parse("2021-12-07"),
                CreatedAt = DateTime.Parse("2021-11-12"),
                UpdatedAt = DateTime.Parse("2021-11-12")
            },
            new EventReadDto()
            {
                Id = eventId.ToString(),
                OwnerId = "OwnerId",
                Title = "UpdatedTitle",
                EventType = EventType.Other,
                Description = "UpdatedDescription",
                IsActive = true,
                Latitude = 10,
                Longitude = 15,
                StartTime = DateTime.Parse("2021-12-07"),
                EndTime = DateTime.Parse("2021-12-07"),
                CreatedAt = DateTime.Parse("2021-11-12"),
                UpdatedAt = DateTime.Parse("2021-11-12")
            },
        };

        [Fact]
        public void GetEvent_WithUnexistingEvent_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetEvent(It.IsAny<string>()))
            .Returns((Event)null);
            var controller = new EventsController(repositoryStub.Object, _mapper);

            // Act
            var result = controller.GetEvent(eventId.ToString());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetEvent_WithExistingEvent_ReturnsExpectedEvent()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetEvent(It.IsAny<string>()))
            .Returns(_event);
            var controller = new EventsController(repositoryStub.Object, _mapper);

            // Act
            var actionResult = controller.GetEvent(eventId.ToString());

            // Assert
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();

            result.Value.Should().BeEquivalentTo(
                _event,
                opt => opt.Excluding(su => su.Location)
                          .Excluding(su => su.Id));

            var point = geometryFactory.CreatePoint(new Coordinate(((EventReadDto)result.Value).Longitude, ((EventReadDto)result.Value).Latitude));
            Assert.Equal(_event.Location, point);
            Assert.Equal(_event.Id.ToString(), ((EventReadDto)result.Value).Id);
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
                        .Excluding(su => su.OwnerId)
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
            var actionResult = controller.RegisterToEvent(eventId.ToString());

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
            var actionResult = controller.DeRegisterFromEvent(eventId.ToString());

            // Assert
            actionResult.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void GetWaitingList_WithCurrentUser_ReturnsEventList()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetWaitingList(It.IsAny<string>()))
            .Returns(new List<EventUser> {
                        new EventUser() { EventId = eventId.ToString(), UserId = "User1" },
                        new EventUser() { EventId = eventId.ToString(), UserId = "User2" }
                        });
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = currentUser };

            // Act
            var actionResult = controller.GetWaitingList(eventId.ToString());

            // Assert
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(_eventUserReadDtoList);
        }

        [Fact]
        public void GetApprovedList_WithCurrentUser_ReturnsEventList()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetApprovedList(It.IsAny<string>()))
            .Returns(new List<EventUser> {
                        new EventUser() { EventId = eventId.ToString(), UserId = "User1" },
                        new EventUser() { EventId = eventId.ToString(), UserId = "User2" }
                        });
            var controller = new EventsController(repositoryStub.Object, _mapper)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = currentUser };

            // Act
            var actionResult = controller.GetApprovedList(eventId.ToString());

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
            repositoryStub.Setup(repo => repo.GetNearbyEvents(It.IsAny<UserLocation>(), It.IsAny<double>()))
            .Returns(new List<Event>(){
                _event,
                _updatedEvent
            });
            var controller = new EventsController(repositoryStub.Object, _mapper);

            // Act
            var actionResult = controller.GetNearbyEvents(new UserLocationDto { Latitude = 1, Longitude = 1 }, 3);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();

            result.Value.Should().BeEquivalentTo(_eventReadDtoList);

        }

        [Fact]
        public void GetEventsInArea_WithExistingEvents_ReturnsExpectedList()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetEventsInArea(It.IsAny<Area>()))
            .Returns(new List<Event>(){
                _event,
                _updatedEvent
            });
            var controller = new EventsController(repositoryStub.Object, _mapper);

            // Act
            var actionResult = controller.GetEventsInArea(new AreaDto
            {
                NorthEastLocation = new UserLocationDto { Latitude = 1, Longitude = 1 },
                SouthWestLocation = new UserLocationDto { Latitude = 2, Longitude = 2 }
            });

            // Assert
            var result = actionResult.Result as OkObjectResult;
            result.Should().NotBeNull();

            result.Value.Should().BeEquivalentTo(_eventReadDtoList);

        }

    }
}