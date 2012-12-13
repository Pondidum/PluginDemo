
This is mostly in response to this [StackOverflow Question][2], and partly because I always forget some of the details on using Shadow Copy.

Obviously this would need a lot more error checking etc, but should be a good starting point.

If you are writing a plugin framework for your application, I recommend looking into an existing framework for it such as MEF, rather than rolling your own.

PluginDemo.Common
---

This contains all your common code for plugins - pretty much what you would distribute as your app's SDK.
In this case, we just have one interface called IPlugin, which we use for finding our plugin's types.

TestPluginOne
---

This is your plugin - note the reference to PluginDemo.Common is `CopyLocal = false` - we don't need a copy of it as we have our own version in the host.

PluginDemo.Host
---

This is your main application - note it only has a reference to PluginDemo.Common, not to TestPluginOne.
Points of interest here are:
* Scanner:
    - This must inherit from `MarshalByRefObject`, as it will be marshalled across AppDomains (our application's domain, and the ones we create for our plugins.)
    - You must not polute your main AppDomain with types from the plugins - if a method in `Scanner` returns a `Type` that is not available in your AppDomain, you will get an `FileNotFoundException`, with the name of the assembly which the type came from.  This is what `Scanner.ShowCrossDomainPolutionExceptions` demonstrates.
    - If you are going to have plugins which live for a long time (greater than 5mins, if I remember correctly), then change the `Scanner` to inherit from the classes in this question on [StackOverflow][1], as otherwise you will run into `System.Runtime.Remoting.RemotingException: Object [...] has been disconnected or does not exist at the server.` exceptions.

* AssemblyManager:
    - To prevent file locking on our plugins we use *Shadow Copying*.
    - This will currently leave debris in your temp folder, so don't forget to clear your AppDomain's `CachePath` after unloading it.  You can also set your own `CachePath` to your own location if you prefer.
    - To allow us to communicate across AppDomains, we create an instance of `Scanner` using `AppDomain.CreateInstanceAndUnwrap`.  This creates an instance of the `Scanner` class in the new AppDomain, and returns a proxy to it, which is stored in the current AppDomain (in the `_scanner` field in this case.)  You can see this by setting a break point on the line and inspecting _scanner (it will be a `__TransparentProxy` type.)

* Program:
    - Uses the `AssemblyManager` to load `TestPluginOne`.
    - Calls a method on a type in `TestPluginOne`.
    - Deletes the `TestPluginOne` loaded from.
    - Calls the same method in `TestPluginOne` (showing that we are not using the local copy of the dll).
    - Calls the Cross Domain Pollution test method.

[1]: http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
[2]: http://stackoverflow.com/questions/1031431/system-reflection-assembly-loadfile-locks-file/1031449#1031449