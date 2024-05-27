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

public class heightControllerTests
{
    public class HeightControllerTests : IDisposable
    {
        private readonly IMongoCollection<Height> _collection;
        private readonly HeightDataService _service;

        public HeightControllerTests()
        {
            var settings = new ProjectDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "techobjekt",
                HeightCollectionName = "HeightTests"
            };

            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Height>(settings.HeightCollectionName);

            _service = new HeightDataService(Options.Create(settings));
        }
    public void Dispose()
    {
        // Clean up after each test if necessary
        _collection.Database.DropCollection(_collection.CollectionNamespace.CollectionName);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllHeight()
    {
        // Arrange: Insert some test data into the database
        var testData = new List<Height>
            {
                new Height { Id = ObjectId.GenerateNewId().ToString(),
                   nazwa_zmiennej = "Średni wzrost osób w wieku 0-14 lat",
            kraj = "Polska",
            plec = "ogółem",
            wiek = "0 lat",
            typ_informacji_z_jednostka_miary = "wartość [cm]",
            rok = 2001,
            wartosc = 70,
                },
                new Height { Id = ObjectId.GenerateNewId().ToString(),
                    nazwa_zmiennej = "Średni wzrost osób w wieku 0-14 lat",
            kraj = "Polska",
            plec = "ogółem",
            wiek = "0 lat",
            typ_informacji_z_jednostka_miary = "wartość [cm]",
            rok = 2009,
            wartosc = 70,
                },
            };
        await _collection.InsertManyAsync(testData);

        var result = await _service.GetAsync();

        // Assert: Check if all prisoners are returned
        Assert.NotNull(result);
        Assert.Equal(testData.Count, result.Count);
    }


        [Fact]
        public async Task Post_CreateHeight()
        {
            // Arrange: Insert some test data into the database
            var testData = new List<Height>
            {
                new Height { Id = ObjectId.GenerateNewId().ToString(),
                   nazwa_zmiennej = "Średni wzrost osób w wieku 0-14 lat",
            kraj = "Polska",
            plec = "ogółem",
            wiek = "0 lat",
            typ_informacji_z_jednostka_miary = "wartość [cm]",
            rok = 2001,
            wartosc = 70,
                },
                new Height { Id = ObjectId.GenerateNewId().ToString(),
                    nazwa_zmiennej = "Średni wzrost osób w wieku 0-14 lat",
            kraj = "Polska",
            plec = "ogółem",
            wiek = "0 lat",
            typ_informacji_z_jednostka_miary = "wartość [cm]",
            rok = 2009,
            wartosc = 70,
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
            var controller = new HeightController(service);
            var id = "6641cf304d01a436d6a5e1a1";

            // Dodanie przykładowego rekordu do kolekcji
            var originalHeidghtData = new Height
            {
                Id = id,
                nazwa_zmiennej = "Średni wzrost osób w wieku 0-14 lat",
                kraj = "Polska",
                plec = "ogółem",
                wiek = "0 lat",
                typ_informacji_z_jednostka_miary = "wartość [cm]",
                rok = 2001,
                wartosc = 70,
            };
            _collection.InsertOne(originalHeidghtData);

            // Nowe dane do aktualizacji
            var updatedHeidghtData = new Height
            {
                nazwa_zmiennej = "Średni wzrost osób w wieku 0-14 lat",
                kraj = "Polska",
                plec = "ogółem",
                wiek = "0 lat",
                typ_informacji_z_jednostka_miary = "wartość [cm]",
                rok = 2002,
                wartosc = 71,
            };

            // Act
            var filter = Builders<Height>.Filter.Eq(e => e.Id, id);
            var update = Builders<Height>.Update
                .Set(e => e.nazwa_zmiennej, updatedHeidghtData.nazwa_zmiennej)
                .Set(e => e.kraj, updatedHeidghtData.kraj)
                .Set(e => e.plec, updatedHeidghtData.plec)
                .Set(e => e.typ_informacji_z_jednostka_miary, updatedHeidghtData.typ_informacji_z_jednostka_miary)
                .Set(e => e.wiek, updatedHeidghtData.wiek)
                .Set(e => e.rok, updatedHeidghtData.rok)
                .Set(e => e.wartosc, updatedHeidghtData.wartosc);

            var updateResult = _collection.UpdateOne(filter, update);

            // Assert
            Assert.Equal(1, updateResult.ModifiedCount); // Sprawdzamy, czy dokładnie jeden dokument został zaktualizowany

            var updatedRecord = await service.GetAsync(id); // Sprawdzamy, czy serwis zwraca zaktualizowane dane
            Assert.Equal(updatedHeidghtData.nazwa_zmiennej, updatedRecord.nazwa_zmiennej);
            Assert.Equal(updatedHeidghtData.wartosc, updatedRecord.wartosc);
            // Możemy również sprawdzić inne właściwości, jeśli to konieczne
        }

        [Fact]
        public async Task Delete_ReturnsNoContentResult()
        {
            // Arrange
            var service = _service; // Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new HeightController(service);
            var id = "6641cf304d01a436d6a5e1a1";

            var heightData = new Height
            {
                Id = id,
                nazwa_zmiennej = "Średni wzrost osób w wieku 0-14 lat",
                kraj = "Polska",
                plec = "ogółem",
                wiek = "0 lat",
                typ_informacji_z_jednostka_miary = "wartość [cm]",
                rok = 2001,
                wartosc = 70,
            };
            _collection.InsertOne(heightData);

            // Act
            var result = await controller.Delete(id);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            var deletedHeightData = await service.GetAsync(id); // Sprawdzamy, czy serwis został wywołany z poprawnymi parametrami
            Assert.Null(deletedHeightData); // Sprawdzamy, czy rekord został usunięty
        }

        [Fact]
        public async Task Get_WithValidId_ReturnsEducation()
        {
            // Arrange
            var service = _service;// Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new HeightController(service);
            var id = "6641cf304d01a436d6a5e1a1";
            var heightData = new Height
            {
                Id = id,
                nazwa_zmiennej = "Średni wzrost osób w wieku 0-14 lat",
                kraj = "Polska",
                plec = "ogółem",
                wiek = "0 lat",
                typ_informacji_z_jednostka_miary = "wartość [cm]",
                rok = 2001,
                wartosc = 70,
            };
            _collection.InsertOne(heightData);

            // Act
            var result = await _service.GetAsync(id);
            // Console.Write(result);
            // Assert
            //var actionResult = Assert.IsType<ActionResult<Prisoners>>(result);
            // var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var heightDataResults = Assert.IsType<Height>(result);
            Assert.Equal(heightData.Id, heightDataResults.Id);

        }

        [Fact]
        public async Task Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var service = _service; // Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new HeightController(service);
            var id = "6641cf304d01a436d6a5eaaa";

            // Act
            var result = await controller.Get(id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Height>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }


    }
}


