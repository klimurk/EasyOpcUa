using AutoMapper;

namespace OpcUa.Applications.Common.Mappings;

public interface IMapWith<T>
{
	void Mapping(Profile profile);
}
