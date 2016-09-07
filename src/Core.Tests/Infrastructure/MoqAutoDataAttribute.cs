using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;

namespace Core.Tests.Infrastructure
{
    public class MoqAutoDataAttribute : AutoDataAttribute
    {
        public MoqAutoDataAttribute()
          : base(new Fixture().Customize(new Conventions()))
        {
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            if (methodUnderTest == null)
                throw new ArgumentNullException(nameof(methodUnderTest));
            var list = new List<object>();
            foreach (ParameterInfo p in methodUnderTest.GetParameters())
            {
                CustomizeFixture(p);
                var obj = Resolve(p);
                list.Add(obj);
            }
            return new[]
            {
                list.ToArray()
            };
        }

        private void CustomizeFixture(ParameterInfo p)
        {
            foreach (var customizeAttribute in p.GetCustomAttributes(typeof(CustomizeAttribute), false).OfType<CustomizeAttribute>())
                Fixture.Customize(customizeAttribute.GetCustomization(p));
            if (p.ParameterType.BaseType == typeof(Mock))
                Fixture.Customize(new FrozenAttribute().GetCustomization(p));
        }

        private object Resolve(ParameterInfo p)
        {
            var result = new SpecimenContext(Fixture).Resolve(p);
            var mock = result as Mock;

            if (mock != null && mock.GetType().IsGenericType)
            {
                var type = mock.GetType().GetGenericArguments().First();
                var objExpression = Expression.Constant(mock.Object);
                var convertObjExpr = Expression.Convert(objExpression, type);

                var fixtureExpression = Expression.Constant(Fixture);
                var convertFixtureExpression = Expression.Convert(fixtureExpression, typeof(IFixture));

                var methodCallExpression = Expression.Call(typeof(FixtureRegistrar), "Inject", new[] { type }, convertFixtureExpression, convertObjExpr);
                var lambda = Expression.Lambda(methodCallExpression);
                lambda.Compile().DynamicInvoke();
            }

            return result;
        }
    }
}