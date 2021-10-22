[ef core migration step]
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

5. dotnet ef database update