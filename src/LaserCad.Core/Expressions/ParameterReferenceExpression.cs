using LaserCad.Core.Parameters;

namespace LaserCad.Core.Expressions;

public sealed class ParameterReferenceExpression : Expression
{
    public ParameterReferenceExpression(ParameterId parameterId)
    {
        ParameterId = parameterId;
    }

    public ParameterId ParameterId { get; }
}
