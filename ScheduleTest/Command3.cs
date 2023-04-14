#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace ScheduleTest
{
    [Transaction(TransactionMode.Manual)]
    public class Command3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            View curView = doc.ActiveView;

            if(curView is ViewSchedule)
            {
                ViewSchedule curSchedule = curView as ViewSchedule;

                var table = curSchedule.GetTableData();
                var section = table.GetSectionData(SectionType.Body);
                var nRows = section.NumberOfRows;
                var nColumns = section.NumberOfColumns;

                FilteredElementCollector collector = new FilteredElementCollector(doc, curSchedule.Id);
                List<Element> elementsList = collector.ToList();

                ScheduleDefinition curDef = curSchedule.Definition;
                
                List<ElementId> elemIdList = new List<ElementId>();

                foreach(SchedulableField curField in curDef.GetSchedulableFields())
                {
                    elemIdList.Add(curField.ParameterId);
                }

                List<List<string>> scheduleData = new List<List<string>>(); 
                foreach(Element curScheduleItem in collector)
                {
                    List<string> curRow = new List<string>();
                    curRow.Add(curScheduleItem.Id.ToString());

                    foreach (Parameter curParam in curScheduleItem.Parameters)
                    {
                        if (curParam.Definition is InternalDefinition internalDefinition)
                        {
                            // Get the BuiltInParameter
                            BuiltInParameter builtInParameter = (BuiltInParameter)internalDefinition.Id.IntegerValue;

                            foreach(ElementId curId in elemIdList)
                            {


                                if(curId.IntegerValue == (int)builtInParameter)
                                {
                                    string curValue = curScheduleItem.get_Parameter(builtInParameter).AsValueString();
                                    curRow.Add(curValue);
                                }
                            }   
                        }
                        else
                        {
                            // do something about shared parameters / project parameters
                        }
                    }  
                    
                    scheduleData.Add(curRow);
                }

                Debug.Print(scheduleData.Count().ToString());
            }

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
