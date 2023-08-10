using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using MonkeyTax.Application.Monotributo.Model;
using MonkeyTax.Application.Monotributo.Services.Monotributo.Config;
using MonkeyTax.Application.UserAgents.Services;
using System.Globalization;

namespace MonkeyTax.Application.Monotributo.Services.Monotributo
{
    public class MonotributoService : IMonotributoService
    {
        private const string CACHE_KEY = "Monotributo";

        private readonly MonotributoServiceConfig _config;
        private readonly IUserAgentService _userAgentService;
        private readonly IMemoryCache _memoryCache;

        public MonotributoService(MonotributoServiceConfig config, IUserAgentService userAgentService, IMemoryCache memoryCache)
        {
            _config = config;
            _userAgentService = userAgentService;
            _memoryCache = memoryCache;
        }

        #region Private

        public async Task<HtmlDocument> LoadHtmlAsync(CancellationToken cancellationToken = default)
        {
            string? userAgent = await _userAgentService.GetRandomUserAgent(cancellationToken);
            
            if(_memoryCache.TryGetValue(CACHE_KEY, out string html))
            {
                HtmlDocument cachedDocument = new();
                cachedDocument.LoadHtml(html);
                return cachedDocument;
            }

            HtmlWeb web = new();
            web.PreRequest += (request) =>
            {
                foreach (var header in _config.Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
                if (!string.IsNullOrWhiteSpace(userAgent))
                {
                    request.Headers.Add("User-Agent", userAgent);
                }

                return true;
            };
            HtmlDocument document = await web.LoadFromWebAsync(_config.MonotributoUrl, cancellationToken);
            _memoryCache.Set(CACHE_KEY, document.DocumentNode.OuterHtml);
            return document;
        }

        private static MonotributoUnit<decimal>? ParseCurrency(HtmlNode? node)
        {
            if (node != null && !string.IsNullOrWhiteSpace(node.InnerText))
            {
                string[] splittedText = node.InnerText.Trim().Split(' ');
                if (splittedText.Length == 2)
                {
                    string unit = splittedText[0].Replace("$", "ARS");
                    decimal amount = decimal.TryParse(splittedText[1], CultureInfo.GetCultureInfo("es-AR"), out decimal result) ? result : default;
                    return new(amount, unit);
                }
            }

            return default;
        }

        private static MonotributoUnit<int>? ParseIntUnit(HtmlNode? node)
        {
            if (node != null && !string.IsNullOrWhiteSpace(node.InnerText))
            {
                string[] splittedText = node.InnerText.Trim().Split(' ');
                if (splittedText.Length == 3)
                {
                    string unit = splittedText[2];
                    int value = int.TryParse(splittedText[1], out int result) ? result : default;
                    return new(value, unit);
                }
            }

            return default;
        }

        #endregion

        public async Task<MonotributoResponse> GetValuesAsync(CancellationToken cancellationToken = default)
        {
            List<MonotributoCategory> categories = new();

            HtmlDocument document = await LoadHtmlAsync(cancellationToken);
            HtmlNode table = document.DocumentNode.SelectSingleNode("//div[@id='vigentes']/div[2]/div[1]/table[1]/tbody[1]");
            HtmlNodeCollection rows = table.SelectNodes("tr");
            foreach (HtmlNode row in rows)
            {
                categories.Add(new()
                {
                    Categoria = row.SelectSingleNode("th").InnerText,
                    IngresosBrutosAnuales = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_ing_br_t15')]")),
                    Actividad = row.SelectSingleNode("td[contains(@headers,'th_act_t15')]").InnerText,
                    CantidadMinimaDeEmpleados = row.SelectSingleNode("td[contains(@headers,'th_cant_min_emp_t15')]").InnerText,
                    SuperficieMaximaAfectada = ParseIntUnit(row.SelectSingleNode("td[contains(@headers,'th_sup_af_t15')]")),
                    EnergiaElectricaMaximaAnual = ParseIntUnit(row.SelectSingleNode("td[contains(@headers,'th_energ_t15')]")),
                    AlquileresDevengadosAnuales = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_alq_t15')]")),
                    PrecioUnitarioMaximoVentaCosasMuebles = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_venta_cosas_muebles_t15')]")),
                    ImpuestoIntegrado = new()
                    {
                        Servicios = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_imp_int_loc_t15')]")),
                        VentaCosasMuebles = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_imp_int_ven_t15')]")),
                    },
                    AportesMensuales = new()
                    {
                        SistemaPrevisional = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_ap_sipa_t15')]")),
                        ObraSocial = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_ap_obra_soc_t15')]")),
                    },
                    CostosMensuales = new()
                    {
                        PrestacionServicios = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_total_loc_t15')]")),
                        VentaCosasMuebles = ParseCurrency(row.SelectSingleNode("td[contains(@headers,'th_total_ven_t15')]")),
                    },
                });
            }

            return new()
            {
                Categorias = categories,
            };
        }
    }
}
