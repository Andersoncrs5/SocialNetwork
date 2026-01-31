using SocialNetwork.Write.IntegrationTests.Config;
using Xunit.Abstractions;

namespace SocialNetwork.Write.IntegrationTests.Tests.Category;

public class CategoryControllerTest: BaseIntegrationTest
{
    private readonly HelperTest _helper;
    private readonly ITestOutputHelper _output;
    private readonly string _url = "api/v1/category";

    public CategoryControllerTest(WriteApiFactory factory, ITestOutputHelper output) 
        : base(factory)
    {
        _output = output;
        _helper = new HelperTest(Client); 
    }

    [Fact]
    public async Task CreateCategory_Success()
    {
        
    }
    
}