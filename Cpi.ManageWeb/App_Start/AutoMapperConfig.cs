using AutoMapper;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cpi.ManageWeb.App_Start
{
	public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.CreateMap<CallDm, CallDm>();
                }
            );
            Mapper.AssertConfigurationIsValid();
        }

        private static void RegisterInMappers()
        {
            // entities to entities
            CreateMap<CallDm, CallDm>();
            
        }

        // all inmappers to entities should ignore the following base fields that are assigned by system only
        private static IMappingExpression<T1, T2> CreateMap<T1, T2>() where T2 : BaseDm
        {
            return CreateMap<T1, T2>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CreatedById, opt => opt.Ignore())
                .ForMember(m => m.CreatedDate, opt => opt.Ignore())
                .ForMember(m => m.ModifiedById, opt => opt.Ignore())
                .ForMember(m => m.ModifiedDate, opt => opt.Ignore())
                .ForMember(m => m.Deleted, opt => opt.Ignore());
                //.IgnoreNavigationProperties();
        }

        //public static IMappingExpression<TSource, TDestination> IgnoreNavigationProperties<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        //{
        //    var sourceType = typeof(TSource);

        //    foreach (PropertyInfo property in sourceType.GetProperties())
        //    {
        //        var isNavProp = property.GetCustomAttributes(typeof(NavigationPropertyAttribute), false).Count() == 1;
        //        if (isNavProp)
        //            expression.ForMember(property.Name, opt => opt.Ignore());
        //    }
        //    return expression;
        //}
    }
}