using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Playground.Mvc.Models;

namespace Playground.Mvc.Controllers
{
    public class DiffController : Controller
    {
        public ActionResult Index(int id = 101)
        {
            const int parentSetId = 101;
            var viewModel = new DiffViewModel();
            var childrenSet = new List<int>() { 102, 103, 104 };
            var types = new List<string>() { "TECH", "ENGINEERING" /*, "OTHERS"*/ };
            var diffContent = new Dictionary<string, DiffContent>();
            var childSetId = 0;

            if (id == 101)
            {
                viewModel.ParentSetId = id;
                viewModel.SelectedSetId = id;
                viewModel.ChildrenSetIds = childrenSet;
            }
            else
            {
                childSetId = id;
                viewModel.ParentSetId = 101;
                viewModel.SelectedSetId = id;
                viewModel.ChildrenSetIds = childrenSet;

                if (!childrenSet.Contains(childSetId))
                {
                    ViewData["ID"] = childSetId;
                    return View("DiffNotFound");
                }
            }

            var mainStudentSet = GetMainStudentSet();
            var parentStudentSet = GetParentStudentSet();
            var childStudentSet = GetChildStudentSet(childSetId);

            foreach (var type in types)
            {
                // ReSharper disable once RedundantAssignment
                var mainDiff = new List<StudentSet>();
                // ReSharper disable once RedundantAssignment
                var parentDiff = new List<StudentSet>();
                var childDiff = new List<StudentSet>();

                // ReSharper disable once IdentifierTypo
                var mainfilteredStudents = (from x in mainStudentSet where x.Type == type select x).ToList();
                // ReSharper disable once IdentifierTypo
                var parentfilteredStudents = (from x in parentStudentSet where x.Type == type select x).ToList();
                // ReSharper disable once IdentifierTypo
                var childfilteredStudents = (from x in childStudentSet where x.Type == type select x).ToList();
                var recordsInChildSet = (from x in childfilteredStudents where x.Type == type select x).ToList();

                var item = new DiffContent {Type = type};

                if (string.IsNullOrEmpty(parentfilteredStudents[0].ImpactStatus))
                {
                    item.ParentImpactStatus = string.Empty;
                }
                else
                {
                    item.ParentImpactStatus = $"'{item.Type}' in Parent-Set {parentSetId} is {parentfilteredStudents[0].ImpactStatus}";
                }

                if (recordsInChildSet.Count > 0)
                {
                    if (string.IsNullOrEmpty(recordsInChildSet[0].ImpactStatus))
                    {
                        item.ChildImpactStatus = string.Empty;
                    }
                    else
                    {
                        item.ChildImpactStatus = $"'{item.Type}' in Child-Set {childSetId} is {recordsInChildSet[0].ImpactStatus}";
                    }
                }

                item.SelectedChildSetId = childSetId;

                if (childSetId == 0 || recordsInChildSet.Count == 0 || recordsInChildSet[0].ImpactStatus == "SKIPPED")
                {
                    GetMainAndParentDiff(mainfilteredStudents, parentfilteredStudents, out mainDiff, out parentDiff);
                    item.HtmlTableString = GetDiffHtmlString(mainDiff, parentDiff, childDiff, item.Type, parentSetId, item.ParentImpactStatus, 0, "");
                    diffContent.Add(type, item);
                    continue;
                }

                GetMainParentAndChildDiff(mainfilteredStudents, parentfilteredStudents, childfilteredStudents, out mainDiff, out parentDiff, out childDiff);

                item.HtmlTableString = GetDiffHtmlString(mainDiff, parentDiff, childDiff, item.Type, parentSetId, item.ParentImpactStatus, childSetId, item.ChildImpactStatus);

                diffContent.Add(type, item);
            }

            viewModel.DiffContent = diffContent;

            return View(viewModel);
        }

