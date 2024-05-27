using System;
using Xunit;
using TechObjektBackend.Services;
using TechObjektBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace TechObjektBackend.Tests;

using Accord.Math;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TechObjektBackend.Controllers;

public class newbuilderControllerTests
{
    public class NewbuilderServicesTests : IDisposable
    {
        private readonly IMongoCollection<NewBuildings> _collection;
        private readonly NewBuildingsServices _service;

        public NewbuilderServicesTests()
        {
            var settings = new ProjectDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "techobjekt",
                NewBuildingsCollectionName = "newBuildingsTests"
            };

            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<NewBuildings>(settings.NewBuildingsCollectionName);

            _service = new NewBuildingsServices(Options.Create(settings));
        }

        public void Dispose()
        {
            // Clean up after each test if necessary
            _collection.Database.DropCollection(_collection.CollectionNamespace.CollectionName);
        }

        [Fact]
        public async Task GetAsync_ReturnsAllPrisoners()
        {
            // Arrange: Insert some test data into the database
            var testData = new List<NewBuildings>
            {
                new NewBuildings { Id = ObjectId.GenerateNewId().ToString(),
                    IdRegion = "PolskaID",
                    Name = "Polska",
                    Values = new List<YearData>
                    {
                        new YearData { Year = "2021", Val = 100, AttrId = 1 },
                        new YearData { Year = "2022", Val = 200, AttrId = 2 }
                    }
                },
                new NewBuildings { Id = ObjectId.GenerateNewId().ToString(),
                    IdRegion = "PolskaID",
                    Name = "Polska",
                    Values = new List<YearData>
                    {
                        new YearData { Year = "2021", Val = 200, AttrId = 1 },
                        new YearData { Year = "2022", Val = 300, AttrId = 2 }
                    }
                },
            };
            await _collection.InsertManyAsync(testData);

            var result = await _service.GetAsync();

            // Assert: Check if all prisoners are returned
            Assert.NotNull(result);
            Assert.Equal(testData.Count, result.Count);
        }

        [Fact]
        public async Task Post_CreatePrisoners()
        {
            // Arrange: Insert some test data into the database
            var testData = new List<NewBuildings>
            {
                new NewBuildings { Id = ObjectId.GenerateNewId().ToString(),
                    IdRegion = "PolskaID",
                    Name = "Polska",
                    Values = new List<YearData>
                    {
                        new YearData { Year = "2021", Val = 100, AttrId = 1 },
                        new YearData { Year = "2022", Val = 200, AttrId = 2 }
                    }
                },
                new NewBuildings { Id = ObjectId.GenerateNewId().ToString(),
                    IdRegion = "PolskaID",
                    Name = "Polska",
                    Values = new List<YearData>
                    {
                        new YearData { Year = "2021", Val = 200, AttrId = 1 },
                        new YearData { Year = "2022", Val = 300, AttrId = 2 }
                    }
                },
            };
            await _collection.InsertManyAsync(testData);

            // Act: Call the GetAsync method from the service
            var result = await _service.GetAsync();

            // Assert: Check if all prisoners are returned
            Assert.NotNull(result);
            Assert.Equal(testData.Count, result.Count);
        }

        [Fact]
        public async Task Update_ReturnsNoContentResult()
        {
            // Arrange
            var service = _service; // Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new NewBuildingsController(_service);
            var id = "6641cf304d01a436d6a5e1a1";
            var updatedNewBuildings = new NewBuildings
            {
                Id = "6641cf304d01a436d6a5e1a1",
                IdRegion = "PolskaID",
                Name = "Polska",
                Values = new List<YearData>
                    {
                        new YearData { Year = "2021", Val = 200, AttrId = 1 },
                        new YearData { Year = "2022", Val = 300, AttrId = 2 }
                    },
            };
            _collection.InsertOne(updatedNewBuildings);
            // Act
            var result = await controller.Update(id, updatedNewBuildings);

            // Assert

            var newBuildings = await service.GetAsync(id);
            Assert.Equal(updatedNewBuildings.IdRegion, newBuildings.IdRegion);
            
        }

        [Fact]
        public async Task Delete_ReturnsNoContentResult()
        {
            var controller = new NewBuildingsController(_service);
            var id = "6641cf304d01a436d6a5e1a1";
            var updatedNewBuildings = new NewBuildings
            {
                Id = "6641cf304d01a436d6a5e1a1",
                IdRegion = "PolskaID",
                Name = "Polska",
                Values = new List<YearData>
                    {
                        new YearData { Year = "2021", Val = 200, AttrId = 1 },
                        new YearData { Year = "2022", Val = 300, AttrId = 2 }
                    },
            };
            _collection.InsertOne(updatedNewBuildings);
            // Act
            var result = await controller.Delete(id);

            // Assert

            var actionResult = Assert.IsType<NoContentResult>(result);
            var newBuildings = await _service.GetAsync(id); // Sprawdzamy, czy serwis został wywołany z poprawnymi parametrami
            Assert.Null(newBuildings);
            // Możemy również sprawdzić inne właściwości, jeśli to konieczne
        }

        [Fact]
        public async Task Get_WithValidId_ReturnsNewBuilding()
        {
            // Arrange
            var service = _service;// Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new NewBuildingsController(_service);
            var id = "6641cf304d01a436d6a5e1a1";
            var expectedNewBuilding = new NewBuildings
            {
                Id = "6641cf304d01a436d6a5e1a1",
                IdRegion = "PolskaID",
                Name = "Polska",
                Values = new List<YearData>
                    {
                        new YearData { Year = "2021", Val = 200, AttrId = 1 },
                        new YearData { Year = "2022", Val = 300, AttrId = 2 }
                    },
            };
            _collection.InsertOne(expectedNewBuilding);
           
            
            var result = await _service.GetAsync(id);
            var buildings = Assert.IsType<NewBuildings>(result);

            Assert.Equal(expectedNewBuilding.Id, buildings.Id);

        }

        [Fact]
        public async Task Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var service = _service; // Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new NewBuildingsController(service);
            var id = "6641cf304d01a436d6a5eaaa";

            // Act
            var result = await controller.Get(id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<NewBuildings>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }



        // Similar tests can be written for other methods like GetAsync(string id), CreateAsync, UpdateAsync, RemoveAsync, etc.
    }
}
