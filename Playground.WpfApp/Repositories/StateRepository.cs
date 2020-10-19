using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Repositories
{
    public class States
    {
        public int StateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Cities
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
    }

    public class DataGridState : ValidationPropertyChangedBase, IEditableObject
    {
        private int _stateId;

        public int StateId
        {
            get => _stateId;
            set
            {
                if (_stateId == value)
                {
                    return;
                }

                SetPropertyValue(ref _stateId, value);
            }
        }

        private string _stateName;

        public string StateName
        {
            get => _stateName;
            set
            {
                if (_stateName == value)
                {
                    return;
                }

                SetPropertyValue(ref _stateName, value);
            }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                if (_description == value)
                {
                    return;
                }

                SetPropertyValue(ref _description, value);
            }
        }

        #region IEditableObject implementation

        private DataGridState _backupCopy;
        private bool _inEdit;

        public void BeginEdit()
        {
            if (_inEdit) return;
            _inEdit = true;
            _backupCopy = MemberwiseClone() as DataGridState;
            IsDirty = true;
        }

        public void CancelEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            StateId = _backupCopy.StateId;
            StateName = _backupCopy.StateName;
            Description = _backupCopy.Description;
        }

        public void EndEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            _backupCopy = null;
        }

        #endregion IEditableObject implementation
    }

    public class DataGridCity : ValidationPropertyChangedBase, IEditableObject
    {
        private int _cityId;

        public int CityId
        {
            get => _cityId;
            set
            {
                if (_cityId == value)
                {
                    return;
                }

                SetPropertyValue(ref _cityId, value);
            }
        }

        private string _cityName;

        public string CityName
        {
            get => _cityName;
            set
            {
                if (_cityName == value)
                {
                    return;
                }

                SetPropertyValue(ref _cityName, value);
            }
        }

        private int _stateId;

        public int StateId
        {
            get => _stateId;
            set
            {
                if (_stateId == value)
                {
                    return;
                }

                SetPropertyValue(ref _stateId, value);
            }
        }

        #region IEditableObject implementation

        private DataGridCity _backupCopy;
        private bool _inEdit;

        public void BeginEdit()
        {
            if (_inEdit) return;
            _inEdit = true;
            _backupCopy = MemberwiseClone() as DataGridCity;
            IsDirty = true;
        }

        public void CancelEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            CityId = _backupCopy.CityId;
            CityName = _backupCopy.CityName;
            StateId = _backupCopy.StateId;
        }

        public void EndEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            _backupCopy = null;
        }

        #endregion IEditableObject implementation
    }
    public static class StateRepository
    {
        public static List<States> GetAllStates()
        {
            var allStates = new List<States>();
            allStates.Add(new States { StateId = 1, Name = "Alabama", Description = "South Central" });
            allStates.Add(new States { StateId = 2, Name = "California", Description = "West" });
            allStates.Add(new States { StateId = 3, Name = "Florida", Description = "South East" });
            allStates.Add(new States { StateId = 4, Name = "Texas", Description = "South" });

            return allStates;
        }

        public static List<Cities> GetAllCities()
        {
            var allCities = new List<Cities>();
            //Alabama
            allCities.Add(new Cities { CityId = 101, Name = "Mobile", StateId = 1 });
            allCities.Add(new Cities { CityId = 102, Name = "Montgomery", StateId = 1 });
            allCities.Add(new Cities { CityId = 103, Name = "Troy", StateId = 1 });

            //California
            allCities.Add(new Cities { CityId = 104, Name = "San Francisco", StateId = 2 });
            allCities.Add(new Cities { CityId = 105, Name = "Oakland", StateId = 2 });
            allCities.Add(new Cities { CityId = 106, Name = "San Diego", StateId = 2 });
            allCities.Add(new Cities { CityId = 107, Name = "Hollywood", StateId = 2 });

            //Florida
            allCities.Add(new Cities { CityId = 108, Name = "Pensacola", StateId = 3 });
            allCities.Add(new Cities { CityId = 109, Name = "Miami", StateId = 3 });
            allCities.Add(new Cities { CityId = 110, Name = "TempaBay", StateId = 3 });
            allCities.Add(new Cities { CityId = 111, Name = "Jacksonville", StateId = 3 });

            //Texas
            allCities.Add(new Cities { CityId = 112, Name = "Dallas", StateId = 4 });
            allCities.Add(new Cities { CityId = 113, Name = "Austin", StateId = 4 });
            allCities.Add(new Cities { CityId = 114, Name = "Waco", StateId = 4 });
            allCities.Add(new Cities { CityId = 115, Name = "Houston", StateId = 4 });

            return allCities;
        }

        public static List<States> GetStatesForDataGrid()
        {
            var retVal = new List<States>();

            foreach (var item in GetAllStates())
            {
                if (item.StateId == 3 || item.StateId == 4)
                {
                    retVal.Add(item);
                }
            }

            return retVal;
        }

        public static List<Cities> GetCitiesForState(int stateId)
        {
            return GetAllCities().Where(x => x.StateId == stateId).ToList();
        }
    }
}
