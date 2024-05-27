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

public class prisonersControllerTests
{
    public class PrisonersServicesTests : IDisposable
    {
        private readonly IMongoCollection<Prisoners> _collection;
        private readonly PrisonersServices _service;

        public PrisonersServicesTests()
        {
            var settings = new ProjectDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "techobjekt",
                PrisonersCollectionName = "prisonersTests"
            };

            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Prisoners>(settings.PrisonersCollectionName);

            _service = new PrisonersServices(Options.Create(settings));
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
            var testData = new List<Prisoners>
            {
                new Prisoners { Id = ObjectId.GenerateNewId().ToString(),
                    VariableName = "Osadzeni w zakładach karnych i aresztach śledczych",
                    Country = "Polska",
                    PrisionerType = "ogółem",
                    CategoriesInmates = "ogółem",
                    Sex = "kobiety",
                    InformationTypeUnitofMeasure = "wartość [osoba]",
                    Year = 2022,
                    Value = 3422 },
                new Prisoners { Id = ObjectId.GenerateNewId().ToString(),
                    VariableName = "Osadzeni w zakładach karnych i aresztach śledczych",
                    Country = "Polska",
                    PrisionerType = "ogółem",
                    CategoriesInmates = "ogółem",
                    Sex = "Mezczyzni",
                    InformationTypeUnitofMeasure = "wartość [osoba]",
                    Year = 2023,
                    Value = 3422 }
            };
            await _collection.InsertManyAsync(testData);
            
            // Act: Call the GetAsync method from the service
            var result = await _service.GetAsync();

            // Assert: Check if all prisoners are returned
            Assert.NotNull(result);
            Assert.Equal(testData.Count, result.Count);
        }

        [Fact]
        public async Task Post_CreatePrisoners()
        {
            // Arrange: Insert some test data into the database
            var testData = new List<Prisoners>
            {
                new Prisoners { Id = ObjectId.GenerateNewId().ToString(),
                    VariableName = "Osadzeni w zakładach karnych i aresztach śledczych",
                    Country = "Polska",
                    PrisionerType = "ogółem",
                    CategoriesInmates = "ogółem",
                    Sex = "kobiety",
                    InformationTypeUnitofMeasure = "wartość [osoba]",
                    Year = 2022,
                    Value = 3422 },
                new Prisoners { Id = ObjectId.GenerateNewId().ToString(),
                    VariableName = "Osadzeni w zakładach karnych i aresztach śledczych",
                    Country = "Polska",
                    PrisionerType = "ogółem",
                    CategoriesInmates = "ogółem",
                    Sex = "Mezczyzni",
                    InformationTypeUnitofMeasure = "wartość [osoba]",
                    Year = 2023,
                    Value = 3422 }
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
            var controller = new PrisonersController(service);
            var id = "6641cf304d01a436d6a5e1a1";
            var updatedPrisoner = new Prisoners
            {
                Id = "6641cf304d01a436d6a5e1a1",
                VariableName = "Osadzeni w zakładach karnych i aresztach śledczych",
                Country = "Polska",
                PrisionerType = "ogółem",
                CategoriesInmates = "ogółem",
                Sex = "Mezczyzni",
                InformationTypeUnitofMeasure = "wartość [osoba]",
                Year = 2023,
                Value = 3422
            };
            _collection.InsertOne(updatedPrisoner);
            // Act
            var result = await controller.Update(id, updatedPrisoner);

            // Assert
    
            var prisoner = await service.GetAsync(id); // Sprawdzamy, czy serwis został wywołany z poprawnymi parametrami
            Assert.Equal(updatedPrisoner.VariableName, prisoner.VariableName);
            // Możemy również sprawdzić inne właściwości, jeśli to konieczne
        }

        [Fact]
        public async Task Delete_ReturnsNoContentResult()
        {
            // Arrange
            var service = _service; // Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new PrisonersController(service);
            var id = "6641cf304d01a436d6a5e1a1";
            var updatedPrisoner = new Prisoners
            {
                Id = "6641cf304d01a436d6a5e1a1",
                VariableName = "Osadzeni w zakładach karnych i aresztach śledczych",
                Country = "Polska",
                PrisionerType = "ogółem",
                CategoriesInmates = "ogółem",
                Sex = "Mezczyzni",
                InformationTypeUnitofMeasure = "wartość [osoba]",
                Year = 2023,
                Value = 3422
            };
            _collection.InsertOne(updatedPrisoner);
            // Act
            var result = await controller.Delete(id);

            // Assert

            var actionResult = Assert.IsType<NoContentResult>(result);
            var prisoner = await service.GetAsync(id); // Sprawdzamy, czy serwis został wywołany z poprawnymi parametrami
            Assert.Null(prisoner);
            // Możemy również sprawdzić inne właściwości, jeśli to konieczne
        }

        [Fact]
        public async Task Get_WithValidId_ReturnsPrisoner()
        {
            // Arrange
            var service = _service;// Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new PrisonersController(service);
            var id = "6641cf304d01a436d6a5e1a1";
            var expectedPrisoner = new Prisoners
            {
                Id = "6641cf304d01a436d6a5e1a1",
                VariableName = "Osadzeni w zakładach karnych i aresztach śledczych",
                Country = "Polska",
                PrisionerType = "ogółem",
                CategoriesInmates = "ogółem",
                Sex = "Mezczyzni",
                InformationTypeUnitofMeasure = "wartość [osoba]",
                Year = 2023,
                Value = 3422
            };
            _collection.InsertOne(expectedPrisoner);
            //await service.CreateAsync(expectedPrisoner); // Wstawiamy fikcyjnego więźnia do serwisu

            // Act
            var result = await _service.GetAsync(id);
           // Console.Write(result);
            // Assert
            //var actionResult = Assert.IsType<ActionResult<Prisoners>>(result);
           // var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var prisoner = Assert.IsType<Prisoners>(result);
            Assert.Equal(expectedPrisoner.Id, prisoner.Id);
          
        }

        [Fact]
        public async Task Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var service = _service; // Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new PrisonersController(service);
            var id = "6641cf304d01a436d6a5eaaa";

            // Act
            var result = await controller.Get(id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Prisoners>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

       

        // Similar tests can be written for other methods like GetAsync(string id), CreateAsync, UpdateAsync, RemoveAsync, etc.
    }
}
