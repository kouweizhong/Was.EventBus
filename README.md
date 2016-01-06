# Was.EventBus
A simple event bus helper.

## Details
Was.EventBus provides classes for budilding a proxy class that invokes multiple implementation of interfaces.

``` c#
interface IInfo
{
    void SetInfo(string info);
}

var oneInfo = new SomeInfoImplementation();
var twoInfo = new OtherInfoImplementation();

var implementationProvider = new SingleTypeProvider(new[] { oneInfo, twoInfo }, typeof(IInfo));

IInfo info = MultiInterfaceProxy.For(typeof(IInfo), implementationProvider);
info.SetInfo("Some info"); // a call is passed to oneInfo and twoInfo - both are called.

```

# Autofac integration
There is an module for integrating Was.EventBus with autofac:
``` c#
interface IInfo : IEvent
{
     void SetInfo(string info);
}

builder.RegisterModule(new EventBusModule(new[] { Assembly.GetCallingAssembly() })); // assemblies to search types for
builder.RegisterType<SomeInfo>().As<IInfo>();
builder.RegisterType<OtherInfoImpl>().As<IInfo>();

...
container.Resolve<IInfo>().SetInfo("Some info"); 
// implementations are automatically found, resolved are invoked.
```
