# email-verification-saas
A Saas solution for email verification in dotNet.Core

Run the following db migration to create the db and tables required
```
dotnet ef migrations add InitialCreate`
```

```
dotnet ef database update
```

Build and run the webapp in a container
```
docker build -t aspnetapp .
```

```
docker run -d -p 8080:80 --name myapp aspnetapp
```
