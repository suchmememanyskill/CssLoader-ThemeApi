namespace DeckPersonalisationApi.Model.Dto.External.GET;

public class MinimalCssThemeDto
{
    public string Id { get; }
    public string Name { get; }
    public string Type { get; }
    public string Version { get; }
    public string Target { get; }
    public string SpecifiedAuthor { get; }
    public int ManifestVersion { get; }

    public MinimalCssThemeDto(CssTheme theme)
    {
        Id = theme.Id;
        Name = theme.Name;
        Version = theme.Version;
        Target = theme.Target;
        ManifestVersion = theme.ManifestVersion;
        SpecifiedAuthor = theme.SpecifiedAuthor;
        Type = theme.Type.ToString();
    }
}