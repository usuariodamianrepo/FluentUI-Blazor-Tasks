using AutoMapper;
using BackEnd.API.Data;
using Shared;

namespace BackEnd.Api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Contact, ContactDTO>();
            CreateMap<ContactDTO, Contact>();

            CreateMap<TxskType, TxskTypeDTO>();
            CreateMap<TxskTypeDTO, TxskType>();

            CreateMap<TxskStatus, TxskStatusDTO>();
            CreateMap<TxskStatusDTO, TxskStatus>();

            CreateMap<Txsk, TxskDTO>();
            CreateMap<TxskDTO, Txsk>();

            CreateMap<Txsk, TxskDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.ContactId, opt => opt.MapFrom(src => src.ContactId))
                .ForMember(d => d.ContactName, opt => opt.MapFrom(src => src.Contact.Name))
                .ForMember(d => d.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(d => d.TxskTypeId, opt => opt.MapFrom(src => src.TxskTypeId))
                .ForMember(d => d.TxskTypeName, opt => opt.MapFrom(src => src.TxskType.Name))
                .ForMember(d => d.TxskStatusId, opt => opt.MapFrom(src => src.TxskStatusId))
                .ForMember(d => d.TxskStatusName, opt => opt.MapFrom(src => src.TxskStatus!.Name));
        }
    }
}
