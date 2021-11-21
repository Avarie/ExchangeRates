using System.Collections.Generic;
using System.Data;
using System.Linq;
using CurrencyAggregator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyAggregator.Repository
{
    public interface IVisitorRepository
    {
        IQueryable<VisitorDataItem> GetAll();
        VisitorDataItem Add(VisitorDataItem toAdd);
        VisitorDataItem Update(VisitorDataItem toUpdate);
        int Save();
        VisitorDataItem Find(Dictionary<string, FingerPrintData> fingers);
        VisitorDataItem CreateOrUpdate(VisitorDataItem item, Dictionary<string, FingerPrintData> fingers, string ip);

        IQueryable<FingerPrintData> GetFingerPrints(VisitorDataItem item);
        void DeleteFingerPrints(VisitorDataItem item);
    }

    public class VisitorRepository : IVisitorRepository
    {
        private readonly CurrencyDbContext _ctx;

        public VisitorRepository(CurrencyDbContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<VisitorDataItem> GetAll()
        {
            return _ctx.VisitorStatistics;
        }
        public IQueryable<FingerPrintData> GetAllFingerPrints()
        {
            return _ctx.FingerPrintData;
        }
        public IQueryable<FingerPrintData> GetFingerPrints(VisitorDataItem item)
        {
            return _ctx.FingerPrintData.Where(x => x.VisitorDataItem.Id == item.Id);
        }
        public void DeleteFingerPrints(VisitorDataItem item)
        {
            var prints =  _ctx.FingerPrintData.Where(x => x.VisitorDataItem.Id == item.Id);
            foreach (var fingerPrintData in prints)
            {
                _ctx.FingerPrintData.RemoveRange(prints);
            }

            _ctx.SaveChanges();


            var test = GetFingerPrints(item).ToList();
        }

        public VisitorDataItem Add(VisitorDataItem toAdd)
        {
            _ctx.VisitorStatistics.Add(toAdd);
            return toAdd;
        }

        public VisitorDataItem Update(VisitorDataItem toUpdate)
        {
            _ctx.VisitorStatistics.Update(toUpdate);

            var parent = _ctx.VisitorStatistics
                .Where(p => p.Id == toUpdate.Id)
                .Include(p => p.FingerPrints)
                .FirstOrDefault();

            if (parent != null)
            {
                foreach (var print in parent.FingerPrints)
                {
                    _ctx.Entry(print).State = EntityState.Modified;
                }

                _ctx.Entry(parent).State = EntityState.Modified;

                _ctx.SaveChanges();
            }

            return toUpdate;
        }

        public int Save()
        {
            return _ctx.SaveChanges();
        }

        public VisitorDataItem Find(Dictionary<string, FingerPrintData> fingers)
        {
            var all = GetAllFingerPrints().ToList();
            var all2 = GetAllFingerPrints().FirstOrDefault(x => fingers.Keys.Contains(x.Id));
            var id = GetAllFingerPrints().FirstOrDefault(x => fingers.Keys.Contains(x.Id))?.VisitorDataItemId;

            return string.IsNullOrEmpty(id) ? null : Find(id);
        }
        public VisitorDataItem Find(string id)
        {
            return _ctx.VisitorStatistics.Find(id);
        }

        public VisitorDataItem CreateOrUpdate(VisitorDataItem item, Dictionary<string, FingerPrintData> fingers, string ip)
        {
            foreach (var finger in fingers)
            {
                finger.Value.Id = finger.Key;
            }

            var prints = fingers.Select(x => x.Value).ToList();

            if (item == null)
            {
                item = new VisitorDataItem
                {
                    Created = prints.Min(x => x.i),
                };

                foreach (var data in prints)
                {
                    item.FingerPrints.Add(data);
                }

                Add(item);
            }
            else
            {
                foreach (var data in prints)
                {
                    var existPrint = item.FingerPrints.FirstOrDefault(x => x.Id == data.Id);
                    if (existPrint != null)
                    {
                        if (data.c > existPrint.c) existPrint.c = data.c;
                        existPrint.d = data.d;
                    }
                    else
                    {
                        item.FingerPrints.Add(data);
                    }
                }
            }

            item.Ip = ip;

            item.ActualizeData();

            return item;

        }
        public VisitorDataItem UpdateStats(VisitorDataItem item, Dictionary<string, FingerPrintData> fingers)
        {
            var prints = fingers.Select(x => x.Value).ToList();

            return new VisitorDataItem
            {
                Created = prints.Min(x => x.i),
                LastVisit = prints.Max(x => x.d),
                Count = prints.Sum(x => x.c),
                FingerPrints = fingers.Select(x =>
                        new FingerPrintData() {Id = x.Key, c = x.Value.c, d = x.Value.d, i = x.Value.i})
                    .ToList()
            };
        }
    }
}