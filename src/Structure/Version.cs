using System.Text.Json.Serialization;
using JaduFromJson.Utils;

namespace JaduFromJson.Structure;
public class Version(string[] versions)
{
    [JsonPropertyName("COLLECTIONS_VERSION")]
    public string CollectionsVersion {private set; get;} = versions[0];

    [JsonPropertyName("SITEMAP_GENERATOR_VERSION")]
    public string SitemapGeneratorVersion {private set; get;} = versions[1];

    [JsonPropertyName("PAYBRIDGE_VODAFONE_STORM_PADLOCK_VERSION")]
    public string PaybridgeVodafoneStormPadlockVersion {private set; get;} = versions[2];

    [JsonPropertyName("CLIENT_VERSION")]
    public string ClientVersion {private set; get;} = versions[3];

    [JsonPropertyName("CENTRAL_VERSION")]
    public string CentralVersion {private set; get;} = versions[4];

    [JsonPropertyName("VERSION")]
    public string VERSION {private set; get;} = versions[5];

    [JsonPropertyName("PAYBRIDGE_HEYCENTRIC_VERSION")]
    public string PaybridgeHeycentricVersion {private set; get;} = versions[6];

    [JsonPropertyName("XFP_VERSION")]
    public string XfpVersion {private set; get;} = versions[7];

    [JsonPropertyName("PAYBRIDGE_CIVICA_ICON_SURCHARGES_VERSION")]
    public string PaybridgeCivicaIconSurchargesVersion {private set; get;} = versions[8];

    [JsonPropertyName("XFP_CONTINUUM_VERSION")]
    public string XfpContinuumVersion {private set; get;} = versions[9];

    [JsonPropertyName("PAYBRIDGE_CAPITASCP_VERSION")]
    public string PaybridgeCapitascpVersion {private set; get;} = versions[10];

    [JsonPropertyName("WIDGET_FACTORY_VERSION")]
    public string WidgetFactoryVersion {private set; get;} = versions[11];

    public void Save(string formName = "")
    {
        SaveFiles.Save("version", this, formName);
    }
}