using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyAggregator.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyAggregator.Repository
{
    public interface ICurrencyRepository
    {
        IQueryable<CurrencyItem> GetAll();
        CurrencyItem Add(CurrencyItem toAdd);
        CurrencyItem Update(CurrencyItem toUpdate);
        void Delete(CurrencyItem toDelete);
        int Save();
        void SavePrepared(string key, object data);
        PreparedDataItem GetPrepared(string key);
        Task<PreparedDataItem> GetPreparedAsync(string key);
        void CleanPrepared();
    }

    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CurrencyDbContext _ctx;

        public CurrencyRepository(CurrencyDbContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<CurrencyItem> GetAll()
        {
            return _ctx.Items;
        }

        public CurrencyItem Add(CurrencyItem toAdd)
        {
            _ctx.Items.Add(toAdd);
            return toAdd;
        }

        public CurrencyItem Update(CurrencyItem toUpdate)
        {
            _ctx.Items.Update(toUpdate);
            return toUpdate;
        }

        public void Delete(CurrencyItem toDelete)
        {
            _ctx.Items.Remove(toDelete);
        }

        public int Save()
        {
            return _ctx.SaveChanges();
        }

        public void SavePrepared(string key, object data)
        {
            var old = GetPrepared(key);
            if (old != null)
            {
                old.Data = JsonSerializer.Serialize(data);
                _ctx.PreparedData.Update(old);
            }
            else
            {
                _ctx.PreparedData.Add(new PreparedDataItem(key, JsonSerializer.Serialize(data)));
            }
        }

        public PreparedDataItem GetPrepared(string key) => _ctx.PreparedData.FirstOrDefault(x => x.Key == key);
        public async Task<PreparedDataItem> GetPreparedAsync(string key) => await _ctx.PreparedData.FirstOrDefaultAsync(x => x.Key == key);

        public void CleanPrepared()
        {
            foreach (var s in _ctx.PreparedData)
            {
                _ctx.PreparedData.Remove(s);
            }

            Save();
        }
    }

}
