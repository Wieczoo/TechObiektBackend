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

public class educationsControllerTests
{
    public class EducationsControllerTests : IDisposable
    {
        private readonly IMongoCollection<Education> _collection;
        private readonly EducationDataService _service;

        public EducationsControllerTests()
        {
            var settings = new ProjectDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "techobjekt",
                EducationCollectionName = "educationTests"
            };

            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Education>(settings.EducationCollectionName);

            _service = new EducationDataService(Options.Create(settings));
        }

        public void Dispose()
        {
            // Clean up after each test if necessary
            _collection.Database.DropCollection(_collection.CollectionNamespace.CollectionName);
        }

        [Fact]
        public async Task GetAsync_ReturnsAllEducationsData()
        {
            // Arrange: Insert some test data into the database
            var testData = new List<Education>
            {
                new Education { Id = ObjectId.GenerateNewId().ToString(),
                    nazwa_zmiennej = "Wskaźnik ukończenia edukacji",
                    kraj = "Polska",
            wojewodztwo = "Ogółem",
            typ_szkoly = "Gimnazjum",
            plec_absolwenta = "Ogółem",
            rodzaj_wskaznika = "Brutto",
            typ_informacji_z_jednostka_miary = "relacja [%]",
            rok_szkolny = "2008/09",
            wartosc = 92,
            flaga = 9 },
                new Education { Id = ObjectId.GenerateNewId().ToString(),
                    nazwa_zmiennej = "Wskaźnik ukończenia edukacji",
                    kraj = "Polska",
            wojewodztwo = "Ogółem",
            typ_szkoly = "Gimnazjum",
            plec_absolwenta = "Ogółem",
            rodzaj_wskaznika = "Brutto",
            typ_informacji_z_jednostka_miary = "relacja [%]",
            rok_szkolny = "2009/10",
            wartosc = 92,
            flaga = 9}
            };
            await _collection.InsertManyAsync(testData);

            // Act: Call the GetAsync method from the service
            var result = await _service.GetAsync();

            // Assert: Check if all prisoners are returned
            Assert.NotNull(result);
            Assert.Equal(testData.Count, result.Count);
        }

