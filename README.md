# Sportsbet ASP.NET Core

A projekt egy ASP.NET Core 3.1 webalkalmazás, Entity Framework Core 3.1 felhasználásával. A solution megvalósítja a CRUD műveleteket REST API-val, illetve tartalmaz klienst is a hívások kezeléséhez. 3 féle entitás tárolását teszi lehetővé: események, kategóriák és szelvények. Adatbázis szolgáltaltásnak Azure CosmosDB-t használok, hosztolásra pedig Azure Web Service-t, a következő linken érhető el: 
http://dotnethfappservice.azurewebsites.net/api/events

Az API egyes részei (pl. törlés) a felhasználó által igényelhető API kulccsal van védve.
