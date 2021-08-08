using System;
using System.Threading;
using System.Threading.Tasks;

namespace ResponseAutoWrapper.TestHost
{
    public class InheritedTask<T> : Task<T>
    {
        public InheritedTask(Func<T> function) : base(function)
        {
        }

        public InheritedTask(Func<object?, T> function, object? state) : base(function, state)
        {
        }

        public InheritedTask(Func<T> function, CancellationToken cancellationToken) : base(function, cancellationToken)
        {
        }

        public InheritedTask(Func<T> function, TaskCreationOptions creationOptions) : base(function, creationOptions)
        {
        }

        public InheritedTask(Func<object?, T> function, object? state, CancellationToken cancellationToken) : base(function, state, cancellationToken)
        {
        }

        public InheritedTask(Func<object?, T> function, object? state, TaskCreationOptions creationOptions) : base(function, state, creationOptions)
        {
        }

        public InheritedTask(Func<T> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, cancellationToken, creationOptions)
        {
        }

        public InheritedTask(Func<object?, T> function, object? state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, state, cancellationToken, creationOptions)
        {
        }
    }

    public class TwiceInheritedTask : InheritedTask<WeatherForecast[]>
    {
        public TwiceInheritedTask(Func<WeatherForecast[]> function) : base(function)
        {
        }

        public TwiceInheritedTask(Func<object?, WeatherForecast[]> function, object? state) : base(function, state)
        {
        }

        public TwiceInheritedTask(Func<WeatherForecast[]> function, CancellationToken cancellationToken) : base(function, cancellationToken)
        {
        }

        public TwiceInheritedTask(Func<WeatherForecast[]> function, TaskCreationOptions creationOptions) : base(function, creationOptions)
        {
        }

        public TwiceInheritedTask(Func<object?, WeatherForecast[]> function, object? state, CancellationToken cancellationToken) : base(function, state, cancellationToken)
        {
        }

        public TwiceInheritedTask(Func<object?, WeatherForecast[]> function, object? state, TaskCreationOptions creationOptions) : base(function, state, creationOptions)
        {
        }

        public TwiceInheritedTask(Func<WeatherForecast[]> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, cancellationToken, creationOptions)
        {
        }

        public TwiceInheritedTask(Func<object?, WeatherForecast[]> function, object? state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, state, cancellationToken, creationOptions)
        {
        }
    }
}