using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Reng.BPMN.ApplicationService;

namespace Quantum.Reng
{
    public class RengApi : IRengApi
    {
        private readonly RendConfig _config;

        public RengApi(RendConfig config)
        {
            _config = config;
        }

        public async Task Delete(string businessProcessName)
        {
            using HttpClient httpClient = new();

            await httpClient.DeleteAsync($"{_config.RengUrl}/api/{businessProcessName}");
        }

        public async Task<string> Execute(string instanceId, Dictionary<string, string> context)
        {
            using HttpClient httpClient = new();

            var result = await httpClient
                .PutAsync($"{_config.RengUrl}/api/instances/{instanceId}/execute",new StringContent(Newtonsoft                .Json.JsonConvert.SerializeObject(context), Encoding.UTF8                , "application/json"));

            return await To<string>(result);
        }

        public async Task<string> ExecuteANewInsatnce(string businessProcessName
            , Dictionary<string, string> context)
        {
            using HttpClient httpClient = new();

            var result = await httpClient.PutAsync($"{_config.RengUrl}/api/{businessProcessName}/executeNewInstance",
                new StringContent(Newtonsoft
                .Json.JsonConvert.SerializeObject(context), Encoding.UTF8
                , "application/json"));

            return await To<string>(result);
        }

        public async Task<List<BusinessProcessLogViewModel>> GetExecutionDescription(string instanceId)
        {
            using HttpClient httpClient = new();

            var result = await httpClient.GetAsync($"{_config.RengUrl}/api/instances/{instanceId}/logs");

            return await To<List<BusinessProcessLogViewModel>>(result);
        }

        private async Task<T> To<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
        }
    }
}