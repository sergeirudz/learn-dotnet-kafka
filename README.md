## Commands
```shell
dotnet watch run --project Post.Cmd/Post.Cmd.Api --launch-profile http
dotnet watch run --project Post.Query/Post.Query.Api --launch-profile http
```

```sql
--- Dont create user if it already exists
Use
SocialMedia
GO

IF NOT EXISTS(SELECT *
              FROM sys.server_principals
              WHERE name = 'SMUser')
BEGIN
        CREATE
LOGIN SMUser WITH PASSWORD =N'SmPA$$12345', DEFAULT_DATABASE = SocialMedia
END

IF
NOT EXISTS(SELECT *
              FROM sys.database_principals
              WHERE name = 'SMUser')
BEGIN
--- Add user to owner of DB owner
EXEC sp_adduser 'SMUser', 'SMUser', 'db_owner'
END
```

```sh
# 006 Basic Project Setup.mp4 
# Create class lib in CQRS.Core folder
dotnet new classlib -o CQRS.Core
# Create solution
dotnet new sln

# In SM-Post
dotnet new webapi -o Post.Cmd.Api
dotnet new classlib -o Post.Cmd.Domain

# In Post.Query
dotnet new webapi -o Post.Query.Api
dotnet new classlib -o Post.Query.Domain
dotnet new classlib -o Post.Query.Infrastructure

# in SM-Post 
# Add All projects to solution
dotnet sln add ../CQRS-ES/CQRS.Core/CQRS.Core.csproj

dotnet sln add Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj 
dotnet sln add Post.Cmd/Post.Cmd.Domain/Post.Cmd.Domain.csproj
dotnet sln add Post.Cmd/Post.Cmd.Infrastructure/Post.Cmd.Infrastructure.csproj

dotnet sln add Post.Query/Post.Query.Api/Post.Query.Api.csproj
dotnet sln add Post.Query/Post.Query.Domain/Post.Query.Domain.csproj
dotnet sln add Post.Query/Post.Query.Infrastructure/Post.Query.Infrastructure.csproj 

# Add project references
dotnet add Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj reference ../CQRS-ES/CQRS.Core/CQRS.Core.csproj
dotnet add Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj reference Post.Cmd/Post.Cmd.Domain/Post.Cmd.Domain.csproj
dotnet add Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj reference Post.Cmd/Post.Cmd.Infrastructure/Post.Cmd.Infrastructure.csproj

# Add common class Lib
dotnet new classlib -o Post.Common

# Add project references
dotnet add Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj reference Post.Common/Post.Common.csproj
dotnet add Post.Cmd/Post.Cmd.Domain/Post.Cmd.Domain.csproj reference ../CQRS-ES/CQRS.Core/CQRS.Core.csproj 
dotnet add Post.Cmd/Post.Cmd.Domain/Post.Cmd.Domain.csproj reference Post.Common/Post.Common.csproj

dotnet add Post.Cmd/Post.Cmd.Infrastructure/Post.Cmd.Infrastructure.csproj reference ../CQRS-ES/CQRS.Core/CQRS.Core.csproj
dotnet add Post.Cmd/Post.Cmd.Infrastructure/Post.Cmd.Infrastructure.csproj reference Post.Cmd/Post.Cmd.Domain/Post.Cmd.Domain.csproj

dotnet add Post.Common/Post.Common.csproj reference ../CQRS-ES/CQRS.Core/CQRS.Core.csproj 

dotnet add Post.Query/Post.Query.Api/Post.Query.Api.csproj reference ../CQRS-ES/CQRS.Core/CQRS.Core.csproj
dotnet add Post.Query/Post.Query.Api/Post.Query.Api.csproj reference Post.Query/Post.Query.Domain/Post.Query.Domain.csproj
dotnet add Post.Query/Post.Query.Api/Post.Query.Api.csproj reference Post.Query/Post.Query.Infrastructure/Post.Query.Infrastructure.csproj
dotnet add Post.Query/Post.Query.Api/Post.Query.Api.csproj reference Post.Common/Post.Common.csproj

dotnet add Post.Query/Post.Query.Domain/Post.Query.Domain.csproj reference Post.Common/Post.Common.csproj 
dotnet add Post.Query/Post.Query.Domain/Post.Query.Domain.csproj reference ../CQRS-ES/CQRS.Core/CQRS.Core.csproj
dotnet add Post.Query/Post.Query.Domain/Post.Query.Domain.csproj reference Post.Common/Post.Common.csproj 

dotnet add Post.Query/Post.Query.Infrastructure/Post.Query.Infrastructure.csproj reference ../CQRS-ES/CQRS.Core/CQRS.Core.csproj
dotnet add Post.Query/Post.Query.Infrastructure/Post.Query.Infrastructure.csproj reference Post.Query/Post.Query.Domain/Post.Query.Domain.csproj


```