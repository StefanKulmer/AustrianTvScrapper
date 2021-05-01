using System;
using Xunit;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesParserTests
    {
        private OrfTvSeriesParser sut = new OrfTvSeriesParser();

        [Fact]
        public void Test1()
        {
            var html  =
            @"<article class=""b-teaser"">
        <a class=""teaser-link js-teaser-link"" title=""Am Schauplatz"" href=""https://tvthek.orf.at/profile/Am-Schauplatz/1239/Am-Schauplatz-Weggelegt/14090535"" tabindex=""0"">
            <div class=""img-container  "" data-jsb=""{&quot;hls&quot;:[{&quot;quality_key&quot;:&quot;Q6A&quot;,&quot;src&quot;:&quot;https:\/\/apasfiis.sf.apa.at\/ipad\/cms-preview-clips\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q6A.mp4\/playlist.m3u8&quot;},{&quot;quality_key&quot;:&quot;Q8C&quot;,&quot;src&quot;:&quot;https:\/\/apasfiis.sf.apa.at\/ipad\/cms-preview-clips\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q8C.mp4\/playlist.m3u8&quot;}],&quot;hds&quot;:[{&quot;quality_key&quot;:&quot;Q6A&quot;,&quot;src&quot;:&quot;https:\/\/apasfiis.sf.apa.at\/f4m\/cms-preview-clips\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q6A.mp4\/manifest.f4m&quot;},{&quot;quality_key&quot;:&quot;Q8C&quot;,&quot;src&quot;:&quot;https:\/\/apasfiis.sf.apa.at\/f4m\/cms-preview-clips\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q8C.mp4\/manifest.f4m&quot;}],&quot;smooth&quot;:[{&quot;quality_key&quot;:&quot;Q6A&quot;,&quot;src&quot;:&quot;https:\/\/apasfiis.sf.apa.at\/smooth\/cms-preview-clips\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q6A.mp4\/Manifest&quot;},{&quot;quality_key&quot;:&quot;Q8C&quot;,&quot;src&quot;:&quot;https:\/\/apasfiis.sf.apa.at\/smooth\/cms-preview-clips\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q8C.mp4\/Manifest&quot;}],&quot;dash&quot;:[{&quot;quality_key&quot;:&quot;Q6A&quot;,&quot;src&quot;:&quot;https:\/\/apasfiis.sf.apa.at\/dash\/cms-preview-clips\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q6A.mp4\/manifest.mpd&quot;},{&quot;quality_key&quot;:&quot;Q8C&quot;,&quot;src&quot;:&quot;https:\/\/apasfiis.sf.apa.at\/dash\/cms-preview-clips\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q8C.mp4\/manifest.mpd&quot;}],&quot;progressive_download&quot;:[{&quot;quality_key&quot;:&quot;Q6A&quot;,&quot;src&quot;:&quot;https:\/\/apasfpd.sf.apa.at\/cms-preview-clips\/online\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q6A.mp4&quot;},{&quot;quality_key&quot;:&quot;Q8C&quot;,&quot;src&quot;:&quot;https:\/\/apasfpd.sf.apa.at\/cms-preview-clips\/online\/2021-04-29_2105_in_02_Am-Schauplatz--_____14090535__o__1586215603__s14909220_clip_Q8C.mp4&quot;}]}"">                <div class=""ratio-box"">
                                        <video class=""teaser-img hide
                                                "" muted="""" loop="""">

                    </video>
                    <figure class=""teaser-img"">
                        <img data-src=""https://api-tvthek.orf.at/uploads/media/profiles/0114/54/thumb_11353970_profiles_list.jpeg"" data-srcset=""https://api-tvthek.orf.at/uploads/media/profiles/0114/54/thumb_11353970_profiles_small.jpeg 120w, https://api-tvthek.orf.at/uploads/media/profiles/0114/54/thumb_11353970_profiles_list.jpeg 320w, https://api-tvthek.orf.at/uploads/media/profiles/0114/54/thumb_11353970_profiles_player.jpeg 640w"" data-sizes=""auto"" class=""lazyautosizes lazyloaded"" alt="""" title=""Am Schauplatz"" src=""https://api-tvthek.orf.at/uploads/media/profiles/0114/54/thumb_11353970_profiles_list.jpeg"" sizes=""164px"" srcset=""https://api-tvthek.orf.at/uploads/media/profiles/0114/54/thumb_11353970_profiles_small.jpeg 120w, https://api-tvthek.orf.at/uploads/media/profiles/0114/54/thumb_11353970_profiles_list.jpeg 320w, https://api-tvthek.orf.at/uploads/media/profiles/0114/54/thumb_11353970_profiles_player.jpeg 640w"">
                    </figure>
                                            <span class=""hover-duration"">48:00 Min.</span>
                                    </div>
            </div>
            <div class=""text-container"">
                <p class=""channel"">ORF 2</p>
                <h4 class=""profile"">
                                            Am Schauplatz
                                    </h4>
                <h5 class=""teaser-title  "" data-jsb=""{&quot;rows&quot;: 1}"">
                                            Am Schauplatz
                                    </h5>
                <p class=""description  "" data-jsb=""{&quot;rows&quot;: 3}"">
                    Investigative Reportagen - engagiert und nahe am Menschen
                </p>
                <div class=""availability-block"">
                    <p class=""availability"" title=""Verfügbarkeit"" aria-label=""Verfügbarkeit"">4 Tage</p>
                                    </div>
                <time datetime=""2021-04-29CEST21:05:31"" class=""datetime"">
                    <span class=""date"">29.4.2021</span>
                    <span class=""datetime-separator"">|</span>
                    <span class=""time"">21.05 Uhr</span>
                </time>
                <p class=""date-as-string"">Do., 29.4.2021</p>
                <p class=""visible-duration"">48:00 Min.</p>
                <button data-teaser-id=""1239"" class=""b-favorite-icon favorite-icon js-favorite-button"" title=""Von den Favoriten entfernen"" aria-label=""Von den Favoriten entfernen""></button>
                            </div>
        </a>
    </article>";

            var orfTvSeries = sut.Parse(HtmlAgilityPack.HtmlNode.CreateNode(html));

            Assert.NotNull(orfTvSeries);
            Assert.Equal("Am Schauplatz", orfTvSeries.Title);
        }
    }
}
