using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;


namespace BrachyRingChecker.Services
{
    public class CheckForRing
    {
        public CheckForRing()
        {
        }
        public void check_brachyplan_for_rings(BrachyPlanSetup brachyPlan)
        {
            IEnumerable<Catheter> catheters = brachyPlan.Catheters;
            foreach (Catheter cat in catheters)
            {
                check_for_ring(cat);
            }
        }
        public void check_for_ring(Catheter catheter)
        {
            bool angle_is_ring = is_ring_from_angle(catheter); // Determine if we have more than 90 degrees of curvature
            if (angle_is_ring)
            {
                bool error_free = true;
                if (catheter.ApplicatorLength != 1320.0) // Check applicator length
                {
                    System.Windows.MessageBox.Show($"Potential ring in channel {catheter.ChannelNumber}, {catheter}, but the applicator " +
                        $"length was {catheter.ApplicatorLength / 10}cm and should be 132cm");
                    error_free = false;
                }
                if (catheter.DeadSpaceLength > 0) // Check dead space
                {
                    System.Windows.MessageBox.Show($"Potential ring in channel {catheter.ChannelNumber}, {catheter}, but the dead space " +
                        $" was {catheter.DeadSpaceLength / 10}cm and should be 0cm");
                    error_free = false;
                }
                List<SourcePosition> sourcepositions = catheter.SourcePositions.ToList();
                if (sourcepositions.Count == 0)
                {
                    error_free = false;
                    System.Windows.MessageBox.Show($"Potential ring in channel {catheter.ChannelNumber}, {catheter}, but there are no source positions!");
                }
                else
                {
                    double dwell_time_0 = sourcepositions[0].DwellTime;
                    if (dwell_time_0 != 0.4)
                    {
                        error_free = false;
                        System.Windows.MessageBox.Show($"Potential ring in channel {catheter.ChannelNumber}, {catheter}, " +
                            $"but the first dwell time was {dwell_time_0}s and not 0.4s");
                    }
                }

                if (error_free) // Reminder for distal correction
                {
                    System.Windows.MessageBox.Show($"Potential ring in channel {catheter.ChannelNumber}, {catheter}." +
                        $" Use distal correction!");
                }
            }
        }
        public bool is_ring_from_angle(Catheter catheter)
        {
            double angle = 0;
            for (int i = 0; i < catheter.Shape.Length - 2; i++)
            {
                VVector vector_1 = catheter.Shape[i + 1] - catheter.Shape[i];
                double norm_1 = Math.Sqrt(Math.Pow(vector_1.x, 2) + Math.Pow(vector_1.y, 2) + Math.Pow(vector_1.z, 2));
                vector_1 /= norm_1;

                VVector vector_2 = catheter.Shape[i + 2] - catheter.Shape[i + 1];
                double norm_2 = Math.Sqrt(Math.Pow(vector_2.x, 2) + Math.Pow(vector_2.y, 2) + Math.Pow(vector_2.z, 2));
                vector_2 /= norm_2;

                double dot_product = vector_2.x * vector_1.x + vector_2.y * vector_1.y + vector_2.z * vector_1.z;
                double angle_degrees = Math.Acos(dot_product) * 180 / Math.PI;
                angle += angle_degrees;
            }
            // Original idea was to make it 180, but for particularly torterous paths, like soft tissue sarcoma patients
            // the program was identifying them as rings
            if (angle > 270) // 270 degrees is 3/4 of the full circle, this should be an effective minimum requirement
            {
                return true;
            }
            return false;
        }
    }
}
