using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playground.Mvc.DAL
{
    public interface IEmployeeRepository : IDisposable
    {
        byte[] GetEmpPhoto(int employeeId);

        IEnumerable<EmployeeViewModel> GetAllEmployees();

        IEnumerable<EmployeeViewModel> GetSampleEmployeeData();

        EmployeeViewModel GetEmployeeById(int employeeId);

        IEnumerable<EmployeeViewModel> GetEmployeesByName(string employeeName);

        IEnumerable<EmployeeViewModel> GetEmployeesByEmail(string employeeEmail);

        bool AddNewEmployee(EmployeeViewModel newEmpModel);

        bool UpdateEmployee(EmployeeViewModel updatedEmpModel);

        bool DeleteEmployee(int empId);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private bool _disposed;
        private readonly SeraphEntities _context;

        public EmployeeRepository(SeraphEntities context)
        {
            _context = context;
        }

        public byte[] GetEmpPhoto(int employeeId)
        {
            byte[] retVal = null;

            var empPhotoId = GetEmpPhotoId(employeeId);

            if (empPhotoId.HasValue)
            {
                var empImg = (from e in _context.EMPLOYEE_PHOTO
                               where e.EMP_PHOTO_ID == empPhotoId
                               select e.PHOTO).FirstOrDefault();
                retVal = empImg;
            }

            return retVal;
        }

        private int? GetEmpPhotoId(int employeeId)
        {
            return
                     (from e in _context.EMPLOYEEs
                      where e.EMP_ID == employeeId && e.EMP_PHOTO_ID != null
                      select e.EMP_PHOTO_ID).FirstOrDefault();
        }

        public IEnumerable<EmployeeViewModel> GetAllEmployees()
        {
            var emp = from e in _context.EMPLOYEEs
                       select new EmployeeViewModel
                       {
                           EmpID = e.EMP_ID,
                           EmpName = e.EMP_NAME,
                           EmpEmail = e.EMP_EMAIL,
                           EmpPhone = e.EMP_PHONE,
                           EmpSalary = e.EMP_SALARY,
                           EmpHireDate = e.EMP_HIRE_DATE,
                           EmpGender = e.EMP_GENDER,
                           EmpIsActive = e.EMP_IS_ACTIVE,
                           EmpPhotoId = e.EMP_PHOTO_ID
                       };

            return emp.ToList();
        }

        public IEnumerable<EmployeeViewModel> GetSampleEmployeeData()
        {
            var emp = from e in _context.EMPLOYEEs
                       where e.EMP_ID <= 10
                       select new EmployeeViewModel
                       {
                           EmpID = e.EMP_ID,
                           EmpName = e.EMP_NAME,
                           EmpEmail = e.EMP_EMAIL,
                           EmpPhone = e.EMP_PHONE,
                           EmpSalary = e.EMP_SALARY,
                           EmpHireDate = e.EMP_HIRE_DATE,
                           EmpGender = e.EMP_GENDER,
                           EmpIsActive = e.EMP_IS_ACTIVE,
                           EmpPhotoId = e.EMP_PHOTO_ID
                       };

            return emp.ToList();
        }

        public EmployeeViewModel GetEmployeeById(int employeeId)
        {
            return GetAllEmployees().FirstOrDefault(e => e.EmpID == employeeId);
        }

        public IEnumerable<EmployeeViewModel> GetEmployeesByName(string employeeName)
        {
            return GetAllEmployees().Where(e => e.EmpName.ToLower().Contains(employeeName.ToLower()));
        }

        public IEnumerable<EmployeeViewModel> GetEmployeesByEmail(string employeeEmail)
        {
            return GetAllEmployees().Where(e => e.EmpEmail.ToLower().Contains(employeeEmail.ToLower()));
        }

        public bool AddNewEmployee(EmployeeViewModel newEmpModel)
        {
            int? empPhotoId = null;

            if (newEmpModel.File != null)
            {
                EMPLOYEE_PHOTO empPhotoModel = new EMPLOYEE_PHOTO();
                var attachedFile = newEmpModel.File;
                var fileSize = attachedFile.ContentLength;

                empPhotoModel.PHOTO = new byte[fileSize];
                newEmpModel.File.InputStream.Read(empPhotoModel.PHOTO, 0, fileSize);

                _context.EMPLOYEE_PHOTO.Add(empPhotoModel);
                _context.SaveChanges();
                empPhotoId = empPhotoModel.EMP_PHOTO_ID;
            }

            EMPLOYEE empModel = new EMPLOYEE
            {
                EMP_ID = 0,
                EMP_NAME = newEmpModel.EmpName,
                EMP_EMAIL = newEmpModel.EmpEmail,
                EMP_PHONE = newEmpModel.EmpPhone,
                EMP_IS_ACTIVE = true,
                EMP_HIRE_DATE = DateTime.Today.Date,
                EMP_GENDER = newEmpModel.EmpGender,
                EMP_SALARY = newEmpModel.EmpSalary,
                EMP_PHOTO_ID = empPhotoId
            };

            _context.EMPLOYEEs.Add(empModel);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateEmployee(EmployeeViewModel updatedEmpModel)
        {
            int? empPhotoId = null;

            if (updatedEmpModel.File != null)
            {
                empPhotoId = GetEmpPhotoId(updatedEmpModel.EmpID);
                if (empPhotoId.HasValue)
                {
                    var origEmpPhotoModel = _context.EMPLOYEE_PHOTO.Find(empPhotoId);
                    var updatedEmpPhotoModel = new EMPLOYEE_PHOTO();
                    var attachedFile = updatedEmpModel.File;
                    var fileSize = attachedFile.ContentLength;

                    updatedEmpPhotoModel.PHOTO = new byte[fileSize];
                    updatedEmpPhotoModel.EMP_PHOTO_ID = (int)empPhotoId;
                    updatedEmpModel.File.InputStream.Read(updatedEmpPhotoModel.PHOTO, 0, fileSize);
                    _context.Entry(origEmpPhotoModel).CurrentValues.SetValues(updatedEmpPhotoModel);
                    _context.SaveChanges();
                }
            }

            var empToUpdate = _context.EMPLOYEEs.Find(updatedEmpModel.EmpID);

            if (empToUpdate != null)
            {
                EMPLOYEE empModel = new EMPLOYEE
                {
                    EMP_ID = updatedEmpModel.EmpID,
                    EMP_NAME = updatedEmpModel.EmpName,
                    EMP_EMAIL = updatedEmpModel.EmpEmail,
                    EMP_PHONE = updatedEmpModel.EmpPhone,
                    EMP_IS_ACTIVE = updatedEmpModel.EmpIsActive,
                    EMP_HIRE_DATE = Convert.ToDateTime(updatedEmpModel.EmpHireDate),
                    EMP_GENDER = updatedEmpModel.EmpGender,
                    EMP_SALARY = updatedEmpModel.EmpSalary,
                    EMP_PHOTO_ID = empPhotoId
                };

                _context.Entry(empToUpdate).CurrentValues.SetValues(empModel);
                _context.SaveChanges();
            }

            return true;
        }

        public bool DeleteEmployee(int empId)
        {
            var empToDelete = _context.EMPLOYEEs.Find(empId);

            if (empToDelete != null)
            {
                var empPhotoId = GetEmpPhotoId(empToDelete.EMP_ID);

                if (empPhotoId.HasValue)
                {
                    var empPhotoModel = _context.EMPLOYEE_PHOTO.Find(empPhotoId);
                    if (empPhotoModel != null)
                    {
                        _context.EMPLOYEE_PHOTO.Remove(empPhotoModel);

                    }
                    _context.SaveChanges();
                }

                _context.EMPLOYEEs.Remove(empToDelete);
                _context.SaveChanges();
            }

            return true;
        }

        #region Disposing

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Disposing
    }
}