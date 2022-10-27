using Dapper;
using IntranetApi.Enum;
using IntranetApi.Models;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;

namespace IntranetApi.Services
{
    public interface IMemoryCacheService
    {
        List<BaseDropdown> GetRoles();
        List<BaseDropdown> GetBanks();
        List<BaseDropdown> GetDepartments();
        List<BaseDropdown> GetRanks();
        List<BaseDropdown> GetBrands();
        List<CurrencySimpleDto> GetCurrencies();

        List<BaseDropdown> GetRolesDropdown();
        List<BaseDropdown> GetBanksDropdown();
        List<BaseDropdown> GetDepartmentsDropdown();
        List<BaseDropdown> GetRanksDropdown();
        List<BaseDropdown> GetBrandsDropdown();
    }

    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache memoryCache;
        private readonly string sqlConnectionStr;
        private MemoryCacheEntryOptions cacheOptions => 
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.UtcNow.AddHours(24));
 
        public MemoryCacheService(IMemoryCache memoryCache, string sqlConnectionStr)
        {
            this.memoryCache = memoryCache;
            this.sqlConnectionStr = sqlConnectionStr;
        }

        private List<BaseDropdown> GetDataList(string tableName)
        {
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>($"select Id, Name from {tableName}s where IsDeleted = 0")
                             .ToList();
        }

        private List<BaseDropdown> GetActiveDataList(string tableName)
        {
            //Console.WriteLine($"CacheService - Get data dropdown from query {tableName}");
            using var connection = new MySqlConnection(sqlConnectionStr);
            return connection.Query<BaseDropdown>($"select Id, Name from {tableName}s where IsDeleted = 0 and Status = 1")
                             .ToList();
        }

        public List<BaseDropdown> GetRoles()
        {
            List<BaseDropdown> values;
            if (!memoryCache.TryGetValue(CacheKeys.GetRoles, out values))
            {
                values = GetDataList(nameof(Role));
                memoryCache.Set(CacheKeys.GetRoles, values, cacheOptions);
            }
            return values;
        }

        public List<BaseDropdown> GetBanks()
        {
            List<BaseDropdown> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetBanks, out values))
            {
                values = GetDataList(nameof(Bank));
                memoryCache.Set(CacheKeys.GetBanks, values, cacheOptions);
            }
            return values;
        }

        public List<BaseDropdown> GetDepartments()
        {
            List<BaseDropdown> values;
            if (!memoryCache.TryGetValue(CacheKeys.GetDepartments, out values))
            {
                values = GetDataList(nameof(Department));
                memoryCache.Set(CacheKeys.GetDepartments, values, cacheOptions);
            }
            return values;
        }

        public List<BaseDropdown> GetBrands()
        {
            List<BaseDropdown> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetBrands, out values))
            {
                values = GetDataList(nameof(Brand));
                memoryCache.Set(CacheKeys.GetBrands, values, cacheOptions);
            }
            return values;
        }

        public List<BaseDropdown> GetRanks()
        {
            List<BaseDropdown> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetRanks, out values))
            {
                values = GetDataList(nameof(Rank));
                memoryCache.Set(CacheKeys.GetRanks, values, cacheOptions);
            }
            return values;
        }

        public List<CurrencySimpleDto> GetCurrencies()
        {
            List<CurrencySimpleDto> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetCurrencies, out values))
            {
                using var connection = new MySqlConnection(sqlConnectionStr);
                values =  connection.Query<CurrencySimpleDto>($"select Id, Name, CurrencyCode, CurrencySymbol  from Currencies where IsDeleted = 0")
                                 .ToList();
                memoryCache.Set(CacheKeys.GetCurrencies, values, cacheOptions);
            }
            return values;
        }


        #region Dropdown
        public List<BaseDropdown> GetRolesDropdown()
        {
            List<BaseDropdown> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetRolesDropdown, out values))
            {
                values = GetActiveDataList(nameof(Role));
                memoryCache.Set(CacheKeys.GetRolesDropdown, values, cacheOptions);
            }
            return values;
        }

        public List<BaseDropdown> GetBanksDropdown()
        {
            List<BaseDropdown> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetBanksDropdown, out values))
            {
                values = GetActiveDataList(nameof(Bank));
                memoryCache.Set(CacheKeys.GetBanksDropdown, values, cacheOptions);
            }
            return values;
        }

        public List<BaseDropdown> GetDepartmentsDropdown()
        {
            List<BaseDropdown> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetDepartmentsDropdown, out values))
            {
                values = GetActiveDataList(nameof(Department));
                memoryCache.Set(CacheKeys.GetDepartmentsDropdown, values, cacheOptions);
            }
            return values;
        }

        public List<BaseDropdown> GetBrandsDropdown()
        {
            List<BaseDropdown> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetBrandsDropdown, out values))
            {
                values = GetActiveDataList(nameof(Brand));
                memoryCache.Set(CacheKeys.GetBrandsDropdown, values, cacheOptions);
            }
            return values;
        }

        public List<BaseDropdown> GetRanksDropdown()
        {
            List<BaseDropdown> values = null;
            if (!memoryCache.TryGetValue(CacheKeys.GetRanksDropdown, out values))
            {
                values = GetActiveDataList(nameof(Rank));
                memoryCache.Set(CacheKeys.GetRanksDropdown, values, cacheOptions);
            }
            return values;
        }
        #endregion
    }
}