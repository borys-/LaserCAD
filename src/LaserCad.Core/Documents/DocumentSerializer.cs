using System.Text.Json;
using System.Text.Json.Serialization;
using LaserCad.Core.Parameters;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Documents;

/// <summary>
/// Serializuje i deserializuje dokument CAD do stabilnego formatu pliku projektu.
/// Obecnie obslugiwanym formatem jest JSON w wersji 1.
/// </summary>
public sealed class DocumentSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Biezaca wersja formatu pliku obslugiwana przez serializer.
    /// </summary>
    public const int SupportedFormatVersion = 1;

    /// <summary>
    /// Serializuje dokument CAD do tekstu JSON.
    /// </summary>
    public string Serialize(CadDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var dto = new DocumentDto
        {
            FormatVersion = document.FormatVersion,
            Id = document.Id,
            Name = document.Name,
            Parameters = document.Parameters.Parameters.Select(ToDto).ToArray(),
            Layers = document.Layers.Select(ToDto).ToArray(),
            MaterialProfile = document.MaterialProfile is null ? null : ToDto(document.MaterialProfile)
        };

        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    /// <summary>
    /// Odtwarza dokument CAD z tekstu JSON.
    /// </summary>
    public CadDocument Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Document JSON cannot be empty.", nameof(json));
        }

        var dto = JsonSerializer.Deserialize<DocumentDto>(json, JsonOptions)
            ?? throw new InvalidOperationException("Document JSON could not be deserialized.");

        var parameters = new ParameterSet((dto.Parameters ?? Array.Empty<ParameterDto>()).Select(ToDomain));
        var layers = dto.Layers?.Select(ToDomain).ToArray();
        var materialProfile = dto.MaterialProfile is null ? null : ToDomain(dto.MaterialProfile);
        var formatVersion = dto.FormatVersion == 0 ? SupportedFormatVersion : dto.FormatVersion;

        return new CadDocument(dto.Id, dto.Name, formatVersion, parameters, layers, materialProfile: materialProfile);
    }

    private static MaterialProfileDto ToDto(MaterialProfile materialProfile)
    {
        return new MaterialProfileDto
        {
            Name = materialProfile.Name,
            ThicknessMillimeters = materialProfile.Thickness.Millimeters,
            DefaultKerfMillimeters = materialProfile.DefaultKerf.Millimeters,
            DefaultClearanceMillimeters = materialProfile.DefaultClearance.Millimeters,
            MinimumFingerWidthMillimeters = materialProfile.MinimumFingerWidth.Millimeters
        };
    }

    private static MaterialProfile ToDomain(MaterialProfileDto dto)
    {
        return new MaterialProfile(
            dto.Name,
            Length.FromMillimeters(dto.ThicknessMillimeters),
            Length.FromMillimeters(dto.DefaultKerfMillimeters),
            Length.FromMillimeters(dto.DefaultClearanceMillimeters),
            Length.FromMillimeters(dto.MinimumFingerWidthMillimeters));
    }

    private static LayerDto ToDto(Layer layer)
    {
        return new LayerDto
        {
            Name = layer.Name,
            Color = layer.Color.Hex,
            Role = layer.Role.ToString()
        };
    }

    private static Layer ToDomain(LayerDto dto)
    {
        return new Layer(
            dto.Name,
            LayerColor.FromHex(dto.Color),
            Enum.Parse<LayerRole>(dto.Role, ignoreCase: false));
    }

    private static ParameterDto ToDto(Parameter parameter)
    {
        return new ParameterDto
        {
            Id = parameter.Id.Value,
            Name = parameter.Name,
            Type = parameter.Type.ToString(),
            Value = ToSerializableParameterValue(parameter.Type, parameter.Value),
            DisplayUnit = parameter.DisplayUnit,
            MinimumValue = ToSerializableParameterValue(parameter.Type, parameter.MinimumValue),
            MaximumValue = ToSerializableParameterValue(parameter.Type, parameter.MaximumValue)
        };
    }

    private static Parameter ToDomain(ParameterDto dto)
    {
        var type = Enum.Parse<ParameterType>(dto.Type, ignoreCase: false);

        return new Parameter(
            new ParameterId(dto.Id),
            dto.Name,
            type,
            ToDomainParameterValue(type, dto.Value),
            dto.DisplayUnit,
            ToDomainParameterValue(type, dto.MinimumValue),
            ToDomainParameterValue(type, dto.MaximumValue));
    }

    private static object? ToSerializableParameterValue(ParameterType type, object? value)
    {
        if (value is null)
        {
            return null;
        }

        return type switch
        {
            ParameterType.Length => ((Length)value).Millimeters,
            _ => value
        };
    }

    private static object? ToDomainParameterValue(ParameterType type, object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is JsonElement jsonElement)
        {
            return type switch
            {
                ParameterType.Length => Length.FromMillimeters(jsonElement.GetDouble()),
                ParameterType.Number => jsonElement.GetDouble(),
                ParameterType.Boolean => jsonElement.GetBoolean(),
                ParameterType.Text => jsonElement.GetString() ?? string.Empty,
                ParameterType.Choice => jsonElement.GetString() ?? string.Empty,
                _ => throw new InvalidOperationException($"Unsupported parameter type '{type}'.")
            };
        }

        return type switch
        {
            ParameterType.Length => value,
            _ => value
        };
    }

    private sealed class DocumentDto
    {
        public int FormatVersion { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ParameterDto[]? Parameters { get; set; }

        public LayerDto[]? Layers { get; set; }

        public MaterialProfileDto? MaterialProfile { get; set; }
    }

    private sealed class ParameterDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public object? Value { get; set; }

        public string? DisplayUnit { get; set; }

        public object? MinimumValue { get; set; }

        public object? MaximumValue { get; set; }
    }

    private sealed class LayerDto
    {
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }

    private sealed class MaterialProfileDto
    {
        public string Name { get; set; } = string.Empty;

        public double ThicknessMillimeters { get; set; }

        public double DefaultKerfMillimeters { get; set; }

        public double DefaultClearanceMillimeters { get; set; }

        public double MinimumFingerWidthMillimeters { get; set; }
    }
}
