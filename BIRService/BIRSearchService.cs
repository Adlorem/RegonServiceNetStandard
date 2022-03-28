using BIRService.Models;
using BIRServiceReference;
using System.Collections;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Linq;
using System.Xml.Serialization;
using WcfCoreMtomEncoder;

namespace BIRService
{
    public class BIRSearchService : IBIRSearchService
    {
        private readonly string _serviceKey;
        private readonly bool _isProduction;
        public BIRSearchService(string serviceKey, bool isProduction = true)
        {
            if (string.IsNullOrEmpty(serviceKey)) throw new ArgumentNullException("Brak klucza BIR");
            _serviceKey = serviceKey;
            _isProduction = isProduction;
        }

        public async Task<DanePodmiotu> GetCompanyDataByNipIdAsync(string nipId)
        {
            if (string.IsNullOrEmpty(nipId)) throw new ArgumentNullException("Parametr wyszukiwania nip nie może być pusty.");

            var searchParameters = new ParametryWyszukiwania
            {
                Nip = nipId
            };

            return await GetSearchResultModelAsync<DanePodmiotu>(searchParameters);
        }

        public async Task<DanePodmiotu> GetCompanyDataByRegonAsync(string regonId)
        {
            if (string.IsNullOrEmpty(regonId)) throw new ArgumentNullException("Parametr wyszukiwania regon nie może być pusty.");
            var searchParameters = new ParametryWyszukiwania
            {
                Regon = regonId
            };

            return await GetSearchResultModelAsync<DanePodmiotu>(searchParameters);        
        }

        /// <summary>
        /// Zwraca model wg parametrów wyszukiwania w sewisie Regon.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchParameters"></param>
        /// <returns></returns>
        private async Task<T> GetSearchResultModelAsync<T>(ParametryWyszukiwania searchParameters) where T : class, new()
        {
            var seachResult = await GetSearchResultAsync(searchParameters);

            var doc = XDocument.Parse(seachResult);

            if (doc.Descendants("ErrorCode").Any())
            {
                return GetServiceErrors<T>(doc);
            }

            return DeserializeXMLElement<T>(doc.Descendants("dane").First());
        }

        /// <summary>
        /// Zaraca model wyszukiwania z listą błędów.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <returns></returns>
        private T GetServiceErrors<T>(XDocument doc) where T : class, new()
        {
            var model = new T();

            if (!doc.Descendants("ErrorCode").Any()) return model;

            var errors = doc.Descendants("dane");

            var property = model.GetType().GetProperty("Errors");

            Type element = typeof(ErrorModel);
            Type listType = typeof(List<>).MakeGenericType(element);
            var errorsList = Activator.CreateInstance(listType) as IList;

            foreach (var item in errors)
            {
                var error = DeserializeXMLElement<ErrorModel>(item);
                errorsList.Add(error);                
            }

            if (property != null) property.SetValue(model, errorsList, null);

            return model;
        }

        /// <summary>
        /// Zwraca resultat wyszukiwania w serwisie na podstawie zdefiniowanych parametrów wyszukiwania.
        /// </summary>e
        /// <param name="searchParameters"></param>
        /// <returns></returns>
        private async Task<string> GetSearchResultAsync(ParametryWyszukiwania searchParameters)
        {
            string requestResult = string.Empty;

            var encoding = new MtomMessageEncoderBindingElement(new TextMessageEncodingBindingElement());
            var transport = new HttpsTransportBindingElement();
            var customBinding = new CustomBinding(encoding, transport);

            EndpointAddress endPoint = new EndpointAddress("https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc");
            if (!_isProduction)
            {
                endPoint = new EndpointAddress("https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc"); // test
            }

            UslugaBIRzewnPublClient client = new UslugaBIRzewnPublClient(customBinding, endPoint);
            await client.OpenAsync();
            var session = await client.ZalogujAsync(_serviceKey);

            using (new OperationContextScope(client.InnerChannel))
            {
                HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers["sid"] = session.ZalogujResult;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                var result = client.DaneSzukajPodmiotyAsync(searchParameters).GetAwaiter().GetResult();
                requestResult = result.DaneSzukajPodmiotyResult;
            }
            await client.WylogujAsync(session.ZalogujResult);
            await client.CloseAsync();

            return requestResult;
        }

        /// <summary>
        /// Deserializuje element XML do danego modelu.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        private T DeserializeXMLElement<T>(XElement element) where T : class
        {
            if (element == null) return null;

            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(element.CreateReader()) as T;

        }
    }
}
