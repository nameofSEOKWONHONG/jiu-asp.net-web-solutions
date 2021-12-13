[ef core code first migration step]
1. nuget download
   1-1. Microsoft.EntityFrameworkCore  
   1-2. Microsoft.EntityFrameworkCore.SqlServer  
   1-3. Microsoft.EntityFrameworkCore  
   1-4. Microsoft.EntityFrameworkCore.SqlServer       
   1-5. Microsoft.EntityFrameworkCore.Tools  
   1-6. Microsoft.EntityFrameworkCore.Design

2. ef tool setting  
   cmd : dotnet tool install --global dotnet-ef

3. write DbContext and Entity class, refer to AccountDbContext.cs

4. dotnet ef migrations add [script name]  
   or dotnet ef migrations add [script name] --project "[project path]"

5. dotnet ef database update

[ef core code first migration existing database step]
1. same
2. same
3. same
4. same
5. migration cs file edit, Up() or Down()
6. dotnet ef database update

[ef core database first migration step]
1. nuget download
2. ef tool setting
3. dotnet ef dbcontext saffold "[connection strign]" -o Models -f --table "[table name]"