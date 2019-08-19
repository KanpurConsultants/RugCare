using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Model.ViewModels;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;

namespace Service
{
    public interface IPersonGodownService : IDisposable
    {
        PersonGodown Create(PersonGodown pt);
        void Delete(int id);
        void Delete(PersonGodown pt);
        //PersonGodown Find(string Name);
        PersonGodown Find(int id);
        IEnumerable<PersonGodown> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(PersonGodown pt);
        PersonGodown Add(PersonGodown pt);
        IEnumerable<PersonGodownViewModel> GetPersonGodownList();
        Task<IEquatable<PersonGodown>> GetAsync();
        Task<PersonGodown> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class PersonGodownService : IPersonGodownService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<PersonGodown> _PersonGodownRepository;
        RepositoryQuery<PersonGodown> PersonGodownRepository;
        public PersonGodownService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _PersonGodownRepository = new Repository<PersonGodown>(db);
            PersonGodownRepository = new RepositoryQuery<PersonGodown>(_PersonGodownRepository);
        }

        //public PersonGodown Find(string Name)
        //{
        //    return PersonGodownRepository.Get().Where(i => i.PersonGodownName == Name).FirstOrDefault();
        //}


        public PersonGodown Find(int id)
        {
            return _unitOfWork.Repository<PersonGodown>().Find(id);
        }

        public PersonGodown Create(PersonGodown pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<PersonGodown>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<PersonGodown>().Delete(id);
        }

        public void Delete(PersonGodown pt)
        {
            _unitOfWork.Repository<PersonGodown>().Delete(pt);
        }

        public void Update(PersonGodown pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<PersonGodown>().Update(pt);
        }

        public IEnumerable<PersonGodown> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<PersonGodown>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.PersonGodownId))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<PersonGodownViewModel> GetPersonGodownList()
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            return (from p in db.PersonGodown
                    orderby p.ProductCategory.ProductCategoryName , p.Godown.GodownName, p.Person.Name 
                    where p.DivisionId == DivisionId && p.SiteId == SiteId
                    select new PersonGodownViewModel
                    {
                        PersonGodownId = p.PersonGodownId,
                        ProductCategoryName = p.ProductCategory.ProductCategoryName,
                        GodownName = p.Godown.GodownName,
                        PersonName = p.Person.Name,
                        GodownCode = p.GodownCode,
                        ModifiedBy = p.ModifiedBy,
                    }).ToList();

        }

        public PersonGodown Add(PersonGodown pt)
        {
            _unitOfWork.Repository<PersonGodown>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.PersonGodown
                        orderby p.PersonGodownId
                        select p.PersonGodownId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.PersonGodown
                        orderby p.PersonGodownId
                        select p.PersonGodownId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.PersonGodown
                        orderby p.PersonGodownId
                        select p.PersonGodownId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.PersonGodown
                        orderby p.PersonGodownId
                        select p.PersonGodownId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<PersonGodown>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PersonGodown> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
