using BIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace BIRService.Tests
{
    public class BirSearchServiceTests
    {
        private BIRSearchService _service;
        /// <summary>
        /// Własny klucz wymagany dla testów. Testy z użyciem serwisu nie mogą przekroczyć
        /// ilości 3 zapytań na sekundę.
        /// </summary>
        private string _bIRKey = "";
        public BirSearchServiceTests()
        {
            _service = new BIRSearchService(_bIRKey);
        }

        [Fact]
        public void BIRSearchService_ShouldThrowExeption()
        {
            Assert.Throws<ArgumentNullException>(() => new BIRSearchService(null));
        }

        [Fact]
        public async void GetCompanyDataByRegon_ShouldReturnNip()
        {
            var expected = "9541966084";
            var actual = await _service.GetCompanyDataByNipIdAsync(expected);
            Assert.Equal(expected, actual.Nip);
        }

        [Fact]
        public async void GetCompanyDataByRegon_ShouldReturnRegon()
        {
            var expected = "380273108";
            var actual = await _service.GetCompanyDataByRegonAsync(expected);
            Assert.Equal(expected, actual.Regon);
        }

        [Fact]
        public async void GetCompanyDataByNipId_ShouldThrowExeption()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.GetCompanyDataByNipIdAsync(String.Empty));
        }

        [Fact]
        public async void GetCompanyDataByRegon_ShouldThrowExeption()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.GetCompanyDataByRegonAsync(String.Empty));
        }
    }
}
