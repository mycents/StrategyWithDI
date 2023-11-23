using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace StrategyMediator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            #region //setup our DI - Config
            var sp = BuildServices();
            var mediator = sp.GetService<IMediator>();
            #endregion

            var result = await mediator.Send(new CalculateTax("TX200", 100m));

            Console.WriteLine($"Step 004 - Resultado do calculo: {result}");
        }

        public static ServiceProvider BuildServices()
        {
            return new ServiceCollection()
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ICalculation).Assembly))
                .AddKeyedTransient<ICalculation, CalculationTX100>("TX100")
                .AddKeyedTransient<ICalculation, CalculationTX200>("TX200")
                .AddKeyedTransient<ICalculation, CalculationTX300>("TX300")
                .AddKeyedTransient<ICalculation, CalculationTX400>("TX400")
                .AddKeyedTransient<ICalculation, CalculationTX400>("TX500")
                .BuildServiceProvider();
        }

    }

    public class CalculateTax : IRequest<decimal>
    {
        public string Code { get; private set; }
        public decimal Value { get; private set; }
        public CalculateTax(string taxCode, decimal expenseValue)
        {
            //codigos de validação. -- Fail Fast
            Code = taxCode;
            Value = expenseValue;
        }
    }

    public class TaxCalculator : IRequestHandler<CalculateTax, decimal>
    {
        private readonly IServiceProvider _ksp;

        public TaxCalculator(IServiceProvider ksp)
        {
            _ksp = ksp;
        }

        public Task<decimal> Handle(CalculateTax request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Step 001 - Get Calculation Tax: {request.Code}");
            var calculation = _ksp.GetRequiredKeyedService<ICalculation>(request.Code);
            return calculation.Execute(request.Value);
        }
    }

    public interface ICalculation
    {
        Task<decimal> Execute(decimal value);
    }
    public class CalculationTX100 : ICalculation
    {
        public CalculationTX100() => Console.WriteLine($"Step 002 - Creating Object TX100");

        public Task<decimal> Execute(decimal value)
        {
            Console.WriteLine($"Step 003 - Calculating Tax(TX100): {value} * 0.5");
            return Task.Run(() => value * 0.35m);
        }
    }
    public class CalculationTX200 : ICalculation
    {
        public CalculationTX200() => Console.WriteLine($"Step 002 - Creating Object TX200");

        public Task<decimal> Execute(decimal value)
        {
            Console.WriteLine($"Step 003 - Calculating Tax(TX200): {value} * 0.5");


            return Task.Run(() => value * 0.5m);
        }
    }
    public class CalculationTX300 : ICalculation
    {
        public CalculationTX300() => Console.WriteLine($"Step 002 - Creating Object TX300");

        public Task<decimal> Execute(decimal value)
        {
            Console.WriteLine($"Step 003 - Calculating Tax(TX300): {value} * 0.75");
            return Task.Run(() => value * 0.75m);
        }
    }
    public class CalculationTX400 : ICalculation
    {
        public CalculationTX400() => Console.WriteLine($"Step 002 - Creating Object TX400");

        public Task<decimal> Execute(decimal value)
        {
            Console.WriteLine($"Step 003 - Calculating Tax(TX400): {value} * 1.1");
            return Task.Run(() => value * 1.1m);
        }
    }
    public class CalculationTX500 : ICalculation
    {
        public CalculationTX500() => Console.WriteLine($"Step 005 - Creating Object TX500");

        public Task<decimal> Execute(decimal value)
        {
            Console.WriteLine($"Sep 005 - Calculating Tax(TX500): {value} * 1.1");

            return Task.Run(() => value * 1.1m);
        }
    }
}
