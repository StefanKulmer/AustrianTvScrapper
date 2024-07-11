using System.Text.Json.Serialization;

namespace OrfDataProvider.Model;
public class Profile
{
    public bool AudioDescription { get; set; }
    public string BreakText { get; set; }
    public string BreakTitle { get; set; }
    public string DatasetName { get; set; }
    public string DdsIdentifier { get; set; }
    public string Description { get; set; }
    public bool HasActiveYouthProtection { get; set; }
    public bool HasYouthProtection { get; set; }
    public string Headline { get; set; }
    public bool HideInLetterGroup { get; set; }
    public int Id { get; set; }
    public string LetterGroup { get; set; }
    public string LivestreamSubHeadline { get; set; }
    public int NewestEpisodeId { get; set; }
    public bool Oegs { get; set; }
    public string OewaBasePath { get; set; }
    public string ProfileWebsite { get; set; }
    public string SubHeadline { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string YouthProtectionType { get; set; }
    [JsonPropertyName("_links")]
    public Links Links { get; set; }
    public Embedded Embedded { get; set; }
}

public class Links
{
    public Link Self { get; set; }
    public Link Image { get; set; }
    public Link Image16x9WithLogo { get; set; }
    public Link Image2x3WithLogo { get; set; }
    public Link Image2x3 { get; set; }
    public Link Theme { get; set; }
    public Link Genre { get; set; }
    [JsonPropertyName("links")]
    public Link TheLinks { get; set; }
    public Link Episodes { get; set; }
    public Link RelatedProfiles { get; set; }
    public Link LatestEpisode { get; set; }
    public Link Tags { get; set; }
    public Link AdvertisingTags { get; set; }
}

public class Link
{
    public string Href { get; set; }
}

public class Embedded
{
    public Image Image { get; set; }
    public Image Image16x9WithLogo { get; set; }
    public Image Image2x3WithLogo { get; set; }
    public Image Image2x3 { get; set; }
}

public class Image
{
    public string DynamicColor { get; set; }
    public bool IsFallback { get; set; }
    public PublicUrls PublicUrls { get; set; }
}

public class PublicUrls
{
    public Url HighlightTeaser { get; set; }
    public Url Player { get; set; }
    public Url List { get; set; }
    public Url Small { get; set; }
    public Url Reference { get; set; }
}

public class Url
{
    [JsonPropertyName("url")]
    public string TheUrl { get; set; }
    public int Width { get; set; }
}
