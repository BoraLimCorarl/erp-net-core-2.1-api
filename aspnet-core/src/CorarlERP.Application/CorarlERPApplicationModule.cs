using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CorarlERP.Authorization;
using CorarlERP.Currencies;
using CorarlERP.Formats;
using CorarlERP.ItemTypes;
using System.Collections.Generic;

namespace CorarlERP
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(CorarlERPCoreModule)
        )]
    public class CorarlERPApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CorarlERPApplicationModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var defaultValues = IocManager.Resolve<IDefaultValues>();

            InitCurrencyDefaultValue(defaultValues);
            InitItemTypeDefaultValue(defaultValues);
            InitFormatDefaulValue(defaultValues);
        }

        private void InitItemTypeDefaultValue(IDefaultValues defaultValues)
        {
            defaultValues.ItemTypes = new List<ItemType>()
            {
                  ItemType.Create(null,"Item", displayInventoryAccount:true,displayPurchase:true,displaySale:true,displayReorderPoint:true,displayTrackSerialNumber:true,displaySubItem:false, displayItemMenu:false),
                  ItemType.Create(null,"Assembly", displayInventoryAccount:false,displayPurchase:true,displaySale:true,displayReorderPoint:false,displayTrackSerialNumber:false,displaySubItem:true,displayItemMenu:false),
                  ItemType.Create(null,"Service", displayInventoryAccount:false,displayPurchase:true,displaySale:true,displayReorderPoint:false,displayTrackSerialNumber:false,displaySubItem:false,displayItemMenu:false),
                  ItemType.Create(null,"Asset", displayInventoryAccount:true,displayPurchase:true,displaySale:true,displayReorderPoint:true,displayTrackSerialNumber:true,displaySubItem:false,displayItemMenu:false),
                  ItemType.Create(null,"Menu", displayInventoryAccount:false,displayPurchase:true,displaySale:true,displayReorderPoint:false,displayTrackSerialNumber:false,displaySubItem:false,displayItemMenu:true),
            };
        }

        public void InitFormatDefaulValue(IDefaultValues defaultValues)
        {
            defaultValues.Formats = new List<Format>()
            {
                 Format.Create(null,name:"MM/DD/yyyy",key:"Date",web:"MM/dd/yyyy"),
                Format.Create(null,name:"dd/MM/YYYY",key:"Date",web:"dd/MM/yyyy"),
                Format.Create(null,name:"yyyy/MM/dd",key:"Date",web:"yyyy/MM/dd"),
                Format.Create(null,name:"dd-MM-yyyy",key:"Date",web:"dd-MM-yyyy"),
                Format.Create(null,name:"MM-dd-yyyy",key:"Date",web:"MM-dd-yyyy"),
                Format.Create(null,name:"yyyy-MM-dd",key:"Date",web:"yyyy-MM-dd"),
                Format.Create(null,name:"dd.MM.yyyy",key:"Date",web:"dd.MM.yyyy"),
                Format.Create(null,name:"MM.dd.yyyy",key:"Date",web:"MM.dd.yyyy"),
                Format.Create(null,name:"yyyy.MM.dd",key:"Date",web:"yyyy.MM.dd"),

                Format.Create(null,name:"123,456.00",key:"Number",web:"comma"),
                Format.Create(null,name:"123.456.00",key:"Number",web:"dot"),
                Format.Create(null,name:"123 456.00",key:"Number",web:"space"),
            };
        }
        private void InitCurrencyDefaultValue(IDefaultValues defaultValues)
        {
            defaultValues.Currencies = new List<Currency>()
            {
                Currency.Create(null, "USD", "US Dollar", "$", "US dollars"),
                Currency.Create(null, "CAD", "Canadian Dollar", "$", "Canadian dollars"),
                Currency.Create(null, "EUR", "Euro", "€", "euros"),
                Currency.Create(null, "AED", "United Arab Emirates Dirham", "د.إ.‏", "UAE dirhams"),
                Currency.Create(null, "AFN", "Afghan Afghani", "؋", "Afghan Afghanis"),
                Currency.Create(null, "ALL", "Albanian Lek", "Lek", "Albanian lekë"),
                Currency.Create(null, "AMD", "Armenian Dram", "դր.", "Armenian drams"),
                Currency.Create(null, "ARS", "Argentine Peso", "$", "Argentine pesos"),
                Currency.Create(null, "AUD", "Australian Dollar", "$", "Australian dollars"),
                Currency.Create(null, "AZN", "Azerbaijani Manat", "ман.", "Azerbaijani manats"),
                Currency.Create(null, "BAM", "Bosnia-Herzegovina Convertible Mark", "KM", "Bosnia-Herzegovina convertible marks"),
                Currency.Create(null, "BDT", "Bangladeshi Taka", "৳", "Bangladeshi takas"),
                Currency.Create(null, "BGN", "Bulgarian Lev", "лв.", "Bulgarian leva"),
                Currency.Create(null, "BHD", "Bahraini Dinar", "د.ب.‏", "Bahraini dinars"),
                Currency.Create(null, "BIF", "Burundian Franc", "FBu", "Burundian francs"),
                Currency.Create(null, "BND", "Brunei Dollar", "$", "Brunei dollars"),
                Currency.Create(null, "BOB", "Bolivian Boliviano", "Bs", "Bolivian bolivianos"),
                Currency.Create(null, "BRL", "Brazilian Real", "R$", "Brazilian reals"),
                Currency.Create(null, "BWP", "Botswanan Pula", "P", "Botswanan pulas"),
                Currency.Create(null, "BYR", "Belarusian Ruble", "BYR", "Belarusian rubles"),
                Currency.Create(null, "BZD", "Belize Dollar", "$", "Belize dollars"),
                Currency.Create(null, "CDF", "Congolese Franc", "FrCD", "Congolese francs"),
                Currency.Create(null, "CHF", "Swiss Franc", "CHF", "Swiss francs"),
                Currency.Create(null, "CLP", "Chilean Peso", "$", "Chilean pesos"),
                Currency.Create(null, "CNY", "Chinese Yuan", "CN¥", "Chinese yuan"),
                Currency.Create(null, "COP", "Colombian Peso", "$", "Colombian pesos"),
                Currency.Create(null, "CRC", "Costa Rican Colón", "₡", "Costa Rican colóns"),
                Currency.Create(null, "CVE", "Cape Verdean Escudo", "CV$", "Cape Verdean escudos"),
                Currency.Create(null, "CZK", "Czech Republic Koruna", "Kč", "Czech Republic korunas"),
                Currency.Create(null, "DJF", "Djiboutian Franc", "Fdj", "Djiboutian francs"),
                Currency.Create(null, "DKK", "Danish Krone", "kr", "Danish kroner"),
                Currency.Create(null, "DOP", "Dominican Peso", "RD$", "Dominican pesos"),
                Currency.Create(null, "DZD", "Algerian Dinar", "د.ج.‏", "Algerian dinars"),
                Currency.Create(null, "EEK", "Estonian Kroon", "kr", "Estonian kroons"),
                Currency.Create(null, "EGP", "Egyptian Pound", "ج.م.‏", "Egyptian pounds"),
                Currency.Create(null, "ERN", "Eritrean Nakfa", "Nfk", "Eritrean nakfas"),
                Currency.Create(null, "ETB", "Ethiopian Birr", "Br", "Ethiopian birrs"),
                Currency.Create(null, "GBP", "British Pound Sterling", "£", "British pounds sterling"),
                Currency.Create(null, "GEL", "Georgian Lari", "GEL", "Georgian laris"),
                Currency.Create(null, "GHS", "Ghanaian Cedi", "GH₵", "Ghanaian cedis"),
                Currency.Create(null, "GNF", "Guinean Franc", "FG", "Guinean francs"),
                Currency.Create(null, "GTQ", "Guatemalan Quetzal", "Q", "Guatemalan quetzals"),
                Currency.Create(null, "HKD", "Hong Kong Dollar", "$", "Hong Kong dollars"),
                Currency.Create(null, "HNL", "Honduran Lempira", "L", "Honduran lempiras"),
                Currency.Create(null, "HRK", "Croatian Kuna", "kn", "Croatian kunas"),
                Currency.Create(null, "HUF", "Hungarian Forint", "Ft", "Hungarian forints"),
                Currency.Create(null, "IDR", "Indonesian Rupiah", "Rp", "Indonesian rupiahs"),
                Currency.Create(null, "ILS", "Israeli New Sheqel", "₪", "Israeli new sheqels"),
                Currency.Create(null, "INR", "Indian Rupee", "টকা", "Indian rupees"),
                Currency.Create(null, "IQD", "Iraqi Dinar", "د.ع.‏", "Iraqi dinars"),
                Currency.Create(null, "IRR", "Iranian Rial", "﷼", "Iranian rials"),
                Currency.Create(null, "ISK", "Icelandic Króna", "kr", "Icelandic krónur"),
                Currency.Create(null, "JMD", "Jamaican Dollar", "$", "Jamaican dollars"),
                Currency.Create(null, "JOD", "Jordanian Dinar", "د.أ.‏", "Jordanian dinars"),
                Currency.Create(null, "JPY", "Japanese Yen", "￥", "Japanese yen"),
                Currency.Create(null, "KES", "Kenyan Shilling", "Ksh", "Kenyan shillings"),
                Currency.Create(null, "KHR", "Cambodian Riel", "៛", "Cambodian riels"),
                Currency.Create(null, "KMF", "Comorian Franc", "FC", "Comorian francs"),
                Currency.Create(null, "KRW", "South Korean Won", "₩", "South Korean won"),
                Currency.Create(null, "KWD", "Kuwaiti Dinar", "د.ك.‏", "Kuwaiti dinars"),
                Currency.Create(null, "KZT", "Kazakhstani Tenge", "тңг.", "Kazakhstani tenges"),
                Currency.Create(null, "LBP", "Lebanese Pound", "ل.ل.‏", "Lebanese pounds"),
                Currency.Create(null, "LKR", "Sri Lankan Rupee", "SL Re", "Sri Lankan rupees"),
                Currency.Create(null, "LTL", "Lithuanian Litas", "Lt", "Lithuanian litai"),
                Currency.Create(null, "LVL", "Latvian Lats", "Ls", "Latvian lati"),
                Currency.Create(null, "LYD", "Libyan Dinar", "د.ل.‏", "Libyan dinars"),
                Currency.Create(null, "MAD", "Moroccan Dirham", "د.م.‏", "Moroccan dirhams"),
                Currency.Create(null, "MDL", "Moldovan Leu", "MDL", "Moldovan lei"),
                Currency.Create(null, "MGA", "Malagasy Ariary", "MGA", "Malagasy Ariaries"),
                Currency.Create(null, "MKD", "Macedonian Denar", "MKD", "Macedonian denari"),
                Currency.Create(null, "MMK", "Myanma Kyat", "K", "Myanma kyats"),
                Currency.Create(null, "MOP", "Macanese Pataca", "MOP$", "Macanese patacas"),
                Currency.Create(null, "MUR", "Mauritian Rupee", "MURs", "Mauritian rupees"),
                Currency.Create(null, "MXN", "Mexican Peso", "$", "Mexican pesos"),
                Currency.Create(null, "MYR", "Malaysian Ringgit", "RM", "Malaysian ringgits"),
                Currency.Create(null, "MZN", "Mozambican Metical", "MTn", "Mozambican meticals"),
                Currency.Create(null, "NAD", "Namibian Dollar", "N$", "Namibian dollars"),
                Currency.Create(null, "NGN", "Nigerian Naira", "₦", "Nigerian nairas"),
                Currency.Create(null, "NIO", "Nicaraguan Córdoba", "C$", "Nicaraguan córdobas"),
                Currency.Create(null, "NOK", "Norwegian Krone", "kr", "Norwegian kroner"),
                Currency.Create(null, "NPR", "Nepalese Rupee", "नेरू", "Nepalese rupees"),
                Currency.Create(null, "NZD", "New Zealand Dollar", "$", "New Zealand dollars"),
                Currency.Create(null, "OMR", "Omani Rial", "ر.ع.‏", "Omani rials"),
                Currency.Create(null, "PAB", "Panamanian Balboa", "B/.", "Panamanian balboas"),
                Currency.Create(null, "PEN", "Peruvian Nuevo Sol", "S/.", "Peruvian nuevos soles"),
                Currency.Create(null, "PHP", "Philippine Peso", "₱", "Philippine pesos"),
                Currency.Create(null, "PKR", "Pakistani Rupee", "₨", "Pakistani rupees"),
                Currency.Create(null, "PLN", "Polish Zloty", "zł", "Polish zlotys"),
                Currency.Create(null, "PYG", "Paraguayan Guarani", "₲", "Paraguayan guaranis"),
                Currency.Create(null, "QAR", "Qatari Rial", "ر.ق.‏", "Qatari rials"),
                Currency.Create(null, "RON", "Romanian Leu", "RON", "Romanian lei"),
                Currency.Create(null, "RSD", "Serbian Dinar", "дин.", "Serbian dinars"),
                Currency.Create(null, "RUB", "Russian Ruble", "руб.", "Russian rubles"),
                Currency.Create(null, "RWF", "Rwandan Franc", "FR", "Rwandan francs"),
                Currency.Create(null, "SAR", "Saudi Riyal", "ر.س.‏", "Saudi riyals"),
                Currency.Create(null, "SDG", "Sudanese Pound", "SDG", "Sudanese pounds"),
                Currency.Create(null, "SEK", "Swedish Krona", "kr", "Swedish kronor"),
                Currency.Create(null, "SGD", "Singapore Dollar", "$", "Singapore dollars"),
                Currency.Create(null, "SOS", "Somali Shilling", "Ssh", "Somali shillings"),
                Currency.Create(null, "SYP", "Syrian Pound", "ل.س.‏", "Syrian pounds"),
                Currency.Create(null, "THB", "Thai Baht", "฿", "Thai baht"),
                Currency.Create(null, "TND", "Tunisian Dinar", "د.ت.‏", "Tunisian dinars"),
                Currency.Create(null, "TOP", "Tongan Paʻanga", "T$", "Tongan paʻanga"),
                Currency.Create(null, "TRY", "Turkish Lira", "TL", "Turkish Lira"),
                Currency.Create(null, "TTD", "Trinidad and Tobago Dollar", "$", "Trinidad and Tobago dollars"),
                Currency.Create(null, "TWD", "New Taiwan Dollar", "NT$", "New Taiwan dollars"),
                Currency.Create(null, "TZS", "Tanzanian Shilling", "TSh", "Tanzanian shillings"),
                Currency.Create(null, "UAH", "Ukrainian Hryvnia", "₴", "Ukrainian hryvnias"),
                Currency.Create(null, "UGX", "Ugandan Shilling", "USh", "Ugandan shillings"),
                Currency.Create(null, "UYU", "Uruguayan Peso", "$", "Uruguayan pesos"),
                Currency.Create(null, "UZS", "Uzbekistan Som", "UZS", "Uzbekistan som"),
                Currency.Create(null, "VEF", "Venezuelan Bolívar", "Bs.F.", "Venezuelan bolívars"),
                Currency.Create(null, "VND", "Vietnamese Dong", "₫", "Vietnamese dong"),
                Currency.Create(null, "XAF", "CFA Franc BEAC", "FCFA", "CFA francs BEAC"),
                Currency.Create(null, "XOF", "CFA Franc BCEAO", "CFA", "CFA francs BCEAO"),
                Currency.Create(null, "YER", "Yemeni Rial", "ر.ي.‏", "Yemeni rials"),
                Currency.Create(null, "ZAR", "South African Rand", "R", "South African rand"),
                Currency.Create(null, "ZMK", "Zambian Kwacha", "ZK", "Zambian kwachas")


            };

        }
    }
}