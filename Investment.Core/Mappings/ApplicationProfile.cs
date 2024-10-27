using AutoMapper;
using Investment.Core.DTOs;
using Investment.Core.Entities;

namespace Investment.Core.Mappings;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<StockLot, StockLotDto>().ReverseMap();
        CreateMap<SaleRecord, SaleRecordDto>()
            .ForMember(dest => dest.TotalSharesSold, opt => 
                opt.MapFrom(src => src.StockLotSales.Any() ? src.StockLotSales.Sum(i => i.SharesSold) : 0))
            .ReverseMap();
        CreateMap<SaleRecord, SaleRecordDetailsDto>()
            .ForMember(dest => dest.SaleRecordDetailItems, opt => opt.MapFrom(src => src.StockLotSales))
            .ForMember(dest => dest.TotalSold, opt => 
                opt.MapFrom(src => src.StockLotSales.Sum(i => i.SharesSold) ))
            .ReverseMap();
        
        CreateMap<StockLotSale, SaleRecordDetailItemDto>()
            .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.StockLot.PurchaseDate))
            .ForMember(dest => dest.PricePerShare, opt => opt.MapFrom(src => src.StockLot.PricePerShare))
            .ReverseMap();

        CreateMap<StockLot, SaleRecordDetailItemDto>().ReverseMap();
    }
}

