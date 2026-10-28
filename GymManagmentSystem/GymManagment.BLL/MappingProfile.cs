using AutoMapper;
using GymManagment.BLL.ViewModels.MemberViewModel;
using GymManagment.BLL.ViewModels.SessionViewModel;
using GymManagment.DAL.Data.Models;
namespace GymManagment.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapMember();
            MapSession();

        }

        private void MapMember()
        {
            CreateMap<Member, MemberViewModel>()
                   .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString()))
                   .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Adress.BuildingNumber + "-" + src.Adress.Street + "-" + src.Adress.City));
            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();
            CreateMap<Member, MemberToUpdateViewModel>()

                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Adress.BuildingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Adress.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Adress.City));

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())

                .AfterMap((src, dest) =>
                {
                    dest.Adress.BuildingNumber = src.BuildingNumber;
                    dest.Adress.Street = src.Street;
                    dest.Adress.City = src.City;
                });
            //.ForMember(dest => dest.Adress, opt => opt.MapFrom(src => new Adress
            //{
            //    BuildingNumber = src.BuildingNumber,
            //    Street = src.Street,
            //    City = src.City
            //}));


            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.Adress, opt => opt.MapFrom(src => new Adress
                {
                    BuildingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel)
                );


        }
        private void MapSession()
        {
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Session, SessionViewModel>()
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Session, SessionViewModel>()
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<Session, UpdateSessionViewModel>();

        }
    }
}