        private List<StudentSet> GetMainStudentSet()
        {
            var retVal = new List<StudentSet>();

            //TECH
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student1", ImpactStatus = "", Comments = "C#", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student2", ImpactStatus = "", Comments = "C", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student3", ImpactStatus = "", Comments = "C++", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student4", ImpactStatus = "", Comments = "Java", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student5", ImpactStatus = "", Comments = "Javascript", ApplyDifferentStyles = false });

            //ENGINEERING
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student6", ImpactStatus = "", Comments = "Civil", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student7", ImpactStatus = "", Comments = "Electrical", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student8", ImpactStatus = "", Comments = "Mechanical", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student9", ImpactStatus = "", Comments = "Chemical", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student10", ImpactStatus = "", Comments = "Aerospace", ApplyDifferentStyles = false });

            //OTHERS
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student11", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student12", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student13", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student14", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student15", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });

            return retVal;
        }

        private List<StudentSet> GetParentStudentSet()
        {
            var retVal = new List<StudentSet>();

            //TECH
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student1", ImpactStatus = "MODIFIED", Comments = "C#", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student2", ImpactStatus = "MODIFIED", Comments = "C", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student2.1", ImpactStatus = "MODIFIED", Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student3", ImpactStatus = "MODIFIED", Comments = "C++", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student4", ImpactStatus = "MODIFIED", Comments = "Java", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student4.1", ImpactStatus = "MODIFIED", Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student4.2", ImpactStatus = "MODIFIED", Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student4.3", ImpactStatus = "MODIFIED", Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student5", ImpactStatus = "MODIFIED", Comments = "Javascript", ApplyDifferentStyles = false });

            //ENGINEERING
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student6", ImpactStatus = "MODIFIED", Comments = "Civil", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student7", ImpactStatus = "MODIFIED", Comments = "Electrical", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student10", ImpactStatus = "MODIFIED", Comments = "Aerospace", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student7.1", ImpactStatus = "MODIFIED", Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student8", ImpactStatus = "MODIFIED", Comments = "Mechanical", ApplyDifferentStyles = false });
            retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student9", ImpactStatus = "MODIFIED", Comments = "Chemical", ApplyDifferentStyles = false });

            ////OTHERS
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student11", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student12", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student13", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student14", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });
            //_retVal.Add(new StudentSet { Type = "OTHERS", StudentName = "Student15", ImpactStatus = "", Comments = "", ApplyDifferentStyles = false });

