using Domain;

namespace Core
{
    public interface IStructuralUnitService
    {
        Workplace GetHeadOfLevel(StructuralUnit  structuralUnit, int headLevel);
        StructuralUnit GetForWorkplace(Workplace workplace);
    }
}