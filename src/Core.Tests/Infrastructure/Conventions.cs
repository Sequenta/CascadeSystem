using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Core.Tests.Infrastructure
{
    internal class Conventions : CompositeCustomization
    {
        public Conventions() : base(new AutoMoqCustomization(),
                                    new WayPointCustomization(),
                                    new WorkplaceCustomization(),
                                    new StructuralUnitCustomization())
        {
            
        }
    }
}