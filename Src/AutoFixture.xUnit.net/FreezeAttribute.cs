using System;
using System.Reflection;
using Ploeh.AutoFixture.Kernel;

namespace Ploeh.AutoFixture.Xunit
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FreezeAttribute : CustomizeAttribute
    {
        private IRequestSpecification matcher;
        private Type targetType;

        public FreezeAttribute()
        {
            this.By = Matching.ExactType;
        }

        public Matching By { get; set; }

        public string TargetName { get; set; }

        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            this.targetType = parameter.ParameterType;

            MatchByType();
            MatchByName();
            return FreezeCustomizationForTargetType();
        }

        private void MatchByType()
        {
            AlwaysMatchByExactType();
            MatchByBaseType();
            MatchByImplementedInterfaces();
        }

        private void MatchByName()
        {
            MatchByPropertyName();
            MatchByParameterName();
            MatchByFieldName();
        }

        private void AlwaysMatchByExactType()
        {
            MatchBy(
                new OrRequestSpecification(
                    new ExactTypeSpecification(targetType),
                    new SeedRequestSpecification(targetType)));
        }

        private void MatchByBaseType()
        {
            if (ShouldMatchBy(Matching.BaseType))
            {
                MatchBy(new BaseTypeSpecification(targetType));
            }
        }

        private void MatchByImplementedInterfaces()
        {
            if (ShouldMatchBy(Matching.ImplementedInterfaces))
            {
                MatchBy(new ImplementedInterfaceSpecification(targetType));
            }
        }

        private void MatchByParameterName()
        {
            if (ShouldMatchBy(Matching.ParameterName))
            {
                MatchBy(new ParameterSpecification(targetType, TargetName));
            }
        }

        private void MatchByPropertyName()
        {
            if (ShouldMatchBy(Matching.PropertyName))
            {
                MatchBy(new PropertySpecification(targetType, TargetName));
            }
        }

        private void MatchByFieldName()
        {
            if (ShouldMatchBy(Matching.FieldName))
            {
                MatchBy(new FieldSpecification(targetType, TargetName));
            }
        }

        private bool ShouldMatchBy(Matching criteria)
        {
            return By.HasFlag(criteria);
        }

        private void MatchBy(IRequestSpecification criteria)
        {
            this.matcher = this.matcher == null
                ? criteria
                : new OrRequestSpecification(this.matcher, criteria);
        }

        private ICustomization FreezeCustomizationForTargetType()
        {
            return new FreezeOnMatchCustomization(targetType, matcher);
        }
    }
}
