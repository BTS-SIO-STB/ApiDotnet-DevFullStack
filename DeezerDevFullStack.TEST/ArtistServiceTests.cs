using DeezerDevFullStack.BL;
using DeezerDevFullStack.DTO;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;
using System.Text.Json;

namespace DeezerDevFullStack.TEST
{
    public class ArtistServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private ArtistService _artistService;

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _artistService = new ArtistService(_httpClient);
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
            var result = await _artistService.SearchArtists(artistName);

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
            var result = await _artistService.SearchArtists(artistName);

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
            Assert.ThrowsAsync<HttpRequestException>(async () => await _artistService.SearchArtists(artistName));
        }

        [Test]
        public async Task GetSongsByArtistId_ReturnsSongs()
        {
            // Arrange
            var artistId = 1;
            var expectedSongs = new List<Song>
            {
                new Song { Id = 1, Title = "Song 1", ArtistId = artistId },
                new Song { Id = 2, Title = "Song 2", ArtistId = artistId }
            };
            var jsonResponse = JsonSerializer.Serialize(new { data = expectedSongs });
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
            var result = await _artistService.GetSongsByArtistId(artistId);

            // Assert
            Assert.AreEqual(expectedSongs.Count, result.Count());
            Assert.AreEqual(expectedSongs[0].Id, result.First().Id);
            Assert.AreEqual(expectedSongs[0].Title, result.First().Title);
        }

        [Test]
        public async Task GetSongsByArtistId_NoSongs_ReturnsEmptyList()
        {
            // Arrange
            var artistId = 1;
            var jsonResponse = JsonSerializer.Serialize(new { data = new List<Song>() });
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
            var result = await _artistService.GetSongsByArtistId(artistId);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetSongsByArtistId_HttpClientThrowsException_ThrowsException()
        {
            // Arrange
            var artistId = 1;
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _artistService.GetSongsByArtistId(artistId));
        }
    }
}