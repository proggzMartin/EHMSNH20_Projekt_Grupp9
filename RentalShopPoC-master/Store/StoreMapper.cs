using AutoMapper;
using DatabaseConnection;
using Store.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    //source: https://stackoverflow.com/questions/44749727/how-and-where-to-implement-automapper-in-wpf-application
    public static class StoreMapper
    {
        public static IMapper projectMapper;
        static StoreMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Customer, CustomerNameIdDto>();
            });

            projectMapper = config.CreateMapper();
        }
    }
}
