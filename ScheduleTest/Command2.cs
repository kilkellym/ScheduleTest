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
    public class Command2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(Level));

            // get room category id
            ElementId catId = new ElementId(BuiltInCategory.OST_Rooms);

            // get parameter fields for schedule
            ElementId numId = new ElementId(BuiltInParameter.ROOM_NUMBER);
            ElementId nameId = new ElementId(BuiltInParameter.ROOM_NAME);
            ElementId baseId = new ElementId(BuiltInParameter.ROOM_FINISH_BASE);
            ElementId floorId = new ElementId(BuiltInParameter.ROOM_FINISH_FLOOR);
            ElementId ceilingId = new ElementId(BuiltInParameter.ROOM_FINISH_CEILING);
            ElementId wallId = new ElementId(BuiltInParameter.ROOM_FINISH_WALL);
            ElementId commentId = new ElementId(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
            List<ElementId> fieldList = new List<ElementId> { numId, nameId, baseId, floorId, wallId, ceilingId, commentId };

            using(Transaction t = new Transaction(doc))
            {
                t.Start("Create room schedules");
                foreach (Level curLevel in collector)
                {
                    string curSchedName = "Room Schedule " + curLevel.Name;
                    ViewSchedule curSchedule = Utils.CreateSchedule(doc, BuiltInCategory.OST_Rooms, curSchedName);

                    // add fields to schedule
                    foreach(ElementId elementId in fieldList)
                    {
                        ScheduleField field = curSchedule.Definition.AddField(ScheduleFieldType.Instance, elementId);
                    }

                    // create level field and use it for filter
                    ScheduleField levelField = curSchedule.Definition.AddField(ScheduleFieldType.Instance, new ElementId(BuiltInParameter.ROOM_LEVEL_ID));
                    levelField.IsHidden = true;

                    // create filter
                    ScheduleFilter filter = new ScheduleFilter(levelField.FieldId, ScheduleFilterType.Equal, curLevel.Id);
                    curSchedule.Definition.AddFilter(filter);

                    // sort by room number

                }
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
