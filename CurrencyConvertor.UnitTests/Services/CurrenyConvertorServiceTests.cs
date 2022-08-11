using AutoFixture;
using AutoFixture.AutoMoq;
using CurrenyConvertor.Configuration.Options;
using CurrenyConvertor.Model;
using CurrenyConvertor.Model.Enums;
using CurrenyConvertor.Services;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CurrencyConvertor.UnitTests.Services
{
    public class CurrenyConvertorServiceTests
    {
        private readonly Fixture _fixture;

        private readonly ICurrenyConvertorService _subject;

        public CurrenyConvertorServiceTests()
        {

            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization() { ConfigureMembers = true });

            var options = new CurrenyConvertorOptions() { ApiBaseUrl = "https://www.banqueducanada.ca" };

            var mock = new Mock<IOptionsSnapshot<CurrenyConvertorOptions>>();
            mock.Setup(m => m.Value).Returns(options);

            _fixture.Register(() => mock.Object);

            _subject = _fixture.Create<CurrenyConvertorService>();

        }

        [Theory]
        [InlineData(50, CurrencyType.CAD, CurrencyType.USD)]
        [InlineData(100, CurrencyType.USD, CurrencyType.CAD)]
        [InlineData(150, CurrencyType.CAD, CurrencyType.JPY)]
        [InlineData(250, CurrencyType.GBP, CurrencyType.CAD)]
        public async Task ExchangeRate_SimpleModel_ReturnsData(double amount, CurrencyType from, CurrencyType to)
        {
            // Act

            var result = await _subject.ExchangeRateAsync(amount, from, to);
            // Assert

            Assert.NotNull(result);
            Assert.IsType<CurrenyConvertorResponse>(result.First());
            Assert.True(result.First().Value != 0.0d);
            Assert.NotNull(result.First().Date);
        }

        public static readonly object[][] CorrectData =
        {
            new object[] { 51, CurrencyType.CAD, CurrencyType.USD, new DateTime(2019, 03, 01), new DateTime(2020, 12, 03)},
            new object[] { 151, CurrencyType.USD, CurrencyType.CAD, new DateTime(2020, 03, 01), new DateTime(2021, 12, 11)},
            new object[] { 151, CurrencyType.USD, CurrencyType.CAD, null, new DateTime(2021,12,31)},
            new object[] { 151, CurrencyType.USD, CurrencyType.CAD, new DateTime(2020, 3, 1), null},
        };

        [Theory, MemberData(nameof(CorrectData))]
        public async Task ExchangeRate_With_Dates_ReturnsData(double amount, CurrencyType from, CurrencyType to, DateTime? startDate, DateTime? endDate)
        {
            // Act

            var result = await _subject.ExchangeRateAsync(amount, from, to, startDate, endDate);

            // Assert

            Assert.NotNull(result);
            Assert.IsType<CurrenyConvertorResponse>(result.First());
            Assert.True(result.First().Value != 0.0d);
            Assert.NotNull(result.First().Date);
        }

        public static readonly object[][] IncorrectData =
        {
            new object[] { 51, CurrencyType.CAD, CurrencyType.USD, new DateTime(2023,3,1), null },
        };


        [Theory, MemberData(nameof(IncorrectData))]
        public async Task ExchangeRate_With_Today_Date_Fail(double amount, CurrencyType from, CurrencyType to, DateTime? startDate, DateTime? endDate)
        {
            // Act

            var result = await _subject.ExchangeRateAsync(amount, from, to, startDate, endDate);

            // Assert

            Assert.Throws<InvalidOperationException>(() => result.First());
        }
    }
}
