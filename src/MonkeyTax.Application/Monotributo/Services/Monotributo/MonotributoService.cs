using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using MonkeyTax.Application.Monotributo.Model;
using MonkeyTax.Application.Monotributo.Services.Monotributo.Config;
using MonkeyTax.Application.Proxies.Services;
using MonkeyTax.Application.UserAgents.Services;
using System.Globalization;
using System.Net;

namespace MonkeyTax.Application.Monotributo.Services.Monotributo
{
    public class MonotributoService(
        MonotributoServiceConfig config,
        IUserAgentService userAgentService,
        IProxyService proxyService,
        IMemoryCache memoryCache
        ) : IMonotributoService
    {
        private const string CACHE_KEY = "Monotributo";

        private readonly MonotributoServiceConfig _config = config;
        private readonly IUserAgentService _userAgentService = userAgentService;
        private readonly IProxyService _proxyService = proxyService;
        private readonly IMemoryCache _memoryCache = memoryCache;

        #region Private

        public async Task<HtmlDocument> LoadHtmlAsync(bool allowCache, CancellationToken cancellationToken = default)
        {
            string? userAgent = await _userAgentService.GetRandomUserAgentAsync(cancellationToken);
                        
            if(allowCache && _memoryCache.TryGetValue(CACHE_KEY, out string html))
            {
                HtmlDocument cachedDocument = new();
                cachedDocument.LoadHtml(html);
                return cachedDocument;
            }

            HtmlWeb web = new();
            IWebProxy? proxy = await _proxyService.GetRandomProxyAsync(cancellationToken);
            web.PreRequest += (HttpWebRequest request) =>
            {                
                request.Proxy = proxy;
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
                    decimal amount = decimal.TryParse(splittedText[1], NumberStyles.Any, CultureInfo.GetCultureInfo("es-AR"), out decimal result) ? result : default;
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

        public async Task<MonotributoResponse> GetValuesAsync(string? cacheControlHeader, CancellationToken cancellationToken = default)
        {
            List<MonotributoCategory> categories = [];

            bool allowCache = string.IsNullOrWhiteSpace(cacheControlHeader) || !cacheControlHeader.Equals("No-Cache", StringComparison.OrdinalIgnoreCase);
            HtmlDocument document = await LoadHtmlAsync(allowCache, cancellationToken);
            HtmlNode table = document.DocumentNode.SelectSingleNode("//div[@id='vigentes']/div[2]/div[1]/table[1]/tbody[1]");
            HtmlNodeCollection rows = table.SelectNodes("tr");
            foreach (HtmlNode row in rows)
            {
                HtmlNode? nodeCategoria = row.SelectSingleNode("th");
                HtmlNode? nodeIngresosBrutosAnuales = row.SelectSingleNode("td[contains(@headers,'th_ing_br_t15')]");
                HtmlNode? nodeActividad = row.SelectSingleNode("td[contains(@headers,'th_act_t15')]");
                //HtmlNode? nodeCantidadMinimaDeEmpleados = row.SelectSingleNode("td[contains(@headers,'th_cant_min_emp_t15')]");
                HtmlNode? nodeSuperficieMaximaAfectada = row.SelectSingleNode("td[contains(@headers,'th_sup_af_t15')]");
                HtmlNode? nodeEnergiaElectricaMaximaAnual = row.SelectSingleNode("td[contains(@headers,'th_energ_t15')]");
                HtmlNode? nodeAlquileresDevengadosAnuales = row.SelectSingleNode("td[contains(@headers,'th_alq_t15')]");
                HtmlNode? nodePrecioUnitarioMaximoVentaCosasMuebles = row.SelectSingleNode("td[contains(@headers,'th_venta_cosas_muebles_t15')]");
                HtmlNode? nodeImpuestoIntegradoServicios = row.SelectSingleNode("td[contains(@headers,'th_imp_int_loc_t15')]");
                HtmlNode? nodeImpuestoIntegradoVentaCosasMuebles = row.SelectSingleNode("td[contains(@headers,'th_imp_int_ven_t15')]");
                HtmlNode? nodeAportesMensualesSistemaPrevisional = row.SelectSingleNode("td[contains(@headers,'th_ap_sipa_t15')]");
                HtmlNode? nodeAportesMensualesObraSocial = row.SelectSingleNode("td[contains(@headers,'th_ap_obra_soc_t15')]");
                HtmlNode? nodeCostosMensualesPrestacionServicios = row.SelectSingleNode("td[contains(@headers,'th_total_loc_t15')]");
                HtmlNode? nodeCostosMensualesVentaCosasMuebles = row.SelectSingleNode("td[contains(@headers,'th_total_ven_t15')]");
                
                MonotributoCategory category = new()
                {
                    Categoria = nodeCategoria?.InnerText ?? string.Empty,
                    IngresosBrutosAnuales = ParseCurrency(nodeIngresosBrutosAnuales),
                    Actividad = nodeActividad?.InnerText ?? string.Empty,
                    //CantidadMinimaDeEmpleados = nodeCantidadMinimaDeEmpleados?.InnerText ?? string.Empty,
                    SuperficieMaximaAfectada = ParseIntUnit(nodeSuperficieMaximaAfectada),
                    EnergiaElectricaMaximaAnual = ParseIntUnit(nodeEnergiaElectricaMaximaAnual),
                    AlquileresDevengadosAnuales = ParseCurrency(nodeAlquileresDevengadosAnuales),
                    PrecioUnitarioMaximoVentaCosasMuebles = ParseCurrency(nodePrecioUnitarioMaximoVentaCosasMuebles),
                    ImpuestoIntegrado = new()
                    {
                        Servicios = ParseCurrency(nodeImpuestoIntegradoServicios),
                        VentaCosasMuebles = ParseCurrency(nodeImpuestoIntegradoVentaCosasMuebles),
                    },
                    AportesMensuales = new()
                    {
                        SistemaPrevisional = ParseCurrency(nodeAportesMensualesSistemaPrevisional),
                        ObraSocial = ParseCurrency(nodeAportesMensualesObraSocial),
                    },
                    CostosMensuales = new()
                    {
                        PrestacionServicios = ParseCurrency(nodeCostosMensualesPrestacionServicios),
                        VentaCosasMuebles = ParseCurrency(nodeCostosMensualesVentaCosasMuebles),
                    },
                };
                categories.Add(category);
            }

            return new()
            {
                Categorias = categories,
            };
        }
    }
}
