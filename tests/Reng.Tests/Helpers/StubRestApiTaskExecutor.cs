using FluentAssertions;
using Reng.BPMN.Domain;
using Reng.BPMN.Domain.Domain;

namespace Reng.Tests.Helpers;

public class StubRestApiTaskExecutor : RestApiServiceTaskExecutor
{

    public static StubRestApiTaskExecutor WhichIExpectedToBeCallWithUrl(string expectedUrl, IAmAServiceTask serviceTask)
        => new(expectedUrl, serviceTask.TaskExecutorDescription.Url);

    private int _numberOfCalled;
    private string _actualUrl;
    private string _expectedUrl;

    private StubRestApiTaskExecutor(string expectedUrl, string url) : base()
    {
        _expectedUrl = expectedUrl;
    }

    protected override Task<HttpResponseMessage> PostRequest(HttpClient client, string url, Dictionary<string, object> dic)
    {
        _numberOfCalled++;
        _actualUrl = url;
        return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage());
    }

    public void Verify()
    {
        _numberOfCalled.Should().Be(1);
        _expectedUrl.Should().BeEquivalentTo(_actualUrl);
    }
}