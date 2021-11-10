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
        double min_dwell_time = 0.4;
        double ring_length_mm = 1320.0;
        double ring_deadspace_mm = 0;
        double ring_angle_degrees = 270;
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
        public void general_checks(Catheter catheter)
        {

            List<SourcePosition> sourcepositions = catheter.SourcePositions.ToList();
            if (sourcepositions.Count == 0)
            {
                System.Windows.MessageBox.Show($"Catheter ring in channel {catheter.ChannelNumber}, {catheter}, but there are no source positions!");
            }
        }
        public void check_for_ring(Catheter catheter)
        {
            bool angle_is_ring = is_ring_from_angle(catheter); // Determine if we have more than 270 degrees of curvature
            if (angle_is_ring)
            {
                bool error_free = true;
                if (catheter.ApplicatorLength != ring_length_mm) // Check applicator length
                {
                    System.Windows.MessageBox.Show($"Potential ring in channel {catheter.ChannelNumber}, {catheter}, but the applicator " +
                        $"length was {catheter.ApplicatorLength / 10}cm and should be 132cm");
                    error_free = false;
                }
                if (catheter.DeadSpaceLength > ring_deadspace_mm) // Check dead space
                {
                    System.Windows.MessageBox.Show($"Potential ring in channel {catheter.ChannelNumber}, {catheter}, but the dead space " +
                        $" was {catheter.DeadSpaceLength / 10}cm and should be {ring_deadspace_mm / 10} cm");
                    error_free = false;
                }

                else
                {
                    List<SourcePosition> sourcepositions = catheter.SourcePositions.ToList();
                    double dwell_time_0 = sourcepositions[0].DwellTime;
                    if (dwell_time_0 < min_dwell_time)
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
            if (angle > ring_angle_degrees) // 270 degrees is 3/4 of the full circle, this should be an effective minimum requirement
            {
                return true;
            }
            return false;
        }
    }
}
