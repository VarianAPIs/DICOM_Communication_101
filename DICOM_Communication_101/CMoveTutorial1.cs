using EvilDICOM.Core.Helpers;
using EvilDICOM.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOM_Communication_101
{
    /// <summary>
    /// This tutorial is outlined in chapter 4 of Scripting in RT for Physicists (C-MOVE)
    /// </summary>
    public class CMoveTutorial1
    {
        public static void Run()
        {
            //Store the details of the daemon (Ae Title, IP, port)
            var daemon = new Entity("PHYSX_DICOM", "10.22.86.64", 51402);
            //Store the details of the mobius DICOM entity (Ae Title, IP, port)
            var mobius = new Entity("MOBIUST", "10.241.20.41", 104);
            //Store the details of the client (Ae Title, port) -> IP address is determined by CreateLocal() method
            var local = Entity.CreateLocal("DICOMEC1", 9999);
            //Set up a client (DICOM SCU = Service Class User)
            var client = new DICOMSCU(local);

            //Build a finder class to help with C-FIND operations
            var finder = client.GetCFinder(daemon);
            var studies = finder.FindStudies("DA00001");
            var series = finder.FindSeries(studies);

            //Filter series by modality, then create list of 
            var plans = series.Where(s => s.Modality == "RTPLAN")
                .SelectMany(ser => finder.FindImages(ser));
            var doses = series.Where(s => s.Modality == "RTDOSE")
                    .SelectMany(ser => finder.FindImages(ser));
            var cts = series.Where(s => s.Modality == "CT")
                    .SelectMany(ser => finder.FindImages(ser));

            var mover = client.GetCMover(daemon);
            ushort msgId = 1;
            foreach (var plan in plans)
            {
                Console.WriteLine($"Sending plan {plan.SOPInstanceUID}...");
                //Make sure Mobius is on the whitelist of the daemon
                var response = mover.SendCMove(plan, mobius.AeTitle, ref msgId);
                Console.WriteLine($"DICOM C-Move Results : ");
                Console.WriteLine($"Number of Completed Operations : {response.NumberOfCompletedOps}");
                Console.WriteLine($"Number of Failed Operations : {response.NumberOfFailedOps}");
                Console.WriteLine($"Number of Remaining Operations : {response.NumberOfRemainingOps}");
                Console.WriteLine($"Number of Warning Operations : {response.NumberOfWarningOps}");
            }

            Console.Read(); //Stop here
        }
    }
}
