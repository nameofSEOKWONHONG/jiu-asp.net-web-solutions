using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using Infrastructure.Context;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly JIUDbContext dbContext;
        
        public UserService(JIUDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public async Task<IEnumerable<User>> FindAllUserAsync(string searchCol = "", string searchVal = "", int pageIndex = 1, int pageSize = 10)
        {
            if(searchCol.xIsNotEmpty())
            {
                var filter = ExpressionUtils.BuildPredicate<User>(searchCol, "Contains", searchVal);
                return await dbContext.Users
                    .Where(filter)
                    .Include(m => m.Role)
                    .Include(m => m.Role.RolePermission)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            
            return await dbContext.Users
                .Include(m => m.Role)
                .Include(m => m.Role.RolePermission)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User> FindUserByIdAsync(Guid userId)
        {
            return await dbContext.Users
                .Include(m => m.Role)
                .Include(m => m.Role.RolePermission)
                .FirstOrDefaultAsync(m => m.Id == userId);
        }

        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await dbContext.Users
                .Include(m => m.Role)
                .Include(m => m.Role.RolePermission)
                .FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<bool> ExistsSuperUserAsync()
        {
            var exists = await dbContext.Users
                .Include(m => m.Role)
                .Take(1)
                .FirstOrDefaultAsync(m => m.Role.RoleType == ENUM_ROLE_TYPE.SUPER);
            return exists.xIsNotEmpty();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            #region [user check]

            var exists = await FindUserByEmailAsync(user.Email);
            if (exists != null) throw new Exception("already exists.");

            if (user.Role.RoleType == ENUM_ROLE_TYPE.SUPER)
            {
                var superUser = await ExistsSuperUserAsync();
                if (superUser)
                {
                    throw new Exception("Super user is already exists.");
                }
            }

            #endregion

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            if (BCrypt.Net.BCrypt.Verify(user.Password, hashedPassword))
            {
                user.Password = hashedPassword;
            }
            user.Id = Guid.NewGuid();
            user.WriteId = user.Id.ToString();
            user.WriteDt = DateTime.UtcNow;
            var result = await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<User> UpdateUserAsync(User userData)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.Email == userData.Email);
            if (exists == null) throw new Exception("not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userData.Password);
            if (BCrypt.Net.BCrypt.Verify(userData.Password, hashedPassword))
            {
                exists.Password = hashedPassword;
            }

            await dbContext.SaveChangesAsync();
            return exists;
        }

        public async Task<User> RemoveUserAsync(Guid userId, string email)
        {
            var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.Id == userId && m.Email == email);
            if (exists == null) throw new Exception("not found");

            dbContext.Users.Remove(exists);

            await dbContext.SaveChangesAsync();

            return exists;
        }
    }
    
    public static partial class ExpressionUtils
{
    public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
        var body = MakeComparison(left, comparison, value);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression MakeComparison(Expression left, string comparison, string value)
    {
        switch (comparison)
        {
            case "==":
                return MakeBinary(ExpressionType.Equal, left, value);
            case "!=":
                return MakeBinary(ExpressionType.NotEqual, left, value);
            case ">":
                return MakeBinary(ExpressionType.GreaterThan, left, value);
            case ">=":
                return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
            case "<":
                return MakeBinary(ExpressionType.LessThan, left, value);
            case "<=":
                return MakeBinary(ExpressionType.LessThanOrEqual, left, value);
            case "Contains":
            case "StartsWith":
            case "EndsWith":
                return Expression.Call(MakeString(left), comparison, Type.EmptyTypes, Expression.Constant(value, typeof(string)));
            default:
                throw new NotSupportedException($"Invalid comparison operator '{comparison}'.");
        }
    }

    private static Expression MakeString(Expression source)
    {
        return source.Type == typeof(string) ? source : Expression.Call(source, "ToString", Type.EmptyTypes);
    }

    private static Expression MakeBinary(ExpressionType type, Expression left, string value)
    {
        object typedValue = value;
        if (left.Type != typeof(string))
        {
            if (string.IsNullOrEmpty(value))
            {
                typedValue = null;
                if (Nullable.GetUnderlyingType(left.Type) == null)
                    left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
            }
            else
            {
                var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                typedValue = valueType.IsEnum ? Enum.Parse(valueType, value) :
                    valueType == typeof(Guid) ? Guid.Parse(value) :
                    Convert.ChangeType(value, valueType);
            }
        }
        var right = Expression.Constant(typedValue, left.Type);
        return Expression.MakeBinary(type, left, right);
    }
}
    
    public static partial class QueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string propertyName, string comparison, string value)
        {
            return source.Where(ExpressionUtils.BuildPredicate<T>(propertyName, comparison, value));
        }
    }
}