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
[assembly: AssemblyVersion("1.0.0.1")]
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
      // TODO : Add here the code that is called when the script is launched from Eclipse.
    }
        public bool is_ring(Catheter catheter)
        {
            if (catheter.ApplicatorLength != 1320.0)
            {
                return false;
            }
            if (catheter.DeadSpaceLength > 0)
            {
                return false;
            }
            double[] distances;
            foreach ()
            distances.
            return true;
        }
  }
}
