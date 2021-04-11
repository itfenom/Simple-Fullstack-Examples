using Playground.WpfApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Playground.WpfApp.Forms.AsyncEx.LoadDataAsyncAwait
{
    public class ProcessDataAsyncAwait
    {
        private readonly IDemoEmpRepository _repository;

        public event EventHandler<WorkPerformedEventArgs> WorkPerformed;

        public ProcessDataAsyncAwait()
        {
            _repository = new DemoEmpRepository();
        }

        public async Task<string> LoadDataAsync(CancellationToken token)
        {
            var employees = _repository.GetAllEmployees();

            await Task.Run(async () =>
            {
                var totalEmployees = employees.Count();
                var currentEmployeeNumber = 0;
                var currentEmployee = string.Empty;

                foreach (var emp in employees)
                {
                    currentEmployeeNumber++;
                    var isPresident = emp.JobTitle.ToUpper() == "ISPRESIDENT";

                    double percent = (currentEmployeeNumber/totalEmployees) * 100;
                    var percentage = Math.Round(percent, 0);
                    var msg = $"{emp.FullName} - {emp.Salary} - {emp.JobTitle}";
                    OnWorkPerformed(msg, emp.FullName, isPresident, percentage);

                    await Task.Delay(2000);

                    OnWorkPerformed("-----------------------------------------", "", false, percentage);

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }
            });

            return $"{DateTime.Now} - Complete!";
        }

        private void OnWorkPerformed(string msg, string currentEmp, bool isPresident, double percentage)
        {
            WorkPerformed?.Invoke(this, new WorkPerformedEventArgs(msg, currentEmp, isPresident, percentage));
        }
    }

    public class WorkPerformedEventArgs : EventArgs
    {
        public WorkPerformedEventArgs(string msg, string currentEmp, bool isPresident, double percentage)
        {
            Msg = msg;
            CurrentlyProcessingEmployee = currentEmp;
            IsPresident = isPresident;
            Percentage = percentage;
        }

        public string CurrentlyProcessingEmployee { get; }

        public string Msg { get; }

        public bool IsPresident { get; }

        public double Percentage { get; }
    }
}
