using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;

using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using Model.ViewModel;

namespace Service
{
    public interface IUnitConversionService : IDisposable
    {
        UnitConversion Create(UnitConversion pt);
        void Delete(int id);
        void Delete(UnitConversion pt);
        UnitConversion Find(int ptId);
        UnitConversion GetUnitConversion(int ProductId, string FromUnitId, string ToUnitId);
        decimal GetCustomUnitConversionMultiplier(int ProductId, string FromUnitId, int UnitConversionForId, string ToUnitId);
        void Update(UnitConversion pt);
        UnitConversion Add(UnitConversion pt);
        IEnumerable<UnitConversion> GetUnitConversionList();
        IEnumerable<UnitConversionViewModel> GetProductUnitConversions(int id);

        // IEnumerable<UnitConversion> GetUnitConversionList(int buyerId);
        Task<IEquatable<UnitConversion>> GetAsync();
        Task<UnitConversion> FindAsync(int id);
        int NextId(int id, int ProductId);
        int PrevId(int id, int ProductId);

    }

    public class UnitConversionService : IUnitConversionService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<UnitConversion> _UnitConversionRepository;
        RepositoryQuery<UnitConversion> UnitConversionRepository;
        public UnitConversionService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _UnitConversionRepository = new Repository<UnitConversion>(db);
            UnitConversionRepository = new RepositoryQuery<UnitConversion>(_UnitConversionRepository);
        }

        public UnitConversion Find(int pt)
        {
            //return UnitConversionRepository.Include(r => r.SalesOrder).Get().Where(i => i.SalesOrderDetailId == pt).FirstOrDefault();
            return _unitOfWork.Repository<UnitConversion>().Find(pt);
        }


        public UnitConversion GetUnitConversion(int ProductId, string FromUnitId, string ToUnitId)
        {            
            return _unitOfWork.Repository<UnitConversion>().Query()
                                .Get().Where(i => i.ProductId ==ProductId && i.FromUnitId ==FromUnitId && i.ToUnitId == ToUnitId)
                                .FirstOrDefault();
        }

        public UnitConversion GetUnitConversionForUCF(int ProductId, string FromUnitId, string ToUnitId,int UnitConversionForId)
        {
            return _unitOfWork.Repository<UnitConversion>().Query()
                                .Get().Where(i => i.ProductId == ProductId && i.FromUnitId == FromUnitId && i.ToUnitId == ToUnitId && i.UnitConversionForId==UnitConversionForId)
                                .FirstOrDefault();
        }
        public UnitConversion GetUnitConversion(int ProductId, int UnitConversionForId,string ToUnitId)
        {
            try
            {
                return _unitOfWork.Repository<UnitConversion>().Query()
                    .Get().Where(i => i.ProductId == ProductId && i.UnitConversionForId == UnitConversionForId && i.ToUnitId == ToUnitId)
                    .FirstOrDefault();
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public UnitConversion GetUnitConversion(int ProductId, string FromUnitId,int UnitConversionForId, string ToUnitId)
        {
            return _unitOfWork.Repository<UnitConversion>().Query()
                                .Get().Where(i => i.ProductId == ProductId && i.UnitConversionForId == UnitConversionForId && i.ToUnitId == ToUnitId&& i.FromUnitId==FromUnitId)
                                .FirstOrDefault();
        }

        public decimal GetCustomUnitConversionMultiplier(int ProductId, string FromUnitId, int UnitConversionForId, string ToUnitId)
        {
            decimal UnitConversionMultiplier = 0;

            UnitConversionFor UC = (from H in db.UnitConversonFor
                                    where H.UnitconversionForId == UnitConversionForId
                                    select H).FirstOrDefault();


            var Size = (from H in db.ViewRugSize
                      join S in db.Size on H.ManufaturingSizeID equals S.SizeId into STable
                      from STab in STable.DefaultIfEmpty()
                      where H.ProductId == ProductId 
                      select new 
                      {
                          SizeId = STab.SizeId,
                          UnitId = STab.UnitId,
                          Length = STab.Length,
                          LengthFraction = STab.LengthFraction,
                          Width = STab.Width,
                          WidthFraction = STab.WidthFraction,
                      }).FirstOrDefault();

            if (UC.UnitconversionForName== "1L1W Perimeter")
            {
                if(Size.UnitId == "FT")
                UnitConversionMultiplier = Size.Length  + Size.LengthFraction/12 + Size.Width + Size.WidthFraction/12;

            }
            else if (UC.UnitconversionForName == "2L1W Perimeter")
            {
                if (Size.UnitId == "FT")
                    UnitConversionMultiplier = (Size.Length + Size.LengthFraction / 12)*2 + (Size.Width + Size.WidthFraction / 12);

            }
            else if (UC.UnitconversionForName == "1L2W Perimeter")
            {
                if (Size.UnitId == "FT")
                    UnitConversionMultiplier = (Size.Length + Size.LengthFraction / 12)  + (Size.Width + Size.WidthFraction / 12) * 2;

            }

            return UnitConversionMultiplier;
        }


         
        public UnitConversion Create(UnitConversion pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<UnitConversion>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<UnitConversion>().Delete(id);
        }

        public void Delete(UnitConversion pt)
        {
            _unitOfWork.Repository<UnitConversion>().Delete(pt);
        }

        public void Update(UnitConversion pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<UnitConversion>().Update(pt);
        }


        public IEnumerable<UnitConversion> GetUnitConversionList()
        {
            var pt = _unitOfWork.Repository<UnitConversion>().Query().Get();

            return pt;
        }

        public UnitConversion Add(UnitConversion pt)
        {
            _unitOfWork.Repository<UnitConversion>().Insert(pt);
            return pt;
        }

        public IEnumerable<UnitConversionViewModel> GetProductUnitConversions(int id)
        {
            return (from p in db.UnitConversion
                    where p.ProductId == id
                    select new UnitConversionViewModel
                    {
                        ProductId = p.ProductId,
                        FromQty=p.FromQty,
                        FromUnitId = p.FromUnitId,
                        FromUnitName=p.FromUnit.UnitName,
                        ToQty=p.ToQty,
                        ToUnitId = p.ToUnitId,
                        ToUnitName=p.ToUnit.UnitName,
                        UnitConversionForName=p.UnitConversionFor.UnitconversionForName,
                        UnitConversionId=p.UnitConversionId,      
                        UnitConversionForId = p.UnitConversionForId,
                        Description=p.Description,

                    }                    
                    );
        }
        public int NextId(int id, int ProductId)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.UnitConversion
                        where p.ProductId == ProductId
                        orderby p.UnitConversionId
                        select p.UnitConversionId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.UnitConversion
                        where p.ProductId == ProductId
                        orderby p.UnitConversionId
                        select p.UnitConversionId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id, int ProductId)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.UnitConversion
                        where p.ProductId == ProductId
                        orderby p.UnitConversionId
                        select p.UnitConversionId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.UnitConversion
                        where p.ProductId == ProductId
                        orderby p.UnitConversionId
                        select p.UnitConversionId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public void Dispose()
        {
        }


        public Task<IEquatable<UnitConversion>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UnitConversion> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
