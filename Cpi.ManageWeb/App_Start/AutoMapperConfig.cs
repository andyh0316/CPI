using AutoMapper;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.Base;
using System.Linq;

namespace Cpi.ManageWeb.App_Start
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.CreateMap<CallDm, CallDm>().InheritBase();
                    cfg.CreateMap<CallCommodityDm, CallCommodityDm>().InheritBase();
                    cfg.CreateMap<InvoiceDm, InvoiceDm>().InheritBase();
                    cfg.CreateMap<InvoiceCommodityDm, InvoiceCommodityDm>().InheritBase();
                    cfg.CreateMap<ExpenseDm, ExpenseDm>().InheritBase();
                    cfg.CreateMap<UserDm, UserDm>().InheritBase();
                }
            );
            Mapper.AssertConfigurationIsValid();
        }

        public static IMappingExpression<TSource, TDestination> InheritBase<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression) where TDestination : BaseDm
        {
            expression.ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CreatedById, opt => opt.Ignore())
                .ForMember(m => m.CreatedDate, opt => opt.Ignore())
                .ForMember(m => m.ModifiedById, opt => opt.Ignore())
                .ForMember(m => m.ModifiedDate, opt => opt.Ignore())
                .ForMember(m => m.Deleted, opt => opt.Ignore());

            var desType = typeof(TDestination);
            foreach (var property in desType.GetProperties().Where(p => p.GetGetMethod().IsVirtual))
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }

            return expression;
        }
    }
}