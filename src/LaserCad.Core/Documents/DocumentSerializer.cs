using System.Text.Json;
using System.Text.Json.Serialization;
using LaserCad.Core.MaterialModel;
using LaserCad.Core.Parameters;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;
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
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }
        EnsureSupportedFormatVersion(document.FormatVersion);

        var dto = new DocumentDto
        {
            FormatVersion = document.FormatVersion,
            Id = document.Id,
            Name = document.Name,
            Parameters = document.Parameters.Parameters.Select(ToDto).ToArray(),
            Layers = document.Layers.Select(ToDto).ToArray(),
            Sketches = document.Sketches.Select(ToDto).ToArray(),
            MaterialProfile = document.MaterialProfile is null ? null : ToDto(document.MaterialProfile),
            MaterialSolids = document.MaterialSolids.Select(ToDto).ToArray()
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
        var sketches = (dto.Sketches ?? Array.Empty<SketchDto>()).Select(ToDomain).ToArray();
        var materialProfile = dto.MaterialProfile is null ? null : ToDomain(dto.MaterialProfile);
        var materialSolids = (dto.MaterialSolids ?? Array.Empty<MaterialSolidDto>()).Select(ToDomain).ToArray();
        var formatVersion = dto.FormatVersion == 0 ? SupportedFormatVersion : dto.FormatVersion;
        EnsureSupportedFormatVersion(formatVersion);

        return new CadDocument(
            dto.Id,
            dto.Name,
            formatVersion,
            parameters,
            layers,
            sketches,
            materialProfile: materialProfile,
            materialSolids: materialSolids);
    }

    private static void EnsureSupportedFormatVersion(int formatVersion)
    {
        if (formatVersion != SupportedFormatVersion)
        {
            throw new NotSupportedException($"Document format version {formatVersion} is not supported.");
        }
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

    private static MaterialSolidDto ToDto(MaterialSolid materialSolid)
    {
        return new MaterialSolidDto
        {
            Id = materialSolid.Id,
            Name = materialSolid.Name,
            MaterialProfile = ToDto(materialSolid.MaterialProfile),
            Mesh = ToDto(materialSolid.Mesh),
            Orientation = ToDto(materialSolid.Orientation),
            Cutouts = materialSolid.Cutouts.Select(ToDto).ToArray()
        };
    }

    private static MaterialSolid ToDomain(MaterialSolidDto dto)
    {
        return new MaterialSolid(
            dto.Id,
            dto.Name,
            ToDomain(RequiredMaterialProfile(dto.MaterialProfile)),
            ToDomain(RequiredMesh(dto.Mesh)),
            ToDomain(RequiredOrientation(dto.Orientation)),
            (dto.Cutouts ?? Array.Empty<CutoutFeatureDto>()).Select(ToDomain));
    }

    private static CutoutFeatureDto ToDto(CutoutFeature cutout)
    {
        return new CutoutFeatureDto
        {
            Id = cutout.Id,
            Name = cutout.Name,
            Kind = cutout.Kind.ToString(),
            FaceName = cutout.FaceName,
            Contour = cutout.Contour.Vertices.Select(ToDto).ToArray()
        };
    }

    private static CutoutFeature ToDomain(CutoutFeatureDto dto)
    {
        return new CutoutFeature(
            dto.Id,
            dto.Name,
            Enum.Parse<CutoutFeatureKind>(dto.Kind, ignoreCase: false),
            new Polygon2D(RequiredPoints(dto.Contour, "Cutout")),
            dto.FaceName);
    }

    private static MeshDto ToDto(Mesh3D mesh)
    {
        return new MeshDto
        {
            Vertices = mesh.Vertices.Select(ToDto).ToArray(),
            TriangleIndices = mesh.TriangleIndices.ToArray()
        };
    }

    private static Mesh3D ToDomain(MeshDto dto)
    {
        return new Mesh3D(
            (dto.Vertices ?? Array.Empty<Point3DDto>()).Select(ToDomain),
            dto.TriangleIndices ?? Array.Empty<int>());
    }

    private static OrientationDto ToDto(MaterialSolidOrientation orientation)
    {
        return new OrientationDto
        {
            Position = ToDto(orientation.Position),
            RotationRadians = orientation.RotationRadians,
            SurfaceNormal = ToDto(orientation.SurfaceNormal)
        };
    }

    private static MaterialSolidOrientation ToDomain(OrientationDto dto)
    {
        return new MaterialSolidOrientation(
            ToDomain(dto.Position),
            dto.RotationRadians,
            ToDomain(dto.SurfaceNormal));
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

    private static SketchDto ToDto(Sketch sketch)
    {
        return new SketchDto
        {
            Id = sketch.Id,
            Name = sketch.Name,
            Entities = sketch.Entities.Select(ToDto).ToArray()
        };
    }

    private static Sketch ToDomain(SketchDto dto)
    {
        return new Sketch(dto.Id, dto.Name, (dto.Entities ?? Array.Empty<EntityDto>()).Select(ToDomain));
    }

    private static EntityDto ToDto(Entity entity)
    {
        var dto = new EntityDto
        {
            Id = entity.Id,
            LayerName = entity.LayerName,
            DimensionBindings = entity.DimensionBindings.Select(ToDto).ToArray()
        };

        return entity switch
        {
            LineEntity line => dto with
            {
                Type = "Line",
                Start = ToDto(line.Segment.Start),
                End = ToDto(line.Segment.End)
            },
            RectangleEntity rectangle => dto with
            {
                Type = "Rectangle",
                Corners = rectangle.Corners.Select(ToDto).ToArray()
            },
            CircleEntity circle => dto with
            {
                Type = "Circle",
                Center = ToDto(circle.Circle.Center),
                Radius = circle.Circle.Radius
            },
            ArcEntity arc => dto with
            {
                Type = "Arc",
                Center = ToDto(arc.Arc.Center),
                Radius = arc.Arc.Radius,
                StartAngleRadians = arc.Arc.StartAngleRadians,
                EndAngleRadians = arc.Arc.EndAngleRadians,
                Direction = arc.Arc.Direction.ToString()
            },
            PolylineEntity polyline => dto with
            {
                Type = "Polyline",
                Points = polyline.Polyline.Points.Select(ToDto).ToArray(),
                IsClosed = polyline.Polyline.IsClosed
            },
            TextEntity text => dto with
            {
                Type = "Text",
                Text = text.Text,
                Position = ToDto(text.Position),
                Height = text.Height,
                FontFamily = text.Font.FamilyName,
                FontFilePath = text.Font.FilePath,
                Alignment = text.Alignment.ToString()
            },
            _ => throw new NotSupportedException($"Sketch entity type '{entity.GetType().Name}' is not supported.")
        };
    }

    private static Entity ToDomain(EntityDto dto)
    {
        return dto.Type switch
        {
            "Line" => new LineEntity(
                new LineSegment2D(ToDomain(dto.Start), ToDomain(dto.End)),
                dto.Id,
                dto.LayerName),
            "Rectangle" => new RectangleEntity(
                RequiredPoints(dto.Corners, dto.Type),
                dto.Id,
                dto.LayerName,
                ToDomain(dto.DimensionBindings)),
            "Circle" => new CircleEntity(
                new Circle2D(ToDomain(dto.Center), RequiredDouble(dto.Radius, nameof(dto.Radius))),
                dto.Id,
                dto.LayerName,
                ToDomain(dto.DimensionBindings)),
            "Arc" => new ArcEntity(
                new Arc2D(
                    ToDomain(dto.Center),
                    RequiredDouble(dto.Radius, nameof(dto.Radius)),
                    RequiredDouble(dto.StartAngleRadians, nameof(dto.StartAngleRadians)),
                    RequiredDouble(dto.EndAngleRadians, nameof(dto.EndAngleRadians)),
                    Enum.Parse<ArcDirection>(RequiredString(dto.Direction, nameof(dto.Direction)), ignoreCase: false)),
                dto.Id,
                dto.LayerName),
            "Polyline" => new PolylineEntity(
                new Polyline2D(RequiredPoints(dto.Points, dto.Type), dto.IsClosed),
                dto.Id,
                dto.LayerName),
            "Text" => new TextEntity(
                RequiredString(dto.Text, nameof(dto.Text)),
                ToDomain(dto.Position),
                RequiredDouble(dto.Height, nameof(dto.Height)),
                dto.Id,
                dto.LayerName,
                dto.FontFamily ?? TextFontSource.Default.FamilyName,
                dto.Alignment is null ? TextAlignment.Left : Enum.Parse<TextAlignment>(dto.Alignment, ignoreCase: false),
                dto.FontFilePath),
            _ => throw new NotSupportedException($"Sketch entity type '{dto.Type}' is not supported.")
        };
    }

    private static DimensionBindingDto ToDto(EntityDimensionBinding binding)
    {
        return new DimensionBindingDto
        {
            Dimension = binding.Dimension.ToString(),
            ParameterId = binding.ParameterId.Value
        };
    }

    private static EntityDimensionBinding[] ToDomain(DimensionBindingDto[]? dtos)
    {
        return (dtos ?? Array.Empty<DimensionBindingDto>())
            .Select(dto => new EntityDimensionBinding(
                Enum.Parse<EntityDimensionKind>(dto.Dimension, ignoreCase: false),
                new ParameterId(dto.ParameterId)))
            .ToArray();
    }

    private static PointDto ToDto(Point2D point)
    {
        return new PointDto
        {
            X = point.X,
            Y = point.Y
        };
    }

    private static Point3DDto ToDto(Point3D point)
    {
        return new Point3DDto
        {
            X = point.X,
            Y = point.Y,
            Z = point.Z
        };
    }

    private static Vector3DDto ToDto(Vector3D vector)
    {
        return new Vector3DDto
        {
            X = vector.X,
            Y = vector.Y,
            Z = vector.Z
        };
    }

    private static Point2D ToDomain(PointDto? dto)
    {
        if (dto is null)
        {
            throw new InvalidOperationException("Point DTO is required.");
        }

        return new Point2D(dto.X, dto.Y);
    }

    private static Point3D ToDomain(Point3DDto? dto)
    {
        if (dto is null)
        {
            throw new InvalidOperationException("Point3D DTO is required.");
        }

        return new Point3D(dto.X, dto.Y, dto.Z);
    }

    private static Vector3D ToDomain(Vector3DDto? dto)
    {
        if (dto is null)
        {
            throw new InvalidOperationException("Vector3D DTO is required.");
        }

        return new Vector3D(dto.X, dto.Y, dto.Z);
    }

    private static MaterialProfileDto RequiredMaterialProfile(MaterialProfileDto? dto)
    {
        return dto ?? throw new InvalidOperationException("Material solid requires material profile.");
    }

    private static MeshDto RequiredMesh(MeshDto? dto)
    {
        return dto ?? throw new InvalidOperationException("Material solid requires mesh.");
    }

    private static OrientationDto RequiredOrientation(OrientationDto? dto)
    {
        return dto ?? throw new InvalidOperationException("Material solid requires orientation.");
    }

    private static Point2D[] RequiredPoints(PointDto[]? points, string entityType)
    {
        return points?.Select(ToDomain).ToArray()
            ?? throw new InvalidOperationException($"Entity '{entityType}' requires points.");
    }

    private static double RequiredDouble(double? value, string propertyName)
    {
        return value ?? throw new InvalidOperationException($"Property '{propertyName}' is required.");
    }

    private static string RequiredString(string? value, string propertyName)
    {
        return value ?? throw new InvalidOperationException($"Property '{propertyName}' is required.");
    }

    private sealed class DocumentDto
    {
        public int FormatVersion { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ParameterDto[]? Parameters { get; set; }

        public LayerDto[]? Layers { get; set; }

        public SketchDto[]? Sketches { get; set; }

        public MaterialProfileDto? MaterialProfile { get; set; }

        public MaterialSolidDto[]? MaterialSolids { get; set; }
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

    private sealed class MaterialSolidDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public MaterialProfileDto? MaterialProfile { get; set; }

        public MeshDto? Mesh { get; set; }

        public OrientationDto? Orientation { get; set; }

        public CutoutFeatureDto[]? Cutouts { get; set; }
    }

    private sealed class CutoutFeatureDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Kind { get; set; } = string.Empty;

        public string? FaceName { get; set; }

        public PointDto[]? Contour { get; set; }
    }

    private sealed class MeshDto
    {
        public Point3DDto[]? Vertices { get; set; }

        public int[]? TriangleIndices { get; set; }
    }

    private sealed class OrientationDto
    {
        public Point3DDto? Position { get; set; }

        public double RotationRadians { get; set; }

        public Vector3DDto? SurfaceNormal { get; set; }
    }

    private sealed class SketchDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public EntityDto[]? Entities { get; set; }
    }

    private sealed record EntityDto
    {
        public string Type { get; init; } = string.Empty;

        public Guid Id { get; init; }

        public string LayerName { get; init; } = string.Empty;

        public PointDto? Start { get; init; }

        public PointDto? End { get; init; }

        public PointDto[]? Corners { get; init; }

        public PointDto? Center { get; init; }

        public double? Radius { get; init; }

        public double? StartAngleRadians { get; init; }

        public double? EndAngleRadians { get; init; }

        public string? Direction { get; init; }

        public PointDto[]? Points { get; init; }

        public bool IsClosed { get; init; }

        public string? Text { get; init; }

        public PointDto? Position { get; init; }

        public double? Height { get; init; }

        public string? FontFamily { get; init; }

        public string? FontFilePath { get; init; }

        public string? Alignment { get; init; }

        public DimensionBindingDto[]? DimensionBindings { get; init; }
    }

    private sealed class PointDto
    {
        public double X { get; set; }

        public double Y { get; set; }
    }

    private sealed class Point3DDto
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }
    }

    private sealed class Vector3DDto
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }
    }

    private sealed class DimensionBindingDto
    {
        public string Dimension { get; set; } = string.Empty;

        public string ParameterId { get; set; } = string.Empty;
    }
}
