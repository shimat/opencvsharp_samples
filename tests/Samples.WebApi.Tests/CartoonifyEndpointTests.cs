using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Samples.WebApi.Tests;

public class CartoonifyEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CartoonifyEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Cartoonify_WithoutUpload_ReturnsPngOfDefaultImage()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = _factory.CreateClient();

        using var response = await client.PostAsync("/api/cartoonify", content: null, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        Assert.NotEmpty(bytes);
    }

    [Fact]
    public async Task FrameAnalysis_WithoutUpload_ReturnsAtLeastOneFrame()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = _factory.CreateClient();

        using var response = await client.PostAsync("/api/frame-analysis", content: null, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(json);
        Assert.True(document.RootElement.GetArrayLength() > 0);
    }
}
