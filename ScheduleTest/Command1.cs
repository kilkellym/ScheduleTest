#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace ScheduleTest
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            using(Transaction t = new Transaction(doc))
            {
                t.Start("Create Schedule");

                AreaScheme curAreaScheme = Utils.GetAreaSchemeByName(doc, "Gross Building");
                ViewSchedule newSched = Utils.CreateAreaSchedule(doc, "New Area Schedule (Gross Building)", curAreaScheme);

                List<string> paramNames = new List<string>() { "Number", "Name", "Level", "Area", "Comments", "TEST PARAM" };

                List<Parameter> paramList = Utils.GetParametersByName(doc, paramNames, BuiltInCategory.OST_Areas);
                Utils.AddFieldsToSchedule(doc, newSched, paramList);

                t.Commit();
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
