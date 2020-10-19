using System;
using AutoMapper;

namespace Playground.ConsoleApp.AutoMapperEx
{
    public class AutoMapperExample
    {
        public void CreateObjectMappingOne()
        {
            ObjectMappingEmployee employee = new ObjectMappingEmployee
            {
                Name = "John SMith",
                Email = "john@codearsenal.net",
                Address = new ObjectMappingAddress
                {
                    Country = "USA",
                    City = "New York",
                    Street = "Wall Street",
                    Number = 7
                },
                Position = "Manager",
                Gender = true,
                Age = 35,
                YearsInCompany = 5,
                StartDate = new DateTime(2007, 11, 2)
            };

            Mapper.CreateMap<ObjectMappingEmployee, ObjectMappingEmployeeViewItem>()
                .ForMember(ev => ev.Address, m => m.MapFrom(a => a.Address.City + ", " +
                                                                 a.Address.Street + " " +
                                                                 a.Address.Number))
                .ForMember(ev => ev.Gender, m => m.ResolveUsing<GenderResolver>().FromMember(e => e.Gender))
                .ForMember(ev => ev.StartDate, m => m.AddFormatter<DateFormatter>());

            ObjectMappingEmployeeViewItem employeeVIewItem = Mapper.Map<ObjectMappingEmployee, ObjectMappingEmployeeViewItem>(employee);
        }

        public void CreateObjectMappingTwo()
        {
            var person = new ObjectMappingPerson();
            person.FirstName = "Kashif";
            person.LastName = "Mubarak";

            Mapper.CreateMap<ObjectMappingPerson, ObjectMappingEmp>()
                .ForMember(dest => dest.LName, opt => opt.MapFrom(src => src.LastName));

            var emp = Mapper.Map<ObjectMappingEmp>(person);
        }
    }

    public class GenderResolver : ValueResolver<bool, string>
    {
        protected override string ResolveCore(bool source)
        {
            return source ? "Man" : "Female";
        }
    }

    public class DateFormatter : IValueFormatter
    {
        public string FormatValue(ResolutionContext context)
        {
            return ((DateTime)context.SourceValue).ToShortDateString();
        }
    }

    public class ObjectMappingEmployee
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public ObjectMappingAddress Address { get; set; }
        public string Position { get; set; }
        public bool Gender { get; set; }
        public int Age { get; set; }
        public int YearsInCompany { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class ObjectMappingAddress
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
    }

    public class ObjectMappingEmployeeViewItem
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public int YearsInCompany { get; set; }
        public string StartDate { get; set; }
    }

    public class ObjectMappingPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ObjectMappingEmp
    {
        public string FirstName { get; set; }
        public string LName { get; set; }
        public int Salary { get; set; }
    }
}