            return retVal;
        }

        private List<StudentSet> GetChildStudentSet(int id)
        {
            var retVal = new List<StudentSet>();

            if (id == 102)
            {
                //TECH
                retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student1", ImpactStatus = "MODIFIED", Comments = "C#", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student2", ImpactStatus = "MODIFIED", Comments = "C", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student2.1", ImpactStatus = "MODIFIED", Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student3", ImpactStatus = "MODIFIED", Comments = "C++", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student4", ImpactStatus = "MODIFIED", Comments = "Java", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "TECH", StudentName = "Student5", ImpactStatus = "MODIFIED", Comments = "Javascript", ApplyDifferentStyles = false });
            }
            else if (id == 103)
            {
                //TECH
                retVal.Add(new StudentSet { Type = "TECH", StudentName = "", ImpactStatus = "SKIPPED", Comments = "", ApplyDifferentStyles = true });

                //ENGINEERING
                retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student6", ImpactStatus = "MODIFIED", Comments = "Civil", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student7", ImpactStatus = "MODIFIED", Comments = "Electrical", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student7.1", ImpactStatus = "MODIFIED", Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student8", ImpactStatus = "MODIFIED", Comments = "Mechanical", ApplyDifferentStyles = false });
                retVal.Add(new StudentSet { Type = "ENGINEERING", StudentName = "Student10", ImpactStatus = "MODIFIED", Comments = "Chemical", ApplyDifferentStyles = false });
            }
            else if (id == 104)
            {
            }

            return retVal;
        }

        private void GetMainAndParentDiff(List<StudentSet> list1, List<StudentSet> list2, out List<StudentSet> outputList1, out List<StudentSet> outputList2)
        {
            var blankItem = new StudentSet { Type = "--", StudentName = "--", ImpactStatus = "--", Comments = "--", ApplyDifferentStyles = true };
            // ReSharper disable once InconsistentNaming
            var _outputList1 = new List<StudentSet>();
            // ReSharper disable once InconsistentNaming
            var _outputList2 = new List<StudentSet>();
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once TooWideLocalVariableScope
            StudentSet _itemToAdd;

            var diffResult = DiffMethods.Diff(list1, list2);
            foreach (var row in diffResult)
            {
                // ReSharper disable once RedundantAssignment
                _itemToAdd = null;

                if (row.SourceItemStatus == DiffStatus.Unchanged && row.DestinationItemStatus == DiffStatus.Unchanged)
                {
                    _itemToAdd = (from r in list1 where r.StudentName == row.SourceItem.StudentName select r).FirstOrDefault();
                    _outputList1.Add(_itemToAdd);

                    _itemToAdd = (from r in list2 where r.StudentName == row.DestinationItem.StudentName select r).FirstOrDefault();
                    _outputList2.Add(_itemToAdd);

                    continue;
                }

                if (row.SourceItem != null && row.DestinationItem == null)
                {
                    _itemToAdd = (from r in list1 where r.StudentName == row.SourceItem.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList1.Add(_itemToAdd);
                    _outputList2.Add(blankItem);
                }
                else if (row.SourceItem == null && row.DestinationItem != null)
                {
                    _outputList1.Add(blankItem);

                    _itemToAdd = (from r in list2 where r.StudentName == row.DestinationItem.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList2.Add(_itemToAdd);
                }
            }

            outputList1 = _outputList1;
            outputList2 = _outputList2;
        }

        private void GetMainParentAndChildDiff(List<StudentSet> list1, List<StudentSet> list2, List<StudentSet> list3, out List<StudentSet> outputList1, out List<StudentSet> outputList2, out List<StudentSet> outputList3)
        {
            var blankItem = new StudentSet { Type = "--", StudentName = "--", ImpactStatus = "--", Comments = "--", ApplyDifferentStyles = true };
            // ReSharper disable once InconsistentNaming
            var _outputList1 = new List<StudentSet>();
            // ReSharper disable once InconsistentNaming
            var _outputList2 = new List<StudentSet>();
            // ReSharper disable once InconsistentNaming
            var _outputList3 = new List<StudentSet>();
            // ReSharper disable once InconsistentNaming
            StudentSet _itemToAdd;

            var diffResult = DiffMethods.Diff(list1, list2, list3);

            foreach (var row in diffResult)
            {
                // ReSharper disable once RedundantAssignment
                _itemToAdd = null;

                if (row.SourceItemStatus == DiffStatus.Unchanged && row.DestinationItem1Status == DiffStatus.Unchanged && row.DestinationItem2Status == DiffStatus.Unchanged)
                {
                    _itemToAdd = (from r in list1 where r.StudentName == row.SourceItem.StudentName select r).FirstOrDefault();
                    _outputList1.Add(_itemToAdd);

                    _itemToAdd = (from r in list2 where r.StudentName == row.DestinationItem1.StudentName select r).FirstOrDefault();
                    _outputList2.Add(_itemToAdd);

                    _itemToAdd = (from r in list3 where r.StudentName == row.DestinationItem2.StudentName select r).FirstOrDefault();
                    _outputList3.Add(_itemToAdd);

                    continue;
                }

                if (row.SourceItem != null && row.DestinationItem1 != null && row.DestinationItem2 == null)
                {
                    _itemToAdd = (from r in list1 where r.StudentName == row.SourceItem.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList1.Add(_itemToAdd);

                    _itemToAdd = (from r in list2 where r.StudentName == row.DestinationItem1.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList2.Add(_itemToAdd);

                    _outputList3.Add(blankItem);
                }
                else if (row.SourceItem != null && row.DestinationItem1 == null && row.DestinationItem2 == null)
                {
                    _itemToAdd = (from r in list1 where r.StudentName == row.SourceItem.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList1.Add(_itemToAdd);

                    _outputList2.Add(blankItem);

                    _outputList3.Add(blankItem);
                }
                else if (row.SourceItem == null && row.DestinationItem1 != null && row.DestinationItem2 != null)
                {
                    _outputList1.Add(blankItem);

                    _itemToAdd = (from r in list2 where r.StudentName == row.DestinationItem1.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList2.Add(_itemToAdd);

                    _itemToAdd = (from r in list3 where r.StudentName == row.DestinationItem2.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList3.Add(_itemToAdd);
                }
                else if (row.SourceItem == null && row.DestinationItem1 == null && row.DestinationItem2 != null)
                {
                    _outputList1.Add(blankItem);

                    _outputList2.Add(blankItem);

                    _itemToAdd = (from r in list3 where r.StudentName == row.DestinationItem2.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList3.Add(_itemToAdd);
                }
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                else if (row.SourceItem == null && row.DestinationItem1 != null && row.DestinationItem2 == null)
                {
                    _outputList1.Add(blankItem);

                    _itemToAdd = (from r in list2 where r.StudentName == row.DestinationItem1.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList2.Add(_itemToAdd);

                    _outputList3.Add(blankItem);
                }
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                else if (row.SourceItem != null && row.DestinationItem1 == null && row.DestinationItem2 != null)
                {
                    _itemToAdd = (from r in list1 where r.StudentName == row.SourceItem.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList1.Add(_itemToAdd);

                    _outputList2.Add(blankItem);

                    _itemToAdd = (from r in list3 where r.StudentName == row.DestinationItem2.StudentName select r).FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    _itemToAdd.ApplyDifferentStyles = true;
                    _outputList3.Add(_itemToAdd);
                }
            }

            outputList1 = _outputList1;
            outputList2 = _outputList2;
            outputList3 = _outputList3;
        }

        private string GetDiffHtmlString(List<StudentSet> mainDiff, List<StudentSet> parentDiff, List<StudentSet> childDiff, string type, int parentSetId, string parentImpactStatus, int childSetId, string childImpactStatus)
        {
            var retVal = new StringBuilder();
            var diffStyle = @"style ='font-weight:bold;color:red';";
            var diffStyleChild = @"style ='font-weight:bold;color:#c71585';";
            var blankItem = new StudentSet { Type = "--", StudentName = "--", ImpactStatus = "--", Comments = "--", ApplyDifferentStyles = true };
            // ReSharper disable once RedundantAssignment
            var mainItem = new StudentSet();
            // ReSharper disable once RedundantAssignment
            var parentItem = new StudentSet();
            // ReSharper disable once RedundantAssignment
            var childItem = new StudentSet();
            // ReSharper disable once TooWideLocalVariableScope
            int counter;
            // ReSharper disable once RedundantAssignment
            var htmlContent = string.Empty;

            if (mainDiff.Count > 0 && parentDiff.Count > 0 && childDiff.Count == 0)
            {
                retVal.Append("<span style='display: inline - block; vertical - align: top'>");
                retVal.Append("<table class='aTable'><tr><td'></td></tr><tr><table>"
                              + "<tr><th class='ath'>Student Name</th><th class='ath'>Comments</th>"
                              + "<th class='ath'>---</th>"
                              + "<th class='ath'>Student Name</th><th class='ath'>Comments</th>"
                              + "</tr>");

                for (int i = 0; i < mainDiff.Count; i++)
                {
                    mainItem = mainDiff[i];
                    parentItem = parentDiff[i];
                    htmlContent = string.Empty;

                    if (mainItem.StudentName.Trim() == "--" && parentItem.StudentName != "--")
                    {
                        htmlContent =
                            "<tr>"
                            + "<td class='atd'" + diffStyle + ">--</td><td class='atd' " + diffStyle + ">--</td><td class='atd' " + diffStyle + "></td>"
                            + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + parentItem.StudentName + "</td>"
                            + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + (string.IsNullOrEmpty(parentItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + parentItem.Comments + "' >" + parentItem.Comments + "</textarea>") + "</td>"
                            + "</tr>";
                    }
                    else if (mainItem.StudentName.Trim() == "--" && parentItem.StudentName == "--")
                    {
                        htmlContent =
                            "<tr " + diffStyle + " >"
                          + "<td class='atd'>--</td><td class='atd'>--</td><td class='atd'></td>"

                          + "<td class='atd'>--</td>"
                          + "<td class='atd'>--</td>"

                          + "</tr>";
                    }
                    else if (mainItem.StudentName.Trim() != "--" && parentItem.StudentName == "--")
                    {
                        htmlContent =
                            "<tr>"
                          + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.StudentName + "</td>"
                          + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.Comments + "</td>"

                          + "<td class='atd'></td>"

                          + "<td class='atd' " + diffStyle + ">--</td>"
                          + "<td class='atd' " + diffStyle + ">--</td>"

                          + "</tr>";
                    }
                    else if (mainItem.StudentName.Trim() != "--" && parentItem.StudentName != "--")
                    {
                        htmlContent =
                            "<tr>"
                          + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.StudentName + "</td>"
                          + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.Comments + "</td>"

                          + "<td class='atd'></td>"

                          + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + parentItem.StudentName + "</td>"
                          + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + (string.IsNullOrEmpty(parentItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + parentItem.Comments + "' >" + parentItem.Comments + "</textarea>") + "</td>"
                          + "</tr>";
                    }

                    retVal.Append(htmlContent);
                }

                if (retVal.Length > 0)
                {
                    retVal.Append("</table></tr></table>");
                    retVal.Append("</span>");
                }
            }
            else if (mainDiff.Count > 0 && parentDiff.Count > 0 && childDiff.Count > 0)
            {
                if (mainDiff.Count == parentDiff.Count && parentDiff.Count == childDiff.Count)
                {
                    counter = mainDiff.Count;
                }
                else
                {
                    var listWithMostElements = (new List<List<StudentSet>> { mainDiff, parentDiff, childDiff })
                                                .OrderByDescending(x => x.Count())
                                                .First();
                    counter = listWithMostElements.Count;
                }

                if (counter > 0)
                {
                    retVal.Append("<span style='display: inline - block; vertical - align: top'>");
                    retVal.Append("<table class='aTable'><tr><td'></td></tr><tr><table>"
                                  + "<tr><th class='ath'>Student Name</th><th class='ath'>Comments</th><th class='ath'>---</th>"
                                  + "<th class='ath'>Student Name</th><th class='ath'>Comments</th>"
                                  + "<th class='ath'>---</th>"
                                  + "<th class='ath'>Student Name</th><th class='ath'>Comments</th>"
                                  + "</tr>");

                    for (int i = 0; i < counter; i++)
                    {
                        var existsInMain = mainDiff.ElementAtOrDefault(i) != null;
                        var existsInParent = parentDiff.ElementAtOrDefault(i) != null;
                        var existsInChild = childDiff.ElementAtOrDefault(i) != null;
                        htmlContent = string.Empty;

                        if (existsInMain)
                        {
                            mainItem = mainDiff[i];
                        }
                        else
                        {
                            mainItem = blankItem;
                        }

                        if (existsInParent)
                        {
                            parentItem = parentDiff[i];
                        }
                        else
                        {
                            parentItem = blankItem;
                        }

                        if (existsInChild)
                        {
                            childItem = childDiff[i];
                        }
                        else
                        {
                            childItem = blankItem;
                        }

                        if (!mainItem.StudentName.Contains("--") && parentItem.StudentName.Contains("--") && childItem.StudentName.Contains("--"))
                        {
                            htmlContent =
                                "<tr>"

                              + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.StudentName + "</td>"
                              + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.Comments + "</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd' " + diffStyle + ">--</td>"
                              + "<td class='atd' " + diffStyle + ">--</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd' " + diffStyleChild + ">--</td>"
                              + "<td class='atd' " + diffStyleChild + ">--</td>"

                              + "</tr>";
                        }
                        else if (!mainItem.StudentName.Contains("--") && !parentItem.StudentName.Contains("--") && childItem.StudentName.Contains("--"))
                        {
                            htmlContent =
                                "<tr>"

                              + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.StudentName + "</td>"
                              + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.Comments + "</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + parentItem.StudentName + "</td>"
                              + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + (string.IsNullOrEmpty(parentItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + parentItem.Comments + "' >" + parentItem.Comments + "</textarea>") + "</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd' " + diffStyleChild + ">--</td>"
                              + "<td class='atd' " + diffStyleChild + ">--</td>"

                              + "</tr>";
                        }
                        else if (!mainItem.StudentName.Contains("--") && !parentItem.StudentName.Contains("--") && !childItem.StudentName.Contains("--"))
                        {
                            htmlContent =
                                "<tr>"

                              + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.StudentName + "</td>"
                              + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.Comments + "</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + parentItem.StudentName + "</td>"
                              + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + (string.IsNullOrEmpty(parentItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + parentItem.Comments + "' >" + parentItem.Comments + "</textarea>") + "</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd'" + (childItem.ApplyDifferentStyles ? diffStyleChild : "") + ">" + childItem.StudentName + "</td>"
                              + "<td class='atd'" + (childItem.ApplyDifferentStyles ? diffStyleChild : "") + ">" + (string.IsNullOrEmpty(childItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + childItem.Comments + "' >" + childItem.Comments + "</textarea>") + "</td>"

                              + "</tr>";
                        }
                        else if (mainItem.StudentName.Contains("--") && parentItem.StudentName.Contains("--") && childItem.StudentName.Contains("--"))
                        {
                            //This results in a blanks all across! (not needed)
                            //_htmlContent =
                            //    "<tr>"

                            //  + "<td class='atd' " + _diffStyle + "></td>"
                            //  + "<td class='atd' " + _diffStyle + ">--</td>"

                            //  + "<td class='atd'></td>"

                            //  + "<td class='atd' " + _diffStyle + ">--</td>"
                            //  + "<td class='atd' " + _diffStyle + ">--</td>"
                            //  + "<td class='atd' " + _diffStyle + ">--</td>"
                            //  + "<td class='atd' " + _diffStyle + ">--</td>"
                            //  + "<td class='atd' " + _diffStyle + ">--</td>"
                            //  + "<td class='atd' " + _diffStyle + ">--</td>"
                            //  + "<td class='atd' " + _diffStyle + ">--</td>"

                            //  + "<td class='atd'></td>"

                            //  + "<td class='atd' " + _diffStyleChild + ">--</td>"
                            //  + "<td class='atd' " + _diffStyleChild + ">--</td>"
                            //  + "<td class='atd' " + _diffStyleChild + ">--</td>"
                            //  + "<td class='atd' " + _diffStyleChild + ">--</td>"
                            //  + "<td class='atd' " + _diffStyleChild + ">--</td>"
                            //  + "<td class='atd' " + _diffStyleChild + ">--</td>"
                            //  + "<td class='atd' " + _diffStyleChild + ">--</td>"

                            //  + "</tr>";
                            continue;
                        }
                        else if (mainItem.StudentName.Contains("--") && parentItem.StudentName.Contains("--") && !childItem.StudentName.Contains("--"))
                        {
                            htmlContent =
                                "<tr>"

                                + "<td class='atd' " + diffStyle + "></td>"
                                + "<td class='atd' " + diffStyle + ">--</td>"

                                + "<td class='atd'></td>"

                                + "<td class='atd' " + diffStyle + ">--</td>"
                                + "<td class='atd' " + diffStyle + ">--</td>"

                                + "<td class='atd'></td>"

                                + "<td class='atd'" + (childItem.ApplyDifferentStyles ? diffStyleChild : "") + ">" + childItem.StudentName + "</td>"
                                + "<td class='atd'" + (childItem.ApplyDifferentStyles ? diffStyleChild : "") + ">" + (string.IsNullOrEmpty(childItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + childItem.Comments + "' >" + childItem.Comments + "</textarea>") + "</td>"

                                + "</tr>";
                        }
                        else if (mainItem.StudentName.Contains("--") && !parentItem.StudentName.Contains("--") && childItem.StudentName.Contains("--"))
                        {
                            htmlContent =
                              "<tr>"

                              + "<td class='atd' " + diffStyle + "></td>"
                              + "<td class='atd' " + diffStyle + ">--</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + parentItem.StudentName + "</td>"
                              + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + (string.IsNullOrEmpty(parentItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + parentItem.Comments + "' >" + parentItem.Comments + "</textarea>") + "</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd' " + diffStyleChild + ">--</td>"
                              + "<td class='atd' " + diffStyleChild + ">--</td>"

                              + "</tr>";
                        }
                        else if (mainItem.StudentName.Contains("--") && !parentItem.StudentName.Contains("--") && !childItem.StudentName.Contains("--"))
                        {
                            htmlContent =
                              "<tr>"

                              + "<td class='atd' " + diffStyle + "></td>"
                              + "<td class='atd' " + diffStyle + ">--</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + parentItem.StudentName + "</td>"
                              + "<td class='atd'" + (parentItem.ApplyDifferentStyles ? diffStyle : "") + ">" + (string.IsNullOrEmpty(parentItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + parentItem.Comments + "' >" + parentItem.Comments + "</textarea>") + "</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd'" + (childItem.ApplyDifferentStyles ? diffStyleChild : "") + ">" + childItem.StudentName + "</td>"
                              + "<td class='atd'" + (childItem.ApplyDifferentStyles ? diffStyleChild : "") + ">" + (string.IsNullOrEmpty(childItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + childItem.Comments + "' >" + childItem.Comments + "</textarea>") + "</td>"

                              + "</tr>";
                        }
                        else if (!mainItem.StudentName.Contains("--") && parentItem.StudentName.Contains("--") && !childItem.StudentName.Contains("--"))
                        {
                            htmlContent =
                              "<tr>"

                              + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.StudentName + "</td>"
                              + "<td class='atd'" + (mainItem.ApplyDifferentStyles ? diffStyle : "") + ">" + mainItem.Comments + "</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd' " + diffStyle + ">--</td>"
                              + "<td class='atd' " + diffStyle + ">--</td>"

                              + "<td class='atd'></td>"

                              + "<td class='atd'" + (childItem.ApplyDifferentStyles ? diffStyleChild : "") + ">" + childItem.StudentName + "</td>"
                              + "<td class='atd'" + (childItem.ApplyDifferentStyles ? diffStyleChild : "") + ">" + (string.IsNullOrEmpty(childItem.Comments) ? "" : "<textarea class='scrollabletextbox showToolTip' rows='3' col='10' title='" + childItem.Comments + "' >" + childItem.Comments + "</textarea>") + "</td>"

                              + "</tr>";
                        }

                        if (!string.IsNullOrEmpty(htmlContent))
                        {
                            retVal.Append(htmlContent);
                        }
                    }

                    if (retVal.Length > 0)
                    {
                        retVal.Append("</table></tr></table>");
                        retVal.Append("</span>");
                    }
                }
                else
                {
                    retVal.Append("<div><p style='color:red;text-align:center;'><b>Unable to create Route flow difference!</b></p></div>");
                }
            }

            return retVal.ToString();
        }
    }
}