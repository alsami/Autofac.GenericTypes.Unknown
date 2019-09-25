using System;
using System.Linq;

namespace Autofac.GenericTypes.Unknown
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var closedGenericTypeInterfaces = typeof(Program)
                .Assembly
                .GetTypes()
                .Where(type => type.IsInterface && type.IsGenericType);

            var containerBuilder = new ContainerBuilder();

            foreach (var closedGenericType in closedGenericTypeInterfaces)
            {
                containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
                    .AsClosedTypesOf(closedGenericType)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }

            var openGenericTypeImplementations = typeof(Program)
                .Assembly
                .GetTypes()
                .Where(type => !type.IsInterface && type.IsGenericType)
                .ToList();

            foreach (var genericType in openGenericTypeImplementations)
            {
                var genericTypeInterface = genericType.GetGenericTypeDefinition().GetInterfaces()
                    .First(@interface => @interface.IsGenericType);

                containerBuilder
                    .RegisterGeneric(genericType)
                    .As(genericTypeInterface)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }

            var container = containerBuilder.Build();

            var closedGenericA = container.Resolve<IClosedTypeGeneric<ClosedTypeGenericA>>();
            var closedGenericB = container.Resolve<IClosedTypeGeneric<ClosedTypeGenericB>>();

            var openGeneric = container.Resolve<IOpenTypeGeneric<object>>();
        }
    }

    public interface IClosedTypeGenericParam { }

    public class ClosedTypeGenericA : IClosedTypeGenericParam { }

    public class ClosedTypeGenericB : IClosedTypeGenericParam { }

    public interface IClosedTypeGeneric<TGeneric> where TGeneric : class, IClosedTypeGenericParam
    {

    }

    public class ClosedTypeA : IClosedTypeGeneric<ClosedTypeGenericA> { }

    public class ClosedTypeB : IClosedTypeGeneric<ClosedTypeGenericB> { }

    public interface IOpenTypeGeneric<TGeneric> { }

    public class OpenTypeGeneric<TGeneric> : IOpenTypeGeneric<TGeneric> { }
}
