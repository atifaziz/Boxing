# Boxing

[Boxing is published as a NuGet package on nuget.org][pkg].

Boxing is a .NET Standard Library that implements a generic container,
called `Box<T>`, for storing any type of value and with no restrictions
whatsoever. As a container, it adds no functionality to the type but what
it enables is to use any value in LINQ's query syntax, e.g.:

```c#
Console.WriteLine(
    from n in Box.Return(Console.ReadLine())
    select int.Parse(n)
    into n
    from d in Box.Return(Console.ReadLine())
    select new
    {
        Numerator   = n,
        Denominator = int.Parse(d)
    }
    into op
    select op.Denominator == 0
         ? "Can't divide by zero"
         : $"{op.Numerator / op.Denominator}");
```

Unlike `IEnumerable<T>`, however, `Box<T>` does not have deferred execution
or evaluation semantics. To get the same effect, use `Box.Defer` during
construction and `Apply` later, as shown in the example below:

```c#
var calc =
    Box.Defer((TextReader stdin) =>
        from n in Box.Return(stdin.ReadLine())
        select int.Parse(n)
        into n
        from d in Box.Return(stdin.ReadLine())
        select new
        {
            Numerator = n,
            Denominator = int.Parse(d)
        }
        into op
        select op.Denominator == 0
             ? "Can't divide by zero"
             : $"{op.Numerator / op.Denominator}");

Console.WriteLine(calc.Apply(Console.In));
```


[pkg]: https://www.nuget.org/packages/Boxing/
