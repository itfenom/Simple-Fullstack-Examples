using System.Collections.Generic;
using System.Linq;

namespace Playground.Winforms.Forms.Examples.WorldTreeViewEx
{
    public class WorldTreeViewRepository
    {
        public static List<ContinentObject> GetAllContinents()
        {
            var retVal = new List<ContinentObject>();

            retVal.Add(new ContinentObject { Id = 1001, FieldName = "Africa", ParentId = 0 });
            retVal.Add(new ContinentObject { Id = 1002, FieldName = "Antarctica", ParentId = 0 });
            retVal.Add(new ContinentObject { Id = 1003, FieldName = "Asia", ParentId = 0 });
            retVal.Add(new ContinentObject { Id = 1004, FieldName = "Australia", ParentId = 0 });
            retVal.Add(new ContinentObject { Id = 1005, FieldName = "Europe", ParentId = 0 });
            retVal.Add(new ContinentObject { Id = 1006, FieldName = "North America", ParentId = 0 });
            retVal.Add(new ContinentObject { Id = 1007, FieldName = "South America", ParentId = 0 });

            return retVal;
        }

        public static List<ContinentObject> GetContinentById(int continentId)
        {
            var continents = GetAllContinents();
            var c = from a in continents
                     where a.Id == continentId
                     select a;
            return c.ToList();
        }

        public static List<ContinentObject> GetContinentsByContinentName(string continentName)
        {
            var continents = GetAllContinents();

            var q = from c in continents
                     where c.FieldName.ToLower().Contains(continentName)
                     select c;
            return q.ToList();
        }

        public static List<ContinentObject> GetContinentsByCountryName(string countryName)
        {
            var retVal = new List<ContinentObject>();

            var continentId = (from c in GetAllCountries()
                                where c.FieldName.ToLower().Contains(countryName)
                                select c.ParentId).FirstOrDefault();
            if (continentId > 0)
            {
                retVal = GetContinentById(continentId);
            }

            return retVal;
        }

        public static List<ContinentObject> GetContinentsByStateName(string stateName)
        {
            var retVal = new List<ContinentObject>();

            var countryId = (from c in GetAllStates()
                              where c.FieldName.ToLower().Contains(stateName)
                              select c.ParentId).FirstOrDefault();
            if (countryId > 0)
            {
                var continentId = (from c in GetAllCountries()
                                    where c.Id == countryId
                                    select c.ParentId).FirstOrDefault();
                if (continentId > 0)
                {
                    retVal = GetContinentById(continentId);
                }
            }

            return retVal;
        }

        public static List<ContinentObject> GetContinentsByCityName(string cityName)
        {
            var retVal = new List<ContinentObject>();

            var stateId = (from s in GetAllCities()
                            where s.FieldName.ToLower().Contains(cityName)
                            select s.ParentId).FirstOrDefault();
            if (stateId > 0)
            {
                var countryId = (from c in GetAllStates()
                                  where c.Id == stateId
                                  select c.ParentId).FirstOrDefault();
                if (countryId > 0)
                {
                    var continentId = (from c in GetAllCountries()
                                        where c.Id == countryId
                                        select c.ParentId).FirstOrDefault();
                    if (continentId > 0)
                    {
                        retVal = GetContinentById(continentId);
                    }
                }
            }

            return retVal;
        }

        public static List<CountryObject> GetAllCountries()
        {
            var retVal = new List<CountryObject>();

            //African Countries 1001 
            retVal.Add(new CountryObject { Id = 2001, FieldName = "Egypt", ParentId = 1001 });
            retVal.Add(new CountryObject { Id = 2002, FieldName = "Sudan", ParentId = 1001 });

            //Antarctica Countries 1002 -- NONE!

            //Asian Countries 1003
            retVal.Add(new CountryObject { Id = 2003, FieldName = "Pakistan", ParentId = 1003 });
            retVal.Add(new CountryObject { Id = 2004, FieldName = "India", ParentId = 1003 });

            //Australian Countries 1004
            retVal.Add(new CountryObject { Id = 2005, FieldName = "Australia", ParentId = 1004 });
            retVal.Add(new CountryObject { Id = 2006, FieldName = "New Zealand", ParentId = 1004 });

            //European Countries 1005
            retVal.Add(new CountryObject { Id = 2007, FieldName = "France", ParentId = 1005 });
            retVal.Add(new CountryObject { Id = 2008, FieldName = "Germany", ParentId = 1005 });

            //North America 1006
            retVal.Add(new CountryObject { Id = 2009, FieldName = "Canada", ParentId = 1006 });
            retVal.Add(new CountryObject { Id = 2010, FieldName = "USA", ParentId = 1006 });

            //South American 1007
            retVal.Add(new CountryObject { Id = 2011, FieldName = "Brazil", ParentId = 1007 });
            retVal.Add(new CountryObject { Id = 2012, FieldName = "Mexico", ParentId = 1007 });

            return retVal;
        }

        public static List<CountryObject> GetCountriesByContinentId(int continentId)
        {
            return GetAllCountries().Where(str => str.ParentId == continentId).ToList();
        }

        public static List<StateObject> GetAllStates()
        {
            var retVal = new List<StateObject>();

            //Pakistan States
            retVal.Add(new StateObject { Id = 3001, FieldName = "Punjab", ParentId = 2003 });
            retVal.Add(new StateObject { Id = 3002, FieldName = "Sindh", ParentId = 2003 });

            //USA States
            retVal.Add(new StateObject { Id = 3003, FieldName = "Alabama", ParentId = 2010 });
            retVal.Add(new StateObject { Id = 3004, FieldName = "Illinois", ParentId = 2010 });
            retVal.Add(new StateObject { Id = 3005, FieldName = "Ohio", ParentId = 2010 });
            retVal.Add(new StateObject { Id = 3006, FieldName = "Texas", ParentId = 2010 });

            return retVal;
        }

        public static List<StateObject> GetStatesByCountryId(int countryId)
        {
            return GetAllStates().Where(str => str.ParentId == countryId).ToList();
        }

        public static List<CityObject> GetAllCities()
        {
            var retVal = new List<CityObject>();

            //Punjab Cities
            retVal.Add(new CityObject { Id = 4001, FieldName = "Faisalabad", ParentId = 3001 });
            retVal.Add(new CityObject { Id = 4002, FieldName = "Lahore", ParentId = 3001 });

            //Texas cities
            retVal.Add(new CityObject { Id = 4003, FieldName = "Dallas", ParentId = 3006 });
            retVal.Add(new CityObject { Id = 4004, FieldName = "Houston", ParentId = 3006 });
            retVal.Add(new CityObject { Id = 4005, FieldName = "San Antonio", ParentId = 3006 });
            retVal.Add(new CityObject { Id = 4006, FieldName = "Austin", ParentId = 3006 });

            return retVal;
        }

        public static List<CityObject> GetCitiesByStateId(int stateId)
        {
            return GetAllCities().Where(str => str.ParentId == stateId).ToList();
        }
    }
}
