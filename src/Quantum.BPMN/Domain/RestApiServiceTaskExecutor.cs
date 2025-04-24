using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Reng.BPMN.Domain.Domain;

public class RestApiServiceTaskExecutor : IServiceTaskExecutor
{
    public void Execute(BpmnExecutionContext context, IAmAServiceTask serviceTask)
    {
        if (serviceTask.TaskExecutorDescription.Type != TaskExecutorType.RestApi)
            throw new RengDomainException("اجرای این تسک از نوع Rest API نیست");

        var dataProvider = CreateDataProvider(serviceTask.InputDataProvider, context);

        Execute(context, dataProvider, serviceTask.TaskExecutorDescription.Url);
    }


    public void Compensate(BpmnExecutionContext bpmnExecutionContext, IAmAServiceTask serviceTask)
    {
        if (serviceTask.CompensateDescription.Type == TaskExecutorType.NotSetYet)
            return;

        if (serviceTask.CompensateDescription.Type != TaskExecutorType.RestApi)
            throw new RengDomainException("اجرای این تسک از نوع Rest API نیست");

        var dataProvider = CreateDataProvider(serviceTask.InputDataProvider, bpmnExecutionContext);

        Execute(bpmnExecutionContext, dataProvider, serviceTask.CompensateDescription.Url);
    }

    private void Execute(BpmnExecutionContext context, IDataProvider dataProvider, string url = "")
    {
        AssertThatUrlIsValid(url);

        var normalizedUrl = NormalizeUrl(url, dataProvider);

        // Handles HttpRequestException, Http status codes >= 500 (server errors) and status code 408 (request timeout)

        using var _httpClient = new HttpClient();

        var policy = GetPolicy();
        var httpResponseTask = policy.ExecuteAsync(() => PostRequest(_httpClient, normalizedUrl, context.GetDictionary()));
        var httpResponseMessage = httpResponseTask.Result;

        AssertThatResponseIsOk(normalizedUrl, httpResponseMessage);

        ParseTheResultAndPopulateTheContextWithResultIfNeeded(httpResponseMessage, context);
    }

    private static void AssertThatUrlIsValid(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ObjectNotFoundException("آدرس یو آر ال نمی تواند خالی باشد");

        if (Uri.TryCreate(url, UriKind.Absolute, out _) == false)
            throw new ObjectNotFoundException("آدرس یو آر ال با فرمت صحیح و بصورت ابسولوت باشد");
    }

    private AsyncRetryPolicy<HttpResponseMessage> GetPolicy()
    {
        return HttpPolicyExtensions
          .HandleTransientHttpError()
          .RetryAsync(3);
    }

    private void ParseTheResultAndPopulateTheContextWithResultIfNeeded(HttpResponseMessage httpResponseMessage, BpmnExecutionContext context)
    {
        var result = httpResponseMessage.Content.ReadAsStringAsync().Result;

        if (string.IsNullOrEmpty(result))
            return;

        var deserializeObject = DeserializeObject(result);
        if (deserializeObject != null)
            context.Populate(deserializeObject);
    }

    private static void AssertThatResponseIsOk(string normalizedUrl, HttpResponseMessage httpResponseMessage)
    {
        if (httpResponseMessage.IsSuccessStatusCode is true) 
            return;

        var errorMessage = "در اجرای متد " + normalizedUrl + " خطایی رخ داده است." + " کد خطا = " +
                           httpResponseMessage.StatusCode.ToString() + " و متن خطا =" +
                           httpResponseMessage.ReasonPhrase;

        var errorMessageWithDetail = "در اجرای متد " + normalizedUrl + " خطایی رخ داده است." + " کد خطا = " +
                                     httpResponseMessage.StatusCode.ToString()
                                     + " و متن خطا =" + httpResponseMessage.Content.ReadAsStringAsync().Result;

        throw new RengDomainException(errorMessage);
    }

    private static Dictionary<string, object>? DeserializeObject(string content)
    {
        try
        {
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            return result;
        }
        catch (Exception ex)
        {
            throw new RengDomainException("در دیسریالایز کردن پیغام پاسخ به دیکشنری مشکلی رخ داده است." + ex.Message);
        }
    }

    protected virtual Task<HttpResponseMessage> PostRequest(HttpClient client, string url, Dictionary<string, object> dic) 
        => client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(dic), System.Text.Encoding.UTF8, "application/json"));

    private string NormalizeUrl(string url, IDataProvider dataProvider)
    {
        var lastPos = url.IndexOf('{');
        while (lastPos != -1)
        {
            var startIndexOf = url.IndexOf('{', lastPos);
            if (startIndexOf == -1)
            {
                lastPos = -1;
                continue;
            }

            var endIndexOf = url.IndexOf('}', lastPos);

            var paramName = url.Substring(startIndexOf + 1, endIndexOf - startIndexOf - 1);

            var value = dataProvider.Get(paramName);
            url = url.Replace("{" + paramName + "}", value as string);
            lastPos = endIndexOf;
        }
        return url;
    }

    private IDataProvider CreateDataProvider(InputDataProvider userTaskInputDataProvider, BpmnExecutionContext bpmnExecutionContext)
    {
        switch (userTaskInputDataProvider.GetType())
        {
            case DataProviderType.ExecutionContext:
                return new ExecutionContextDataProvider(bpmnExecutionContext);
            case DataProviderType.Nothing:
                return NullDataProvider.New();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}