        [Fact]
        public async Task Post_CreateEducationsData()
        {
            // Arrange: Insert some test data into the database
            var testData = new List<Education>
            {
                new Education { Id = ObjectId.GenerateNewId().ToString(),
                    nazwa_zmiennej = "Wskaźnik ukończenia edukacji",
                    kraj = "Polska",
            wojewodztwo = "Ogółem",
            typ_szkoly = "Gimnazjum",
            plec_absolwenta = "Ogółem",
            rodzaj_wskaznika = "Brutto",
            typ_informacji_z_jednostka_miary = "relacja [%]",
            rok_szkolny = "2008/09",
            wartosc = 92,
            flaga = 9 },
                new Education { Id = ObjectId.GenerateNewId().ToString(),
                    nazwa_zmiennej = "Wskaźnik ukończenia edukacji",
                    kraj = "Polska",
            wojewodztwo = "Ogółem",
            typ_szkoly = "Gimnazjum",
            plec_absolwenta = "Ogółem",
            rodzaj_wskaznika = "Brutto",
            typ_informacji_z_jednostka_miary = "relacja [%]",
            rok_szkolny = "2009/10",
            wartosc = 92,
            flaga = 9}
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
            var controller = new EducationController(service);
            var id = "6641cf304d01a436d6a5e1a1";

            // Dodanie przykładowego rekordu do kolekcji
            var originalEducation = new Education
            {
                Id = id,
                nazwa_zmiennej = "Stary wskaźnik",
                kraj = "Polska",
                wojewodztwo = "Ogółem",
                typ_szkoly = "Gimnazjum",
                plec_absolwenta = "Ogółem",
                rodzaj_wskaznika = "Brutto",
                typ_informacji_z_jednostka_miary = "relacja [%]",
                rok_szkolny = "2009/10",
                wartosc = 85,
                flaga = 1
            };
            _collection.InsertOne(originalEducation);

            // Nowe dane do aktualizacji
            var updatedEducation = new Education
            {
                nazwa_zmiennej = "Wskaźnik ukończenia edukacji",
                kraj = "Polska",
                wojewodztwo = "Ogółem",
                typ_szkoly = "Gimnazjum",
                plec_absolwenta = "Ogółem",
                rodzaj_wskaznika = "Brutto",
                typ_informacji_z_jednostka_miary = "relacja [%]",
                rok_szkolny = "2009/10",
                wartosc = 92,
                flaga = 9
            };

            // Act
            var filter = Builders<Education>.Filter.Eq(e => e.Id, id);
            var update = Builders<Education>.Update
                .Set(e => e.nazwa_zmiennej, updatedEducation.nazwa_zmiennej)
                .Set(e => e.kraj, updatedEducation.kraj)
                .Set(e => e.wojewodztwo, updatedEducation.wojewodztwo)
                .Set(e => e.typ_szkoly, updatedEducation.typ_szkoly)
                .Set(e => e.plec_absolwenta, updatedEducation.plec_absolwenta)
                .Set(e => e.rodzaj_wskaznika, updatedEducation.rodzaj_wskaznika)
                .Set(e => e.typ_informacji_z_jednostka_miary, updatedEducation.typ_informacji_z_jednostka_miary)
                .Set(e => e.rok_szkolny, updatedEducation.rok_szkolny)
                .Set(e => e.wartosc, updatedEducation.wartosc)
                .Set(e => e.flaga, updatedEducation.flaga);

            var updateResult = _collection.UpdateOne(filter, update);

            // Assert
            Assert.Equal(1, updateResult.ModifiedCount); // Sprawdzamy, czy dokładnie jeden dokument został zaktualizowany

            var updatedRecord = await service.GetAsync(id); // Sprawdzamy, czy serwis zwraca zaktualizowane dane
            Assert.Equal(updatedEducation.nazwa_zmiennej, updatedRecord.nazwa_zmiennej);
            Assert.Equal(updatedEducation.wartosc, updatedRecord.wartosc);
            Assert.Equal(updatedEducation.flaga, updatedRecord.flaga);
            // Możemy również sprawdzić inne właściwości, jeśli to konieczne
        }

        [Fact]
        public async Task Delete_ReturnsNoContentResult()
        {
            // Arrange
            var service = _service; // Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new EducationController(service);
            var id = "6641cf304d01a436d6a5e1a1";

            var education = new Education
            {
                Id = id,
                nazwa_zmiennej = "Stary wskaźnik",
                kraj = "Polska",
                wojewodztwo = "Ogółem",
                typ_szkoly = "Gimnazjum",
                plec_absolwenta = "Ogółem",
                rodzaj_wskaznika = "Brutto",
                typ_informacji_z_jednostka_miary = "relacja [%]",
                rok_szkolny = "2009/10",
                wartosc = 85,
                flaga = 1
            };
            _collection.InsertOne(education);

            // Act
            var result = await controller.Delete(id);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            var deletedEducation = await service.GetAsync(id); // Sprawdzamy, czy serwis został wywołany z poprawnymi parametrami
            Assert.Null(deletedEducation); // Sprawdzamy, czy rekord został usunięty
        }

        [Fact]
        public async Task Get_WithValidId_ReturnsEducation()
        {
            // Arrange
            var service = _service;// Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new EducationController(service);
            var id = "6641cf304d01a436d6a5e1a1";
            var expectedEducation = new Education
            {
                Id = id,
                nazwa_zmiennej = "Stary wskaźnik",
                kraj = "Polska",
                wojewodztwo = "Ogółem",
                typ_szkoly = "Gimnazjum",
                plec_absolwenta = "Ogółem",
                rodzaj_wskaznika = "Brutto",
                typ_informacji_z_jednostka_miary = "relacja [%]",
                rok_szkolny = "2009/10",
                wartosc = 85,
                flaga = 1
            };
            _collection.InsertOne(expectedEducation);
            
            // Act
            var result = await _service.GetAsync(id);
            // Console.Write(result);
            // Assert
            //var actionResult = Assert.IsType<ActionResult<Prisoners>>(result);
            // var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var education = Assert.IsType<Education>(result);
            Assert.Equal(expectedEducation.Id, education.Id);

        }

        [Fact]
        public async Task Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var service = _service; // Używamy klasy pomocniczej, która implementuje rzeczywisty serwis, ale z fikcyjnymi danymi
            var controller = new EducationController(service);
            var id = "6641cf304d01a436d6a5eaaa";

            // Act
            var result = await controller.Get(id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Education>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }



        // Similar tests can be written for other methods like GetAsync(string id), CreateAsync, UpdateAsync, RemoveAsync, etc.
    }
}
