namespace SocialNetwork.Write.IntegrationTests.Config;

[CollectionDefinition("Integration Tests")]
public class IntegrationCollection : ICollectionFixture<WriteApiFactory> { }

[Collection("Integration Tests")]
public abstract class BaseIntegrationTest
{
    protected readonly HttpClient Client;
    protected readonly WriteApiFactory Factory;

    protected BaseIntegrationTest(WriteApiFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
}