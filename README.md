# EDrouter

### PROJECT CLOSED 
New jet cone boost feature be added into Elite Dangerous in 2.4 update.
##### 24.10.2017 Ivan Zhong

#### Developers
1. EDRouter development environment requires .NET Framework 4.6.2 in order to autoload in Visual Studio 2017. The environment could be downloaded [here](http://getdotnet.azurewebsites.net/target-dotnet-platforms.html)
2. In order to parser and serialize the JSON format files more commonly, we used Newtownsoft.Jsonm, please install it through NuGet package manager.
3. For the developing, we using a MySQL database as a temporary neutron star database which includes all neutron star's coordinate and name (THIS IS NOT UPDATING DATABASE). You can download this database from [here](https://1drv.ms/u/s!ArxUl3tCxGsMiwiI8_EhM-xxeD5a). Please import database and create a new account(Name user, Password 123456789)to allow EDrouter access database.
