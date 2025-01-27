using DeezerDevFullStack.BL;
using DeezerDevFullStack.DTO;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;
using System.Text.Json;

namespace DeezerDevFullStack.TEST
{
    public class ArtistSearchServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private ArtistSearchService _artistSearchService;

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _artistSearchService = new ArtistSearchService(_httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        [Test]
        public async Task SearchArtists_ReturnsMatchingArtists()
        {
            // Arrange
            var artistName = "Artist";
            var expectedArtists = new List<Artist>
            {
                new Artist { Id = 1, Name = "Artist 1" },
                new Artist { Id = 2, Name = "Artist 2" }
            };
            var jsonResponse = JsonSerializer.Serialize(new { data = expectedArtists });
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _artistSearchService.SearchArtists(artistName);

            // Assert
            Assert.AreEqual(expectedArtists.Count, result.Count());
            Assert.AreEqual(expectedArtists[0].Id, result.First().Id);
            Assert.AreEqual(expectedArtists[0].Name, result.First().Name);
        }

        [Test]
        public async Task SearchArtists_NoMatchingArtists_ReturnsEmptyList()
        {
            // Arrange
            var artistName = "NonExistentArtist";
            var jsonResponse = JsonSerializer.Serialize(new { data = new List<Artist>() });
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _artistSearchService.SearchArtists(artistName);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void SearchArtists_HttpClientThrowsException_ThrowsException()
        {
            // Arrange
            var artistName = "Artist";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _artistSearchService.SearchArtists(artistName));
        }
    }
}