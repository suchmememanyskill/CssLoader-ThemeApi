using System.ComponentModel.DataAnnotations.Schema;
using DeckPersonalisationApi.Model.Dto.External.GET;

namespace DeckPersonalisationApi.Model;

public class CssTheme : IToDto<PartialCssThemeDto>, IToDto<MinimalCssThemeDto>, IToDto<FullCssThemeDto>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ThemeType Type { get; set; }
    public List<SavedBlob> Images { get; set; }
    public SavedBlob Download { get; set; }
    public string Version { get; set; }
    public string? Source { get; set; }
    public User Author { get; set; }
    public string SpecifiedAuthor { get; set; }
    public DateTimeOffset Submitted { get; set; }
    public DateTimeOffset Updated { get; set; }
    public string Target { get; set; }
    public int ManifestVersion { get; set; }
    public string Description { get; set; }
    [InverseProperty("DependenciesOf")]
    public List<CssTheme> Dependencies { get; set; }
    [InverseProperty("Dependencies")]
    public List<CssTheme> DependenciesOf { get; set; }
    public PostVisibility Visibility { get; set; }
    public ICollection<User> UserStars { get; set; }
    public long StarCount { get; set; }

    public PartialCssThemeDto ToDto()
        => new(this);

    MinimalCssThemeDto IToDto<MinimalCssThemeDto>.ToDto()
        => new(this);

    FullCssThemeDto IToDto<FullCssThemeDto>.ToDto()
        => new(this);

    public object ToDtoObject()
        => ToDto();
}