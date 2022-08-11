using CurrenyConvertor.DependencyInjection;
using CurrenyConvertor.Model.Enums;
using CurrenyConvertor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

Console.WriteLine("Starting application \n");

string amountInput;
string fromInput;
string toInput;
string startDateInput;
string endDateInput;

//add config file

IConfiguration config = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json")
     .AddEnvironmentVariables()
     .Build();

//configure DI

var serviceProvider = new ServiceCollection()
    .ConfigureServices(config)
    .BuildServiceProvider();

var currencyConvertor = serviceProvider.GetService<ICurrenyConvertorService>();




////Basic conversion
//Conversion("1", "CAD", "USD");

////conversion with sum
//Conversion("10", "CAD", "INR");

//////conversion with sum and start date
//Conversion("100", "CAD", "EUR", "2022-08-10");

//////conversion with sum range 
//Conversion("100", "CAD", "EUR", "2022-01-10", "2022-08-10");

//////conversion with sum and end date
//Conversion("100", "CAD", "EUR", "", "2022-08-10");

Console.WriteLine("Please enter the amount of currency to convert. \n");

amountInput = Console.ReadLine();

Console.WriteLine("Thanks. You entered '{0}' \n", amountInput);


Console.WriteLine("Please enter the name of the currency you want to convert. \n");
Console.WriteLine("Available currencies: \n");
Console.WriteLine("CAD, USD, EUR, JPY, GBP, AUD, CHF, CNY, HKD, MXN, INR \n");

fromInput = Console.ReadLine();

Console.WriteLine("Thanks. You entered '{0}'\n", fromInput);

Console.WriteLine("What currency do you want to convert to? \n");

toInput = Console.ReadLine();

Console.WriteLine("Thanks. You entered '{0}'\n", toInput);

Console.WriteLine("(optional) Enter the start date of the period for which you want to receive data. \n");
Console.WriteLine("If you skip, the date for today will be taken \n");
Console.WriteLine("Date format example: 2022-08-10");

startDateInput = Console.ReadLine();

Console.WriteLine("Thanks. You entered '{0}'\n", startDateInput);


Console.WriteLine("(optional)Enter the end date of the period for which you want to receive data? \n");
Console.WriteLine("Date format example: 2022-08-10");

endDateInput = Console.ReadLine();

Console.WriteLine("Thanks. You entered '{0}'\n", startDateInput);

Console.WriteLine("Converting...");

Conversion(amountInput, fromInput, toInput, startDateInput, endDateInput);


Console.ReadKey();

async void Conversion(string amountStr, string fromStr, string toStr, string startDateStr = null, string endDateStr = null)
{

    double amount;

    CurrencyType from, to;

    DateTime? startDate = null;
    DateTime?  endDate = null;

    #region Validations

    if (!Double.TryParse(amountStr, out amount))
    {
        Console.WriteLine("{0} can't convert string to Double.",
                  amountStr);
        return;
    }


    if (!Enum.TryParse<CurrencyType>(fromStr, out from))
    {
        Console.WriteLine("{0} can't convert string to CurrencyType", fromStr);

        return;
    }


    if (!Enum.TryParse<CurrencyType>(toStr, out to))
    {
        Console.WriteLine("{0} can't convert string to CurrencyType", toStr);

        return;
    }

    if (!String.IsNullOrEmpty(startDateStr))
    {
        DateTime temp;
        if (!DateTime.TryParseExact(startDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp))
        {
            Console.WriteLine("{0} can't convert string to Datetime", startDateStr);

            return;
        }

        startDate = temp;
    }

    if (!String.IsNullOrEmpty(endDateStr))
    {
        DateTime temp;
        if (!DateTime.TryParseExact(endDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp))
        {
            Console.WriteLine("{0} can't convert string to Datetime", endDateStr);

            return;
        }

        endDate = temp;
    }

    #endregion

    var result = await currencyConvertor.ExchangeRateAsync(amount, from, to, startDate, endDate);

    foreach (var item in result)
    {
        Console.WriteLine(
            $"Conversion done! Date from {amount} {from} to {to}. Start date is {startDate}. End date is {endDate} \n" +
            $"The result is : \n" +
            $"Date: {item.Date}. Value: {item.Value} \n");
    }

}








