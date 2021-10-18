using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.7")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Execute(ScriptContext context /*, System.Windows.Window window, ScriptEnvironment environment*/)
    {
            Patient patient = context.Patient;
            BrachyPlanSetup brachy_plan = context.BrachyPlanSetup;
            IEnumerable<Catheter> catheters = brachy_plan.Catheters;
            foreach (Catheter cat in catheters)
            {
                check_for_ring(cat);
            }
      // TODO : Add here the code that is called when the script is launched from Eclipse.
    }
        public void check_for_ring(Catheter catheter)
        {
            bool angle_is_ring = is_ring_from_angle(catheter);
            if (angle_is_ring)
            {
                if (catheter.ApplicatorLength != 1320.0)
                {
                    System.Windows.MessageBox.Show($"Angle indicates a ring for {catheter.Name}, channel {catheter.ChannelNumber}  but the applicator " +
                        $"length was {catheter.ApplicatorLength}");
                }
                if (catheter.DeadSpaceLength > 0)
                {
                    System.Windows.MessageBox.Show($"Angle indicates a ring for {catheter.Name}, channel {catheter.ChannelNumber} but the dead space " +
                        $" was {catheter.DeadSpaceLength}");
                }
                System.Windows.MessageBox.Show($"Angle indicates a ring for {catheter.Name}, channel {catheter.ChannelNumber}." +
                    $"Use distal correction!");
            }
        }
        public bool is_ring_from_angle(Catheter catheter)
        {
            int counter = 0;
            double[,] points_all;
            List<VVector> points = new List<VVector>();
            foreach (VVector i in catheter.Shape)
            {
                counter += 1;
                points.Add(i);
            }
            double angle = 0;
            for (int i = 0; i < counter - 2; i ++)
            {
                VVector vector_1 = points[i + 1] - points[i];
                double norm_1 = Math.Sqrt(Math.Pow(vector_1.x, 2) + Math.Pow(vector_1.y, 2) + Math.Pow(vector_1.z, 2));
                vector_1 /= norm_1;

                VVector vector_2 = points[i + 2] - points[i + 1];
                double norm_2 = Math.Sqrt(Math.Pow(vector_2.x, 2) + Math.Pow(vector_2.y, 2) + Math.Pow(vector_2.z, 2));
                vector_2 /= norm_2;

                double dot_product = vector_2.x * vector_1.x + vector_2.y * vector_1.y + vector_2.z * vector_1.z;
                double angle_degrees = Math.Acos(dot_product) * 180 / Math.PI;
                angle += angle_degrees;
            }
            if (angle > 90) // If we have over 90 degrees, this is definitely a ring...
            {
                return true;
            }
            return false;
        }
  }
}
