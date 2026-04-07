
using BackEnd.API.Data;
using Mapster;
using Shared;

namespace BackEnd.Api.Mappers
{
    public class MapsterMappings : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Txsk, TxskDTO>()
                .Map(d => d.Id, src => src.Id)
                .Map(d => d.Title, src => src.Title)
                .Map(d => d.ContactId, src => src.ContactId)
                .Map(d => d.ContactName, src => src.Contact != null ? src.Contact.Name : null)
                .Map(d => d.DueDate, src => src.DueDate)
                .Map(d => d.TxskTypeId, src => src.TxskTypeId)
                .Map(d => d.TxskTypeName, src => src.TxskType != null ? src.TxskType.Name : null)
                .Map(d => d.TxskStatusId, src => src.TxskStatusId)
                .Map(d => d.TxskStatusName, src => src.TxskStatus != null ? src.TxskStatus.Name : null);
        }
    }
}

